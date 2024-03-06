using System;
using System.Collections;

namespace Tensorflow.litemicro{

    public struct TfLiteQuantizationParams{
        public float scale;
        public int zero_point;
    };

}