using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using TensorFlow.litemicro;
using MagicWandMode;
using Meadow;

namespace MeadowApp{

    public class TensorFlow
    {
        private static readonly Lazy<TensorFlow> instance = new Lazy<TensorFlow>(() => new TensorFlow());
        private const int kChannelNumber = 3; 
        private const int kGestureCount = 4;
        private const int kPredictionHistoryLength = 5;
        private const int kNoGesture = 3;
        private const float kDetectionThreshould = 0.8f;
        private const int kPredictionSuppressionDuration = 25;
        private int []kConsecutiveInterfaceThresholds = {9, 7, 6};
        private string []labels = {"M", "L", "O", "U" };
        private int predictionHistoryIndex = 0;
        private float [,]predictionHistory = new float[kGestureCount, kPredictionHistoryLength];
        private int predictionSupressionCount = 0;
    
        public static TensorFlow Instance => instance.Value;

        TfLiteStatus tfLiteStatus;
        TfLiteTensor input, output;
        IntPtr interpreter;
        int ArenaSize = 60 * 1024;

        public void Initialize()
        {
            MagicWandModel magicWandModel = new MagicWandModel();

            IntPtr model = Marshal.AllocHGlobal(magicWandModel.GetSize() * sizeof(byte));

            if (model == IntPtr.Zero)
            {
                Console.WriteLine("Failed to allocated model");
                return;
            }

            Marshal.Copy(magicWandModel.GetData(), 0, model, magicWandModel.GetSize());

            IntPtr arena = Marshal.AllocHGlobal(ArenaSize* sizeof(int));

            if (arena == IntPtr.Zero)
            {
                Console.WriteLine("Failed to allocated arena");
                Marshal.FreeHGlobal(model);
                return;
            }

            var model_options = c_api_lite_micro.TfLiteMicroGetModel(ArenaSize, arena, model);
            if (model_options == null)
                Console.WriteLine("Failed to loaded the model");

            var interpreter_options = c_api_lite_micro.TfLiteMicroInterpreterOptionCreate(model_options);
            if (interpreter_options == null)
                Console.WriteLine("Failed to create interpreter option");

            interpreter = c_api_lite_micro.TfLiteMicroInterpreterCreate(interpreter_options, model_options);
            if (interpreter == null)
                Console.WriteLine("Failed to Interpreter");

            tfLiteStatus = c_api_lite_micro.TfLiteMicroInterpreterAllocateTensors(interpreter);
            if (tfLiteStatus != TfLiteStatus.kTfLiteOk)
                Console.WriteLine("Failed to allocate tensors");

            input = c_api_lite_micro.TfLiteMicroInterpreterGetInput(interpreter, 0);

            if ((c_api_lite_micro.TfLiteMicroDimsSizeData(input) != 4) ||(c_api_lite_micro.TfLiteMicroDimsData(input, 0)!= 1) || 
                (c_api_lite_micro.TfLiteMicroDimsData(input, 1)!= 128) || (c_api_lite_micro.TfLiteMicroDimsData(input, 2)!= kChannelNumber) ||
                (c_api_lite_micro.TfLiteMicroGetType(input)!= TfLiteType.kTfLiteFloat32))
            {
                Console.Write("DimsSizeData error");
            }

            Console.WriteLine("TensorFlow Initialize");
        }

        public int InputLegth()
        {
            return c_api_lite_micro.TfLiteMicroGetByte(input)/sizeof(float);
        }

        public void InputData(int index, float value)
        { 
            c_api_lite_micro.TfLiteMicroSetFloatData(input, index, value);    
        }

        public float OutputData(int index)
        {
            return c_api_lite_micro.TfLiteMicroGetFloatData(c_api_lite_micro.TfLiteMicroInterpreterGetOutput(interpreter, 0), index);   
        }

        public TfLiteStatus Invoke()
        {
            return c_api_lite_micro.TfLiteMicroInterpreterInvoke(interpreter);
        }

        public int Predict()
        {
            for (int i = 0; i < kGestureCount; ++i)
            {
                predictionHistory[i , predictionHistoryIndex] = OutputData(i);
            }

            ++predictionHistoryIndex;
            if (predictionHistoryIndex >= kPredictionHistoryLength)
            {
                predictionHistoryIndex = 0;
            }

            float maxPredictScore = 0.0f;
            int maxPredictIndex = -1;

            for (int j = 0; j < kGestureCount; j++)
            {
                float predictionSum = 0.0f;
                for (int k = 0; k < kPredictionHistoryLength; ++k)
                {
                    predictionSum += predictionHistory[j,k];
                }

                float predictionAvarage = predictionSum /kPredictionHistoryLength;
                if (maxPredictIndex == -1 || (predictionAvarage > maxPredictScore))
                {
                    maxPredictIndex = j;
                    maxPredictScore = predictionAvarage;
                }
            }
            if (predictionSupressionCount > 0)
            {
                --predictionSupressionCount;
            }

            if (maxPredictIndex == kNoGesture || (maxPredictScore < kDetectionThreshould) || predictionSupressionCount > 0)
            {
                return kNoGesture;
            }

            predictionSupressionCount = kPredictionSuppressionDuration;

            return maxPredictIndex;
        }

        public string HandleOutput( int value)
        {
            for (int i = 0; i < (labels.Length -1); i++)
            {
                if (i == value)
                {
                    return labels[i];
                }
            }
            return null;
        }
    }
} 