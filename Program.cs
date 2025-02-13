using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using Voxels;

public static class Program
{
    public static void Main()
    {
        var nativeWindowSettings = new NativeWindowSettings
        {
            Title = "Voxels",
            Flags = ContextFlags.ForwardCompatible,
        };

        using (var game = new Game(GameWindowSettings.Default, nativeWindowSettings, 1920, 1080))
        {
            game.Run();
        }
    }
}