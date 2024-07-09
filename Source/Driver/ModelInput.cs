using System;

namespace Meadow.TensorFlow;

public class ModelInput
{
    private readonly TensorSafeHandle _inputTensorHandle;
    private readonly IntPtr _interpreter;

    internal ModelInput(IntPtr interpreter, TensorSafeHandle inputTensorHandle)
    {
        _interpreter = interpreter;
        _inputTensorHandle = inputTensorHandle;
    }

    public int TensorCount => TensorFlowLiteBindings.TfLiteMicroInterpreterGetInputCount(_interpreter);

    public void Set(int index, float value)
    {
        // TODO: validate index
        TensorFlowLiteBindings.TfLiteMicroSetFloatData(_inputTensorHandle, index, value);
    }

    public void Set(int index, sbyte value)
    {
        // TODO: validate index
        TensorFlowLiteBindings.TfLiteMicroSetInt8Data(_inputTensorHandle, index, value);
    }
}
