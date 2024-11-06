using Meadow.TensorFlow;

namespace PersonDetection.Models;

public class PersonDetectionModel : Model<sbyte>
{
    private static readonly int ArenaSize = 134 * 1024;

    public enum Detection
    {
        NoPerson = 0,
        Person
    }

    public PersonDetectionModel(byte[] data) : base(data, ArenaSize)
    { }

    public Detection GetOutputDetection(ModelOutput<sbyte> output)
    {
        if (output[0] > output[1])
        {
            return Detection.NoPerson;
        }
        return Detection.Person;
    }
}