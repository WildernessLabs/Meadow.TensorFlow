using Meadow.TensorFlow;
using System;

namespace PersonDetection;

public class PersonDetectionTensorFlow : TensorFlowLite
{
    private const int kNumberRows = 96;
    private const int kNumberCols = 96;
    public PersonDetectionTensorFlow(ITensorModel tensorModel, int areaSize)
        : base(tensorModel, areaSize)
    {
        if(TensorFlowLiteBindings.TfLiteMicroDimsSizeData(InputTensor) != 4 || (TensorFlowLiteBindings.TfLiteMicroDimsData(InputTensor, 0) != 1) ||
        (TensorFlowLiteBindings.TfLiteMicroDimsData(InputTensor, 1) != kNumberRows) || (TensorFlowLiteBindings.TfLiteMicroDimsData(InputTensor, 2) != kNumberCols) ||
        (TensorFlowLiteBindings.TfLiteMicroGetType(InputTensor) != TensorDataType.Int8))
        {
            throw new Exception("Model loaded is incompatible");
        }
        Console.Write("TensorFlow Initialize");
    }

    public void InputData(int index, sbyte value)
    {
        TensorFlowLiteBindings.TfLiteMicroSetInt8Data(InputTensor, index, value);
    }
}