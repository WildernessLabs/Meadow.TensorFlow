using System;

namespace Meadow.Foundation.RTLite;

/// <summary>
/// Represents RTLite for microcontrollers interpreter.
/// </summary>
internal class Interpreter : IInterpreter, IDisposable
{
    /// <summary>
    /// Gets the quantization parameters for the input tensor.
    /// </summary>
    public QuantizationParams InputQuantizationParams { get; private set; }

    /// <summary>
    /// Gets the quantization parameters for the output tensor.
    /// </summary>
    public QuantizationParams OutputQuantizationParams { get; private set; }

    /// <summary>
    /// Gets or sets the status of the last operation performed by the RTLite interpreter.
    /// </summary>
    public RuntimeStatus OperationStatus { get; set; } = RuntimeStatus.Ok;

    /// <summary>
    /// Gets the input tensor used by the RTLite interpreter.
    /// </summary>
    internal Tensor InputTensor { get; }

    /// <summary>
    /// Gets the output tensor produced by the RTLite interpreter.
    /// </summary>
    internal Tensor OutputTensor { get; private set; }

    internal IntPtr Handle => _interpreterPtr;

    private bool disposedValue;
    private IntPtr _interpreterOptionsPtr;
    private IntPtr _interpreterPtr;

    /// <summary>
    /// Initializes a new instance of the Interpreter class with the specified tensor model and arena size.
    /// </summary>
    /// <exception cref="Exception">Thrown when allocation or initialization fails.</exception>
    public Interpreter(IntPtr modelOptionsPtr)
    {
        _interpreterOptionsPtr = Native.TfLiteMicroInterpreterOptionCreate(modelOptionsPtr);
        if (_interpreterOptionsPtr == IntPtr.Zero)
        {
            throw new RTLiteException("Failed to create interpreter options");
        }

        _interpreterPtr = Native.TfLiteMicroInterpreterCreate(_interpreterOptionsPtr, modelOptionsPtr);
        if (_interpreterPtr == IntPtr.Zero)
        {
            throw new RTLiteException("Failed to create interpreter");
        }

        var status = AllocateTensors();
        if (status != RuntimeStatus.Ok)
        {
            throw new RTLiteException("Failed to allocate tensors", status);
        }

        InputTensor = Native.TfLiteMicroInterpreterGetInput(_interpreterPtr, 0);
        OutputTensor = Native.TfLiteMicroInterpreterGetOutput(_interpreterPtr, 0);

        InputQuantizationParams = Native.TfLiteMicroTensorQuantizationParams(InputTensor);
        OutputQuantizationParams = Native.TfLiteMicroTensorQuantizationParams(OutputTensor);
    }

    /// <summary>
    /// Retrieves the length of the input tensor in elements of type float.
    /// </summary>
    /// <returns>The length of the input tensor.</returns>
    public int GetInputTensorLength()
    {
        return Native.TfLiteMicroGetByte(InputTensor) / sizeof(float);
    }

    /// <summary>
    /// Sets the int8 data at the specified index in the input tensor.
    /// </summary>
    /// <param name="index">The index at which to set the data.</param>
    /// <param name="value">The int8 value to set.</param>
    public void SetInputTensorInt8Data(int index, sbyte value)
    {
        Native.TfLiteMicroSetInt8Data(InputTensor, index, value);
    }

    /// <summary>
    /// Retrieves the int8 data at the specified index from the output tensor.
    /// </summary>
    /// <param name="index">The index from which to retrieve the data.</param>
    /// <returns>The int8 data at the specified index.</returns>
    public sbyte GetOutputTensorInt8Data(int index)
    {
        return Native.TfLiteMicroGeInt8tData(OutputTensor, index);
    }

    /// <summary>
    /// Sets the float data at the specified index in the input tensor.
    /// </summary>
    /// <param name="index">The index at which to set the data.</param>
    /// <param name="value">The float value to set.</param>
    public void SetInputTensorFloatData(int index, float value)
    {
        Native.TfLiteMicroSetFloatData(InputTensor, index, value);
    }

