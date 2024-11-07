using Meadow;
using Meadow.Foundation.Graphics;
using Meadow.TensorFlow;
using System;
using System.IO;

namespace MagicWand;

public class MnistDigitModel : Model<float>
{
    private static readonly int ArenaSize = 10 * 1024;

    public MnistDigitModel(FileInfo modelFile)
        : base(modelFile, ArenaSize)
    {
    }

    public static float[,] ResizeAndNormalize(Image image, int targetSize = 28, float lighten = 1.0f)
    {
        Resolver.Log.Info("+Normalizing...");
        // we need to resize to 28x28 for this model
        //        image.DisplayBuffer.GetPixel
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
                        var b = image.DisplayBuffer.GetPixel(kx, ky).Brightness * lighten;
                        if (b > 1.0) { b = 1.0f; }
                        sum += b;
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
        var inputs = ResizeAndNormalize(image, 28, 1.8f);
        var xLength = inputs.GetLength(0);
        var yLength = inputs.GetLength(1);
        var flattened = new float[xLength * yLength];

        var index = 0;

        for (var y = 0; y < yLength; y++)
        {
            for (var x = 0; x < xLength; x++)
            {
                flattened[index++] = inputs[x, y];
            }
        }

        Inputs.SetData(flattened);

        var prediction = this.Predict();

        for (var i = 0; i < 10; i++)
        //            for (var i = 0; i < prediction.TensorCount; i++)
        {
            Resolver.Log.Info($"Class {i}: Confidence {prediction[i]:N3}");
        }
    }
}
