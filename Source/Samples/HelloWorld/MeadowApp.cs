using HelloWorld.Models;
using Meadow;
using Meadow.Devices;
using System;
using System.Threading.Tasks;

namespace HelloWorld;

/// <summary>
/// Meadow application that uses the HelloWorldModel to perform 
/// quantization and prediction over a series of interference cycles.
/// 
/// The application initializes the model with predefined data, then processes 
/// each reference result by quantizing a calculated position, running a prediction, 
/// dequantizing the result, and comparing it with expected values.
/// 
/// After processing all results, the application cleans up resources.
/// </summary>
public class MeadowApp : App<F7CoreComputeV2>
{
    private HelloWorldModel helloWorldModel;

    public override Task Initialize()
    {
        Resolver.Log.Info("Initialize...");

        helloWorldModel = new HelloWorldModel(HelloWorldModelData.Data);

        helloWorldModel.InterferenceCount = 0;

        Resolver.Log.Info("Initialize complete...");

        return Task.CompletedTask;
    }

    public override Task Run()
    {
        Resolver.Log.Info("Run...");

        bool isSuccessful = true;

        var result = helloWorldModel.GetReferenceResults();

        for (int i = 0; i < result.Length; i++)
        {
            float position = helloWorldModel.InterferenceCount / (float)helloWorldModel.InterferencesPerCycles;
            float x = position * helloWorldModel.XRange;

            sbyte xQuantized = Quantize(x);
            helloWorldModel.Inputs.SetData(new sbyte[] { xQuantized });

            var outputs = helloWorldModel.Predict();

            float y = Dequantize(outputs[0]);

            Resolver.Log.Info($" {i} - {(x, y)} ");

            if (!AreFloatsEqual(x, result[i].X) || !AreFloatsEqual(y, result[i].Y))
            {
                Resolver.Log.Info($"{i} failed - expected {(result[i].X, result[i].Y)}");
                isSuccessful = false;
            }

            helloWorldModel.IncrementInterferenceCount();
        }

        helloWorldModel.Dispose();

        if (isSuccessful)
        {
            Resolver.Log.Info("Test passed");
        }
        else
        {
            Resolver.Log.Info("Test failed");
        }

        return Task.CompletedTask;
    }

    private sbyte Quantize(float x)
    {
        return (sbyte)((x / helloWorldModel.InputQuantizationParams.Scale) + helloWorldModel.InputQuantizationParams.ZeroPoint);
    }

    private float Dequantize(sbyte yQuantized)
    {
        return (yQuantized - helloWorldModel.OutputQuantizationParams.ZeroPoint) * helloWorldModel.OutputQuantizationParams.Scale;
    }

    private bool AreFloatsEqual(float x, float y)
    {
        return MathF.Abs(x - y) < 1e-6;
    }
}