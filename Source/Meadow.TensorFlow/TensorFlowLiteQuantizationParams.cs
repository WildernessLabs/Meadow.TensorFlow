namespace Meadow.TensorFlow;

/// <summary>
/// Struct representing the parameters for asymmetric quantization in TensorFlow Lite.
/// </summary>
public struct TensorFlowLiteQuantizationParams
{
    /// <summary>
    /// Scale defined to a quantized value
    /// </summary>
    public float Scale;

    /// <summary>
    /// Zero point to quantized value.
    /// </summary>
    public int ZeroPoint;
};