using System;

namespace TensorFlow.litemicro
{
    /// <summary>
    /// Structure representing a tensor in TensorFlow Lite.
    /// </summary>
    public struct TfLiteTensor
    {
        private IntPtr _handle;

        /// <summary>
        /// Constructor of the TfLiteTensor class.
        /// </summary>
        /// <param name="handle">The tensor handle.</param>
        public TfLiteTensor(IntPtr handle)
            => _handle = handle;

        /// <summary>
        /// Implicit conversion from IntPtr to TfLiteTensor.
        /// </summary>
        /// <param name="handle">The tensor handle.</param>
        /// <returns>The tensor corresponding to the handle.</returns>
        public static implicit operator TfLiteTensor(IntPtr handle)
            => new TfLiteTensor(handle);

        /// <summary>
        /// Implicit conversion from TfLiteTensor to IntPtr.
        /// </summary>
        /// <param name="tensor">The tensor to be converted.</param>
        /// <returns>The handle of the tensor.</returns>
        public static implicit operator IntPtr(TfLiteTensor tensor)
            => tensor._handle;
    }
}