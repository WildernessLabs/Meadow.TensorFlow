using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Graphics;
using System.IO;
using System.Threading.Tasks;

namespace PersonDetector_Demo;

public class MeadowApp : ProjectLabCoreComputeApp
{
    private PersonDetectorModel model;

    public override Task Initialize()
    {
        Resolver.Log.Info("Loading model...");

        var modelFile = new FileInfo("/meadow0/person_detect.tflite");
        model = new PersonDetectorModel(modelFile);

        return Task.CompletedTask;
    }

    public override Task Run()
    {
        Resolver.Log.Info("Run...");

        var tests = new string[]
            {
                "/meadow0/person.bmp",
                "/meadow0/no_person.bmp",
            };

        foreach (var test in tests)
        {
            Resolver.Log.Info($"loading {test}...");
            var img = Image.LoadFromFile(test);

            var result = model.Classify(img);
            Resolver.Log.Info($"this image is {result}");
        }

        return Task.CompletedTask;
    }
}