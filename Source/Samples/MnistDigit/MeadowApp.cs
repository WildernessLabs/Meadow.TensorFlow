using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Graphics;
using System.IO;
using System.Threading.Tasks;

namespace Mnist_Demo;

public class MeadowApp : ProjectLabCoreComputeApp
{
    private MnistDigitModel model;

    public override Task Initialize()
    {
        Resolver.Log.Info("Loading model...");

        var modelFile = new FileInfo("/meadow0/mnist.tflite");
        model = new MnistDigitModel(modelFile);

        return Task.CompletedTask;
    }

    public override Task Run()
    {
        Resolver.Log.Info("Run...");

        var tests = new string[]
            {
                "/meadow0/4.bmp",
                "/meadow0/6.bmp",
                "/meadow0/0.bmp",
                "/meadow0/2.bmp",
            };

        foreach (var test in tests)
        {
            Resolver.Log.Info($"loading {test}...");
            var img = Image.LoadFromFile(test);

            var result = model.Classify(img);
            Resolver.Log.Info($"this image is a {result.Class}: Confidence {result.Confidence:N1}");
        }

        return Task.CompletedTask;
    }
}