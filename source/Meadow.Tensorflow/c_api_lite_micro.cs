
using System;
using System.Threading.Tasks;
using System.Reflection;
using System.Runtime.InteropServices;

namespace Tensorflow.litemicro
{
    public static class c_api_lite_micro
    {
        public const string TensorFlowLibName = "Tensorflow.so";

        [DllImport(TensorFlowLibName)]
        public static extern IntPtr TfLiteMicroGetModel(int arena_size, IntPtr arena ,IntPtr model_data);

        [DllImport(TensorFlowLibName)]
        public static extern IntPtr TfLiteMicroInterpreterOptionCreate(IntPtr option);

        [DllImport(TensorFlowLibName)]
        public static extern IntPtr TfLiteMicroInterpreterCreate(IntPtr interpreter_option, IntPtr model_option);

        [DllImport(TensorFlowLibName)]
        public static extern TfLiteStatus TfLiteMicroInterpreterAllocateTensors(IntPtr interpreter);

        [DllImport(TensorFlowLibName)]
        public static extern TfLiteTensor TfLiteMicroInterpreterGetInput(IntPtr inter, int input_index);
        
        [DllImport(TensorFlowLibName)]
        public static extern TfLiteTensor TfLiteMicroInterpreterGetOutput(IntPtr inter, int input_index);

        [DllImport(TensorFlowLibName)]
        public static extern int TfLiteMicroInterpreterGetOutputCount(IntPtr inter);

        [DllImport(TensorFlowLibName)]
        public static extern int TfLiteMicroInterpreterGetInputCount(IntPtr inter);

        [DllImport(TensorFlowLibName)]
        public static extern TfLiteStatus TfLiteMicroInterpreterInvoke(IntPtr inter);
            
    }
}