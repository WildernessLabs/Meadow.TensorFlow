using HelloWorld.Models;
using Meadow;
using Meadow.Devices;
using Meadow.TensorFlow;
using System.Threading.Tasks;

namespace HelloWorld;

public class MeadowApp : App<F7FeatherV2>
{
    TensorFlowLite tensorFlowLite;

    readonly HelloWorldModel helloWorld = new();
    const int ArenaSize = 2000;

    public override Task Initialize()
    {
        Resolver.Log.Info("Initialize TensorFlow ...");

        tensorFlowLite = new TensorFlowLite(helloWorld, ArenaSize);

        helloWorld.interferenceCount = 0;

        return Task.CompletedTask;
    }

    public override Task Run()
    {
        HelloWorldModel.HelloWorldResult[] result = helloWorld.PopulateResult();

        for (int i = 0; i < result.Length; i++)
        {
            float position = helloWorld.interferenceCount / (float)helloWorld.kInterferencesPerCycles;
            float x = position * helloWorld.kXrange;

            sbyte xQuantized = (sbyte)((x / tensorFlowLite.InputParam.Scale) + tensorFlowLite.InputParam.ZeroPoint);

            tensorFlowLite.InputInt8Data(0, xQuantized);

            tensorFlowLite.Invoke();

            if (tensorFlowLite.Status != TensorFlowLiteStatus.Ok)
            {
                Resolver.Log.Info("Failed to Invoke");
                return Task.CompletedTask;
            }

            sbyte yQuantized = tensorFlowLite.OutputInt8Data(0);

            float y = (yQuantized - tensorFlowLite.OutputParam.ZeroPoint) * tensorFlowLite.OutputParam.Scale;

            Resolver.Log.Info($" {i} - {(x, y)} ");

            if (helloWorld.FloatNotEqual(x, result[i].x) || helloWorld.FloatNotEqual(y, result[i].y))
            {
                Resolver.Log.Info($"Test {i} failed");
                Resolver.Log.Info($"Expeced {(result[i].x, result[i].y)}");
                break;
            }

            helloWorld.interferenceCount += 1;

            if (helloWorld.interferenceCount >= helloWorld.kInterferencesPerCycles)
            {
                helloWorld.interferenceCount = 0;
            }
        }

        Resolver.Log.Info("Sample completed");
        return Task.CompletedTask;
    }
}