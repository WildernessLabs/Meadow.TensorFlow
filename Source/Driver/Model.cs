using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Meadow.TensorFlow;

/// <summary>
/// Represents a TensorFlow Lite model.
/// </summary>
public class Model : ITensorModel, IDisposable
{
    private byte[] _data;
    private GCHandle _handle;
    private IntPtr _arenaHandle;
    private IntPtr _modelOptionsPtr;
    private Interpreter _interpreter;

    public QuantizationParams InputQuantizationParams { get; }
    public QuantizationParams OutputQuantizationParams { get; }

    /// <summary>
    /// Gets a value indicating whether the model is disposed.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <inheritdoc />
    public int Size => _data.Length; //ToDo verify this

    private IntPtr Handle => _handle.IsAllocated ? _handle.AddrOfPinnedObject() : IntPtr.Zero;

    /// <summary>
    /// Initializes a new instance of the <see cref="Model"/> class with the specified model data and arena size.
    /// </summary>
    /// <param name="data">The model data.</param>
    /// <param name="arenaSize">The size of the arena for the interpreter.</param>
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

        _interpreter = new Interpreter(_modelOptionsPtr);

        var status = TensorFlowLiteBindings.TfLiteMicroInterpreterAllocateTensors(_interpreter.Handle);
        if (status != TensorFlowLiteStatus.Ok)
        {
            throw new Exception("Failed to allocate tensors");
        }

        InputQuantizationParams = TensorFlowLiteBindings.TfLiteMicroTensorQuantizationParams(_interpreter.InputTensor);
        OutputQuantizationParams = TensorFlowLiteBindings.TfLiteMicroTensorQuantizationParams(_interpreter.OutputTensor);
    }

    /// <summary>
    /// Creates an input tensor for the model.
    /// </summary>
    /// <typeparam name="T">The type of the input tensor elements.</typeparam>
    /// <returns>A <see cref="ModelInput{T}"/> representing the input tensor.</returns>
    public ModelInput<T> CreateInput<T>(IEnumerable<T> inputs)
        where T : struct
    {
        return new ModelInput<T>(_interpreter, inputs);
    }

    /// <summary>
    /// Creates an input tensor for the model.
    /// </summary>
    /// <typeparam name="T">The type of the input tensor elements.</typeparam>
    /// <returns>A <see cref="ModelInput{T}"/> representing the input tensor.</returns>
    public ModelInput<T> CreateInput<T>()
        where T : struct
    {
        return new ModelInput<T>(_interpreter);
    }

    /// <summary>
    /// Makes a prediction based on the provided input tensor.
    /// </summary>
    /// <typeparam name="T">The type of the input tensor elements.</typeparam>
    /// <param name="inputs">The input tensor.</param>
    /// <returns>A <see cref="ModelOutput{T}"/> representing the output tensor.</returns>
    /// <exception cref="Exception">Thrown when the interpreter invocation fails.</exception>
    public ModelOutput<T> Predict<T>(ModelInput<T> inputs)
        where T : struct
    {
        var status = TensorFlowLiteBindings.TfLiteMicroInterpreterInvoke(_interpreter.Handle);

        if (status != TensorFlowLiteStatus.Ok)
        {
            throw new Exception();
        }

        return new ModelOutput<T>(_interpreter);
    }

    /// <summary>
    /// Releases the resources used by the <see cref="Model"/> class.
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
                    TensorFlowLiteBindings.TfLiteMicroModelDelete(_modelOptionsPtr);
                    _modelOptionsPtr = IntPtr.Zero;
                }
            }
            IsDisposed = true;
        }
    }

    /// <summary>
    /// Releases the resources used by the <see cref="Model"/> class.
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
