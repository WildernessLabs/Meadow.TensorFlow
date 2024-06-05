using Meadow;
using System;
using System.Runtime.InteropServices;
using PersonDetection.Models;
using TensorFlow.litemicro;

namespace PersonDetection;

public class TensorFlow
{
    private static readonly Lazy<TensorFlow> instance = new Lazy<TensorFlow>(() => new TensorFlow());
    public static TensorFlow Instance => instance.Value;

    static private IntPtr interpreter;
    private const int ArenaSize = 134 * 1024;

   static public void Initialize()
    {
        PersonDetectionModel personDetectionModel = new PersonDetectionModel();

        IntPtr model = Marshal.AllocHGlobal(personDetectionModel.GetSize() * sizeof(byte));

        if (model == IntPtr.Zero)
        {
            throw new Exception("Failed to allocated the model");
        }

        Marshal.Copy(personDetectionModel.GetData(), 0, model, personDetectionModel.GetSize());

        IntPtr arena = Marshal.AllocHGlobal(ArenaSize * sizeof(int));

        if (arena == IntPtr.Zero)
        {
            Marshal.FreeHGlobal(model);
            throw new Exception("Failed to allocated the arena");
        }

        var model_options = c_api_lite_micro.TfLiteMicroGetModel(ArenaSize, arena, model);
        if (model_options == null)
        {
            Marshal.FreeHGlobal(model);
            throw new Exception("Failed to loaded the model");
        }

        var interpreter_options = c_api_lite_micro.TfLiteMicroInterpreterOptionCreate(model_options);
        if (interpreter_options == null)
        {
            Marshal.FreeHGlobal(model);
            throw new Exception("Failed to create interpreter option");   
        }

        interpreter = c_api_lite_micro.TfLiteMicroInterpreterCreate(interpreter_options, model_options);
        if (interpreter == null)
        {
            Marshal.FreeHGlobal(model);
            throw new Exception("Failed to Interpreter");
        }

        TfLiteStatus tfLiteStatus = c_api_lite_micro.TfLiteMicroInterpreterAllocateTensors(interpreter);
        if (tfLiteStatus != TfLiteStatus.kTfLiteOk)
        {
            Marshal.FreeHGlobal(model);
            throw new Exception("Failed to allocate tensors");
        }
    }

   static public TfLiteTensor GetInput()
    {
        TfLiteTensor input;
        input = c_api_lite_micro.TfLiteMicroInterpreterGetInput(interpreter, 0);
        return input;
    }

    static public TfLiteTensor GetOutput()
    {
        TfLiteTensor output = c_api_lite_micro.TfLiteMicroInterpreterGetOutput(interpreter, 0);
        return output;
    }
    static public int InputLegth(TfLiteTensor input)
    {
        return c_api_lite_micro.TfLiteMicroGetByte(input) / sizeof(float);
    }

    static public void InputData(TfLiteTensor input, int index, float value)
    {
        c_api_lite_micro.TfLiteMicroSetFloatData(input, index, value);
    }

    static public float OutputData(TfLiteTensor output, int index)
    {
        return c_api_lite_micro.TfLiteMicroGetFloatData(output, index);
    }
    static public int DimsSize(TfLiteTensor tensor)
    {
        return c_api_lite_micro.TfLiteMicroDimsSizeData(tensor);
    }

    static public int DimsData(TfLiteTensor tensor, int index)
    {
        return c_api_lite_micro.TfLiteMicroDimsData(tensor, index);
    }

    static public TfLiteType GetTensorType(TfLiteTensor tensor)
    {
        return c_api_lite_micro.TfLiteMicroGetType(tensor);
    }

    static public sbyte GetInt8Data(TfLiteTensor tensor, int index)
    {
        return c_api_lite_micro.TfLiteMicroGeInt8tData(tensor, index);
    }
    static public void SetInt8Data(TfLiteTensor tensor, int index, sbyte value)
    {
        c_api_lite_micro.TfLiteMicroSetInt8Data(tensor, index, value);
    }
    static public TfLiteStatus Invoke()
    {
        return c_api_lite_micro.TfLiteMicroInterpreterInvoke(interpreter);
    }
}