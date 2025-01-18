using System;
using System.Runtime.InteropServices;

namespace Meadow.Foundation.RTLite;

/// <summary>
/// Provides C API bindings for RTLite for Microcontrollers.
/// </summary>
internal static class Native
{
    /// <summary>
    /// The name of the RTLite library.
    /// </summary>
    private const string LibraryName = "TensorFlow.so";

    /// <summary>
    /// Retrieves the model from RTLite Micro.
    /// </summary>
    /// <param name="arenaSize">The size of the arena.</param>
    /// <param name="arena">The arena pointer.</param>
    /// <param name="modelData">The model data pointer.</param>
    /// <returns>A pointer to the model.</returns>
    [DllImport(LibraryName)]
    public static extern IntPtr TfLiteMicroGetModel(int arenaSize, IntPtr arena, IntPtr modelData);

    /// <summary>
    /// Creates an interpreter options object for RTLite Micro.
    /// </summary>
    /// <param name="option">The interpreter option pointer.</param>
    /// <returns>A pointer to the interpreter options.</returns>
    [DllImport(LibraryName)]
    public static extern IntPtr TfLiteMicroInterpreterOptionCreate(IntPtr option);

    /// <summary>
    /// Creates a RTLite Micro interpreter.
    /// </summary>
    /// <param name="interpreterOption">The interpreter option pointer.</param>
    /// <param name="modelOption">The model option pointer.</param>
    /// <returns>A pointer to the interpreter.</returns>
    [DllImport(LibraryName)]
    public static extern IntPtr TfLiteMicroInterpreterCreate(IntPtr interpreterOption, IntPtr modelOption);

    /// <summary>
    /// Allocates tensors for the RTLite Micro interpreter.
    /// </summary>
    /// <param name="interpreter">The interpreter pointer.</param>
    /// <returns>The operation status of the tensor allocation.</returns>
    [DllImport(LibraryName)]
    public static extern RuntimeStatus TfLiteMicroInterpreterAllocateTensors(IntPtr interpreter);

    /// <summary>
    /// Retrieves an input tensor from the RTLite Micro interpreter.
    /// </summary>
    /// <param name="interpreter">The interpreter pointer.</param>
    /// <param name="index">The index of the input tensor.</param>
    /// <returns>The input tensor.</returns>
    [DllImport(LibraryName)]
    public static extern Tensor TfLiteMicroInterpreterGetInput(IntPtr interpreter, int index);

    /// <summary>
    /// Retrieves an output tensor from the RTLite Micro interpreter.
    /// </summary>
    /// <param name="interpreter">The interpreter pointer.</param>
    /// <param name="index">The index of the output tensor.</param>
    /// <returns>The output tensor.</returns>
    [DllImport(LibraryName)]
    public static extern Tensor TfLiteMicroInterpreterGetOutput(IntPtr interpreter, int index);

    /// <summary>
    /// Retrieves the number of output tensors from the RTLite Micro interpreter.
    /// </summary>
    /// <param name="interpreter">The interpreter pointer.</param>
    /// <returns>The number of output tensors.</returns>
    [DllImport(LibraryName)]
    public static extern int TfLiteMicroInterpreterGetOutputCount(IntPtr interpreter);

    /// <summary>
    /// Retrieves the number of input tensors from the RTLite Micro interpreter.
    /// </summary>
    /// <param name="interpreter">The interpreter pointer.</param>
    /// <returns>The number of input tensors.</returns>
    [DllImport(LibraryName)]
    public static extern int TfLiteMicroInterpreterGetInputCount(IntPtr interpreter);

    /// <summary>
    /// Invokes the RTLite Micro interpreter for inference.
    /// </summary>
    /// <param name="interpreter">The interpreter pointer.</param>
    /// <returns>The operation status of the interpreter invocation.</returns>
    [DllImport(LibraryName)]
    public static extern RuntimeStatus TfLiteMicroInterpreterInvoke(IntPtr interpreter);

