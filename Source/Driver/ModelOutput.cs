using System;

namespace Meadow.Foundation.RTLite;

/// <summary>
/// Represents the output of a RTLite model, allowing access to individual output tensors.
/// </summary>
/// <typeparam name="T">The data type of the output tensor. Must be either <see cref="float"/> or <see cref="sbyte"/>.</typeparam>
public class ModelOutput<T>
    where T : struct, IComparable<T>
{
    private readonly Interpreter _interpreter;

    /// <summary>
    /// Initializes a new instance of the <see cref="ModelOutput{T}"/> class with the specified interpreter.
    /// </summary>
    /// <param name="interpreter">The interpreter used to process the RTLite model.</param>
    internal ModelOutput(Interpreter interpreter)
    {
        _interpreter = interpreter;
    }

    /// <summary>
    /// Gets the number of output tensors in the model.
    /// </summary>
    public int TensorCount => Native.TfLiteMicroInterpreterGetOutputCount(_interpreter.Handle);

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
    /// Gets the maximum class and confidence level of the output tensor
    /// </summary>
    /// <param name="tensorLength"></param>
    /// <returns></returns>
    public (int Class, T Confidence) GetMaxElementIndexAndValue(int tensorLength)
    {
        var index = 0;
        T value = this[0];

        // dev note: TensorCount is currently missing from the binding
        for (var i = 0; i < tensorLength; i++)
        {
            if (this[i].CompareTo(value) > 0)
            {
                index = i;
                value = this[i];
            }
        }

        return (index, value);
    }

    /// <summary>
    /// Gets the float value from the output tensor at the specified index.
    /// </summary>
    /// <param name="index">The index of the output tensor.</param>
    /// <returns>The float value of the output tensor at the specified index.</returns>
    private float GetSingle(int index)
    {
        return Native.TfLiteMicroGetFloatData(_interpreter.OutputTensor, index);
    }

    /// <summary>
    /// Gets the sbyte value from the output tensor at the specified index.
    /// </summary>
    /// <param name="index">The index of the output tensor.</param>
    /// <returns>The sbyte value of the output tensor at the specified index.</returns>
    private sbyte GetSByte(int index)
    {
        return Native.TfLiteMicroGeInt8tData(_interpreter.OutputTensor, index);
    }
}
