using System;

namespace Meadow.TensorFlow;

/// <summary>
/// Structure representing a tensor in TensorFlow Lite.
/// </summary>
public struct TensorFlowLiteTensor
{
    private readonly IntPtr _handle;

    /// <summary>
    /// Constructor of the TensorFlowLiteTensor class.
    /// </summary>
    /// <param name="handle">The tensor handle.</param>
    public TensorFlowLiteTensor(IntPtr handle)
        => _handle = handle;

    /// <summary>
    /// Implicit conversion from IntPtr to TensorFlowLiteTensor.
    /// </summary>
    /// <param name="handle">The tensor handle.</param>
    /// <returns>The tensor corresponding to the handle.</returns>
    public static implicit operator TensorFlowLiteTensor(IntPtr handle)
        => new TensorFlowLiteTensor(handle);

    /// <summary>
    /// Implicit conversion from TensorFlowLiteTensor to IntPtr.
    /// </summary>
    /// <param name="tensor">The tensor to be converted.</param>
    /// <returns>The handle of the tensor.</returns>
    public static implicit operator IntPtr(TensorFlowLiteTensor tensor)
        => tensor._handle;
}