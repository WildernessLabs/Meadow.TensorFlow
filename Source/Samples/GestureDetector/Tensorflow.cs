using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Hardware;

using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using Tensorflow.litemicro;
using GestureDetectorModel;

namespace MeadowApp
{
    public class Tensorflow
    {
        private static readonly Lazy<Tensorflow> instance = new Lazy<Tensorflow>(() => new Tensorflow());
        public static Tensorflow Instance => instance.Value;

        private TfLiteStatus tfLiteStatus;
        private TfLiteTensor input, output;
        private IntPtr interpreter;
        private int ArenaSize = 60 * 1024;

        public void Initialize()
        {
            Model gestureDetectorModel = new Model();

            IntPtr model = Marshal.AllocHGlobal(gestureDetectorModel.GetSize() * sizeof(byte));

            if (model == IntPtr.Zero)
            {
                Resolver.Log.Info("Failed to allocated model");
                return;
            }

            Marshal.Copy(gestureDetectorModel.GetData(), 0, model, gestureDetectorModel.GetSize());

            IntPtr arena = Marshal.AllocHGlobal(ArenaSize * sizeof(int));

            if (arena == IntPtr.Zero)
            {
                Resolver.Log.Info("Failed to allocated arena");
                Marshal.FreeHGlobal(model);
                return;
            }

            var model_options = c_api_lite_micro.TfLiteMicroGetModel(ArenaSize, arena, model);
            if (model_options == null)
                Resolver.Log.Info("Failed to loaded the model");

            var interpreter_options = c_api_lite_micro.TfLiteMicroInterpreterOptionCreate(model_options);
            if (interpreter_options == null)
                Resolver.Log.Info("Failed to create interpreter option");

            interpreter = c_api_lite_micro.TfLiteMicroInterpreterCreate(interpreter_options, model_options);
            if (interpreter == null)
                Resolver.Log.Info("Failed to Interpreter");

            tfLiteStatus = c_api_lite_micro.TfLiteMicroInterpreterAllocateTensors(interpreter);
            if (tfLiteStatus != TfLiteStatus.kTfLiteOk)
                Resolver.Log.Info("Failed to allocate tensors");

            input = c_api_lite_micro.TfLiteMicroInterpreterGetInput(interpreter, 0);
            output = c_api_lite_micro.TfLiteMicroInterpreterGetOutput(interpreter, 0);

            Resolver.Log.Info("Tensor flow Initialize");
        }

        public int InputLegth()
        {
            return c_api_lite_micro.TfLiteMicroGetByte(input) / sizeof(float);
        }

        public void InputData(int index, float value)
        {
            c_api_lite_micro.TfLiteMicroSetFloatData(input, index, value);
        }

        public float OutputData(int index)
        {
            return c_api_lite_micro.TfLiteMicroGetFloatData(c_api_lite_micro.TfLiteMicroInterpreterGetOutput(interpreter, 0), index);
        }

        public TfLiteStatus Invoke()
        {
            return c_api_lite_micro.TfLiteMicroInterpreterInvoke(interpreter);
        }

    }
}