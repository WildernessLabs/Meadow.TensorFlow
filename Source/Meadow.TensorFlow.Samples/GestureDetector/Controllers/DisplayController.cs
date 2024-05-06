using Meadow;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using Meadow.Peripherals.Displays;
using System.Threading;

namespace GestureDetector.Controllers;
public class DisplayController
{
    private readonly Image[] images =
    {
        Image.LoadFromResource("GestureDetector.Resources.img-thumb-up.bmp"),
        Image.LoadFromResource("GestureDetector.Resources.img-thumb-down.bmp"),
        Image.LoadFromResource("GestureDetector.Resources.img-flex.bmp"),
        Image.LoadFromResource("GestureDetector.Resources.img-wave.bmp"),
        Image.LoadFromResource("GestureDetector.Resources.img-fist.bmp"),
        Image.LoadFromResource("GestureDetector.Resources.img-thinking.bmp"),
        Image.LoadFromResource("GestureDetector.Resources.img-ready.bmp"),
        Image.LoadFromResource("GestureDetector.Resources.img-shrug.bmp"),
    };

    private readonly Font16x24 font16X24 = new Font16x24();
    private readonly Font12x16 font12X16 = new Font12x16();

    private readonly DisplayScreen displayScreen;

    private Label title;
    private Picture picture;
    private Label accuracy;

    public DisplayController(IPixelDisplay display)
    {
        displayScreen = new DisplayScreen(display, RotationType._270Degrees)
        {
            BackgroundColor = Color.FromHex("082936")
        };

        displayScreen.Controls.Add(new Picture(10, 190, 56, 40,
            Image.LoadFromResource("GestureDetector.Resources.img-ei-logo.bmp")));

        displayScreen.Controls.Add(new Picture(274, 190, 36, 40,
            Image.LoadFromResource("GestureDetector.Resources.img-tf-logo.bmp")));

        title = new Label(0, 23, displayScreen.Width, font16X24.Height)
        {
            Text = "Ready",
            HorizontalAlignment = HorizontalAlignment.Center,
            Font = font16X24
        };
        displayScreen.Controls.Add(title);

        picture = new Picture(100, 59, 119, 122, images[images.Length - 1]);
        displayScreen.Controls.Add(picture);

        accuracy = new Label(0, 197, displayScreen.Width, font12X16.Height)
        {
            Text = "0%",
            HorizontalAlignment = HorizontalAlignment.Center,
            Font = font12X16,
            IsVisible = false
        };
        displayScreen.Controls.Add(accuracy);
    }

    public void ShowMovementDetected()
    {
        title.Text = "Processing";
        picture.Image = images[(int)GesturesType.Thinking];

    }

    public void ShowReady()
    {
        title.Text = "Ready";
        picture.Image = images[(int)GesturesType.Ready];
    }

    public void ShowGestureNotRecognized()
    {
        title.Text = "Not recognized";
        picture.Image = images[(int)GesturesType.Shrug];
    }

    public void ShowGestureDetected(int state, float accuracyValue)
    {
        title.Text = state switch
        {
            (int)GesturesType.ThumbsUp => "Thumbs Up",
            (int)GesturesType.ThumbsDown => "Thumbs Down",
            (int)GesturesType.Flex => "Flex",
            (int)GesturesType.Wave => "Wave",
            (int)GesturesType.Punch => "Punch",
        };
        picture.Image = images[state];
        accuracy.IsVisible = true;
        accuracy.Text = $"{accuracyValue * 100:N1}%";

        Thread.Sleep(3000);

        picture.Image = images[(int)GesturesType.Ready];
        accuracy.IsVisible = false;
        title.Text = "Ready";
    }
}

public enum GesturesType
{
    ThumbsUp = 0,
    ThumbsDown = 1,
    Flex = 2,
    Wave = 3,
    Punch = 4,
    Thinking = 5,
    Ready = 6,
    Shrug = 7
}