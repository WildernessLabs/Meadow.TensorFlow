using Meadow;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using Meadow.Peripherals.Displays;
using System.Threading;

namespace PersonDetection.Controllers;
public class DisplayController
{
    public readonly Image[] images =
    {
        Image.LoadFromResource("Resources.no_person.bmp"),
        Image.LoadFromResource("Resources.person.bmp"),
    };

    private readonly Font16x24 font16X24 = new Font16x24();
    private readonly Font12x16 font12X16 = new Font12x16();

    private readonly DisplayScreen displayScreen;

    private Label classification;
    private Label title;
    private Picture picture;
    private Label score;

    public DisplayController(IPixelDisplay display)
    {
        displayScreen = new DisplayScreen(display, RotationType._270Degrees)
        {
            BackgroundColor = Color.FromHex("082936")
        };

        displayScreen.Controls.Add(new Picture(274, 190, 36, 40,
            Image.LoadFromResource("Resources.img-tf-logo.bmp")));

        title = new Label(0, 23, displayScreen.Width, font16X24.Height)
        {
            Text = "Person Detection",
            HorizontalAlignment = HorizontalAlignment.Center,
            Font = font16X24
        };

        displayScreen.Controls.Add(title);

        score = new Label(0, 220, displayScreen.Width, font12X16.Height)
        {
            Text = "0%",
            HorizontalAlignment = HorizontalAlignment.Center,
            Font = font12X16,
            IsVisible = false
        };
        displayScreen.Controls.Add(score);
    
        classification = new Label (0, 55, displayScreen.Width, font12X16.Height)
        {
            Text = " ",
            Font = font12X16,
            HorizontalAlignment = HorizontalAlignment.Center,
            IsVisible = false
        };
        displayScreen.Controls.Add(classification);
    }

    public void ShowClassification(int state, int scoreValue)
    {
        classification.IsVisible = true;
        classification.Text = state switch
        {
            (int)Classification.Person => "Person",
            (int)Classification.NoPerson => "No Person",
        };

        score.IsVisible = true;
        score.Text = $"Score : {scoreValue}%";
    }

    public void ShowImage(int width, int height, IPixelBuffer buffer)
    {
        picture = new Picture(100, 100, width, height, Image.LoadFromPixelData(buffer))
        {
            HorizontalAlignment = HorizontalAlignment.Center,
            IsVisible = true,
        };

        displayScreen.Controls.Add(picture);
    }

    public void Clear()
    {
        picture = null;
    }
}

public enum Classification
{
    NoPerson = 0,
    Person = 1,
}