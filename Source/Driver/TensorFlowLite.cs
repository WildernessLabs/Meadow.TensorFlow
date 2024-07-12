using System;
using System.Runtime.InteropServices;

namespace Meadow.TensorFlow;

/// <summary>
/// Represents TensorFlow Lite for microcontrollers interpreter.
/// </summary>
public class TensorFlowLite : ITensorFlowLiteInterpreter
{
    /// <summary>
    /// Gets the quantization parameters for the input tensor.
    /// </summary>
    public TensorFlowLiteQuantizationParams InputQuantizationParams { get; private set; }

    /// <summary>
    /// Gets the quantization parameters for the output tensor.
    /// </summary>
    public TensorFlowLiteQuantizationParams OutputQuantizationParams { get; private set; }

    /// <summary>
    /// Gets or sets the status of the last operation performed by the TensorFlow Lite interpreter.
    /// </summary>
    public TensorFlowLiteStatus OperationStatus { get; set; } = TensorFlowLiteStatus.Ok;

    /// <summary>
    /// Gets the input tensor used by the TensorFlow Lite interpreter.
    /// </summary>
    protected TensorFlowLiteTensor InputTensor { get; set; }

    /// <summary>
    /// Gets the output tensor produced by the TensorFlow Lite interpreter.
    /// </summary>
    protected TensorFlowLiteTensor OutputTensor { get; set; }

    private readonly IntPtr interpreter;

    /// <summary>
    /// Initializes a new instance of the TensorFlowLite class with the specified tensor model and arena size.
    /// </summary>
    /// <param name="tensorModel">The tensor model containing the TensorFlow Lite model data.</param>
    /// <param name="arenaSize">The size of the memory arena allocated for TensorFlow Lite operations.</param>
    /// <exception cref="Exception">Thrown when allocation or initialization fails.</exception>
    public TensorFlowLite(ITensorModel tensorModel, int arenaSize)
    {
        IntPtr modelPtr = Marshal.AllocHGlobal(tensorModel.Size * sizeof(int));

        if (modelPtr == IntPtr.Zero)
        {
            throw new Exception("Failed to allocate model memory");
        }

        IntPtr arenaPtr = Marshal.AllocHGlobal(arenaSize * sizeof(int));

        if (arenaPtr == IntPtr.Zero)
        {
            Marshal.FreeHGlobal(modelPtr);
            throw new Exception("Failed to allocate arena memory");
        }

        Marshal.Copy(tensorModel.Data, 0, modelPtr, tensorModel.Size);

        var modelOptionsPtr = TensorFlowLiteBindings.TfLiteMicroGetModel(arenaSize, arenaPtr, modelPtr);
        if (modelOptionsPtr == IntPtr.Zero)
        {
            throw new Exception("Failed to load the model");
        }

        var interpreterOptionsPtr = TensorFlowLiteBindings.TfLiteMicroInterpreterOptionCreate(modelOptionsPtr);
        if (interpreterOptionsPtr == IntPtr.Zero)
        {
            throw new Exception("Failed to create interpreter options");
        }

        interpreter = TensorFlowLiteBindings.TfLiteMicroInterpreterCreate(interpreterOptionsPtr, modelOptionsPtr);
        if (interpreter == IntPtr.Zero)
        {
            throw new Exception("Failed to create interpreter");
        }

        OperationStatus = TensorFlowLiteBindings.TfLiteMicroInterpreterAllocateTensors(interpreter);

        if (OperationStatus != TensorFlowLiteStatus.Ok)
        {
            throw new Exception("Failed to allocate tensors");
        }

        InputTensor = TensorFlowLiteBindings.TfLiteMicroInterpreterGetInput(interpreter, 0);
        OutputTensor = TensorFlowLiteBindings.TfLiteMicroInterpreterGetOutput(interpreter, 0);

        InputQuantizationParams = TensorFlowLiteBindings.TfLiteMicroTensorQuantizationParams(InputTensor);
        OutputQuantizationParams = TensorFlowLiteBindings.TfLiteMicroTensorQuantizationParams(OutputTensor);
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
        unsafe
        {
            sbyte *ptr = &value;
            TensorFlowLiteBindings.TfLiteMicroSetData(InputTensor, index, (IntPtr)ptr, TensorDataType.Int8);
        }
    }

