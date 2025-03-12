using System;
using System.Runtime.InteropServices;

namespace Meadow.Foundation.RTLite;

internal class ModelSafeHandle<T> : SafeHandle
    where T : struct, IComparable<T>
{
    private readonly IntPtr _handle;

    public override bool IsInvalid => handle == IntPtr.Zero;

    public ModelSafeHandle(ITensorModel<T> tensorModel)
         : base(IntPtr.Zero, true) // Initialize base SafeHandle with IntPtr.Zero and ownsHandle = true
    {
        _handle = Marshal.AllocHGlobal(tensorModel.Size * Marshal.SizeOf<T>());
        SetHandle(_handle); // Set the handle value for SafeHandle
    }

    protected override bool ReleaseHandle()
    {
        if (!IsInvalid)
        {
            Marshal.FreeHGlobal(handle); // Free unmanaged memory
            return true;
        }
        return false;
    }
}