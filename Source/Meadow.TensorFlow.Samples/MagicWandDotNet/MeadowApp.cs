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
    MagicWandTensorFlow wandTensorFlow;

    readonly MagicWandModel wandModel = new();
    const int ArenaSize = 60 * 1024;

    Mpu6050 mpu;
    static long lastTime = 0;
    const int updateTime = 40;
    public const int ringBuffer = 600;
    public double[] saveAccelData = new double[ringBuffer];
    public int beingIndex = 0;
    bool pendingInitialData = true;
    readonly bool clearBuffer = false;
    public int InputLegth;

    long millis => DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;

    public override Task Initialize()
    {
        wandTensorFlow = new MagicWandTensorFlow(wandModel, ArenaSize);

        mpu = new Mpu6050(Device.CreateI2cBus());

        return Task.CompletedTask;
    }

    public override async Task Run()
    {
        Console.WriteLine("Tensor Flow completed");

        while (true)
        {
            if (await ReadAccelerometer(clearBuffer))
            {
                wandTensorFlow.Invoke();
                if (wandTensorFlow.Status != TensorFlowLiteStatus.Ok)
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



    public async Task<bool> ReadAccelerometer(bool resetBuffer)
    {

        if (millis - lastTime < updateTime)
        {
            return false;
        }

        lastTime = millis;

        var result = await mpu.Read();
        double x = (double)result.Acceleration3D?.X.Gravity;
        double y = (double)result.Acceleration3D?.Y.Gravity;
        double z = (double)result.Acceleration3D?.Z.Gravity;

        saveAccelData[beingIndex++] = -1 * x * 1000;
        saveAccelData[beingIndex++] = -1 * y * 1000;
        saveAccelData[beingIndex++] = -1 * z * 1000;

        if (beingIndex >= ringBuffer)
        {
            beingIndex = 0;
        }

        mpu.StartUpdating(TimeSpan.FromMilliseconds(40));

        if (pendingInitialData && beingIndex >= 200)
        {
            pendingInitialData = false;
        }

        if (pendingInitialData)
        {
            return false;
        }

        for (int i = 0; i < InputLegth; ++i)
        {
            int ringArryIndex = beingIndex + i - InputLegth;
            if (ringArryIndex < 0)
            {
                ringArryIndex += ringBuffer;
            }

            //    Console.Write($"{(i,(float)saveAccelData[ringArryIndex])}");
            wandTensorFlow.InputFloatData(i, (float)saveAccelData[ringArryIndex]);
        }

        return true;
    }
}