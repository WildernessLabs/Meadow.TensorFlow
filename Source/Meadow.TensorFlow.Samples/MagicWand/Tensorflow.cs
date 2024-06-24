using MagicWand.Models;
using Meadow.TensorFlow;
using System;
using System.Runtime.InteropServices;

namespace MagicWand;

public class TensorFlow
{
    private static readonly Lazy<TensorFlow> instance = new();
    private const int kChannelNumber = 3;
    private const int kGestureCount = 4;
    private const int kPredictionHistoryLength = 5;
    private const int kNoGesture = 3;
    private const float kDetectionThreshould = 0.8f;
    private const int kPredictionSuppressionDuration = 25;
    private readonly int[] kConsecutiveInterfaceThresholds = { 9, 7, 6 };
    private readonly string[] labels = { "M", "L", "O", "U" };
    private int predictionHistoryIndex = 0;
    private readonly float[,] predictionHistory = new float[kGestureCount, kPredictionHistoryLength];
    private int predictionSupressionCount = 0;

    public static TensorFlow Instance => instance.Value;

    private TensorFlowLiteStatus tfLiteStatus;
    private TensorFlowLiteTensor input;
    IntPtr interpreter;
    readonly int ArenaSize = 60 * 1024;

    public void Initialize()
    {
        MagicWandModel magicWandModel = new();

        IntPtr model = Marshal.AllocHGlobal(magicWandModel.GetSize() * sizeof(byte));

        if (model == IntPtr.Zero)
        {
            Console.WriteLine("Failed to allocated model");
            return;
        }

        Marshal.Copy(magicWandModel.GetData(), 0, model, magicWandModel.GetSize());

        IntPtr arena = Marshal.AllocHGlobal(ArenaSize * sizeof(int));

        if (arena == IntPtr.Zero)
        {
            Console.WriteLine("Failed to allocated arena");
            Marshal.FreeHGlobal(model);
            return;
        }

        var model_options = TensorFlowLiteBindings.TfLiteMicroGetModel(ArenaSize, arena, model);
        if (model_options == null)
            Console.WriteLine("Failed to loaded the model");

        var interpreter_options = TensorFlowLiteBindings.TfLiteMicroInterpreterOptionCreate(model_options);
        if (interpreter_options == null)
            Console.WriteLine("Failed to create interpreter option");

        interpreter = TensorFlowLiteBindings.TfLiteMicroInterpreterCreate(interpreter_options, model_options);
        if (interpreter == null)
            Console.WriteLine("Failed to Interpreter");

        tfLiteStatus = TensorFlowLiteBindings.TfLiteMicroInterpreterAllocateTensors(interpreter);
        if (tfLiteStatus != TensorFlowLiteStatus.Ok)
            Console.WriteLine("Failed to allocate tensors");

        input = TensorFlowLiteBindings.TfLiteMicroInterpreterGetInput(interpreter, 0);

        if ((TensorFlowLiteBindings.TfLiteMicroDimsSizeData(input) != 4) || (TensorFlowLiteBindings.TfLiteMicroDimsData(input, 0) != 1) ||
            (TensorFlowLiteBindings.TfLiteMicroDimsData(input, 1) != 128) || (TensorFlowLiteBindings.TfLiteMicroDimsData(input, 2) != kChannelNumber) ||
            (TensorFlowLiteBindings.TfLiteMicroGetType(input) != TensorDataType.Float32))
        {
            Console.Write("DimsSizeData error");
        }

        Console.WriteLine("TensorFlow Initialize");
    }

    public int InputLegth()
    {
        return TensorFlowLiteBindings.TfLiteMicroGetByte(input) / sizeof(float);
    }

    public void InputData(int index, float value)
    {
        TensorFlowLiteBindings.TfLiteMicroSetFloatData(input, index, value);
    }

    public float OutputData(int index)
    {
        return TensorFlowLiteBindings.TfLiteMicroGetFloatData(TensorFlowLiteBindings.TfLiteMicroInterpreterGetOutput(interpreter, 0), index);
    }

    public TensorFlowLiteStatus Invoke()
    {
        return TensorFlowLiteBindings.TfLiteMicroInterpreterInvoke(interpreter);
    }

    public int Predict()
    {
        for (int i = 0; i < kGestureCount; ++i)
        {
            predictionHistory[i, predictionHistoryIndex] = OutputData(i);
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
                predictionSum += predictionHistory[j, k];
            }

            float predictionAvarage = predictionSum / kPredictionHistoryLength;
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

    public string HandleOutput(int value)
    {
        for (int i = 0; i < (labels.Length - 1); i++)
        {
            if (i == value)
            {
                return labels[i];
            }
        }
        return null;
    }
}