    /// <summary>
    /// Sets the integer data at the specified index in the input tensor.
    /// </summary>
    /// <param name="index">The index at which to set the data.</param>
    /// <param name="value">The integer value to set.</param>
    public void SetInputTensorInt16Data(int index, Int16 value)
    {
        unsafe
        {
            Int16 *ptr = &value;
            TensorFlowLiteBindings.TfLiteMicroSetData(InputTensor, index, (IntPtr)ptr, TensorDataType.Int16);
        }
    }

    /// <summary>
    /// Sets the integer data at the specified index in the input tensor.
    /// </summary>
    /// <param name="index">The index at which to set the data.</param>
    /// <param name="value">The integer value to set.</param>
    public void SetInputTensorInt32Data(int index, Int32 value)
    {
        unsafe
        {
            Int32 *ptr = &value;
            TensorFlowLiteBindings.TfLiteMicroSetData(InputTensor, index, (IntPtr)ptr, TensorDataType.Int32);
        }
    }

    /// <summary>
    /// Sets the integer data at the specified index in the input tensor.
    /// </summary>
    /// <param name="index">The index at which to set the data.</param>
    /// <param name="value">The integer value to set.</param>
    public void SetInputTensorInt64Data(int index, Int64 value)
    {
        unsafe
        {
            Int64 *ptr = &value;
            TensorFlowLiteBindings.TfLiteMicroSetData(InputTensor, index, (IntPtr)ptr, TensorDataType.Int64);
        }
    }


    /// <summary>
    /// Sets the unsigned integer data at the specified index in the input tensor.
    /// </summary>
    /// <param name="index">The index at which to set the data.</param>
    /// <param name="value">The integer value to set.</param>
    public void SetInputTensorUInt8Data(int index, sbyte value)
    {
        unsafe
        {
            sbyte *ptr = &value;
            TensorFlowLiteBindings.TfLiteMicroSetData(InputTensor, index, (IntPtr)ptr, TensorDataType.UInt8);
        }
    }

    /// <summary>
    /// Sets the unsigned integer data at the specified index in the input tensor.
    /// </summary>
    /// <param name="index">The index at which to set the data.</param>
    /// <param name="value">The integer value to set.</param>
    public void SetInputTensorUInt16Data(int index, UInt16 value)
    {
        unsafe
        {
            UInt16 *ptr = &value;
            TensorFlowLiteBindings.TfLiteMicroSetData(InputTensor, index, (IntPtr)ptr, TensorDataType.Int64);
        }
    }

    /// <summary>
    /// Sets the unsigned integer data at the specified index in the input tensor.
    /// </summary>
    /// <param name="index">The index at which to set the data.</param>
    /// <param name="value">The integer value to set.</param>
    public void SetInputTensorUInt32Data(int index, UInt32 value)
    {
        unsafe
        {
            UInt32 *ptr = &value;
            TensorFlowLiteBindings.TfLiteMicroSetData(InputTensor, index, (IntPtr)ptr, TensorDataType.UInt32);
        }
    }

    /// <summary>
    /// Sets the unsigned integer data at the specified index in the input tensor.
    /// </summary>
    /// <param name="index">The index at which to set the data.</param>
    /// <param name="value">The integer value to set.</param>
    public void SetInputTensorUInt64Data(int index, UInt64 value)
    {
        unsafe
        {
            UInt64 *ptr = &value;
            TensorFlowLiteBindings.TfLiteMicroSetData(InputTensor, index, (IntPtr)ptr, TensorDataType.UInt64);
        }
    }

    /// <summary>
    /// Sets the float data at the specified index in the input tensor.
    /// </summary>
    /// <param name="index">The index at which to set the data.</param>
    /// <param name="value">The integer value to set.</param>
    public void SetInputTensorFloat16Data(int index, Int16 value)
    {
        unsafe
        {
            Int16 *ptr = &value;
            TensorFlowLiteBindings.TfLiteMicroSetData(InputTensor, index, (IntPtr)ptr, TensorDataType.Float16);
        }
    }

    /// <summary>
    /// Sets the float data at the specified index in the input tensor.
    /// </summary>
    /// <param name="index">The index at which to set the data.</param>
    /// <param name="value">The integer value to set.</param>
    public void SetInputTensorFloatData(int index, float value)
    {
        unsafe
        {
            float *ptr = &value;
            TensorFlowLiteBindings.TfLiteMicroSetData(InputTensor, index, (IntPtr)ptr, TensorDataType.Float32);
        }
    }

