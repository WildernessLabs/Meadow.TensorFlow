using GestureDetector.Models;
using Meadow;
using Meadow.Devices;
using Meadow.TensorFlow;
using Meadow.Units;
using System;
using System.Threading.Tasks;

namespace GestureDetector;

public class TensorFlowApp : ProjectLabCoreComputeApp
{
    private readonly GestureModel gestureModel = new();
    private ModelInput<float> inputs = default!;

    private const double kDetectionThreshould = 2.5;
    private readonly string[] gestureList = { "thumbs up", "wave" };

    public int samplesRead = 0;
    public const int sampleCount = 119;
    public double[] accelerometerData = new double[3];

    public override Task Initialize()
    {
        Hardware.Accelerometer!.Updated += OnAccelerometerUpdated;

        inputs = gestureModel.CreateInput<float>();

        return base.Initialize();
    }

    public override async Task Run()
    {
        Hardware.Accelerometer!.StartUpdating(TimeSpan.FromMilliseconds(10));
        while (true)
        {
            while (samplesRead == sampleCount)
            {
                if (IsMovement())
                {
                    Resolver.Log.Info("Movement Detected ...");
                    samplesRead = 0;
                    break;
                }
                await Task.Delay(1);
            }

            while (samplesRead < sampleCount)
            {
                if (InputAccelerometerData())
                {
                    ModelOutput<float>? output = null;

                    if (samplesRead == sampleCount)
                    {
                        output = gestureModel.Predict(inputs);
                    }

                    if (output != null)
                    {
                        for (int i = 0; i < gestureList.Length; i++)
                        {
                            var tensorData = output[i];
                            if (tensorData > 0.85)
                            {
                                Resolver.Log.Info($"Gesture = {gestureList[i]} : {tensorData}");
                            }
                        }
                    }
                }
                await Task.Delay(5);
            }
            await Task.Delay(1);
        }
    }

    public bool IsMovement()
    {
        double threshould = Math.Abs(accelerometerData[0]) + Math.Abs(accelerometerData[1]) + Math.Abs(accelerometerData[2]);
        return threshould > kDetectionThreshould;
    }

    public bool InputAccelerometerData()
    {
        if (Hardware.Accelerometer!.IsSampling)
        {
            float aX = (float)((accelerometerData[0] + 4.0) / 8.0);
            float aY = (float)((accelerometerData[1] + 4.0) / 8.0);
            float aZ = (float)((accelerometerData[2] + 4.0) / 8.0);

            inputs[samplesRead * 3 + 0] = aX;
            inputs[samplesRead * 3 + 1] = aY;
            inputs[samplesRead * 3 + 2] = aZ;

            samplesRead++;
            return true;
        }
        return false;
    }

    private void OnAccelerometerUpdated(object sender, IChangeResult<Acceleration3D> e)
    {
        accelerometerData[0] = e.New.X.Gravity;
        accelerometerData[1] = e.New.Y.Gravity;
        accelerometerData[2] = e.New.Z.Gravity;
    }
}