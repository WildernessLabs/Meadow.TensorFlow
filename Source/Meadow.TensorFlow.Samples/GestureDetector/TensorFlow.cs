using GestureDetector.Models;
using Meadow;
using Meadow.TensorFlow;
using System;
using System.Runtime.InteropServices;

namespace GestureDetector;

public class TensorFlow
{
    private static readonly Lazy<TensorFlow> instance = new();
    public static TensorFlow Instance => instance.Value;

    private TensorFlowLiteStatus tfLiteStatus;
    private TensorFlowLiteTensor input;
    private TensorFlowLiteTensor output;

    private IntPtr interpreter;
    private readonly int ArenaSize = 60 * 1024;

    public void Initialize()
    {
        Model gestureDetectorModel = new();

        IntPtr model = Marshal.AllocHGlobal(gestureDetectorModel.Size * sizeof(byte));

        if (model == IntPtr.Zero)
        {
            Resolver.Log.Info("Failed to allocated model");
            return;
        }

        Marshal.Copy(gestureDetectorModel.Data, 0, model, gestureDetectorModel.Size);

        IntPtr arena = Marshal.AllocHGlobal(ArenaSize * sizeof(int));

        if (arena == IntPtr.Zero)
        {
            Resolver.Log.Info("Failed to allocated arena");
            Marshal.FreeHGlobal(model);
            return;
        }

        var model_options = TensorFlowLiteBindings.TfLiteMicroGetModel(ArenaSize, arena, model);
        if (model_options == null)
            Resolver.Log.Info("Failed to loaded the model");

        var interpreter_options = TensorFlowLiteBindings.TfLiteMicroInterpreterOptionCreate(model_options);
        if (interpreter_options == null)
            Resolver.Log.Info("Failed to create interpreter option");

        interpreter = TensorFlowLiteBindings.TfLiteMicroInterpreterCreate(interpreter_options, model_options);
        if (interpreter == null)
            Resolver.Log.Info("Failed to Interpreter");

        tfLiteStatus = TensorFlowLiteBindings.TfLiteMicroInterpreterAllocateTensors(interpreter);
        if (tfLiteStatus != TensorFlowLiteStatus.Ok)
            Resolver.Log.Info("Failed to allocate tensors");

        input = TensorFlowLiteBindings.TfLiteMicroInterpreterGetInput(interpreter, 0);
        output = TensorFlowLiteBindings.TfLiteMicroInterpreterGetOutput(interpreter, 0);

        Resolver.Log.Info("Tensor flow Initialize");
    }

    public int InputLegth()
    {
        return TensorFlowLiteBindings.TfLiteMicroGetByte(input) / sizeof(float);
    }

    public void InputData(int index, float value)
    {
        TensorFlowLiteBindings.TfLiteMicroSetFloatData(input, index, value);
    }

    public float OutputData(int index)
    {
        return TensorFlowLiteBindings.TfLiteMicroGetFloatData(TensorFlowLiteBindings.TfLiteMicroInterpreterGetOutput(interpreter, 0), index);
    }

    public TensorFlowLiteStatus Invoke()
    {
        return TensorFlowLiteBindings.TfLiteMicroInterpreterInvoke(interpreter);
    }
}