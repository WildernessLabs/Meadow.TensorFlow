using HelloWorld.Models;
using Meadow;
using Meadow.Devices;
using Meadow.TensorFlow;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace HelloWorld;

public class MeadowApp : App<F7FeatherV2>
{
    readonly HelloWorldModel helloWorld = new();
    const int ArenaSize = 2000;
    TensorFlowLiteStatus status;
    TensorFlowLiteTensor input, output;
    TensorFlowLiteQuantizationParams inputParam, outputParam;
    IntPtr interpreter;

    public override Task Initialize()
    {
        Resolver.Log.Info("Initialize TensorFlow ...");

        IntPtr model = Marshal.AllocHGlobal(helloWorld.GetSize() * sizeof(int));

        if (model == IntPtr.Zero)
        {
            Resolver.Log.Info("Failed to allocated model");
            return base.Initialize();
        }

        IntPtr arena = Marshal.AllocHGlobal(ArenaSize * sizeof(int));

        if (arena == IntPtr.Zero)
        {
            Resolver.Log.Info("Failed to allocated arena");
            Marshal.FreeHGlobal(model);
            return base.Initialize();
        }

        Marshal.Copy(helloWorld.GetData(), 0, model, helloWorld.GetSize());

        var model_options = TensorFlowLiteBindings.TfLiteMicroGetModel(ArenaSize, arena, model);
        if (model_options == null)
            Resolver.Log.Info("Failed to loaded the model");

        var interpreter_options = TensorFlowLiteBindings.TfLiteMicroInterpreterOptionCreate(model_options);
        if (interpreter_options == null)
            Resolver.Log.Info("Failed to create interpreter option");

        interpreter = TensorFlowLiteBindings.TfLiteMicroInterpreterCreate(interpreter_options, model_options);
        if (interpreter == null)
            Resolver.Log.Info("Failed to Interpreter");

        status = TensorFlowLiteBindings.TfLiteMicroInterpreterAllocateTensors(interpreter);
        if (status != TensorFlowLiteStatus.Ok)
        {
            Resolver.Log.Info("Failed to allocate tensors");
        }

        input = TensorFlowLiteBindings.TfLiteMicroInterpreterGetInput(interpreter, 0);

        output = TensorFlowLiteBindings.TfLiteMicroInterpreterGetOutput(interpreter, 0);

        helloWorld.interferece_count = 0;

        inputParam = TensorFlowLiteBindings.TfLiteMicroTensorQuantizationParams(input);
        outputParam = TensorFlowLiteBindings.TfLiteMicroTensorQuantizationParams(output);

        return Task.CompletedTask;
    }

    public override Task Run()
    {
        HelloWorldModel.HelloWorldResult_t[] result = helloWorld.populateResult();

        for (int i = 0; i < result.Length; i++)
        {

            float position = helloWorld.interferece_count / (float)helloWorld.kInterferencesPerCycles;
            float x = position * helloWorld.kXrange;

            sbyte x_quantized = (sbyte)((x / inputParam.Scale) + inputParam.ZeroPoint);

            TensorFlowLiteBindings.TfLiteMicroSetInt8Data(input, 0, x_quantized);

            status = TensorFlowLiteBindings.TfLiteMicroInterpreterInvoke(interpreter);
            if (status != TensorFlowLiteStatus.Ok)
            {
                Resolver.Log.Info("Failed to Invoke");
                return Task.CompletedTask;
            }

            sbyte y_quantized = TensorFlowLiteBindings.TfLiteMicroGeInt8tData(output, 0);

            float y = ((float)(y_quantized - outputParam.ZeroPoint)) * outputParam.Scale;

            Resolver.Log.Info($" {i} - {(x, y)} ");

            if (helloWorld.floatNoTEqual(x, result[i].x) || helloWorld.floatNoTEqual(y, result[i].y))
            {
                Resolver.Log.Info($"Test {i} failed");
                Resolver.Log.Info($"Expeced {(result[i].x, result[i].y)}");
                break;
            }

            helloWorld.interferece_count += 1;

            if (helloWorld.interferece_count >= helloWorld.kInterferencesPerCycles)
            {
                helloWorld.interferece_count = 0;
            }
        }

        Resolver.Log.Info("Sample completed");
        return Task.CompletedTask;
    }
}