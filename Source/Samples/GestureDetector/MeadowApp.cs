using GestureDetector.Models;
using Meadow;
using Meadow.Devices;
using Meadow.Units;
using System;
using System.Threading.Tasks;

namespace GestureDetector;

public class TensorFlowApp : ProjectLabCoreComputeApp
{
    private GestureModel gestureModel = default!;

    private float[] inputData = new float[0];

    private const double gestureDetectionThreshold = 2.5;
    private readonly string[] gestureList = { "thumbs up", "wave" };

    private int samplesRead = 0;
    private const int TargetSampleCount = 119;

    public override Task Initialize()
    {
        Resolver.Log.Info("Initialize...");

        gestureModel = new GestureModel(GestureModelData.Data);

        Hardware.Accelerometer!.Updated += OnAccelerometerUpdated;

        inputData = new float[TargetSampleCount * 3];

        return base.Initialize();
    }

    public override Task Run()
    {
        Resolver.Log.Info("Run...");

        Hardware.Accelerometer?.StartUpdating(TimeSpan.FromMilliseconds(10));

        return Task.CompletedTask;
    }

    private void OnAccelerometerUpdated(object sender, IChangeResult<Acceleration3D> e)
    {
        if (samplesRead == 0)
        {
            //do we detect movement to start the gesture detection
            double value = Math.Abs(e.New.X.Gravity) + Math.Abs(e.New.Y.Gravity) + Math.Abs(e.New.Z.Gravity);

            if (value < gestureDetectionThreshold)
            {
                return;
            }
        }

        float aX = (float)((e.New.X.Gravity + 4.0) / 8.0);
        float aY = (float)((e.New.Y.Gravity + 4.0) / 8.0);
        float aZ = (float)((e.New.Z.Gravity + 4.0) / 8.0);

        inputData[samplesRead * 3 + 0] = aX;
        inputData[samplesRead * 3 + 1] = aY;
        inputData[samplesRead * 3 + 2] = aZ;

        samplesRead++;

        if (samplesRead == TargetSampleCount)
        {
            Resolver.Log.Info("Samples Read");
            gestureModel.Inputs.SetData(inputData);
            var output = gestureModel.Predict();

            for (int i = 0; i < gestureList.Length; i++)
            {
                var tensorData = output[i];
                if (tensorData > 0.85)
                {
                    Resolver.Log.Info($"Gesture = {gestureList[i]} : {tensorData}");
                }
            }

            samplesRead = 0;
        }
    }
}