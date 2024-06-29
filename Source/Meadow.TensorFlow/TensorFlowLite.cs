using System;
using System.Runtime.InteropServices;

namespace Meadow.TensorFlow;

public class TensorFlowLite
{
    public TensorFlowLiteQuantizationParams InputQuantizationParams { get; private set; }
    public TensorFlowLiteQuantizationParams OutputQuantizationParams { get; private set; }

    public TensorFlowLiteStatus OperationStatus { get; set; } = TensorFlowLiteStatus.Ok;

    protected TensorFlowLiteTensor InputTensor { get; set; }
    protected TensorFlowLiteTensor OutputTensor { get; set; }

    protected readonly IntPtr interpreter;
    private readonly int arenaSize;

    public TensorFlowLite(ITensorModel tensorModel, int arenaSize)
    {
        this.arenaSize = arenaSize;

        IntPtr modelPtr = Marshal.AllocHGlobal(tensorModel.Size * sizeof(int));

        if (modelPtr == IntPtr.Zero)
        {
            throw new Exception("Failed to allocate model memory");
        }

        IntPtr arenaPtr = Marshal.AllocHGlobal(arenaSize * sizeof(int));

        if (arenaPtr == IntPtr.Zero)
        {
            Marshal.FreeHGlobal(modelPtr);
            throw new Exception("Failed to allocate arena memory");
        }

        Marshal.Copy(tensorModel.Data, 0, modelPtr, tensorModel.Size);

        var modelOptionsPtr = TensorFlowLiteBindings.TfLiteMicroGetModel(arenaSize, arenaPtr, modelPtr);
        if (modelOptionsPtr == IntPtr.Zero)
        {
            throw new Exception("Failed to load the model");
        }

        var interpreterOptionsPtr = TensorFlowLiteBindings.TfLiteMicroInterpreterOptionCreate(modelOptionsPtr);
        if (interpreterOptionsPtr == IntPtr.Zero)
        {
            throw new Exception("Failed to create interpreter options");
        }

        interpreter = TensorFlowLiteBindings.TfLiteMicroInterpreterCreate(interpreterOptionsPtr, modelOptionsPtr);
        if (interpreter == IntPtr.Zero)
        {
            throw new Exception("Failed to create interpreter");
        }

        OperationStatus = TensorFlowLiteBindings.TfLiteMicroInterpreterAllocateTensors(interpreter);

        if (OperationStatus != TensorFlowLiteStatus.Ok)
        {
            throw new Exception("Failed to allocate tensors");
        }

        InputTensor = TensorFlowLiteBindings.TfLiteMicroInterpreterGetInput(interpreter, 0);
        OutputTensor = TensorFlowLiteBindings.TfLiteMicroInterpreterGetOutput(interpreter, 0);

        InputQuantizationParams = TensorFlowLiteBindings.TfLiteMicroTensorQuantizationParams(InputTensor);
        OutputQuantizationParams = TensorFlowLiteBindings.TfLiteMicroTensorQuantizationParams(OutputTensor);
    }

    public int GetInputTensorLength()
    {
        return TensorFlowLiteBindings.TfLiteMicroGetByte(InputTensor) / sizeof(float);
    }

    public void SetInputTensorInt8Data(int index, sbyte value)
    {
        TensorFlowLiteBindings.TfLiteMicroSetInt8Data(InputTensor, index, value);
    }

    public sbyte GetOutputTensorInt8Data(int index)
    {
        return TensorFlowLiteBindings.TfLiteMicroGeInt8tData(OutputTensor, index);
    }

    public void SetInputTensorFloatData(int index, float value)
    {
        TensorFlowLiteBindings.TfLiteMicroSetFloatData(InputTensor, index, value);
    }

    public float GetOutputTensorFloatData(int index)
    {
        return TensorFlowLiteBindings.TfLiteMicroGetFloatData(OutputTensor, index);
    }

    public void InvokeInterpreter()
    {
        OperationStatus = TensorFlowLiteBindings.TfLiteMicroInterpreterInvoke(interpreter);
    }

    public int GetOutputTensorCount()
    {
        return TensorFlowLiteBindings.TfLiteMicroInterpreterGetOutputCount(interpreter);
    }

    public int GetInputTensorCount()
    {
        return TensorFlowLiteBindings.TfLiteMicroInterpreterGetInputCount(interpreter);
    }

    public void SetMutableOption(sbyte option)
    {
        OperationStatus = TensorFlowLiteBindings.TfLiteMicroMutableSetOption(option);
    }

    public TensorDataType GetInputTensorType()
    {
        return TensorFlowLiteBindings.TfLiteMicroGetType(InputTensor);
    }

    public int GetInputTensorDimensionsSize()
    {
        return TensorFlowLiteBindings.TfLiteMicroDimsSizeData(InputTensor);
    }

    public int GetInputTensorDimension(int index)
    {
        return TensorFlowLiteBindings.TfLiteMicroDimsData(InputTensor, index);
    }

    public TensorFlowLiteQuantizationParams GetOutputTensorQuantizationParams()
    {
        return TensorFlowLiteBindings.TfLiteMicroTensorQuantizationParams(OutputTensor);
    }
}