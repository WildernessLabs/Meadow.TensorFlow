namespace Meadow.Foundation.RTLite;

/// <summary>
/// Enum representing the internal states in RTLite.
/// </summary>
public enum RuntimeStatus
{
    /// <summary>
    /// Successful execution in the RTLite runtime.
    /// </summary>
    Ok = 0,

    /// <summary>
    /// Error in the RTLite runtime.
    /// </summary>
    Error = 1,

    /// <summary>
    /// Error from a RTLite delegate itself.
    /// </summary>
    DelegateError = 2,

    /// <summary>
    /// Error in applying a delegate due to incompatibility 
    /// between the RTLite runtime and the delegate.
    /// </summary>
    ApplicationError = 3,

    /// <summary>
    /// Serialized delegate data not found.
    /// </summary>
    DelegateDataNotFound = 4,

    /// <summary>
    /// Data-writing issues in delegate serialization.
    /// </summary>
    DelegateDataWriteError = 5,

    /// <summary>
    /// Data-reading issues in delegate serialization.
    /// </summary>
    DelegateDataReadError = 6,

    /// <summary>
    /// Issues when the RTLite model has operations that cannot
    /// be resolved at runtime.
    /// </summary>
    UnresolvedOps = 7,
}
