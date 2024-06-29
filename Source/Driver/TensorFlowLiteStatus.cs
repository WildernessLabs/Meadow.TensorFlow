namespace Meadow.TensorFlow;

/// <summary>
/// Enum representing the internal states in TensorFlow Lite.
/// </summary>
public enum TensorFlowLiteStatus
{
    /// <summary>
    /// Generally refers to a successful execution in the TensorFlow Lite runtime.
    /// </summary>
    Ok = 0,

    /// <summary>
    /// Generally refers to an error in the TensorFlow Lite runtime.
    /// </summary>
    Error = 1,

    /// <summary>
    /// Generally refers to an error from a TensorFlow Lite delegate itself.
    /// </summary>
    DelegateError = 2,

    /// <summary>
    /// Generally refers to an error in applying a delegate due to incompatibility 
    /// between the TensorFlow Lite runtime and the delegate.
    /// </summary>
    ApplicationError = 3,

    /// <summary>
    /// Generally refers to serialized delegate data not being found.
    /// </summary>
    DelegateDataNotFound = 4,

    /// <summary>
    /// Generally refers to data-writing issues in delegate serialization.
    /// </summary>
    DelegateDataWriteError = 5,

    /// <summary>
    /// Generally refers to data-reading issues in delegate serialization.
    /// </summary>
    DelegateDataReadError = 6,

    /// <summary>
    /// Generally refers to issues when the TensorFlow Lite model has operations that cannot
    /// be resolved at runtime.
    /// </summary>
    UnresolvedOps = 7,
}