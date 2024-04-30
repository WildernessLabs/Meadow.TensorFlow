using System;
using System.Collections;

namespace TensorFlow.litemicro
{
    public struct TfLiteQuantizationParams
    {
        public float scale;
        public int zero_point;
    };
}