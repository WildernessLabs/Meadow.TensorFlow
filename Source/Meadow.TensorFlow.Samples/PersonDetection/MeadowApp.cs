using PersonDetection.Models;
using Meadow;
using Meadow.Devices;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TensorFlow.litemicro;
using Meadow.Foundation.Graphics;

namespace PersonDetection;

public class MeadowApp : App<F7FeatherV2>
{
    PersonDetectionModel personDetectionModel = new PersonDetectionModel();
    const int ArenaSize = 134 * 1024;
    TfLiteStatus tfLiteStatus;
    IntPtr interpreter;
    TfLiteTensor input, output;
    public readonly Image []images = 
    { 
        Image.LoadFromResource("Resources.no_person.bmp"),
        Image.LoadFromResource("Resources.person.bmp"),
    };

    public override Task Initialize()
    {
        Resolver.Log.Info("Initialize TensorFlow ...");

        IntPtr model = Marshal.AllocHGlobal(personDetectionModel.GetSize() * sizeof(int));

        if (model == IntPtr.Zero)
        {
            Resolver.Log.Info("Failed to allocated model");
            return base.Initialize();
        }

        IntPtr arena = Marshal.AllocHGlobal(ArenaSize * sizeof(int));

        if (arena == IntPtr.Zero)
        {
            Resolver.Log.Info("Failed to allocated arena");
            Marshal.FreeHGlobal(model);
            return base.Initialize();
        }

        Marshal.Copy(personDetectionModel.GetData(), 0, model, personDetectionModel.GetSize());

        var model_options = c_api_lite_micro.TfLiteMicroGetModel(ArenaSize, arena, model);
        if (model_options == null)
        {
            Resolver.Log.Info("Failed to loaded the model");
        }

        var interpreter_options = c_api_lite_micro.TfLiteMicroInterpreterOptionCreate(model_options);
        if (interpreter_options == null)
        {
            Resolver.Log.Info("Failed to create interpreter option");
        }

        interpreter = c_api_lite_micro.TfLiteMicroInterpreterCreate(interpreter_options, model_options);
        if (interpreter == null)
        {
            Resolver.Log.Info("Failed to Interpreter");
        }

        tfLiteStatus = c_api_lite_micro.TfLiteMicroInterpreterAllocateTensors(interpreter);
        if (tfLiteStatus != TfLiteStatus.kTfLiteOk)
        {
            Resolver.Log.Info("Failed to allocate tensors");
        }

        input = c_api_lite_micro.TfLiteMicroInterpreterGetInput(interpreter, 0);
        CopyImageToTensor(images[1], input);

        return Task.CompletedTask;
    }

    public override Task Run()
    {
        TfLiteStatus tfLiteStatus;
        tfLiteStatus = c_api_lite_micro.TfLiteMicroInterpreterInvoke(interpreter);
        if (tfLiteStatus != TfLiteStatus.kTfLiteOk)
        {
            Resolver.Log.Info("Invoke failed");
            return Task.CompletedTask;
        }

        output = c_api_lite_micro.TfLiteMicroInterpreterGetOutput(interpreter, 0);
        int outputDimsSize = c_api_lite_micro.TfLiteMicroDimsSizeData(output);
        int outputData0 = c_api_lite_micro.TfLiteMicroDimsData(output, 0);
        int outputDimsData0Size = c_api_lite_micro.TfLiteMicroDimsData(output, 1);
        TfLiteType tfLiteType = c_api_lite_micro.TfLiteMicroGetType(output);

        Resolver.Log.Info($" Dims Size:{outputDimsSize}, Data 0: {outputData0},  Size:{outputDimsData0Size}");
        int personScore = (int)c_api_lite_micro.TfLiteMicroGeInt8tData(output, 1);
        int noPersonScore = (int)c_api_lite_micro.TfLiteMicroGeInt8tData(output, 0);
        Resolver.Log.Info($"Score \n - Person:{personScore}\n - No Person:{noPersonScore}");
        Resolver.Log.Info("Sample completed");
        return Task.CompletedTask;
    }

    public static void CopyImageToTensor(Image img, TfLiteTensor tensor)
    {
        int width = img.DisplayBuffer.Width;
        int height = img.DisplayBuffer.Height;
        using var memStream = new MemoryStream(img.DisplayBuffer.Buffer);
        Resolver.Log.Info($"Image Infos - width: {width}, height: {height} , size: {memStream.Length}");
        int index = 0;
        memStream.Seek(0, SeekOrigin.Begin);
        while(index < memStream.Length)
        {
            c_api_lite_micro.TfLiteMicroSetInt8Data(tensor, index++, (sbyte)memStream.ReadByte());
        }
    }
}