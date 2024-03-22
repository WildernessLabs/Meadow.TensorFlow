using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Hardware;
using Meadow.Peripherals.Sensors;
using Meadow.Units;

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

enum TrainingStep
{
    Idle = 0,
    RestartTraining = 1,
    SaveTraining = 2,
    StartTraining = 3,
    StopTraning = 4,
    Traning = 5,
};

namespace MeadowApp
{
    public class MeadowApp : App<F7CoreComputeV2>
    {
        private IProjectLabHardware projLab;
        private static StreamWriter fs;

        public double []accelerometerData = new double[3];

        public string []trainingFile = {"output_M_", "output_W_", "output_O_", "output_L_"};
        public string []gestureTraining = {"M", "W", "O", "L"};
        public string currentfilename;

        
        public const string testerName = "halysson";
        public const double acclerometerThreshould = 2.50;
        public const int numOfSamples = 119;

        TrainingStep CurrentStep;

        public override Task Initialize()
        {
            projLab = ProjectLab.Create();
            projLab.Accelerometer.Updated += onAccelerometerUpdated;

            projLab.RightButton.Clicked += (s, e) => CurrentStep = TrainingStep.SaveTraining;
            projLab.LeftButton.Clicked += (s, e) => CurrentStep =  TrainingStep.RestartTraining;
            projLab.UpButton.Clicked += (s, e) => CurrentStep = TrainingStep.StartTraining;     
            projLab.DownButton.Clicked += (s, e) => CurrentStep = TrainingStep.StopTraning;
            CurrentStep = TrainingStep.Idle;

            Resolver.Log.Info(currentfilename);

            return base.Initialize();
        }

        public override async Task Run()
        {
            int indexTraining = 0;
            projLab.Accelerometer.StartUpdating(TimeSpan.FromMilliseconds(10));
            
            while (true)
            {
                if (CurrentStep != TrainingStep.Idle)
                {
                    switch (CurrentStep)
                    {
                        case TrainingStep.StartTraining:
                        {
                            Resolver.Log.Info($"Starting gesture training - {gestureTraining[indexTraining]} ...");
                            currentfilename = trainingFile[indexTraining] + testerName + ".txt";
                            CreateFile(currentfilename);
                            await DataTask();
                        }
                        break;
        
                        case TrainingStep.RestartTraining:
                        {
                            Resolver.Log.Info("Restart Training");
                            ClearFile(currentfilename);
                        }
                        break;

                        case TrainingStep.SaveTraining:
                        {
                            Resolver.Log.Info($"{currentfilename} save !");
                            CloseFile();
                            indexTraining ++;    
                        }
                        break;
                    }
                    CurrentStep = TrainingStep.Idle;
                }

                if (indexTraining >= trainingFile.Length)
                {
                    Resolver.Log.Info("Training Completed");
                    break;
                }
                await Task.Delay(5);
            }
            fs.Dispose();
        }
        Task DataTask()
        {
            return Task.Run(new Action(CaptureAccelerometerData));
        }
        bool DetectMoviment()
        {
            double threshould = Math.Abs(accelerometerData[0]) + Math.Abs(accelerometerData[1]) + Math.Abs(accelerometerData[2]);
            return threshould > acclerometerThreshould;
        }
        void CaptureAccelerometerData()
        {
            int samples = 0;

            while (true)
            {
                if (CurrentStep == TrainingStep.StopTraning)
                {
                    Resolver.Log.Info("Stop Traning ...");
                    break;
                }

                if (DetectMoviment())
                {
                    Resolver.Log.Info("Moviment detected");
                    while (samples < numOfSamples)
                    {
                        if (projLab.Accelerometer.IsSampling)
                        {
                            string value = $"{accelerometerData[0]}, {accelerometerData[1]}, {accelerometerData[2]}";
                            WriteFile(value);
                            Resolver.Log.Info(value);

                            samples ++;
                        }
                        Thread.Sleep(10);
                    }
                    Resolver.Log.Info("Samples collected");
                    break;
                }
                Thread.Sleep(10);
            }
        }

        void CreateFile(string filename)
        {
            try
            {
                fs = File.CreateText(Path.Combine(MeadowOS.FileSystem.UserFileSystemRoot, filename));
            }
            catch (Exception ex)
            {
                Resolver.Log.Info(ex.Message);
            }
        }
        void CloseFile()
        {
            try
            {
                fs.Close();   
            }
            catch (Exception ex)
            {
                Resolver.Log.Info($"{ex.Message}");
            }
        }
        void WriteFile(string value)
        {
            try
            {
                fs.WriteLine(value);
            }
            catch (Exception ex)
            {
                Resolver.Log.Info($"{ex.Message}");
            }
        }
        void ClearFile(string filename)
        {
            try
            {
                fs.WriteLine(Path.Combine(MeadowOS.FileSystem.UserFileSystemRoot, filename), string.Empty);
            }
            catch (Exception ex)
            {
                Resolver.Log.Info($"{ex.Message}");
            }
        }

        private void onAccelerometerUpdated(object sender, IChangeResult<Acceleration3D> e)
        {
            accelerometerData[0] = e.New.X.Gravity;
            accelerometerData[1] = e.New.Y.Gravity;
            accelerometerData[2] = e.New.Z.Gravity;
        }

    }
}