    /// <summary>
    /// Sets the double data at the specified index in the input tensor.
    /// </summary>
    /// <param name="index">The index at which to set the data.</param>
    /// <param name="value">The integer value to set.</param>
    public void SetInputTensorFloat64Data(int index, double value)
    {
        unsafe
        {
            double *ptr = &value;
            TensorFlowLiteBindings.TfLiteMicroSetData(InputTensor, index, (IntPtr)ptr, TensorDataType.Float64);
        }
    }

    /// <summary>
    /// Sets the bool data at the specified index in the input tensor.
    /// </summary>
    /// <param name="index">The index at which to set the data.</param>
    /// <param name="value">The integer value to set.</param>
    public void SetInputTensorBoolData(int index, bool value)
    {
        unsafe
        {
            bool *ptr = &value;
            TensorFlowLiteBindings.TfLiteMicroSetData(InputTensor, index, (IntPtr)ptr, TensorDataType.Bool);
        }
    }

    /// <summary>
    /// Sets the string data at the specified index in the input tensor.
    /// </summary>
    /// <param name="index">The index at which to set the data.</param>
    /// <param name="value">The integer value to set.</param>
    public void SetInputTensorStringData(int index, sbyte value)
    {
        unsafe
        {
            sbyte *ptr = &value;
            TensorFlowLiteBindings.TfLiteMicroSetData(InputTensor, index, (IntPtr)ptr, TensorDataType.String);
        }
    }

    /// <summary>
    /// Get the integer with 1 byte of data at the specified index from the output tensor.
    /// </summary>
    /// <param name="index">The index from which to retrieve the data.</param>
    /// <returns>The int8 data at the specified index.</returns>
    public sbyte GetOutputTensorInt8Data(int index)
    {
        IntPtr ptr = TensorFlowLiteBindings.TfLiteMicroGetData(OutputTensor, index, TensorDataType.Int8);
        unsafe
        {
            return *(sbyte *)ptr;
        }
    }

    /// <summary>
    /// Get the unsigned integer with 4 byte of data at the specified index from the output tensor.
    /// </summary>
    /// <param name="index">The index from which to retrieve the data.</param>
    /// <returns>The int8 data at the specified index.</returns>
    public UInt32 GetOutputTensorUInt32Data(int index)
    {
        IntPtr ptr = TensorFlowLiteBindings.TfLiteMicroGetData(OutputTensor, index, TensorDataType.UInt32);
        unsafe
        {
            return *(UInt32 *)ptr;
        }
    }

    /// <summary>
    /// Get the integer with 8 byte of data at the specified index from the output tensor.
    /// </summary>
    /// <param name="index">The index from which to retrieve the data.</param>
    /// <returns>The int8 data at the specified index.</returns>
    public UInt64 GetOutputTensorUInt64Data(int index)
    {
        IntPtr ptr = TensorFlowLiteBindings.TfLiteMicroGetData(OutputTensor, index, TensorDataType.UInt64);
        unsafe
        {
            return *(UInt64 *)ptr;
        }
    }

    /// <summary>
    /// Get the unsigned integer with 1 byte of data at the specified index from the output tensor.
    /// </summary>
    /// <param name="index">The index from which to retrieve the data.</param>
    /// <returns>The int8 data at the specified index.</returns>
    public sbyte GetOutputTensorUInt8Data(int index)
    {
        IntPtr ptr = TensorFlowLiteBindings.TfLiteMicroGetData(OutputTensor, index, TensorDataType.UInt8);
        unsafe
        {
            return *(sbyte *)ptr;
        }
    }

    /// <summary>
    /// Get the integer with 2 byte of data at the specified index from the output tensor.
    /// </summary>
    /// <param name="index">The index from which to retrieve the data.</param>
    /// <returns>The int8 data at the specified index.</returns>
    public Int16 GetOutputTensorInt16Data(int index)
    {
        IntPtr ptr = TensorFlowLiteBindings.TfLiteMicroGetData(OutputTensor, index, TensorDataType.Int16);
        unsafe
        {
            return *(Int16 *)ptr;
        }
    }

    /// <summary>
    /// Get the integer with 4 byte of data at the specified index from the output tensor.
    /// </summary>
    /// <param name="index">The index from which to retrieve the data.</param>
    /// <returns>The int8 data at the specified index.</returns>
    public Int32 GetOutputTensorInt32Data(int index)
    {
        IntPtr ptr = TensorFlowLiteBindings.TfLiteMicroGetData(OutputTensor, index, TensorDataType.Int32);
        unsafe
        {
            return *(Int32 *)ptr;
        }
    }

