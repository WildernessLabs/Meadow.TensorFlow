using Meadow.TensorFlow;

namespace HelloWorld.Models;

public class HelloWorldModel : Model<sbyte>
{
    private static readonly int ArenaSize = 2000;

    public HelloWorldModel(byte[] data) : base(data, ArenaSize)
    { }

    public int InterferencesPerCycles => 20;
    public int InterferenceCount { get; set; }
    public float XRange => 6.2831855F;

    public HelloWorldResult[] GetReferenceResults()
    {
        return helloWorldResult;
    }

    private readonly HelloWorldResult[] helloWorldResult =
    {
        new() {X = 0.000000f, Y = 0.000000f },
        new() {X = 0.314159f, Y = 0.372768f },
        new() {X = 0.628318f, Y = 0.559152f },
        new() {X = 0.942477f, Y = 0.838728f },
        new() {X = 1.256637f, Y = 0.965808f },
        new() {X = 1.570796f, Y = 1.042057f },
        new() {X = 1.884956f, Y = 0.957336f },
        new() {X = 2.199115f, Y = 0.821784f },
        new() {X = 2.513274f, Y = 0.533736f },
        new() {X = 2.827433f, Y = 0.237216f },
        new() {X = 3.141593f, Y = 0.008472f },
        new() {X = 3.455752f, Y = -0.304992f },
        new() {X = 3.769912f, Y = -0.533736f },
        new() {X = 4.084070f, Y = -0.779424f },
        new() {X = 4.398230f, Y = -0.965808f },
        new() {X = 4.712389f, Y = -1.109833f },
        new() {X = 5.026548f, Y = -0.982752f },
        new() {X = 5.340708f, Y = -0.745536f },
        new() {X = 5.654867f, Y = -0.533736f },
        new() {X = 5.969026f, Y = -0.355824f }
    };
}