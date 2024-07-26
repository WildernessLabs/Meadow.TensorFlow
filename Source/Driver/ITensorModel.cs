namespace Meadow.TensorFlow;

/// <summary>
/// Represents a tensor model interface for creating inputs and making predictions.
/// </summary>
public interface ITensorModel
{
    /// <summary>
    /// Creates an input tensor of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the input tensor elements.</typeparam>
    /// <returns>A <see cref="ModelInput{T}"/> representing the input tensor.</returns>
    ModelInput<T> CreateInput<T>() where T : struct;

    /// <summary>
    /// Makes a prediction based on the provided input tensor.
    /// </summary>
    /// <typeparam name="T">The type of the input tensor elements.</typeparam>
    /// <param name="inputs">The input tensor.</param>
    /// <returns>A <see cref="ModelOutput{T}"/> representing the output tensor.</returns>
    ModelOutput<T> Predict<T>(ModelInput<T> inputs) where T : struct;

    /// <summary>
    /// Retrieves the length of the input tensor.
    /// </summary>
    int Size { get; }
}