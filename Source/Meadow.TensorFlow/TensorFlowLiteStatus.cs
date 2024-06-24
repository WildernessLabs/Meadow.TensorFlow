namespace Meadow.TensorFlow;

/// <summary>
/// Enum representing the internal states in TensorFlow Lite.
/// </summary>
public enum TensorFlowLiteStatus
{
    /// <summary>
    /// Generally refers to a successful execution in the runtime.
    /// </summary>
    Ok = 0,

    /// <summary>
    /// Generally referring to an error in the runtime.
    /// </summary>
    Error = 1,

    /// <summary>
    /// Generally referring to an error from a TfLiteDelegate itself.
    /// </summary>
    DelegateError = 2,

    /// <summary>
    /// Generally referring to an error in applying a delegate due to incompatibility 
    /// between runtime and delegate.
    /// </summary>
    ApplicationError = 3,

    /// <summary>
    /// Generally referring to serialized delegate data not being found.
    /// </summary>
    kTfLiteDelegateDataNotFound = 4,

    /// <summary>
    /// Generally referring to data-writing issues in delegate serialization.
    /// </summary>
    DelegateDataWriteError = 5,

    /// <summary>
    /// Generally referring to data-reading issues in delegate serialization.
    /// </summary>
    DelegateDataReadError = 6,

    /// <summary>
    /// Generally referring to issues when the TF Lite model has ops that cannot
    /// be resolved at runtime.
    /// </summary>
    UnresolvedOps = 7,
}