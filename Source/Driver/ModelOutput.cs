using System;

namespace Meadow.TensorFlow;

public class ModelOutput<T>
    where T : struct
{
    private readonly TensorSafeHandle _outputTensorHandle;
    private readonly IntPtr _interpreter;

    internal ModelOutput(IntPtr interpreter, TensorSafeHandle outputTensorHandle)
    {
        _interpreter = interpreter;
        _outputTensorHandle = outputTensorHandle;
    }

    public int TensorCount => TensorFlowLiteBindings.TfLiteMicroInterpreterGetOutputCount(_interpreter);

    public T this[int index]
    {
        get
        {
            // TODO: validate index
            if (typeof(T).Equals(typeof(float)))
            {
                return (T)Convert.ChangeType(GetSingle(index), typeof(T));
            }
            else if (typeof(T).Equals(typeof(sbyte)))
            {
                return (T)Convert.ChangeType(GetSByte(index), typeof(T));
            }
            else
            {
                throw new ArgumentException();
            }
        }
    }

    private float GetSingle(int index)
    {
        return TensorFlowLiteBindings.TfLiteMicroGetFloatData(_outputTensorHandle, index);
    }

    private sbyte GetSByte(int index)
    {
        return TensorFlowLiteBindings.TfLiteMicroGeInt8tData(_outputTensorHandle, index);
    }
}
