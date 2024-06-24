using System;

namespace Meadow.TensorFlow;

public class TensorFlowLite
{
    private readonly TensorFlowLiteStatus tfLiteStatus;
    private readonly TensorFlowLiteTensor input;
    readonly IntPtr interpreter;
    readonly int ArenaSize = 60 * 1024;

    public TensorFlowLite()
    {

    }
}
