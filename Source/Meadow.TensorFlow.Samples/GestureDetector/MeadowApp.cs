using GestureDetector.Controllers;
using Meadow;
using Meadow.Devices;
using Meadow.Units;
using System;
using System.Threading.Tasks;
using TensorFlow.litemicro;

namespace GestureDetector;

public class MeadowApp : App<F7CoreComputeV2>
{
    private IProjectLabHardware projLab;
    private const double kDetectionThreshould = 2.5;
    private string[] gestureList = { "thumbs down", "flex", "wave", "punch", "spin" };

    private DisplayController displayController;

    public int samplesRead = 0;
    public const int numOfSamples = 119;
    public double[] accelerometerData = new double[3];

    public override Task Initialize()
    {
        projLab = ProjectLab.Create();
        projLab.Accelerometer.Updated += onAccelerometerUpdated;

        displayController = new DisplayController(projLab.Display);

        TensorFlow.Instance.Initialize();

        return base.Initialize();
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

            TensorFlow.Instance.InputData(samplesRead * 3 + 0, aX);
            TensorFlow.Instance.InputData(samplesRead * 3 + 1, aY);
            TensorFlow.Instance.InputData(samplesRead * 3 + 2, aZ);

            samplesRead++;
            return true;
        }
        return false;
    }

    private void onAccelerometerUpdated(object sender, IChangeResult<Acceleration3D> e)
    {
        accelerometerData[0] = e.New.X.Gravity;
        accelerometerData[1] = e.New.Y.Gravity;
        accelerometerData[2] = e.New.Z.Gravity;
    }

    public override async Task Run()
    {
        projLab.Accelerometer.StartUpdating(TimeSpan.FromMilliseconds(10));

        while (true)
        {
            displayController.ShowReady();

            while (samplesRead == numOfSamples)
            {
                if (IsMovement())
                {
                    Resolver.Log.Info("Movement Detected ...");
                    displayController.ShowMovementDetected();
                    samplesRead = 0;
                    break;
                }
                await Task.Delay(1);
            }

            while (samplesRead < numOfSamples)
            {
                InputAccelerometerData();
                await Task.Delay(1);
            }

            if (samplesRead == numOfSamples)
            {
                if (TensorFlow.Instance.Invoke() != TfLiteStatus.kTfLiteOk)
                {
                    Resolver.Log.Info("Invoke falied");
                    break;
                }
            }

            bool gestureDetected = false;
            for (int i = 0; i < gestureList.Length; i++)
            {
                float tensorData = TensorFlow.Instance.OutputData(i);
                 if (tensorData > 0.95)
                {
                    gestureDetected = true;
                    Resolver.Log.Info($"Gesture = {gestureList[i]} : {tensorData}");
                    displayController.ShowGestureDetected(i, tensorData);
                }

            }

            if (!gestureDetected)
            {
                displayController.ShowGestureNotRecognized();
            }

            await Task.Delay(1000);
        }
    }
}