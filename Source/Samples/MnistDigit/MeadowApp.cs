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

        // need to normalize the image to a float tensor of dimensions [1,28,28]
        // where
        //   dim 0 = 1 (we are sending in 1 image)
        //   dim 1 = pixel X
        //   dim 2 = pixel Y
        // each element in the array is a pixel brightness in the range of 0..1

        var tests = new string[]
            {
                "/meadow0/4.bmp",
                "/meadow0/6.bmp",
                "/meadow0/0.bmp",
                "/meadow0/2.bmp",
            };

        foreach (var test in tests)
        {
            var img = Image.LoadFromFile(test);
            Resolver.Log.Info($"{test}");
            model.Classify(img);
        }

        return Task.CompletedTask;
    }
}