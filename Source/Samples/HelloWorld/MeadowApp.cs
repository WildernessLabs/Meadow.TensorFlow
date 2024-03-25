using Meadow;
using Meadow.Devices;
using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using HelloWorld;
using Tensorflow.litemicro;

namespace MeadowApp
{
    public class MeadowApp : App<F7FeatherV2>
    {
        HelloWorldModel helloWorld = new HelloWorldModel();
        const int ArenaSize = 2000;
        TfLiteStatus tfLiteStatus;
        TfLiteTensor input, output;
        TfLiteQuantizationParams input_param, output_param;
        IntPtr interpreter;
        public override Task Initialize()
        {
            Resolver.Log.Info("Initialize Tensor Flow ...");

            IntPtr model = Marshal.AllocHGlobal(helloWorld.GetSize() * sizeof(int));

            if (model == IntPtr.Zero)
            {
                Resolver.Log.Info("Failed to allocated model");
                return base.Initialize();
            }

            IntPtr arena = Marshal.AllocHGlobal(ArenaSize* sizeof(int));

            if (arena == IntPtr.Zero)
            {
                Resolver.Log.Info("Failed to allocated arena");
                Marshal.FreeHGlobal(model);
                return base.Initialize();
            }

            Marshal.Copy(helloWorld.GetData(), 0, model, helloWorld.GetSize());

            var model_options = c_api_lite_micro.TfLiteMicroGetModel(ArenaSize, arena, model);
            if (model_options == null)
                Resolver.Log.Info("Falied to loaded the model");

            var interpreter_options = c_api_lite_micro.TfLiteMicroInterpreterOptionCreate(model_options);
            if (interpreter_options == null)
                Resolver.Log.Info("Falied to create interpreter option");

            interpreter = c_api_lite_micro.TfLiteMicroInterpreterCreate(interpreter_options, model_options);
            if (interpreter == null)
                Resolver.Log.Info("Failed to Interpreter");

            tfLiteStatus = c_api_lite_micro.TfLiteMicroInterpreterAllocateTensors(interpreter);
            if (tfLiteStatus != TfLiteStatus.kTfLiteOk)
            {
                Resolver.Log.Info("Falied to allocate tensors");
            }

            input = c_api_lite_micro.TfLiteMicroInterpreterGetInput(interpreter, 0);

            output = c_api_lite_micro.TfLiteMicroInterpreterGetOutput(interpreter, 0);

            helloWorld.interferece_count = 0;

            input_param = c_api_lite_micro.TfLiteMicroTensorQuantizationParams(input);
            output_param = c_api_lite_micro.TfLiteMicroTensorQuantizationParams(output);

            return base.Initialize();
        }

        public override async Task Run()
        {
            HelloWorldModel.HelloWorldResult_t []result = helloWorld.populateResult();

            for(int i = 0; i < result.Length; i++)
            {

                float position = helloWorld.interferece_count/(float)helloWorld.kInterferencesPerCycles;
                float x = position * helloWorld.kXrange;

                sbyte x_quantized = (sbyte)((x /input_param.scale) + input_param.zero_point);

                c_api_lite_micro.TfLiteMicroSetInt8Data(input, 0, x_quantized);

                tfLiteStatus = c_api_lite_micro.TfLiteMicroInterpreterInvoke(interpreter);
                if (tfLiteStatus != TfLiteStatus.kTfLiteOk)
                {
                    Resolver.Log.Info("Failed to Invoke");
                    return;
                }

                sbyte y_quantized = c_api_lite_micro.TfLiteMicroGeInt8tData(output, 0);

                float y = ((float)(y_quantized - output_param.zero_point)) * output_param.scale;

                Resolver.Log.Info($" {i} - {(x,y)} ");

                if (helloWorld.floatNoTEqual(x,result[i].x) || helloWorld.floatNoTEqual(y, result[i].y))
                {
                    Resolver.Log.Info($"Test {i} falied");
                    Resolver.Log.Info($"Expeced {(result[i].x, result[i].y)}");
                    break;
                }

                helloWorld.interferece_count += 1;

                if (helloWorld.interferece_count >= helloWorld.kInterferencesPerCycles)
                {
                    helloWorld.interferece_count = 0;
                }
            }

            Resolver.Log.Info("Tensor Flow completed");
        }
    }
}
