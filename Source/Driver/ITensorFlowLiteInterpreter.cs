namespace Meadow.TensorFlow;

/// <summary>
/// Interface for TensorFlow Lite interpreter.
/// </summary>
public interface ITensorFlowLiteInterpreter
{
    /// <summary>
    /// Gets the quantization parameters for the input tensor.
    /// </summary>
    TensorFlowLiteQuantizationParams InputQuantizationParams { get; }

    /// <summary>
    /// Gets the quantization parameters for the output tensor.
    /// </summary>
    TensorFlowLiteQuantizationParams OutputQuantizationParams { get; }

    /// <summary>
    /// Gets or sets the status of the last operation performed by the TensorFlow Lite interpreter.
    /// </summary>
    TensorFlowLiteStatus OperationStatus { get; set; }

    /// <summary>
    /// Retrieves the length of the input tensor in elements of type float.
    /// </summary>
    /// <returns>The length of the input tensor.</returns>
    int GetInputTensorLength();

    /// <summary>
    /// Sets the int8 data at the specified index in the input tensor.
    /// </summary>
    /// <param name="index">The index at which to set the data.</param>
    /// <param name="value">The int8 value to set.</param>
    void SetInputTensorInt8Data(int index, sbyte value);

    /// <summary>
    /// Retrieves the int8 data at the specified index from the output tensor.
    /// </summary>
    /// <param name="index">The index from which to retrieve the data.</param>
    /// <returns>The int8 data at the specified index.</returns>
    sbyte GetOutputTensorInt8Data(int index);

    /// <summary>
    /// Sets the float data at the specified index in the input tensor.
    /// </summary>
    /// <param name="index">The index at which to set the data.</param>
    /// <param name="value">The float value to set.</param>
    void SetInputTensorFloatData(int index, float value);

    /// <summary>
    /// Retrieves the float data at the specified index from the output tensor.
    /// </summary>
    /// <param name="index">The index from which to retrieve the data.</param>
    /// <returns>The float data at the specified index.</returns>
    float GetOutputTensorFloatData(int index);

    /// <summary>
    /// Invokes the TensorFlow Lite interpreter for inference.
    /// </summary>
    void InvokeInterpreter();

    /// <summary>
    /// Retrieves the number of output tensors produced by the TensorFlow Lite interpreter.
    /// </summary>
    /// <returns>The number of output tensors.</returns>
    int GetOutputTensorCount();

    /// <summary>
    /// Retrieves the number of input tensors expected by the TensorFlow Lite interpreter.
    /// </summary>
    /// <returns>The number of input tensors.</returns>
    int GetInputTensorCount();

    /// <summary>
    /// Sets a mutable option for TensorFlow Lite interpreter.
    /// </summary>
    /// <param name="option">The option value to set.</param>
    void SetMutableOption(sbyte option);

    /// <summary>
    /// Retrieves the data type of the input tensor.
    /// </summary>
    /// <returns>The data type of the input tensor.</returns>
    TensorDataType GetInputTensorType();

    /// <summary>
    /// Retrieves the size of the dimensions data of the input tensor.
    /// </summary>
    /// <returns>The size of the dimensions data.</returns>
    int GetInputTensorDimensionsSize();

    /// <summary>
    /// Retrieves the dimension data of the input tensor at the specified index.
    /// </summary>
    /// <param name="index">The index of the dimension data.</param>
    /// <returns>The dimension data at the specified index.</returns>
    int GetInputTensorDimension(int index);

    /// <summary>
    /// Retrieves the quantization parameters of the output tensor.
    /// </summary>
    /// <returns>The quantization parameters of the output tensor.</returns>
    TensorFlowLiteQuantizationParams GetOutputTensorQuantizationParams();
}
