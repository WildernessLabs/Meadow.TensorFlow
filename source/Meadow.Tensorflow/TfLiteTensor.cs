using System;

namespace Tensorflow.litemicro {
    public struct TfLiteTensor
    {
        private IntPtr _handle;
        public TfLiteTensor(IntPtr handle)
            =>_handle = handle;
        public static implicit operator TfLiteTensor(IntPtr handle)
            => new TfLiteTensor(handle);
        public static implicit operator IntPtr(TfLiteTensor tensor)
            => tensor._handle;
    }
}