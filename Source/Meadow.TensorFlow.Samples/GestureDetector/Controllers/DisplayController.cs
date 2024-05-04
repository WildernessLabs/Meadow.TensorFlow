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
    };

    private readonly Font16x24 font16X24 = new Font16x24();

    private readonly DisplayScreen displayScreen;

    private Label title;
    private Picture picture;

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

        picture = new Picture(101, 70, 119, 122, images[images.Length - 1]);
        displayScreen.Controls.Add(picture);

        title = new Label(
            left: 0,
            top: 25,
            width: displayScreen.Width,
            height: font16X24.Height)
        {
            Text = "Ready",
            HorizontalAlignment = HorizontalAlignment.Center,
            Font = font16X24
        };
        displayScreen.Controls.Add(title);
    }

    public void ShowMovementDetected()
    {
        picture.Image = images[(int)GesturesType.Thinking];
        title.Text = "Processing";
    }

    public void ShowReady()
    {
        picture.Image = images[(int)GesturesType.Ready];
        title.Text = "Ready";
    }

    public void ShowGestureDetected(int state)
    {
        picture.Image = images[state];
        title.Text = state switch
        {
            (int)GesturesType.ThumbsUp => "Thumbs Up",
            (int)GesturesType.ThumbsDown => "Thumbs Down",
            (int)GesturesType.Flex => "Flex",
            (int)GesturesType.Wave => "Wave",
            (int)GesturesType.Punch => "Punch",
        };

        Thread.Sleep(3000);

        picture.Image = images[(int)GesturesType.Ready];
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
    Ready = 6
}