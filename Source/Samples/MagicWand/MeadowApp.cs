using MagicWand.Models;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Sensors.Motion;
using System;
using System.Threading.Tasks;

namespace MagicWand;

public class MeadowApp : App<F7FeatherV2>
{
    MagicWandModel wandModel;

    Mpu6050 accelerometer;

    readonly TimeSpan UpdateInverval = TimeSpan.FromMilliseconds(40);

    const int SampleCount = 200;

    public override Task Initialize()
    {
        Resolver.Log.Info("Initialize...");

        wandModel = new MagicWandModel(MagicWandModelData.Data);

        accelerometer = new Mpu6050(Device.CreateI2cBus());

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

            await Task.Delay(UpdateInverval);
        }
    }

    public async Task<float[]> GetInputData(int sampleCount)
    {
        var accelData = new float[sampleCount * 3];

        for (int i = 0; i < accelData.Length; i += 3)
        {
            var data = await accelerometer.Read();
            float x = (float)data.Acceleration3D?.X.Gravity;
            float y = (float)data.Acceleration3D?.Y.Gravity;
            float z = (float)data.Acceleration3D?.Z.Gravity;

            accelData[i] = -1 * x * 1000;
            accelData[i + 1] = -1 * y * 1000;
            accelData[i + 2] = -1 * z * 1000;

            await Task.Delay(UpdateInverval);
        }

        return accelData;
    }
}