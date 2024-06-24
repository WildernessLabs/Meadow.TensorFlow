using System;
using System.Runtime.InteropServices;

namespace Meadow.TensorFlow;

public class TensorFlowLite
{
    public TensorFlowLiteTensor Input { get; private set; }
    public TensorFlowLiteTensor Output { get; private set; }

    private readonly TensorFlowLiteStatus status;

    readonly IntPtr interpreter;
    readonly int arenaSize;

    public TensorFlowLite(ITensorModel tensorModel, int arenaSize)
    {
        this.arenaSize = arenaSize;

        IntPtr model = Marshal.AllocHGlobal(tensorModel.Size * sizeof(int));

        if (model == IntPtr.Zero)
        {
            throw new Exception("Failed to allocated model");
        }

        IntPtr arena = Marshal.AllocHGlobal(arenaSize * sizeof(int));

        if (arena == IntPtr.Zero)
        {
            Marshal.FreeHGlobal(model);
            throw new Exception("Failed to allocated arena");
        }

        Marshal.Copy(tensorModel.Data, 0, model, tensorModel.Size);

        var modelOptions = TensorFlowLiteBindings.TfLiteMicroGetModel(arenaSize, arena, model);
        if (modelOptions == null)
        {
            throw new Exception("Failed to loaded the model");
        }

        var interpreterOptions = TensorFlowLiteBindings.TfLiteMicroInterpreterOptionCreate(modelOptions);
        if (interpreterOptions == null)
        {
            throw new Exception("Failed to create interpreter option");
        }

        interpreter = TensorFlowLiteBindings.TfLiteMicroInterpreterCreate(interpreterOptions, modelOptions);
        if (interpreter == null)
        {
            throw new Exception("Failed to Interpreter");
        }

        status = TensorFlowLiteBindings.TfLiteMicroInterpreterAllocateTensors(interpreter);

        if (status != TensorFlowLiteStatus.Ok)
        {
            throw new Exception("Failed to allocate tensors");
        }

        Input = TensorFlowLiteBindings.TfLiteMicroInterpreterGetInput(interpreter, 0);
        Output = TensorFlowLiteBindings.TfLiteMicroInterpreterGetOutput(interpreter, 0);
    }
}
