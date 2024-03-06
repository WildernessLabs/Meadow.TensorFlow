using System;
using System.Collections.Generic;
using System.Text;

namespace Tensorflow.litemicro
{
    public enum TfLiteStatus
    {
        kTfLiteOk = 0,

        // Generally referring to an error in the runtime (i.e. interpreter)
        kTfLiteError = 1,

        // Generally referring to an error from a TfLiteDelegate itself.
        kTfLiteDelegateError = 2,

        // Generally referring to an error in applying a delegate due to
        // incompatibility between runtime and delegate, e.g., this error is returned
        // when trying to apply a TF Lite delegate onto a model graph that's already
        // immutable.
        kTfLiteApplicationError = 3,

        // Generally referring to serialized delegate data not being found.
        // See tflite::delegates::Serialization.
        kTfLiteDelegateDataNotFound = 4,

        // Generally referring to data-writing issues in delegate serialization.
        // See tflite::delegates::Serialization.
        kTfLiteDelegateDataWriteError = 5,

        // Generally referring to data-reading issues in delegate serialization.
        // See tflite::delegates::Serialization.
        kTfLiteDelegateDataReadError = 6,

        // Generally referring to issues when the TF Lite model has ops that cannot be
        // resolved at runtime. This could happen when the specific op is not
        // registered or built with the TF Lite framework.
        kTfLiteUnresolvedOps = 7,
    }
}
