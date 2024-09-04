using MagicWand.Models;
using Meadow;
using Meadow.Devices;
using System;
using System.Threading.Tasks;

namespace MagicWand;

public class MeadowApp : ProjectLabCoreComputeApp
{
    MagicWandModel wandModel;

    readonly TimeSpan UpdateInverval = TimeSpan.FromMilliseconds(40);

    const int SampleCount = 200;

    public override Task Initialize()
    {
        Resolver.Log.Info("Initialize...");

        wandModel = new MagicWandModel(MagicWandModelData.Data);

        return Task.CompletedTask;
    }

    public override async Task Run()
    {
        Resolver.Log.Info("Run...");

        while (true)
        {
            var data = await GetInputData(SampleCount);

            wandModel.Inputs.SetData(data);

            var modelOutput = wandModel.Predict();

            var gesture = wandModel.GetGesture(modelOutput);

            Console.WriteLine($"Detected {gesture} gesture");

            await Task.Delay(UpdateInverval);
        }
    }

    public async Task<float[]> GetInputData(int sampleCount)
    {
        var accelData = new float[sampleCount * 3];

        for (int i = 0; i < accelData.Length; i += 3)
        {
            var data = await Hardware.Accelerometer.Read();
            float x = (float)data.X.Gravity;
            float y = (float)data.Y.Gravity;
            float z = (float)data.Z.Gravity;

            accelData[i] = -1 * x * 1000;
            accelData[i + 1] = -1 * y * 1000;
            accelData[i + 2] = -1 * z * 1000;

            await Task.Delay(UpdateInverval);
        }

        return accelData;
    }
}