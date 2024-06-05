using BitMiracle.LibJpeg;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.Buffers;
using Meadow.Foundation.Graphics.MicroLayout;
using Meadow.Foundation.Sensors.Camera;
using Meadow.Peripherals.Displays;
using PersonDetection.Controllers;
using System;
using System.IO;
using System.Threading.Tasks;
using TensorFlow.litemicro;

namespace PersonDetection;
using tf = TensorFlow;

public class MeadowApp : App<F7CoreComputeV2>
{
    private IProjectLabHardware projLab;
    private DisplayController displayController;
    public const sbyte Person = 1;
    public const sbyte noPerson = 0;

    TfLiteTensor input, output;
    public readonly Image[] images =
    {
        Image.LoadFromResource("Resources.no_person.bmp"),
        Image.LoadFromResource("Resources.person.bmp"),
    };
    Vc0706 camera;
    private DisplayScreen displayScreen;

    public override Task Initialize()
    {
        projLab = ProjectLab.Create();
        // displayScreen = new DisplayScreen(projLab.Display, RotationType._270Degrees);
        displayController = new DisplayController(projLab.Display);

        camera = new Vc0706(Device, Device.PlatformOS.GetSerialPortName("COM1"), 38400);
        if (camera.SetCaptureResolution(Vc0706.ImageResolution._320x240))
        {
            Resolver.Log.Info("Resolution successfully changed");
        }

        Resolver.Log.Info("Initialize TensorFlow ...");
        tf.Initialize();

        input = tf.GetInput();
        return Task.CompletedTask;
    }

    public override async Task Run()
    {
        var imageBuffer = await TakePicture();
        CopyPixelBufferToTensor(imageBuffer, input);

        TfLiteStatus tfLiteStatus;
        tfLiteStatus = tf.Invoke();
        if (tfLiteStatus != TfLiteStatus.kTfLiteOk)
        {
            Resolver.Log.Info("Invoke failed");
            return;
        }

        output = tf.GetOutput();
        int outputDimsSize = tf.DimsSize(output);
        int outputData0 = tf.DimsData(output, 0);
        int outputDimsData0Size = tf.DimsData(output, 1);

        Resolver.Log.Info($" Dims Size: {outputDimsSize}, Data 0: {outputData0},  Size:{outputDimsData0Size}");

        sbyte personScore = tf.GetInt8Data(output, 1);
        sbyte noPersonScore = tf.GetInt8Data(output, 0);
        
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

    public static void CopyImageToTensor(Image img, TfLiteTensor tensor)
    {
        int width = img.DisplayBuffer.Width;
        int height = img.DisplayBuffer.Height;
        using var memStream = new MemoryStream(img.DisplayBuffer.Buffer);
        Resolver.Log.Info($"Image Infos - width: {width}, height: {height} , size: {memStream.Length}");
        int index = 0;
        memStream.Seek(0, SeekOrigin.Begin);
        while (index < memStream.Length)
        {
            tf.SetInt8Data(tensor, index++, (sbyte)memStream.ReadByte());
        }
    }
    public static void CopyPixelBufferToTensor(IPixelBuffer pixel, TfLiteTensor tensor)
    {
        using var memStream = new MemoryStream(pixel.Buffer);
        int index = 0;
        memStream.Seek(0, SeekOrigin.Begin);
        while (index < memStream.Length)
        {
            tf.SetInt8Data(tensor, index++, (sbyte)memStream.ReadByte());
        }
    }
}