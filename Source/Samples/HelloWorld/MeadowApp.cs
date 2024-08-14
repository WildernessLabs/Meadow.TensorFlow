using HelloWorld.Models;
using Meadow;
using Meadow.Devices;
using System;
using System.Threading.Tasks;

namespace HelloWorld;

public class MeadowApp : App<F7CoreComputeV2>
{
    private readonly HelloWorldModel helloWorldModel = new();

    public override Task Initialize()
    {
        Resolver.Log.Info("Initialize TensorFlow ...");

        helloWorldModel.InterferenceCount = 0;

        return Task.CompletedTask;
    }

    public override Task Run()
    {
        var result = helloWorldModel.PopulateResult();

        for (int i = 0; i < result.Length; i++)
        {
            float position = helloWorldModel.InterferenceCount / (float)helloWorldModel.InterferencesPerCycles;
            float x = position * helloWorldModel.XRange;

            sbyte xQuantized = (sbyte)((x / helloWorldModel.InputQuantizationParams.Scale) + helloWorldModel.InputQuantizationParams.ZeroPoint);

            var inputs = helloWorldModel.CreateInput(new sbyte[] { xQuantized });

            var outputs = helloWorldModel.Predict(inputs);

            sbyte yQuantized = outputs[0];

            float y = (yQuantized - helloWorldModel.OutputQuantizationParams.ZeroPoint) * helloWorldModel.OutputQuantizationParams.Scale;

            Resolver.Log.Info($" {i} - {(x, y)} ");

            if (!AreFloatsEqual(x, result[i].X) || !AreFloatsEqual(y, result[i].Y))
            {
                Resolver.Log.Info($"Test {i} failed");
                Resolver.Log.Info($"Expeced {(result[i].X, result[i].Y)}");
                break;
            }

            helloWorldModel.InterferenceCount += 1;

            if (helloWorldModel.InterferenceCount >= helloWorldModel.InterferencesPerCycles)
            {
                helloWorldModel.InterferenceCount = 0;
            }
        }

        helloWorldModel.Dispose();

        Resolver.Log.Info("Sample completed");
        return Task.CompletedTask;
    }

    private bool AreFloatsEqual(float x, float y)
    {
        return Math.Abs(x - y) < 1e-6;
    }
}