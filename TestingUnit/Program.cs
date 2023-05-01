using Clockwork;
using Clockwork.Rendering;
using Clockwork.Windowing;
using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using TestingUnit;
using TestingUnit.States;

Program program = new Program();

public partial class Program
{
    public static Game Game { get; private set; }

    public Program()
    {
        List<FontInfo> fonts = new List<FontInfo> { new FontInfo() { Name = "arial_12", Size = 12, Src = "fonts.arial.ttf" }, new FontInfo() { Name = "arial_14", Size = 14, Src = "fonts.arial.ttf" } };
        string str = JsonConvert.SerializeObject(fonts);
        Console.WriteLine(str);

        Game = new Game();

        Game.SetRenderer(new ClockworkWindow(border: WindowBorder.Resizable, clearColor: new Color4(51, 51, 51, 255), vSync: VSyncMode.On, state: WindowState.Maximized));
        Game.SetState(new DefaultState());
        Game.Initialize();

        new PerformanceMonitor();

        Game.Start();
    }
}
