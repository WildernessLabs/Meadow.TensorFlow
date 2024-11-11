namespace Meadow.Foundation.RTLite;


/// <summary>
/// Struct representing the parameters for asymmetric quantization in RTLite.
/// </summary>
public struct QuantizationParams
{
    /// <summary>
    /// Gets or sets the scale factor applied to quantized values.
    /// </summary>
    public float Scale;

    /// <summary>
    /// Gets or sets the zero point offset applied to quantized values.
    /// </summary>
    public int ZeroPoint;
};