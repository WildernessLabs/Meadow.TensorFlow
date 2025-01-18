using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.Buffers;
using Meadow.Foundation.RTLite;
using System.IO;
using System.Linq;

namespace PersonDetector_Demo;

public class PersonDetectorModel : Model<sbyte>
{
    public enum PersonClass
    {
        NoPerson = 0,
        Person = 1
    }

    private static readonly int ArenaSize = 134 * 1024;

    public PersonDetectorModel(FileInfo modelFile)
        : base(modelFile, ArenaSize)
    {
    }

    public PersonClass Classify(Image image)
    {
        var resized = (image.DisplayBuffer as PixelBufferBase)?.Resize<BufferGray8>(96, 96);

        var inputs = resized.Buffer
            .Select(b => unchecked((sbyte)b))
            .ToArray();

        Inputs.SetData(inputs);

        var prediction = this.Predict();

        if (prediction[0] > prediction[1])
        {
            return PersonClass.NoPerson;
        }

        return PersonClass.Person;
    }
}
