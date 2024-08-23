using System;

namespace Meadow.TensorFlow;

/// <summary>
/// Represents the output of a TensorFlow model, allowing access to individual output tensors.
/// </summary>
/// <typeparam name="T">The data type of the output tensor. Must be either <see cref="float"/> or <see cref="sbyte"/>.</typeparam>
public class ModelOutput<T>
    where T : struct
{
    private readonly Interpreter _interpreter;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelOutput{T}"/> class with the specified interpreter.
    /// </summary>
    /// <param name="interpreter">The interpreter used to process the TensorFlow model.</param>
    internal ModelOutput(Interpreter interpreter)
    {
        _interpreter = interpreter;
    }

    /// <summary>
    /// Gets the number of output tensors in the model.
    /// </summary>
    public int TensorCount => TensorFlowLiteBindings.TfLiteMicroInterpreterGetOutputCount(_interpreter.Handle);

    /// <summary>
    /// Gets the output tensor at the specified index.
    /// </summary>
    /// <param name="index">The index of the output tensor.</param>
    /// <returns>The value of the output tensor at the specified index, cast to the specified type <typeparamref name="T"/>.</returns>
    /// <exception cref="ArgumentException">Thrown when the type <typeparamref name="T"/> is not supported.</exception>
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

    /// <summary>
    /// Gets the float value from the output tensor at the specified index.
    /// </summary>
    /// <param name="index">The index of the output tensor.</param>
    /// <returns>The float value of the output tensor at the specified index.</returns>
    private float GetSingle(int index)
    {
        return TensorFlowLiteBindings.TfLiteMicroGetFloatData(_interpreter.OutputTensor, index);
    }

    /// <summary>
    /// Gets the sbyte value from the output tensor at the specified index.
    /// </summary>
    /// <param name="index">The index of the output tensor.</param>
    /// <returns>The sbyte value of the output tensor at the specified index.</returns>
    private sbyte GetSByte(int index)
    {
        return TensorFlowLiteBindings.TfLiteMicroGeInt8tData(_interpreter.OutputTensor, index);
    }
}
