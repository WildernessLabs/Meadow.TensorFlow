using System;
using System.Collections.Generic;
using System.Text;


namespace TensorFlow.litemicro
{
    /// <summary>
    /// Enum representing the types supported by tensor in TensorFlow Lite.
    /// </summary>
    public enum TfLiteType
    {

        /// <summary>
        /// Type not defined.
        /// </summary>
        kTfLiteNoType = 0,

        /// <summary>
        /// Floating-point type with regular size of 4 bytes.
        /// </summary>
        kTfLiteFloat32 = 1,

        /// <summary>
        /// Integer type with regular size of 4 bytes.
        /// </summary>
        kTfLiteInt32 = 2,

        /// <summary>
        /// Unsigned integer type with regular size of 1 bytes.
        /// </summary>
        kTfLiteUInt8 = 3,

        /// <summary>
        /// Integer type with regular size of 8 bytes.
        /// </summary>
        kTfLiteInt64 = 4,

        /// <summary>
        /// String type.
        /// </summary>
        kTfLiteString = 5,

        /// <summary>
        /// Boolean type.
        /// </summary>
        kTfLiteBool = 6,

        /// <summary>
        /// Integer type with regular size of 2 bytes.
        /// </summary>
        kTfLiteInt16 = 7,

        /// <summary>
        /// Single-presion complext type.
        /// </summary>
        kTfLiteComplex64 = 8,

        /// <summary>
        /// Integer type with regular size of 1 bytes.
        /// </summary>
        kTfLiteInt8 = 9,

        /// <summary>
        /// Floating-point type with regular size of 2 bytes.
        /// </summary>
        kTfLiteFloat16 = 10,

        /// <summary>
        /// Floating-point type with regular size of 4 bytes.
        /// </summary>
        kTfLiteFloat64 = 11,

        /// <summary>
        /// Double-precision comple type.
        /// </summary>
        kTfLiteComplex128 = 12,

        /// <summary>
        /// Unsigned integer type with regular size of 8 bytes.
        /// </summary>
        kTfLiteUInt64 = 13,

        /// <summary>
        /// Resource type.
        /// </summary>
        kTfLiteResource = 14,

        /// <summary>
        /// Variant type, identifies conversion image objects in a variant.
        /// </summary>
        kTfLiteVariant = 15,

        /// <summary>
        /// Unsigned integer type with regular size of 4 bytes.
        /// </summary>
        kTfLiteUInt32 = 16,
    }
}