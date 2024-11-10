using Meadow;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.Buffers;
using Meadow.Foundation.RTLite;
using Meadow.Peripherals.Displays;
using System.IO;

namespace Mnist_Demo;

public class MnistDigitModel : Model<float>
{
    private static readonly int ArenaSize = 10 * 1024;

    public MnistDigitModel(FileInfo modelFile)
        : base(modelFile, ArenaSize)
    {
    }

    public static float[] ResizeAndNormalize(Image image, int targetSize = 28)
    {
        image.ConvertAndResize(ColorMode.Format8bppGray, 28, 28);
        var resized = (image.DisplayBuffer as PixelBufferBase).Resize<BufferGray8>(28, 28);

        var normalizedData = new float[resized.ByteCount];

        for (int y = 0; y < targetSize; y++)
        {
            for (int x = 0; x < targetSize; x++)
            {
                var b = resized.GetPixel(x, y).Brightness;
                if (b > 1.0) { b = 1.0f; }
                normalizedData[y * targetSize + x] = b;
            }
        }

        return normalizedData;
    }

    public void Classify(Image image)
    {
        var inputs = ResizeAndNormalize(image, 28);

        Inputs.SetData(inputs);

        var prediction = this.Predict();

        var result = prediction.GetMaxElementIndexAndValue(10);

        Resolver.Log.Info($"this image is a {result.Class}: Confidence {result.Confidence:N1}");
    }
}
