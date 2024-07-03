using BitMiracle.LibJpeg;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.Buffers;
using Meadow.Foundation.Graphics.MicroLayout;
using Meadow.Foundation.Sensors.Camera;
using Meadow.Peripherals.Displays;

using System;
using System.IO;
using System.Threading.Tasks;
using Meadow.TensorFlow;

using PersonDetection.Models;
using PersonDetection.Controllers;

namespace PersonDetection;


public class MeadowApp : App<F7CoreComputeV2>
{
    private IProjectLabHardware projLab;
    private DisplayController displayController;
    public const sbyte Person = 1;
    public const sbyte noPerson = 0;

    PersonDetectionTensorFlow personDetectionTF;
    readonly PersonDetectionModel personDetectionModel = new();
    const int ArenaSize = 134 * 1024;

    Vc0706 camera;
    private DisplayScreen displayScreen;

    public override Task Initialize()
    {
        personDetectionTF = new PersonDetectionTensorFlow(personDetectionModel, ArenaSize);

        projLab = ProjectLab.Create();
        displayController = new DisplayController(projLab.Display);

        camera = new Vc0706(Device, Device.PlatformOS.GetSerialPortName("COM1"), 38400);
        if (camera.SetCaptureResolution(Vc0706.ImageResolution._320x240))
        {
            Resolver.Log.Info("Resolution successfully changed");
        }

        return Task.CompletedTask;
    }

    public override async Task Run()
    {
        var imageBuffer = await TakePicture();
        CopyPixelBufferToTensor(imageBuffer);

        
        personDetectionTF.InvokeInterpreter();

        if (personDetectionTF.OperationStatus != TensorFlowLiteStatus.Ok)
        {
            Resolver.Log.Info("Invoke failed");
            return;
        }

        int outputDimsSize = personDetectionTF.GetOutputTensorDimensionsSize();
        int outputData0 = personDetectionTF.GetInputTensorDimension(0);
        int outputDimsData0Size = personDetectionTF.GetOutputTensorDimension(1);

        Resolver.Log.Info($" Dims Size: {outputDimsSize}, Data 0: {outputData0},  Size:{outputDimsData0Size}");

        sbyte personScore = personDetectionTF.GetOutputTensorInt8Data(1);
        sbyte noPersonScore = personDetectionTF.GetOutputTensorInt8Data(0);
        
        Resolver.Log.Info($"Score \n - Person: {personScore}\n - No Person:{noPersonScore}");

        int state = noPersonScore > personScore ? noPerson : Person;
        int score = state != Person ? noPersonScore : personScore;

        displayController.ShowImage(96, 96, imageBuffer);
        displayController.ShowClassification(state, score);
    }

    public async Task<IPixelBuffer> TakePicture()
    {
        Resolver.Log.Info("Take a picture");
        camera.CapturePhoto();

        using var jpegStream = await camera.GetPhotoStream();
        var jpeg = new JpegImage(jpegStream);

        using var memoryStream = new MemoryStream();

        jpeg.WriteBitmap(memoryStream);
        byte[] bitmapData = memoryStream.ToArray();

        // Skip the first 54 bytes (bitmap header)
        byte[] pixelData = new byte[bitmapData.Length - 54];
        Array.Copy(bitmapData, 54, pixelData, 0, pixelData.Length);

        var pixelBuffer = new BufferRgb888(jpeg.Width, jpeg.Height, pixelData);

        return pixelBuffer.Resize<BufferGray8>(96, 96);
    }

    public static void CopyPixelBufferToTensor(IPixelBuffer pixel)
    {
        using var memStream = new MemoryStream(pixel.Buffer);
        int index = 0;
        memStream.Seek(0, SeekOrigin.Begin);
        while (index < memStream.Length)
        {
            personDetectionTF.InputData(index++, (sbyte)memStream.ReadByte());
        }
    }
}