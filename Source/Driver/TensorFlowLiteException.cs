using System;

namespace Meadow.TensorFlow;

/// <summary>
/// Represents a TensorFlow Lite exception
/// </summary>
public class TensorFlowLiteException : Exception
{
    public TensorFlowLiteStatus? Status { get; }

    internal TensorFlowLiteException(string message)
        : base(message)
    {
    }

    internal TensorFlowLiteException(string message, TensorFlowLiteStatus status)
        : base($"{message}: {status}")
    {
    }
}
