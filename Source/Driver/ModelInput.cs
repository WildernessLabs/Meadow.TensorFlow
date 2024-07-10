using System;

namespace Meadow.TensorFlow;

public class ModelInput<T>
    where T : struct
{
    private readonly TensorSafeHandle _inputTensorHandle;
    private readonly IntPtr _interpreter;

    internal ModelInput(IntPtr interpreter, TensorSafeHandle inputTensorHandle)
    {
        _interpreter = interpreter;
        _inputTensorHandle = inputTensorHandle;
    }

    public int Length => TensorFlowLiteBindings.TfLiteMicroInterpreterGetInputCount(_interpreter);

    public T this[int index]
    {
        set
        {
            // TODO: validate index
            if (typeof(T).Equals(typeof(float)))
            {
                Set(index, Convert.ToSingle(value));
            }
            else if (typeof(T).Equals(typeof(sbyte)))
            {
                Set(index, Convert.ToSByte(value));
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }

    private void Set(int index, float value)
    {
        // TODO: validate index
        TensorFlowLiteBindings.TfLiteMicroSetFloatData(_inputTensorHandle, index, value);
    }

    private void Set(int index, sbyte value)
    {
        // TODO: validate index
        TensorFlowLiteBindings.TfLiteMicroSetInt8Data(_inputTensorHandle, index, value);
    }
}
