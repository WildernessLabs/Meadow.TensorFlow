using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Meadow.Foundation.RTLite;

/// <summary>
/// Represents a RTLite model.
/// </summary>
public abstract class Model<T> : ITensorModel<T>, IDisposable
    where T : struct, IComparable<T>
{
    private byte[] _data;
    private GCHandle _handle;
    private IntPtr _arenaHandle;
    private IntPtr _modelOptionsPtr;
    private Interpreter _interpreter;

    /// <summary>
    /// Gets the quantization parameters for the input tensor.
    /// </summary>
    public QuantizationParams InputQuantizationParams { get; private set; }

    /// <summary>
    /// Gets the quantization parameters for the output tensor.
    /// </summary>
    public QuantizationParams OutputQuantizationParams { get; private set; }

    /// <summary>
    /// Gets a value indicating whether the model is disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <inheritdoc />
    public int Size => _data.Length; //ToDo verify this

    /// <summary>
    /// The input tensor for the model.
    /// </summary>
    public ModelInput<T> Inputs { get; private set; }

    private IntPtr Handle => _handle.IsAllocated ? _handle.AddrOfPinnedObject() : IntPtr.Zero;

    public Model(FileInfo modelFile, int arenaSize)
    {
        Console.WriteLine($"Loading file {modelFile.Length} bytes");

        var buffer = new byte[modelFile.Length];

        using var stream = modelFile.OpenRead();
        stream.Read(buffer, 0, buffer.Length);

        Initialize(buffer, arenaSize);
    }

    /// <summary>
    /// Initializes a new instance of the Model class with the specified model data and arena size.
    /// </summary>
    /// <param name="data">The model data.</param>
    /// <param name="arenaSize">The size of the arena for the interpreter.</param>
    public Model(byte[] data, int arenaSize)
    {
        Initialize(data, arenaSize);
    }

    private void Initialize(byte[] data, int arenaSize)
    {
        _data = data;

        _handle = GCHandle.Alloc(data, GCHandleType.Pinned);

        _arenaHandle = Marshal.AllocHGlobal(arenaSize * sizeof(int));

        if (_arenaHandle == IntPtr.Zero)
        {
            throw new Exception("Failed to allocate arena memory");
        }

        _modelOptionsPtr = Native.TfLiteMicroGetModel(arenaSize, _arenaHandle, Handle);
        if (_modelOptionsPtr == IntPtr.Zero)
        {
            throw new Exception("Failed to load the model");
        }

        _interpreter = new Interpreter(_modelOptionsPtr);

        InputQuantizationParams = _interpreter.InputQuantizationParams;
        OutputQuantizationParams = _interpreter.OutputQuantizationParams;

        Inputs = new ModelInput<T>(_interpreter);
    }

    /// <summary>
    /// Makes a prediction based on the provided input tensor.
    /// </summary>
    /// <returns>A <see cref="ModelOutput{T}"/> representing the output tensor.</returns>
    /// <exception cref="Exception">Thrown when the interpreter invocation fails.</exception>
    public ModelOutput<T> Predict()
    {
        var status = _interpreter.InvokeInterpreter();

        if (status != RuntimeStatus.Ok)
        {
            throw new Exception();
        }

        return new ModelOutput<T>(_interpreter);
    }

    /// <summary>
    /// Releases the resources used by the Model class.
    /// </summary>
    /// <param name="disposing">A boolean value indicating whether the method is being called from the Dispose method.</param>
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

                _interpreter?.Dispose();

                if (_arenaHandle != IntPtr.Zero)
                {
                    Marshal.FreeHGlobal(_arenaHandle);
                    _arenaHandle = IntPtr.Zero;
                }

                if (_modelOptionsPtr != IntPtr.Zero)
                {
                    Native.TfLiteMicroModelDelete(_modelOptionsPtr);
                    _modelOptionsPtr = IntPtr.Zero;
                }
            }
            IsDisposed = true;
        }
    }

    /// <summary>
    /// Releases the resources used by the Model class.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}