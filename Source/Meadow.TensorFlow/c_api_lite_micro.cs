
using System;
using System.Runtime.InteropServices;

namespace TensorFlow.litemicro
{
    public static class c_api_lite_micro
    {
        public const string TensorFlowLibName = "TensorFlow.so";

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

        [DllImport(TensorFlowLibName)]
        public static extern sbyte TfLiteMicroGeInt8tData(TfLiteTensor tensor, int index);

        [DllImport(TensorFlowLibName)]
        public static extern void TfLiteMicroSetInt8Data(TfLiteTensor tensor, int index, sbyte value);

        [DllImport(TensorFlowLibName)]
        public static extern TfLiteQuantizationParams TfLiteMicroTensorQuantizationParams (TfLiteTensor tensor);

        [DllImport(TensorFlowLibName)]
        public static extern TfLiteStatus TfLiteMicroMutableSetOption(sbyte option);

        [DllImport(TensorFlowLibName)]
        public static extern TfLiteType TfLiteMicroGetType(TfLiteTensor tensor);

        [DllImport(TensorFlowLibName)]
        public static extern int TfLiteMicroDimsSizeData(TfLiteTensor tensor);

        [DllImport(TensorFlowLibName)]
        public static extern int TfLiteMicroDimsData(TfLiteTensor tensor, int index);

        [DllImport(TensorFlowLibName)]
        public static extern float TfLiteMicroGetFloatData(TfLiteTensor tensor, int index);

        [DllImport(TensorFlowLibName)]
        public static extern void TfLiteMicroSetFloatData(TfLiteTensor tensor, int index, float value);

        [DllImport(TensorFlowLibName)]
        public static extern int TfLiteMicroGetByte(TfLiteTensor tensor);

    }
}