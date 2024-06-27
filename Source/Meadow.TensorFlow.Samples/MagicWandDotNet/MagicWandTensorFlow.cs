using MagicWand.Models;
using Meadow.TensorFlow;
using System;
using System.Runtime.InteropServices;

namespace MagicWand;

public class MagicWandTensorFlow : TensorFlowLite
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
        if ((TensorFlowLiteBindings.TfLiteMicroDimsSizeData(Input) != 4) || (TensorFlowLiteBindings.TfLiteMicroDimsData(Input, 0) != 1) ||
            (TensorFlowLiteBindings.TfLiteMicroDimsData(Input, 1) != 128) || (TensorFlowLiteBindings.TfLiteMicroDimsData(Input, 2) != kChannelNumber) ||
            (TensorFlowLiteBindings.TfLiteMicroGetType(Input) != TensorDataType.Float32))
        {
            Console.Write("DimsSizeData error");
        }

        Console.WriteLine("TensorFlow Initialize");
    }

    public int InputLength()
    {
        return TensorFlowLiteBindings.TfLiteMicroGetByte(Input) / sizeof(float);
    }

    public void InputData(int index, float value)
    {
        TensorFlowLiteBindings.TfLiteMicroSetFloatData(Input, index, value);
    }

    public int Predict()
    {
        for (int i = 0; i < kGestureCount; ++i)
        {
            predictionHistory[i, predictionHistoryIndex] = OutputFloatData(i);
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