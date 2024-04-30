﻿using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Sensors.Motion;
using System;
using System.Threading.Tasks;
using TensorFlow.litemicro;

namespace MagicWand;

public class MeadowApp : App<F7FeatherV2>
{
    Mpu6050 mpu;
    static long lastTime = 0;
    const int updateTime = 40;
    public const int ringBuffer = 600;
    public double[] saveAccelData = new double[ringBuffer];
    public int beingIndex = 0;
    bool pendingInitialData = true;
    bool clearBuffer = false;
    public int InputLegth;

    public override Task Initialize()
    {
        TensorFlow.Instance.Initialize();
        InputLegth = TensorFlow.Instance.InputLegth();

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

                if (TensorFlow.Instance.Invoke() != TfLiteStatus.kTfLiteOk)
                {
                    Resolver.Log.Info("Invoke failed");
                    break;
                }

                int gestureIndex = TensorFlow.Instance.Predict();
                string gesture = TensorFlow.Instance.HandleOutput(gestureIndex);
                if (gesture != null)
                {
                    Resolver.Log.Info($"Gesture = {gesture}");
                }
            }

            await Task.Delay(1);
        }
    }

    long millis()
    {
        return DateTime.Now.Ticks / TimeSpan.TicksPerMillisecond;
    }

    public async Task<bool> ReadAccelerometer(bool resetBuffer)
    {

        if (millis() - lastTime < updateTime)
        {
            return false;
        }

        lastTime = millis();

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
            TensorFlow.Instance.InputData(i, (float)saveAccelData[ringArryIndex]);
        }

        return true;
    }
}