    /// <summary>
    /// Retrieves the int8 data from a tensor at the specified index.
    /// </summary>
    /// <param name="tensor">The tensor containing the int8 data.</param>
    /// <param name="index">The index of the int8 data.</param>
    /// <returns>The int8 data at the specified index.</returns>
    [DllImport(LibraryName)]
    public static extern sbyte TfLiteMicroGeInt8tData(Tensor tensor, int index);

    /// <summary>
    /// Sets the int8 data in a tensor at the specified index.
    /// </summary>
    /// <param name="tensor">The tensor to set the int8 data in.</param>
    /// <param name="index">The index where the int8 data will be set.</param>
    /// <param name="value">The int8 value to set.</param>
    [DllImport(LibraryName)]
    public static extern void TfLiteMicroSetInt8Data(Tensor tensor, int index, sbyte value);

    /// <summary>
    /// Retrieves the quantization parameters of a tensor.
    /// </summary>
    /// <param name="tensor">The tensor.</param>
    /// <returns>The quantization parameters of the tensor.</returns>
    [DllImport(LibraryName)]
    public static extern QuantizationParams TfLiteMicroTensorQuantizationParams(Tensor tensor);

    /// <summary>
    /// Sets a mutable option for RTLite Micro.
    /// </summary>
    /// <param name="option">The option to set.</param>
    /// <returns>The operation status of the option setting.</returns>
    [DllImport(LibraryName)]
    public static extern RuntimeStatus TfLiteMicroMutableSetOption(sbyte option);

    /// <summary>
    /// Retrieves the data type of a tensor.
    /// </summary>
    /// <param name="tensor">The tensor.</param>
    /// <returns>The data type of the tensor.</returns>
    [DllImport(LibraryName)]
    public static extern TensorDataType TfLiteMicroGetType(Tensor tensor);

    /// <summary>
    /// Retrieves the size of the dimensions data of a tensor.
    /// </summary>
    /// <param name="tensor">The tensor.</param>
    /// <returns>The size of the dimensions data.</returns>
    [DllImport(LibraryName)]
    public static extern int TfLiteMicroDimsSizeData(Tensor tensor);

    /// <summary>
    /// Retrieves the dimension data of a tensor at the specified index.
    /// </summary>
    /// <param name="tensor">The tensor.</param>
    /// <param name="index">The index of the dimension data.</param>
    /// <returns>The dimension data at the specified index.</returns>
    [DllImport(LibraryName)]
    public static extern int TfLiteMicroDimsData(Tensor tensor, int index);

    /// <summary>
    /// Retrieves the float data from a tensor at the specified index.
    /// </summary>
    /// <param name="tensor">The tensor containing the float data.</param>
    /// <param name="index">The index of the float data.</param>
    /// <returns>The float data at the specified index.</returns>
    [DllImport(LibraryName)]
    public static extern float TfLiteMicroGetFloatData(Tensor tensor, int index);

    /// <summary>
    /// Sets the float data in a tensor at the specified index.
    /// </summary>
    /// <param name="tensor">The tensor to set the float data in.</param>
    /// <param name="index">The index where the float data will be set.</param>
    /// <param name="value">The float value to set.</param>
    [DllImport(LibraryName)]
    public static extern void TfLiteMicroSetFloatData(Tensor tensor, int index, float value);

    /// <summary>
    /// Retrieves the byte data from a tensor.
    /// </summary>
    /// <param name="tensor">The tensor containing the byte data.</param>
    /// <returns>The byte data from the tensor.</returns>
    [DllImport(LibraryName)]
    public static extern int TfLiteMicroGetByte(Tensor tensor);

    /// <summary>
    /// Delete the Interpreter.
    /// </summary>
    /// <param name="interpreter">.</param>
    [DllImport(LibraryName)]
    public static extern void TfLiteMicroInterpreterDelete(IntPtr interpreter);

    /// <summary>
    /// Delete the Model.
    /// </summary>
    /// <param name="interpreter">.</param>
    [DllImport(LibraryName)]
    public static extern void TfLiteMicroModelDelete(IntPtr interpreter);

    /// <summary>
    /// Delete the Interpreter option.
    /// </summary>
    /// <param name="interpreter">.</param>
    [DllImport(LibraryName)]
    public static extern void TfLiteMicroInterpreterOptionDelete(IntPtr interpreter);
}