using System.Runtime.InteropServices;

namespace Meadow.TensorFlow;

internal class ModelSafeHandle<T> : SafeHandle
    where T : struct
{
    public override bool IsInvalid => throw new System.NotImplementedException();

    public ModelSafeHandle(ITensorModel<T> tensorModel)
         : base(Marshal.AllocHGlobal(tensorModel.Size * sizeof(int)), true)
    {
    }

    protected override bool ReleaseHandle()
    {
        throw new System.NotImplementedException();
    }
}