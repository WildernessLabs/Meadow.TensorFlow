using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Graphics;
using Meadow.TensorFlow;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MagicWand;

public class MnistDigitModel : Model<float>
{
    private static readonly int ArenaSize = 10 * 1024;

    public MnistDigitModel(FileInfo modelFile)
        : base(modelFile, ArenaSize)
    {
    }

    public static float[,] ResizeAndNormalize(Image image)
    {
        Resolver.Log.Info("+Normalizing...");
        // we need to resize to 32x32 for this model
        //        image.DisplayBuffer.GetPixel

        int targetSize = 32;
        float scaleX = image.Width / (float)targetSize;
        float scaleY = image.Height / (float)targetSize;

        float[,] resizedNormalizedPixels = new float[targetSize, targetSize];

        for (int y = 0; y < targetSize; y++)
        {
            for (int x = 0; x < targetSize; x++)
            {
                // Calculate the boundaries of the "block" in the original image
                int startX = (int)(x * scaleX);
                int endX = (int)((x + 1) * scaleX);
                int startY = (int)(y * scaleY);
                int endY = (int)((y + 1) * scaleY);

                // Clamp to the edges of the image in case of rounding issues
                endX = Math.Min(endX, image.Width);
                endY = Math.Min(endY, image.Height);

                // Calculate the average pixel value within this block
                float sum = 0;
                int pixelCount = 0;
                for (int ky = startY; ky < endY; ky++)
                {
                    for (int kx = startX; kx < endX; kx++)
                    {
                        sum += image.DisplayBuffer.GetPixel(kx, ky).Brightness;
                        pixelCount++;
                    }
                }

                // Average the sum and normalize to [0, 1]
                float average = sum / pixelCount;
                resizedNormalizedPixels[x, y] = average / 255.0f;
            }
        }

        Resolver.Log.Info("-Normalizing...");
        return resizedNormalizedPixels;
    }

    public void Classify(Image image)
    {
        var inputs = ResizeAndNormalize(image);
        var flattened = Enumerable
            .Range(0, inputs.GetLength(0))
            .SelectMany(x => Enumerable.Range(0, inputs.GetLength(1))
            .SelectMany(y => new float[] { x, inputs[x, y] }))
            .ToArray();

        Inputs.SetData(flattened);

        var prediction = this.Predict();

        for (var i = 0; i < prediction.TensorCount; i++)
        {
            Resolver.Log.Info($"Prediction[{i}]: {prediction[i]:N3}");
        }
    }
}

public class MeadowApp : ProjectLabCoreComputeApp
{
    private MnistDigitModel model;

    public override Task Initialize()
    {
        Resolver.Log.Info("Loading model...");

        var modelFile = new FileInfo("/meadow0/mnist.quantized.tflite");
        model = new MnistDigitModel(modelFile);

        return Task.CompletedTask;
    }

    public override Task Run()
    {
        Resolver.Log.Info("Run...");

        // need to normalize the image to a float tensor of dimensions [1,28,28]
        // where
        //   dim 0 = 1 (we are sending in 1 image)
        //   dim 1 = pixel X
        //   dim 2 = pixel Y
        // each element in the array is a pixel brightness in the range of 0..1

        var img = Image.LoadFromFile("/meadow0/4.bmp");

        model.Classify(img);

        return Task.CompletedTask;
    }
}