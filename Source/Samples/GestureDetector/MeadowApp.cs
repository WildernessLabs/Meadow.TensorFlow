using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Hardware;
using Meadow.Units;

using System;
using System.IO;
using System.Threading.Tasks;

using Tensorflow;
using Tensorflow.litemicro;

namespace MeadowApp
{
    public class MeadowApp : App<F7CoreComputeV2>
    {
        private IProjectLabHardware projLab;
        private const double kDetectionThreshould = 2.5;
        private string[] gestureList = { "thumbsub", "clap"};

        public int samplesRead = 0;
        public const int numOfSamples = 119;
        public double[] accelerometerData = new double[3];

        public override Task Initialize()
        {
            projLab = ProjectLab.Create();
            projLab.Accelerometer.Updated += onAccelerometerUpdated;

            Tensorflow.Instance.Initialize();

            return base.Initialize();
        }

        public override async Task Run()
        {
            projLab.Accelerometer.StartUpdating(TimeSpan.FromMilliseconds(10));
            while (true)
            {
                while (samplesRead == numOfSamples)
                {
                    if (IsMoviment())
                    {
                        Resolver.Log.Info("Moviment Detected ...");
                        samplesRead = 0;
                        break;
                    }
                    await Task.Delay(1);
                }

                while (samplesRead < numOfSamples)
                {
                    if (InputAccelerometerData())
                    {
                        if (samplesRead == numOfSamples)
                        {
                            if (Tensorflow.Instance.Invoke() != TfLiteStatus.kTfLiteOk)
                            {
                                Resolver.Log.Info("Invoke falied");
                                break;
                            }
                        }
                        for (int i = 0; i < gestureList.Length; i++)
                        {
                            float tensorData = Tensorflow.Instance.OutputData(i);
                            if (tensorData > 0.85)
                            {
                                Resolver.Log.Info($"Gesture = {gestureList[i]} : {tensorData}");
                            }
                        }
                    }
                    await Task.Delay(5);
                }
                await Task.Delay(1);
            }
        }

        public bool IsMoviment()
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

                Tensorflow.Instance.InputData(samplesRead * 3 + 0, aX);
                Tensorflow.Instance.InputData(samplesRead * 3 + 1, aY);
                Tensorflow.Instance.InputData(samplesRead * 3 + 2, aZ);

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

    }
}
