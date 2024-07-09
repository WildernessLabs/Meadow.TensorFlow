using System;

namespace Meadow.TensorFlow;

public class ModelOutput
{
    private readonly TensorSafeHandle _outputTensorHandle;
    private readonly IntPtr _interpreter;

    internal ModelOutput(IntPtr interpreter, TensorSafeHandle outputTensorHandle)
    {
        _interpreter = interpreter;
        _outputTensorHandle = outputTensorHandle;
    }

    public int TensorCount => TensorFlowLiteBindings.TfLiteMicroInterpreterGetOutputCount(_interpreter);

    public float GetSingle(int index)
    {
        return TensorFlowLiteBindings.TfLiteMicroGetFloatData(_outputTensorHandle, index);
    }

    public sbyte GetSByte(int index)
    {
        return TensorFlowLiteBindings.TfLiteMicroGeInt8tData(_outputTensorHandle, index);
    }
}
