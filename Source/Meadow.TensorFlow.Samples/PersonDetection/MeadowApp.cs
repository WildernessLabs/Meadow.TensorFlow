using PersonDetection.Models;
using Meadow;
using Meadow.Devices;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

using TensorFlow.litemicro;

using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using Meadow.Peripherals.Displays;
using Meadow.Foundation.Sensors.Camera;

using BitMiracle.LibJpeg;
using SimpleJpegDecoder;

namespace PersonDetection;

public class MeadowApp : App<F7CoreComputeV2>
{
    private IProjectLabHardware projLab;
    
    const int ArenaSize = 134 * 1024;
    PersonDetectionModel personDetectionModel = new PersonDetectionModel();
    TfLiteStatus tfLiteStatus;
    IntPtr interpreter;
    TfLiteTensor input, output;
    public readonly Image []images = 
    { 
        Image.LoadFromResource("Resources.no_person.bmp"),
        Image.LoadFromResource("Resources.person.bmp"),
    };
    Vc0706 camera;
    private DisplayScreen displayScreen;

    public override Task Initialize()
    {
        projLab = ProjectLab.Create();
        displayScreen = new DisplayScreen(projLab.Display,RotationType._270Degrees);

        camera = new Vc0706(Device, Device.PlatformOS.GetSerialPortName("COM1"), 38400);
        if(camera.SetCaptureResolution(Vc0706.ImageResolution._320x240))
        {
            Resolver.Log.Info("Resolution successfully changed");
        }

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
        _ = TakePicture();

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

    async Task TakePicture()
    {
        Resolver.Log.Info($"Image size is {camera.GetCaptureResolution()}");

        camera.CapturePhoto();

        using var jpegStream = await camera.GetPhotoStream();

        var decoder = new JpegDecoder();
        var jpg = decoder.DecodeJpeg(jpegStream);
        Resolver.Log.Info($"Jpeg decoded is {jpg.Length} bytes, W: {decoder.Width}, H: {decoder.Height}");
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