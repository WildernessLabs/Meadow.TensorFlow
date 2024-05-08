using System;
using System.Collections;

namespace TensorFlow.litemicro
{
    /// <summary>
    /// Struct representing the parameters for asymmetric quantization in TensorFlow Lite .
    /// </summary>
    public struct TfLiteQuantizationParams
    {
        /// <summary>
        /// Scale defined to a quantized value
        /// </summary>
        public float scale;

        /// <summary>
        /// Zero point to quantized value.
        /// </summary>
        public int zero_point;
    };
}