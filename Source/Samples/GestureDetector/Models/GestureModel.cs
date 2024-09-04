using Meadow.TensorFlow;

namespace GestureDetector.Models;

public class GestureModel : Model<float>
{
    private static readonly int ArenaSize = 60 * 1024;

    public GestureModel(byte[] data) : base(data, ArenaSize)
    { }
}