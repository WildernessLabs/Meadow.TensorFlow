/*

using Meadow.TensorFlow;
using System;

namespace MagicWand;

public class MagicWandTensorFlow : TensorFlowLiteInterpreter
{
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

    public MagicWandTensorFlow(ITensorModel tensorModel, int areaSize)
        : base(tensorModel, areaSize)
    {
        if ((TensorFlowLiteBindings.TfLiteMicroDimsSizeData(InputTensor) != 4) ||
            (TensorFlowLiteBindings.TfLiteMicroDimsData(InputTensor, 0) != 1) ||
            (TensorFlowLiteBindings.TfLiteMicroDimsData(InputTensor, 1) != 128) ||
            (TensorFlowLiteBindings.TfLiteMicroDimsData(InputTensor, 2) != kChannelNumber) ||
            (TensorFlowLiteBindings.TfLiteMicroGetType(InputTensor) != TensorDataType.Float32))
        {
            Console.Write("DimsSizeData error");
        }

        Console.WriteLine("TensorFlow Initialize");
    }

    public int InputLength()
    {
        return TensorFlowLiteBindings.TfLiteMicroGetByte(InputTensor) / sizeof(float);
    }

    public void InputData(int index, float value)
    {
        TensorFlowLiteBindings.TfLiteMicroSetFloatData(InputTensor, index, value);
    }

    public int Predict()
    {
        for (int i = 0; i < kGestureCount; ++i)
        {
            predictionHistory[i, predictionHistoryIndex] = GetOutputTensorFloatData(i);
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

*/