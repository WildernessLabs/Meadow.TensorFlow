using HelloWorld.Models;
using Meadow;
using Meadow.Devices;
using Meadow.TensorFlow;
using System;
using System.Threading.Tasks;

namespace HelloWorld;

public class MeadowApp : App<F7CoreComputeV2>
{
    TensorFlowLite tensorFlowLite;

    readonly HelloWorldModel helloWorldModel = new();
    const int ArenaSize = 2000;

    public override Task Initialize()
    {
        Resolver.Log.Info("Initialize TensorFlow ...");

        tensorFlowLite = new TensorFlowLite(helloWorldModel, ArenaSize);

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

            sbyte xQuantized = (sbyte)((x / tensorFlowLite.InputQuantizationParams.Scale) + tensorFlowLite.InputQuantizationParams.ZeroPoint);

            tensorFlowLite.SetInputTensorInt8Data(0, xQuantized);

            tensorFlowLite.InvokeInterpreter();

            if (tensorFlowLite.OperationStatus != TensorFlowLiteStatus.Ok)
            {
                Resolver.Log.Info("Failed to Invoke");
                return Task.CompletedTask;
            }

            sbyte yQuantized = tensorFlowLite.GetOutputTensorInt8Data(0);

            float y = (yQuantized - tensorFlowLite.OutputQuantizationParams.ZeroPoint) * tensorFlowLite.OutputQuantizationParams.Scale;

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

        Resolver.Log.Info("Sample completed");
        return Task.CompletedTask;
    }

    bool AreFloatsEqual(float x, float y)
    {
        return Math.Abs(x - y) < 1e-6;
    }
}