using System;
using System.Runtime.InteropServices;

namespace Meadow.TensorFlow;

/// <summary>
/// Provides C API bindings for TensorFlow Lite for Microcontrollers.
/// </summary>
public static class TensorFlowLiteBindings
{
    /// <summary>
    /// The name of the TensorFlow Lite library.
    /// </summary>
    private const string TensorFlowLibName = "TensorFlow.so";

    /// <summary>
    /// Retrieves the model from TensorFlow Lite Micro.
    /// </summary>
    /// <param name="arenaSize">The size of the arena.</param>
    /// <param name="arena">The arena pointer.</param>
    /// <param name="modelData">The model data pointer.</param>
    /// <returns>A pointer to the model.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern IntPtr TfLiteMicroGetModel(int arenaSize, IntPtr arena, IntPtr modelData);

    /// <summary>
    /// Creates an interpreter options object for TensorFlow Lite Micro.
    /// </summary>
    /// <param name="option">The interpreter option pointer.</param>
    /// <returns>A pointer to the interpreter options.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern IntPtr TfLiteMicroInterpreterOptionCreate(IntPtr option);

    /// <summary>
    /// Creates a TensorFlow Lite Micro interpreter.
    /// </summary>
    /// <param name="interpreterOption">The interpreter option pointer.</param>
    /// <param name="modelOption">The model option pointer.</param>
    /// <returns>A pointer to the interpreter.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern IntPtr TfLiteMicroInterpreterCreate(IntPtr interpreterOption, IntPtr modelOption);

    /// <summary>
    /// Allocates tensors for the TensorFlow Lite Micro interpreter.
    /// </summary>
    /// <param name="interpreter">The interpreter pointer.</param>
    /// <returns>The operation status of the tensor allocation.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern TensorFlowLiteStatus TfLiteMicroInterpreterAllocateTensors(IntPtr interpreter);

    /// <summary>
    /// Retrieves an input tensor from the TensorFlow Lite Micro interpreter.
    /// </summary>
    /// <param name="interpreter">The interpreter pointer.</param>
    /// <param name="index">The index of the input tensor.</param>
    /// <returns>The input tensor.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern TensorFlowLiteTensor TfLiteMicroInterpreterGetInput(IntPtr interpreter, int index);

    /// <summary>
    /// Retrieves an output tensor from the TensorFlow Lite Micro interpreter.
    /// </summary>
    /// <param name="interpreter">The interpreter pointer.</param>
    /// <param name="index">The index of the output tensor.</param>
    /// <returns>The output tensor.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern TensorFlowLiteTensor TfLiteMicroInterpreterGetOutput(IntPtr interpreter, int index);

    /// <summary>
    /// Retrieves the number of output tensors from the TensorFlow Lite Micro interpreter.
    /// </summary>
    /// <param name="interpreter">The interpreter pointer.</param>
    /// <returns>The number of output tensors.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern int TfLiteMicroInterpreterGetOutputCount(IntPtr interpreter);

    /// <summary>
    /// Retrieves the number of input tensors from the TensorFlow Lite Micro interpreter.
    /// </summary>
    /// <param name="interpreter">The interpreter pointer.</param>
    /// <returns>The number of input tensors.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern int TfLiteMicroInterpreterGetInputCount(IntPtr interpreter);

    /// <summary>
    /// Invokes the TensorFlow Lite Micro interpreter for inference.
    /// </summary>
    /// <param name="interpreter">The interpreter pointer.</param>
    /// <returns>The operation status of the interpreter invocation.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern TensorFlowLiteStatus TfLiteMicroInterpreterInvoke(IntPtr interpreter);

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
    /// <returns>The operation status of the option setting.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern TensorFlowLiteStatus TfLiteMicroMutableSetOption(sbyte option);

    /// <summary>
    /// Retrieves the data type of a tensor.
    /// </summary>
    /// <param name="tensor">The tensor.</param>
    /// <returns>The data type of the tensor.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern TensorDataType TfLiteMicroGetType(TensorFlowLiteTensor tensor);

    /// <summary>
    /// Retrieves the size of the dimensions data of a tensor.
    /// </summary>
    /// <param name="tensor">The tensor.</param>
    /// <returns>The size of the dimensions data.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern int TfLiteMicroDimsSizeData(TensorFlowLiteTensor tensor);

    /// <summary>
    /// Retrieves the dimension data of a tensor at the specified index.
    /// </summary>
    /// <param name="tensor">The tensor.</param>
    /// <param name="index">The index of the dimension data.</param>
    /// <returns>The dimension data at the specified index.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern int TfLiteMicroDimsData(TensorFlowLiteTensor tensor, int index);

    /// <summary>
    /// Retrieves the byte data from a tensor.
    /// </summary>
    /// <param name="tensor">The tensor containing the byte data.</param>
    /// <returns>The byte data from the tensor.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern int TfLiteMicroGetByte(TensorFlowLiteTensor tensor);

    /// <summary>
    /// Get a value to the tensor depending on the type of data .
    /// </summary>
    /// <param name="tensor">The tensor containing the byte data.</param>
    /// <param name="index">The index at which to set the data.</param>
    /// <paramref name="type"/>The type of data.</param>
    /// <returns>The byte data from the tensor.</returns>
    [DllImport(TensorFlowLibName)]
    public static extern IntPtr TfLiteMicroGetData(TensorFlowLiteTensor tensor, int index, TensorDataType type);

    /// <summary>
    /// Set a value to the tensor depending on the type of data.
    /// </summary>
    /// <param name="tensor">The tensor containing the byte data.</param>
    /// <param name="index">The index at which to set the data.</param>
    /// <param name="value">The byte data from the tensor.</param>
    [DllImport(TensorFlowLibName)]
    public static extern int TfLiteMicroSetData(TensorFlowLiteTensor tensor, int index, IntPtr value, TensorDataType type);
}