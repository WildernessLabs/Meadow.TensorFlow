namespace Meadow.TensorFlow;

/// <summary>
/// Represents a model data interface for TensorFlow Lite.
/// </summary>
public interface ITensorModel
{
    /// <summary>
    /// Gets the model data as a byte array.
    /// </summary>
    byte[] Data { get; }

    /// <summary>
    /// Gets the size of the model data.
    /// </summary>
    int Size { get; }
}