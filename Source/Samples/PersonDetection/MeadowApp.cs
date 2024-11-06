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
    PersonDetectionModel personDetectionModel;

    Vc0706 camera;
    private DisplayScreen displayScreen;

    public override Task Initialize()
    {
        personDetectionModel = new PersonDetectionModel(PersonDetectionModelData.Data);
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
        var dataInput = await CopyPixelBufferToTensor(imageBuffer);
        personDetectionModel.Inputs.SetData(dataInput);

        var modelOutput =  personDetectionModel.Predict();

        var result = personDetectionModel.GetOutputDetection(modelOutput);

        displayController.ShowImage(96, 96, imageBuffer);
        displayController.ShowClassification((int)result, modelOutput);
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

    public async Task<sbyte[]> CopyPixelBufferToTensor(IPixelBuffer pixel)
    {
        using var memStream = new MemoryStream(pixel.Buffer);
        sbyte[] data = new sbyte[memStream.Length]; 

        int index = 0;
        memStream.Seek(0, SeekOrigin.Begin);
        while (index < memStream.Length)
        {
            data[index] = (sbyte)memStream.ReadByte();
            index++;
        }
        return data;
    }
}