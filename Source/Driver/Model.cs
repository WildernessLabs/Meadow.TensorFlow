using System;
using System.Runtime.InteropServices;

namespace Meadow.TensorFlow;

public class Model : IDisposable
{
    private byte[] _data;
    private GCHandle _handle;
    private IntPtr _arenaHandle;
    private IntPtr _modelOptionsPtr;
    private IntPtr _interpreterOptionsPtr;
    private IntPtr _interpreter;
    private TensorSafeHandle _inputTensor;
    private TensorSafeHandle _outputTensor;
    private QuantizationParams _inputQuantizationParams;
    private QuantizationParams _outputQuantizationParams;

    private IntPtr Handle => _handle.IsAllocated ? _handle.AddrOfPinnedObject() : IntPtr.Zero;
    public bool IsDisposed { get; private set; }

    public Model(byte[] data, int arenaSize)
    {
        _data = data;
        _handle = GCHandle.Alloc(data, GCHandleType.Pinned);

        Initialize(arenaSize);
    }

    private void Initialize(int arenaSize)
    {
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

        _interpreterOptionsPtr = TensorFlowLiteBindings.TfLiteMicroInterpreterOptionCreate(_modelOptionsPtr);
        if (_interpreterOptionsPtr == IntPtr.Zero)
        {
            throw new Exception("Failed to create interpreter options");
        }

        _interpreter = TensorFlowLiteBindings.TfLiteMicroInterpreterCreate(_interpreterOptionsPtr, _modelOptionsPtr);
        if (_interpreter == IntPtr.Zero)
        {
            throw new Exception("Failed to create interpreter");
        }

        var status = TensorFlowLiteBindings.TfLiteMicroInterpreterAllocateTensors(_interpreter);
        if (status != TensorFlowLiteStatus.Ok)
        {
            throw new Exception("Failed to allocate tensors");
        }

        _inputTensor = TensorFlowLiteBindings.TfLiteMicroInterpreterGetInput(_interpreter, 0);
        _outputTensor = TensorFlowLiteBindings.TfLiteMicroInterpreterGetOutput(_interpreter, 0);

        _inputQuantizationParams = TensorFlowLiteBindings.TfLiteMicroTensorQuantizationParams(_inputTensor);
        _outputQuantizationParams = TensorFlowLiteBindings.TfLiteMicroTensorQuantizationParams(_outputTensor);
    }

    public ModelInput<T> CreateInput<T>()
        where T : struct
    {
        return new ModelInput<T>(_interpreter, _inputTensor);
    }

    public ModelOutput<T> Predict<T>(ModelInput<T> inputs)
        where T : struct
    {
        var status = TensorFlowLiteBindings.TfLiteMicroInterpreterInvoke(_interpreter);

        if (status != TensorFlowLiteStatus.Ok)
        {
            throw new Exception();
        }

        return new ModelOutput<T>(_interpreter, _outputTensor);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!IsDisposed)
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
                    // TODO: MEMORY LEAK need a binding to release this
                    _modelOptionsPtr = IntPtr.Zero;
                }

                if (_interpreterOptionsPtr != IntPtr.Zero)
                {
                    // TODO: MEMORY LEAK need a binding to release this
                    _interpreterOptionsPtr = IntPtr.Zero;
                }
            }
            IsDisposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}