    /// <summary>
    /// Get the integer with 8 byte of data at the specified index from the output tensor.
    /// </summary>
    /// <param name="index">The index from which to retrieve the data.</param>
    /// <returns>The int8 data at the specified index.</returns>
    public Int64 GetOutputTensorInt64Data(int index)
    {
        IntPtr ptr = TensorFlowLiteBindings.TfLiteMicroGetData(OutputTensor, index, TensorDataType.Int64);
        unsafe
        {
            return *(Int64 *)ptr;
        }
    }

    /// <summary>
    /// Get the float data at the specified index from the output tensor.
    /// </summary>
    /// <param name="index">The index from which to retrieve the data.</param>
    /// <returns>The int8 data at the specified index.</returns>
    public UInt16 GetOutputTensorFloat16Data(int index)
    {
        IntPtr ptr = TensorFlowLiteBindings.TfLiteMicroGetData(OutputTensor, index, TensorDataType.Float16);
        unsafe
        {
            return *(UInt16 *)ptr;
        }
    }


    /// <summary>
    /// Get the float data at the specified index from the output tensor.
    /// </summary>
    /// <param name="index">The index from which to retrieve the data.</param>
    /// <returns>The int8 data at the specified index.</returns>
    public float GetOutputTensorFloatData(int index)
    {
        IntPtr ptr = TensorFlowLiteBindings.TfLiteMicroGetData(OutputTensor, index, TensorDataType.Float32);
        unsafe
        {
            return *(float *)ptr;
        }
    }

    /// <summary>
    /// Get the double data at the specified index from the output tensor.
    /// </summary>
    /// <param name="index">The index from which to retrieve the data.</param>
    /// <returns>The int8 data at the specified index.</returns>
    public double GetOutputTensorFloat64Data(int index)
    {
        IntPtr ptr = TensorFlowLiteBindings.TfLiteMicroGetData(OutputTensor, index, TensorDataType.Float64);
        unsafe
        {
            return *(double *)ptr;
        }
    }

    /// <summary>
    /// Get the bool data at the specified index from the output tensor.
    /// </summary>
    /// <param name="index">The index from which to retrieve the data.</param>
    /// <returns>The int8 data at the specified index.</returns>
    public bool GetOutputTensorBoolData(int index)
    {
        IntPtr ptr = TensorFlowLiteBindings.TfLiteMicroGetData(OutputTensor, index, TensorDataType.Bool);
        unsafe
        {
            return *(bool *)ptr;
        }
    }

    /// <summary>
    /// Get the string data at the specified index from the output tensor.
    /// </summary>
    /// <param name="index">The index from which to retrieve the data.</param>
    /// <returns>The int8 data at the specified index.</returns>
    public String GetOutputTensorStringData(int index)
    {
        IntPtr ptr = TensorFlowLiteBindings.TfLiteMicroGetData(OutputTensor, index, TensorDataType.String);
        return Marshal.PtrToStringAnsi(ptr);
    }

    /// <summary>
    /// Invokes the TensorFlow Lite interpreter for inference.
    /// </summary>
    public void InvokeInterpreter()
    {
        OperationStatus = TensorFlowLiteBindings.TfLiteMicroInterpreterInvoke(interpreter);
    }

    /// <summary>
    /// Retrieves the number of output tensors produced by the TensorFlow Lite interpreter.
    /// </summary>
    /// <returns>The number of output tensors.</returns>
    public int GetOutputTensorCount()
    {
        return TensorFlowLiteBindings.TfLiteMicroInterpreterGetOutputCount(interpreter);
    }

    /// <summary>
    /// Retrieves the number of input tensors expected by the TensorFlow Lite interpreter.
    /// </summary>
    /// <returns>The number of input tensors.</returns>
    public int GetInputTensorCount()
    {
        return TensorFlowLiteBindings.TfLiteMicroInterpreterGetInputCount(interpreter);
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
    /// Retrieves the dimension data of the input tensor at the specified index.
    /// </summary>
    /// <param name="index">The index of the dimension data.</param>
    /// <returns>The dimension data at the specified index.</returns>
    public int GetInputTensorDimension(int index)
    {
        return TensorFlowLiteBindings.TfLiteMicroDimsData(InputTensor, index);
    }

    /// <summary>
    /// Retrieves the quantization parameters of the output tensor.
    /// </summary>
    /// <returns>The quantization parameters of the output tensor.</returns>
    public TensorFlowLiteQuantizationParams GetOutputTensorQuantizationParams()
    {
        return TensorFlowLiteBindings.TfLiteMicroTensorQuantizationParams(OutputTensor);
    }
}