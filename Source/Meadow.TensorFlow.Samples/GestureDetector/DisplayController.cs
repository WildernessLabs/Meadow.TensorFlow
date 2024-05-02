using Meadow;
using Meadow.Devices;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.MicroLayout;
using Meadow.Peripherals.Displays;
using System;

namespace GestureDetector.Controllers;
public class DisplayController
{
    private static readonly Lazy<DisplayController> instance =
        new Lazy<DisplayController>(() => new DisplayController());
    public static DisplayController Instance => instance.Value;

    MicroGraphics graphics;

    public void Initialize(IPixelDisplay display)
    {
        graphics = new MicroGraphics(display)
        {
            Stroke = 1,
            CurrentFont = new Font12x20()
        };
        graphics.Clear();
    }

    public void UpdateGesture(string gesture)
    {
        graphics.Clear();
        graphics.DrawText(0, 0, gesture);
        graphics.Show();
    }

}