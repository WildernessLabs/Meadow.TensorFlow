using System;
using System.Collections.Generic;

namespace Meadow.TensorFlow;

public class ModelInput<T>
    where T : struct
{
    private readonly Interpreter _interpreter;

    internal ModelInput(Interpreter interpreter, IEnumerable<T> inputs)
        : this(interpreter)
    {
        var i = 0;

        foreach (var input in inputs)
        {
            this[i] = input;
            i++;
        }
    }

    internal ModelInput(Interpreter interpreter)
    {
        _interpreter = interpreter;
    }

    public int Length => TensorFlowLiteBindings.TfLiteMicroInterpreterGetInputCount(_interpreter.Handle);

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
        TensorFlowLiteBindings.TfLiteMicroSetFloatData(_interpreter.InputTensor, index, value);
    }

    private void Set(int index, sbyte value)
    {
        // TODO: validate index
        TensorFlowLiteBindings.TfLiteMicroSetInt8Data(_interpreter.InputTensor, index, value);
    }
}