    /// <summary>
    /// Retrieves the float data at the specified index from the output tensor.
    /// </summary>
    /// <param name="index">The index from which to retrieve the data.</param>
    /// <returns>The float data at the specified index.</returns>
    public float GetOutputTensorFloatData(int index)
    {
        return Native.TfLiteMicroGetFloatData(OutputTensor, index);
    }

    /// <summary>
    /// Invokes the RTLite interpreter for inference.
    /// </summary>
    public RuntimeStatus InvokeInterpreter()
    {
        OperationStatus = Native.TfLiteMicroInterpreterInvoke(_interpreterPtr);
        OutputTensor = Native.TfLiteMicroInterpreterGetOutput(_interpreterPtr, 0);

        return OperationStatus;
    }

    /// <summary>
    /// Retrieves the number of output tensors produced by the RTLite interpreter.
    /// </summary>
    /// <returns>The number of output tensors.</returns>
    public int GetOutputTensorCount()
    {
        return Native.TfLiteMicroInterpreterGetOutputCount(_interpreterPtr);
    }

    /// <summary>
    /// Retrieves the number of input tensors expected by the RTLite interpreter.
    /// </summary>
    /// <returns>The number of input tensors.</returns>
    public int GetInputTensorCount()
    {
        return Native.TfLiteMicroInterpreterGetInputCount(_interpreterPtr);
    }

    /// <summary>
    /// Sets a mutable option for RTLite interpreter.
    /// </summary>
    /// <param name="option">The option value to set.</param>
    public void SetMutableOption(sbyte option)
    {
        OperationStatus = Native.TfLiteMicroMutableSetOption(option);
    }

    /// <summary>
    /// Retrieves the data type of the input tensor.
    /// </summary>
    /// <returns>The data type of the input tensor.</returns>
    public TensorDataType GetInputTensorType()
    {
        return Native.TfLiteMicroGetType(InputTensor);
    }

    /// <summary>
    /// Retrieves the size of the dimensions data of the input tensor.
    /// </summary>
    /// <returns>The size of the dimensions data.</returns>
    public int GetInputTensorDimensionsSize()
    {
        return Native.TfLiteMicroDimsSizeData(InputTensor);
    }

    /// <summary>
    /// Retrieves the size of the dimensions data of the output tensor.
    /// </summary>
    /// <returns>The size of the dimensions data.</returns>

    public int GetOutputTensorDimensionsSize()
    {
        return Native.TfLiteMicroDimsSizeData(OutputTensor);
    }

    /// <summary>
    /// Retrieves the dimension data of the input tensor at the specified index.
    /// </summary>
    /// <param name="index">The index of the dimension data.</param>
    /// <returns>The dimension data at the specified index.</returns>
    public int GetInputTensorDimension(int index)
    {
        return Native.TfLiteMicroDimsData(InputTensor, index);
    }

    /// <summary>
    /// Retrieves the dimension data of the output tensor at the specified index.
    /// </summary>
    /// <param name="index">The index of the dimension data.</param>
    /// <returns>The dimension data at the specified index.</returns>

    public int GetOutputTensorDimension(int index)
    {
        return Native.TfLiteMicroDimsData(OutputTensor, index);
    }
    /// <summary>
    /// Retrieves the quantization parameters of the output tensor.
    /// </summary>
    /// <returns>The quantization parameters of the output tensor.</returns>
    public QuantizationParams GetOutputTensorQuantizationParams()
    {
        return Native.TfLiteMicroTensorQuantizationParams(OutputTensor);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // TODO: dispose managed state (managed objects)
            }

            if (_interpreterOptionsPtr != IntPtr.Zero)
            {
                Native.TfLiteMicroInterpreterOptionDelete(_interpreterOptionsPtr);
                _interpreterOptionsPtr = IntPtr.Zero;
            }

            if (_interpreterPtr != IntPtr.Zero)
            {
                Native.TfLiteMicroInterpreterDelete(_interpreterPtr);
                _interpreterPtr = IntPtr.Zero;
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public RuntimeStatus AllocateTensors()
    {
        return OperationStatus = Native.TfLiteMicroInterpreterAllocateTensors(_interpreterPtr);
    }
}