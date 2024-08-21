using MagicWand.Models;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Sensors.Motion;
using Meadow.TensorFlow;
using System;
using System.Threading.Tasks;

namespace MagicWand;

public class MeadowApp : App<F7FeatherV2>
{
    readonly MagicWandModel wandModel = new();

    Mpu6050 accelerometer;

    readonly TimeSpan UpdateInverval = TimeSpan.FromMilliseconds(40);

    const int SampleCount = 200;

    public override Task Initialize()
    {
        accelerometer = new Mpu6050(Device.CreateI2cBus());

        return Task.CompletedTask;
    }

    public override async Task Run()
    {
        Console.WriteLine("Tensor Flow completed");

        var data = await GetInputData();

        wandModel.Inputs.SetData(data);

        var modelOutput = wandModel.Predict();


        while (true)
        {
            if (await GetInputData())
            {
                var status = wandTensorFlow.InvokeInterpreter();
                if (status != TensorFlowLiteStatus.Ok)
                {
                    Resolver.Log.Info("Invoke failed");
                    break;
                }

                int gestureIndex = wandTensorFlow.Predict();
                string gesture = wandTensorFlow.HandleOutput(gestureIndex);
                if (gesture != null)
                {
                    Resolver.Log.Info($"Gesture = {gesture}");
                }
            }

            await Task.Delay(1);
        }
    }

    public async Task<float[]> GetInputData()
    {
        var saveAccelData = new float[SampleCount * 3];

        for (int i = 0; i < saveAccelData.Length; i += 3)
        {
            var result = await accelerometer.Read();
            float x = (float)result.Acceleration3D?.X.Gravity;
            float y = (float)result.Acceleration3D?.Y.Gravity;
            float z = (float)result.Acceleration3D?.Z.Gravity;

            saveAccelData[i] = -1 * x * 1000;
            saveAccelData[i + 1] = -1 * y * 1000;
            saveAccelData[i + 2] = -1 * z * 1000;

            await Task.Delay(UpdateInverval);
        }

        return saveAccelData;
    }
}