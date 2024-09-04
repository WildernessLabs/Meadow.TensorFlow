namespace Meadow.TensorFlow;

/// <summary>
/// Represents a tensor model interface for creating inputs and making predictions.
/// </summary>
public interface ITensorModel<T>
    where T : struct
{
    /// <summary>
    /// Makes a prediction based on the provided input tensor.
    /// </summary>
    /// <returns>A <see cref="ModelOutput{T}"/> representing the output tensor.</returns>
    ModelOutput<T> Predict();

    /// <summary>
    /// Retrieves the length of the input tensor.
    /// </summary>
    int Size { get; }
}