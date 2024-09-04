using System;
using System.Collections.Generic;

namespace Meadow.TensorFlow;

/// <summary>
/// Represents the input of a TensorFlow model, allowing setting of individual input tensors.
/// </summary>
/// <typeparam name="T">The data type of the input tensor. Must be either <see cref="float"/> or <see cref="sbyte"/>.</typeparam>
public class ModelInput<T>
    where T : struct
{
    private readonly Interpreter _interpreter;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelInput{T}"/> class with the specified interpreter and inputs.
    /// </summary>
    /// <param name="interpreter">The interpreter used to process the TensorFlow model.</param>
    /// <param name="inputs">The initial input values to set in the model.</param>
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

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelInput{T}"/> class with the specified interpreter.
    /// </summary>
    /// <param name="interpreter">The interpreter used to process the TensorFlow model.</param>
    internal ModelInput(Interpreter interpreter)
    {
        _interpreter = interpreter;
    }

    /// <summary>
    /// Gets the number of input tensors in the model.
    /// </summary>
    public int Length => TensorFlowLiteBindings.TfLiteMicroInterpreterGetInputCount(_interpreter.Handle);

    /// <summary>
    /// Sets the input tensor value at the specified index.
    /// </summary>
    /// <param name="index">The index of the input tensor.</param>
    /// <param name="value">The value to set at the specified index, cast to the specified type <typeparamref name="T"/>.</param>
    /// <exception cref="ArgumentException">Thrown when the type <typeparamref name="T"/> is not supported.</exception>
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

    /// <summary>
    /// Sets the input tensor values from the provided enumerable of inputs.
    /// </summary>
    /// <param name="inputs">The input values to set in the model.</param>
    public void SetData(IEnumerable<T> inputs)
    {
        var i = 0;

        foreach (var input in inputs)
        {
            this[i] = input;
            i++;
        }
    }

    /// <summary>
    /// Sets the float value at the specified index in the input tensor.
    /// </summary>
    /// <param name="index">The index of the input tensor.</param>
    /// <param name="value">The float value to set at the specified index.</param>
    private void Set(int index, float value)
    {
        // TODO: validate index
        TensorFlowLiteBindings.TfLiteMicroSetFloatData(_interpreter.InputTensor, index, value);
    }

    /// <summary>
    /// Sets the sbyte value at the specified index in the input tensor.
    /// </summary>
    /// <param name="index">The index of the input tensor.</param>
    /// <param name="value">The sbyte value to set at the specified index.</param>
    private void Set(int index, sbyte value)
    {
        // TODO: validate index
        TensorFlowLiteBindings.TfLiteMicroSetInt8Data(_interpreter.InputTensor, index, value);
    }
}
