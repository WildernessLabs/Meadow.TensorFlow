
using System;
using System.Runtime.InteropServices;

namespace Meadow.TensorFlow;

/// <summary>
/// Provides C API bindings for TensorFlow Lite for Microcontrollers
/// </summary>
public static class TensorFlowLiteBindings
{
    /// <summary>
    /// The name of the TensorFlow Lite library.
    /// </summary>
    private const string TensorFlowLibName = "TensorFlow.so";

    /// <summary>
    /// Get the model from TensorFlow Lite Micro.
    /// </summary>
    /// <param name="arenaSize">The size of the arena.</param>
    /// <param name="arena">The arena.</param>
    /// <param name="modelData">The model data.</param>
    /// <returns>A pointer to the model.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern IntPtr TfLiteMicroGetModel(int arenaSize, IntPtr arena, IntPtr modelData);

    /// <summary>
    /// Creates an interpreter option for TensorFlow Lite Micro.
    /// </summary>
    /// <param name="option">The interpreter option.</param>
    /// <returns>A pointer to the interpreter option.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern IntPtr TfLiteMicroInterpreterOptionCreate(IntPtr option);

    /// <summary>
    /// Creates a TensorFlow Lite Micro interpreter.
    /// </summary>
    /// <param name="interpreterOption">The interpreter option.</param>
    /// <param name="modelOption">The model option.</param>
    /// <returns>A pointer to the interpreter.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern IntPtr TfLiteMicroInterpreterCreate(IntPtr interpreterOption, IntPtr modelOption);

    /// <summary>
    /// Allocates tensors for the TensorFlow Lite Micro interpreter.
    /// </summary>
    /// <param name="interpreter">The interpreter.</param>
    /// <returns>OperationStatus of the tensor allocation.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern TensorFlowLiteStatus TfLiteMicroInterpreterAllocateTensors(IntPtr interpreter);

    /// <summary>
    /// Get an InputTensor tensor from the TensorFlow Lite Micro interpreter.
    /// </summary>
    /// <param name="interpreter">The interpreter.</param>
    /// <param name="index">The index of the InputTensor tensor.</param>
    /// <returns>The InputTensor tensor.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern TensorFlowLiteTensor TfLiteMicroInterpreterGetInput(IntPtr interpreter, int index);

    /// <summary>
    /// Get an OutputTensor tensor from the TensorFlow Lite Micro interpreter.
    /// </summary>
    /// <param name="interpreter">The interpreter.</param>
    /// <param name="index">The index of the OutputTensor tensor.</param>
    /// <returns>The OutputTensor tensor.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern TensorFlowLiteTensor TfLiteMicroInterpreterGetOutput(IntPtr interpreter, int index);

    /// <summary>
    /// Get the number of OutputTensor tensors from the TensorFlow Lite Micro interpreter.
    /// </summary>
    /// <param name="interpreter">The interpreter.</param>
    /// <returns>The number of OutputTensor tensors.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern int TfLiteMicroInterpreterGetOutputCount(IntPtr interpreter);

    /// <summary>
    /// Get the number of InputTensor tensors from the TensorFlow Lite Micro interpreter.
    /// </summary>
    /// <param name="interpreter">The interpreter.</param>
    /// <returns>The number of InputTensor tensors.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern int TfLiteMicroInterpreterGetInputCount(IntPtr interpreter);

    /// <summary>
    /// Invokes the TensorFlow Lite Micro interpreter for inference.
    /// </summary>
    /// <param name="interpreter">The interpreter.</param>
    /// <returns>OperationStatus of the interpreter invocation.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern TensorFlowLiteStatus TfLiteMicroInterpreterInvoke(IntPtr interpreter);

    /// <summary>
    /// Retrieves the int8 data from a tensor at the specified index.
    /// </summary>
    /// <param name="tensor">The tensor containing the int8 data.</param>
    /// <param name="index">The index of the int8 data.</param>
    /// <returns>The int8 data at the specified index.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern sbyte TfLiteMicroGeInt8tData(TensorFlowLiteTensor tensor, int index);

    /// <summary>
    /// Sets the int8 data in a tensor at the specified index.
    /// </summary>
    /// <param name="tensor">The tensor to set the int8 data in.</param>
    /// <param name="index">The index where the int8 data will be set.</param>
    /// <param name="value">The int8 value to set.</param>
    [DllImport(TensorFlowLibName)]
    public static extern void TfLiteMicroSetInt8Data(TensorFlowLiteTensor tensor, int index, sbyte value);

    /// <summary>
    /// Retrieves the quantization parameters of a tensor.
    /// </summary>
    /// <param name="tensor">The tensor.</param>
    /// <returns>The quantization parameters of the tensor.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern TensorFlowLiteQuantizationParams TfLiteMicroTensorQuantizationParams(TensorFlowLiteTensor tensor);

    /// <summary>
    /// Sets a mutable option for TensorFlow Lite Micro.
    /// </summary>
    /// <param name="option">The option to set.</param>
    /// <returns>OperationStatus of the option setting.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern TensorFlowLiteStatus TfLiteMicroMutableSetOption(sbyte option);

    /// <summary>
    /// Get the type of a tensor.
    /// </summary>
    /// <param name="tensor">The tensor.</param>
    /// <returns>The type of the tensor.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern TensorDataType TfLiteMicroGetType(TensorFlowLiteTensor tensor);

    /// <summary>
    /// Get the size of dimensions data of a tensor.
    /// </summary>
    /// <param name="tensor">The tensor.</param>
    /// <returns>The size of dimensions data.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern int TfLiteMicroDimsSizeData(TensorFlowLiteTensor tensor);

    /// <summary>
    /// Get the dimension data of a tensor at the specified index.
    /// </summary>
    /// <param name="tensor">The tensor.</param>
    /// <param name="index">The index of the dimension data.</param>
    /// <returns>The dimension data at the specified index.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern int TfLiteMicroDimsData(TensorFlowLiteTensor tensor, int index);

    /// <summary>
    /// Get the float data from a tensor at the specified index.
    /// </summary>
    /// <param name="tensor">The tensor containing the float data.</param>
    /// <param name="index">The index of the float data.</param>
    /// <returns>The float data at the specified index.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern float TfLiteMicroGetFloatData(TensorFlowLiteTensor tensor, int index);

    /// <summary>
    /// Sets the float data in a tensor at the specified index.
    /// </summary>
    /// <param name="tensor">The tensor to set the float data in.</param>
    /// <param name="index">The index where the float data will be set.</param>
    /// <param name="value">The float value to set.</param>
    [DllImport(TensorFlowLibName)]
    public static extern void TfLiteMicroSetFloatData(TensorFlowLiteTensor tensor, int index, float value);

    /// <summary>
    /// Get the byte data from a tensor.
    /// </summary>
    /// <param name="tensor">The tensor containing the byte data.</param>
    /// <returns>The byte data from the tensor.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern int TfLiteMicroGetByte(TensorFlowLiteTensor tensor);
}