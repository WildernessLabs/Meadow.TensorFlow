using GestureDetector.Models;
using Meadow;
using Meadow.Devices;
using Meadow.TensorFlow;
using Meadow.Units;
using System;
using System.Threading.Tasks;

namespace GestureDetector;

public class TensorFlowApp : App<F7CoreComputeV2>
{
    private Model _model;
    private ModelInput<float> _inputs;
    private readonly GestureModel gestureModelData = new();
    private const int arenaSize = 60 * 1024;

    private IProjectLabHardware projLab;
    private const double kDetectionThreshould = 2.5;
    private readonly string[] gestureList = { "thumbs up", "wave" };

    public int samplesRead = 0;
    public const int sampleCount = 119;
    public double[] accelerometerData = new double[3];

    public override Task Initialize()
    {
        projLab = ProjectLab.Create();
        projLab.Accelerometer.Updated += OnAccelerometerUpdated;

        _model = new Model(gestureModelData.Data, arenaSize);
        _inputs = _model.CreateInput<float>();

        return base.Initialize();
    }

    public override async Task Run()
    {
        projLab.Accelerometer.StartUpdating(TimeSpan.FromMilliseconds(10));
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
                        output = _model.Predict(_inputs);
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
        if (projLab.Accelerometer.IsSampling)
        {
            float aX = (float)((accelerometerData[0] + 4.0) / 8.0);
            float aY = (float)((accelerometerData[1] + 4.0) / 8.0);
            float aZ = (float)((accelerometerData[2] + 4.0) / 8.0);

            _inputs[samplesRead * 3 + 0] = aX;
            _inputs[samplesRead * 3 + 1] = aY;
            _inputs[samplesRead * 3 + 2] = aZ;

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
