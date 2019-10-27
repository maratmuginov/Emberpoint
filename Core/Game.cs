using SadConsole;
using Microsoft.Xna.Framework;

namespace Emberpoint.Core
{
    public static class Game
    {
        static void Main()
        {
            // Setup the engine and create the main window.
            SadConsole.Game.Create(80, 25);

            // Hook the start event so we can add consoles to the system.
            SadConsole.Game.OnInitialize = Init;

            // Start the game.
            SadConsole.Game.Instance.Run();
            SadConsole.Game.Instance.Dispose();
        }

        static void Init()
        {
            var console = new Console(80, 25);
            console.Fill(new Rectangle(3, 3, 23, 3), Color.Violet, Color.Black, 0, 0);
            console.Print(40 - "Emberpoint".Length / 2, 4, "Emberpoint");

            Global.CurrentScreen = console;
        }
    }
}
