using System;
using System.Collections.Generic;
using System.Text;

namespace TensorFlow.litemicro
{
    /// <summary>
    /// Enum representing the internal states in TensorFlow Lite.
    /// </summary>
    public enum TfLiteStatus
    {
        /// <summary>
        /// Generally refers to a successful execution in the runtime.
        /// </summary>
        kTfLiteOk = 0,

        /// <summary>
        /// Generally referring to an error in the runtime.
        /// </summary>
        kTfLiteError = 1,

        /// <summary>
        /// Generally referring to an error from a TfLiteDelegate itself.
        /// </summary>
        kTfLiteDelegateError = 2,

        /// <summary>
        /// Generally referring to an error in applying a delegate due to incompatibility 
        /// between runtime and delegate.
        /// </summary>
        kTfLiteApplicationError = 3,

        /// <summary>
        /// Generally referring to serialized delegate data not being found.
        /// </summary>
        kTfLiteDelegateDataNotFound = 4,

        /// <summary>
        /// Generally referring to data-writing issues in delegate serialization.
        /// </summary>
        kTfLiteDelegateDataWriteError = 5,

        /// <summary>
        /// Generally referring to data-reading issues in delegate serialization.
        /// </summary>
        kTfLiteDelegateDataReadError = 6,

        /// <summary>
        /// Generally referring to issues when the TF Lite model has ops that cannot
        /// be resolved at runtime.
        /// </summary>
        kTfLiteUnresolvedOps = 7,
    }
}
