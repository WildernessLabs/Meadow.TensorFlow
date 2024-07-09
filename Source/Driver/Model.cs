using System;
using System.Runtime.InteropServices;

namespace Meadow.TensorFlow;

internal class Model : IDisposable
{
    byte[] _data;

    private GCHandle _handle;
    private IntPtr Handle => _handle.IsAllocated ? _handle.AddrOfPinnedObject() : IntPtr.Zero;

    private IntPtr _arenaHandle;

    private IntPtr _modelOptionsPtr;

    private bool disposedValue;

    public Model(byte[] data, int arenaSize)
    {
        _data = data;
        _handle = GCHandle.Alloc(data, GCHandleType.Pinned);

        _arenaHandle = Marshal.AllocHGlobal(arenaSize * sizeof(int));

        if (_arenaHandle == IntPtr.Zero)
        {
            throw new Exception("Failed to allocate arena memory");
        }

        _modelOptionsPtr = TensorFlowLiteBindings.TfLiteMicroGetModel(arenaSize, _arenaHandle, Handle);
        if (_modelOptionsPtr == IntPtr.Zero)
        {
            throw new Exception("Failed to load the model");
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                if (_handle.IsAllocated)
                {
                    _handle.Free();
                }

                if (_arenaHandle != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(_arenaHandle);
                    _arenaHandle = IntPtr.Zero;
                }

                if (_modelOptionsPtr != IntPtr.Zero)
                {
                    // ToDo!! need bindings to free this 
                    _modelOptionsPtr = IntPtr.Zero;
                }
            }
            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}