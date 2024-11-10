using System;

namespace Meadow.Foundation.RTLite;

/// <summary>
/// Structure representing a tensor in TFLite.
/// </summary>
public readonly struct Tensor
{
    private readonly IntPtr _handle;

    /// <summary>
    /// Constructor of the Tensor struct.
    /// </summary>
    /// <param name="handle">The tensor handle.</param>
    public Tensor(IntPtr handle)
        => _handle = handle;

    /// <summary>
    /// Implicit conversion from IntPtr to Tensor.
    /// </summary>
    /// <param name="handle">The tensor handle.</param>
    /// <returns>The Tensor corresponding to the handle.</returns>
    public static implicit operator Tensor(IntPtr handle)
        => new Tensor(handle);

    /// <summary>
    /// Implicit conversion from Tensor to IntPtr.
    /// </summary>
    /// <param name="tensor">The Tensor to be converted.</param>
    /// <returns>The handle of the tensor.</returns>
    public static implicit operator IntPtr(Tensor tensor)
        => tensor._handle;
}
