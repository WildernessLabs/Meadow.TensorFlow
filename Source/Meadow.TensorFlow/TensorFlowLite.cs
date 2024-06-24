using System;
using System.Runtime.InteropServices;

namespace Meadow.TensorFlow;

public class TensorFlowLite
{
    public TensorFlowLiteQuantizationParams InputParam { get; private set; }
    public TensorFlowLiteQuantizationParams OutputParam { get; private set; }

    public TensorFlowLiteStatus Status { get; set; } = TensorFlowLiteStatus.Ok;

    private TensorFlowLiteTensor Input { get; set; }
    private TensorFlowLiteTensor Output { get; set; }

    private readonly IntPtr interpreter;
    private readonly int arenaSize;

    public TensorFlowLite(ITensorModel tensorModel, int arenaSize)
    {
        this.arenaSize = arenaSize;

        IntPtr model = Marshal.AllocHGlobal(tensorModel.Size * sizeof(int));

        if (model == IntPtr.Zero)
        {
            throw new Exception("Failed to allocated model");
        }

        IntPtr arena = Marshal.AllocHGlobal(arenaSize * sizeof(int));

        if (arena == IntPtr.Zero)
        {
            Marshal.FreeHGlobal(model);
            throw new Exception("Failed to allocated arena");
        }

        Marshal.Copy(tensorModel.Data, 0, model, tensorModel.Size);

        var modelOptions = TensorFlowLiteBindings.TfLiteMicroGetModel(arenaSize, arena, model);
        if (modelOptions == null)
        {
            throw new Exception("Failed to loaded the model");
        }

        var interpreterOptions = TensorFlowLiteBindings.TfLiteMicroInterpreterOptionCreate(modelOptions);
        if (interpreterOptions == null)
        {
            throw new Exception("Failed to create interpreter option");
        }

        interpreter = TensorFlowLiteBindings.TfLiteMicroInterpreterCreate(interpreterOptions, modelOptions);
        if (interpreter == null)
        {
            throw new Exception("Failed to Interpreter");
        }

        Status = TensorFlowLiteBindings.TfLiteMicroInterpreterAllocateTensors(interpreter);

        if (Status != TensorFlowLiteStatus.Ok)
        {
            throw new Exception("Failed to allocate tensors");
        }

        Input = TensorFlowLiteBindings.TfLiteMicroInterpreterGetInput(interpreter, 0);
        Output = TensorFlowLiteBindings.TfLiteMicroInterpreterGetOutput(interpreter, 0);

        InputParam = TensorFlowLiteBindings.TfLiteMicroTensorQuantizationParams(Input);
        OutputParam = TensorFlowLiteBindings.TfLiteMicroTensorQuantizationParams(Output);
    }

    public int InputLength()
    {
        return TensorFlowLiteBindings.TfLiteMicroGetByte(Input) / sizeof(float);
    }

    public void InputInt8Data(int index, sbyte value)
    {
        TensorFlowLiteBindings.TfLiteMicroSetInt8Data(Input, index, value);
    }

    public sbyte OutputInt8Data(int index)
    {
        return TensorFlowLiteBindings.TfLiteMicroGetInt8tData(Output, index);
    }

    public void InputFloatData(int index, float value)
    {
        TensorFlowLiteBindings.TfLiteMicroSetFloatData(Input, index, value);
    }

    public float OutputFloatData(int index)
    {
        return TensorFlowLiteBindings.TfLiteMicroGetFloatData(TensorFlowLiteBindings.TfLiteMicroInterpreterGetOutput(interpreter, 0), index);
    }

    public void Invoke()
    {
        Status = TensorFlowLiteBindings.TfLiteMicroInterpreterInvoke(interpreter);
    }
}