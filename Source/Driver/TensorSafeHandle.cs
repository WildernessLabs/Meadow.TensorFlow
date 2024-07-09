using System;
using System.Runtime.InteropServices;

namespace Meadow.TensorFlow;

/// <summary>
/// A Safe handle for a TensorFlow Lite tensor.
/// </summary>
public class TensorSafeHandle : SafeHandle
{
    private readonly IntPtr _handle;

    /// <summary>
    /// Constructor of the TensorSafeHandle struct.
    /// </summary>
    /// <param name="handle">The tensor handle.</param>
    public TensorSafeHandle(IntPtr handle)
    {
        _handle = handle;
    }

    public override bool IsInvalid => throw new NotImplementedException();

    protected override bool ReleaseHandle()
    {
        //   TensorFlowLiteBindings.
    }

    /// <summary>
    /// Implicit conversion from IntPtr to TensorSafeHandle.
    /// </summary>
    /// <param name="handle">The tensor handle.</param>
    /// <returns>The TensorSafeHandle corresponding to the handle.</returns>
    public static implicit operator TensorSafeHandle(IntPtr handle)
        => new TensorSafeHandle(handle);

    /// <summary>
    /// Implicit conversion from TensorSafeHandle to IntPtr.
    /// </summary>
    /// <param name="tensor">The TensorSafeHandle to be converted.</param>
    /// <returns>The handle of the tensor.</returns>
    public static implicit operator IntPtr(TensorSafeHandle tensor)
        => tensor._handle;
}