using PersonDetection.Models;
using Meadow;
using Meadow.Devices;
using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using TensorFlow.litemicro;
using Meadow.Foundation.Graphics;

namespace PersonDetection;

public class MeadowApp : App<F7FeatherV2>
{
    PersonDetectionModel personDetectionModel = new PersonDetectionModel();
    const int ArenaSize = 134 * 1024;
    TfLiteStatus tfLiteStatus;
    IntPtr interpreter;
    TfLiteTensor input;
    public readonly Image []images = 
    { 
        Image.LoadFromResource("Resources.no_person.bmp"),
        Image.LoadFromResource("Resources.person.bmp"),
    };

    public override Task Initialize()
    {
        Resolver.Log.Info("Initialize TensorFlow ...");

        IntPtr model = Marshal.AllocHGlobal(personDetectionModel.GetSize() * sizeof(int));

        if (model == IntPtr.Zero)
        {
            Resolver.Log.Info("Failed to allocated model");
            return base.Initialize();
        }

        IntPtr arena = Marshal.AllocHGlobal(ArenaSize * sizeof(int));

        if (arena == IntPtr.Zero)
        {
            Resolver.Log.Info("Failed to allocated arena");
            Marshal.FreeHGlobal(model);
            return base.Initialize();
        }

        Marshal.Copy(personDetectionModel.GetData(), 0, model, personDetectionModel.GetSize());

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
        {
            Resolver.Log.Info("Failed to allocate tensors");
        }

        input = c_api_lite_micro.TfLiteMicroInterpreterGetInput(interpreter, 0);

        return Task.CompletedTask;
    }

    public override Task Run()
    {
        Resolver.Log.Info("Sample completed");
        return Task.CompletedTask;
    }
}