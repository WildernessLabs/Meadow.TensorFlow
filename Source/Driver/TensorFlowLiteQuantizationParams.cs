namespace Meadow.TensorFlow
{
    /// <summary>
    /// Struct representing the parameters for asymmetric quantization in TensorFlow Lite.
    /// </summary>
    public struct TensorFlowLiteQuantizationParams
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
}