using System;

namespace Meadow.TensorFlow;

/// <summary>
/// Represents TensorFlow Lite for microcontrollers interpreter.
/// </summary>
internal class Interpreter : ITensorFlowLiteInterpreter, IDisposable
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
    /// Gets or sets the status of the last operation performed by the TensorFlow Lite interpreter.
    /// </summary>
    public TensorFlowLiteStatus OperationStatus { get; set; } = TensorFlowLiteStatus.Ok;

    /// <summary>
    /// Gets the input tensor used by the TensorFlow Lite interpreter.
    /// </summary>
    internal TensorSafeHandle InputTensor { get; }

    /// <summary>
    /// Gets the output tensor produced by the TensorFlow Lite interpreter.
    /// </summary>
    internal TensorSafeHandle OutputTensor { get; }

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
        _interpreterOptionsPtr = TensorFlowLiteBindings.TfLiteMicroInterpreterOptionCreate(modelOptionsPtr);
        if (_interpreterOptionsPtr == IntPtr.Zero)
        {
            throw new Exception("Failed to create interpreter options");
        }

        _interpreterPtr = TensorFlowLiteBindings.TfLiteMicroInterpreterCreate(_interpreterOptionsPtr, modelOptionsPtr);
        if (_interpreterPtr == IntPtr.Zero)
        {
            throw new Exception("Failed to create interpreter");
        }

        InputTensor = TensorFlowLiteBindings.TfLiteMicroInterpreterGetInput(_interpreterPtr, 0);
        OutputTensor = TensorFlowLiteBindings.TfLiteMicroInterpreterGetOutput(_interpreterPtr, 0);
    }

    /// <summary>
    /// Retrieves the length of the input tensor in elements of type float.
    /// </summary>
    /// <returns>The length of the input tensor.</returns>
    public int GetInputTensorLength()
    {
        return TensorFlowLiteBindings.TfLiteMicroGetByte(InputTensor) / sizeof(float);
    }

    /// <summary>
    /// Sets the int8 data at the specified index in the input tensor.
    /// </summary>
    /// <param name="index">The index at which to set the data.</param>
    /// <param name="value">The int8 value to set.</param>
    public void SetInputTensorInt8Data(int index, sbyte value)
    {
        TensorFlowLiteBindings.TfLiteMicroSetInt8Data(InputTensor, index, value);
    }

    /// <summary>
    /// Retrieves the int8 data at the specified index from the output tensor.
    /// </summary>
    /// <param name="index">The index from which to retrieve the data.</param>
    /// <returns>The int8 data at the specified index.</returns>
    public sbyte GetOutputTensorInt8Data(int index)
    {
        return TensorFlowLiteBindings.TfLiteMicroGeInt8tData(OutputTensor, index);
    }

    /// <summary>
    /// Sets the float data at the specified index in the input tensor.
    /// </summary>
    /// <param name="index">The index at which to set the data.</param>
    /// <param name="value">The float value to set.</param>
    public void SetInputTensorFloatData(int index, float value)
    {
        TensorFlowLiteBindings.TfLiteMicroSetFloatData(InputTensor, index, value);
    }

    /// <summary>
    /// Retrieves the float data at the specified index from the output tensor.
    /// </summary>
    /// <param name="index">The index from which to retrieve the data.</param>
    /// <returns>The float data at the specified index.</returns>
    public float GetOutputTensorFloatData(int index)
    {
        return TensorFlowLiteBindings.TfLiteMicroGetFloatData(OutputTensor, index);
    }

    /// <summary>
    /// Invokes the TensorFlow Lite interpreter for inference.
    /// </summary>
    public TensorFlowLiteStatus InvokeInterpreter()
    {
        return OperationStatus = TensorFlowLiteBindings.TfLiteMicroInterpreterInvoke(_interpreterPtr);
    }

    /// <summary>
    /// Retrieves the number of output tensors produced by the TensorFlow Lite interpreter.
    /// </summary>
    /// <returns>The number of output tensors.</returns>
    public int GetOutputTensorCount()
    {
        return TensorFlowLiteBindings.TfLiteMicroInterpreterGetOutputCount(_interpreterPtr);
    }

    /// <summary>
    /// Retrieves the number of input tensors expected by the TensorFlow Lite interpreter.
    /// </summary>
    /// <returns>The number of input tensors.</returns>
    public int GetInputTensorCount()
    {
        return TensorFlowLiteBindings.TfLiteMicroInterpreterGetInputCount(_interpreterPtr);
    }

    /// <summary>
    /// Sets a mutable option for TensorFlow Lite interpreter.
    /// </summary>
    /// <param name="option">The option value to set.</param>
    public void SetMutableOption(sbyte option)
    {
        OperationStatus = TensorFlowLiteBindings.TfLiteMicroMutableSetOption(option);
    }

    /// <summary>
    /// Retrieves the data type of the input tensor.
    /// </summary>
    /// <returns>The data type of the input tensor.</returns>
    public TensorDataType GetInputTensorType()
    {
        return TensorFlowLiteBindings.TfLiteMicroGetType(InputTensor);
    }

    /// <summary>
    /// Retrieves the size of the dimensions data of the input tensor.
    /// </summary>
    /// <returns>The size of the dimensions data.</returns>
    public int GetInputTensorDimensionsSize()
    {
        return TensorFlowLiteBindings.TfLiteMicroDimsSizeData(InputTensor);
    }

    /// <summary>
    /// Retrieves the size of the dimensions data of the output tensor.
    /// </summary>
    /// <returns>The size of the dimensions data.</returns>

    public int GetOutputTensorDimensionsSize()
    {
        return TensorFlowLiteBindings.TfLiteMicroDimsSizeData(OutputTensor);
    }

    /// <summary>
    /// Retrieves the dimension data of the input tensor at the specified index.
    /// </summary>
    /// <param name="index">The index of the dimension data.</param>
    /// <returns>The dimension data at the specified index.</returns>
    public int GetInputTensorDimension(int index)
    {
        return TensorFlowLiteBindings.TfLiteMicroDimsData(InputTensor, index);
    }

    /// <summary>
    /// Retrieves the dimension data of the output tensor at the specified index.
    /// </summary>
    /// <param name="index">The index of the dimension data.</param>
    /// <returns>The dimension data at the specified index.</returns>

    public int GetOutputTensorDimension(int index)
    {
        return TensorFlowLiteBindings.TfLiteMicroDimsData(OutputTensor, index);
    }
    /// <summary>
    /// Retrieves the quantization parameters of the output tensor.
    /// </summary>
    /// <returns>The quantization parameters of the output tensor.</returns>
    public QuantizationParams GetOutputTensorQuantizationParams()
    {
        return TensorFlowLiteBindings.TfLiteMicroTensorQuantizationParams(OutputTensor);
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
                TensorFlowLiteBindings.TfLiteMicroInterpreterOptionDelete(_interpreterOptionsPtr);
                _interpreterOptionsPtr = IntPtr.Zero;
            }

            if (_interpreterPtr != IntPtr.Zero)
            {
                TensorFlowLiteBindings.TfLiteMicroInterpreterDelete(_interpreterPtr);
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

    public TensorFlowLiteStatus AllocateTensors()
    {
        return OperationStatus = TensorFlowLiteBindings.TfLiteMicroInterpreterAllocateTensors(_interpreterPtr);
    }
}