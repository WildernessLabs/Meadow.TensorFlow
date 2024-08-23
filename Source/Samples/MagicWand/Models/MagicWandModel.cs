using Meadow.TensorFlow;

namespace MagicWand.Models;

public class MagicWandModel : Model<float>
{
    private static readonly int ArenaSize = 60 * 1024;

    public enum Gesture
    {
        Unknown = -1,
        M = 0,
        L = 1,
        O = 2,
        U = 3,
        Count = 4
    }

    private const float detectionThreshold = 0.8f;

    public MagicWandModel(byte[] data) : base(data, ArenaSize)
    { }

    public Gesture GetGesture(ModelOutput<float> output)
    {
        int maxIndex = 0;
        float maxScore = 0f;

        for (int i = 0; i < (int)Gesture.Count; i++)
        {
            if (output[i] > maxScore)
            {
                maxIndex = i;
                maxScore = output[i];
            }
        }

        if (maxScore > detectionThreshold)
        {
            return (Gesture)maxIndex;
        }

        return Gesture.Unknown;
    }
}