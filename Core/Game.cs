using System;
using SadConsole;
using Microsoft.Xna.Framework;
using Emberpoint.Core.GameObjects.Entities;
using Emberpoint.Core.UserInterface.Windows;
using Emberpoint.Core.GameObjects.Managers;

namespace Emberpoint.Core
{
    public static class Game
    {
        public static Player Player { get; private set; }
        public static DialogWindow DialogWindow { get; private set; }
        public static GameOverWindow GameOverWindow { get; private set; }

        private static void Main()
        {
            // Setup the engine and create the main window.
            SadConsole.Game.Create(Constants.GameWindowWidth, Constants.GameWindowHeight);

            // Hook the start event so we can add consoles to the system.
            SadConsole.Game.OnInitialize = Init;
            // Hook the update event so we can check for key presses.
            SadConsole.Game.OnUpdate = Update;

            // Start the game.
            SadConsole.Game.Instance.Run();
            SadConsole.Game.Instance.Dispose();
        }

        private static void Update(GameTime gameTime)
        {
            //Test trigger for game over state
            if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.P))
            {
                GameOverWindow.ShowGameOverWindow();
            }

            if (DialogWindow.IsVisible)
            {
                if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter))
                {
                    DialogWindow.CloseDialog();
                }
            }

            if (GameOverWindow.IsVisible)
            {
                if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Enter))
                {
                    //Returns to main menu
                    //TODO MainMenuWindow
                }
                if (Global.KeyboardState.IsKeyPressed(Microsoft.Xna.Framework.Input.Keys.Escape))
                {
                    Environment.Exit(0);
                }
            }

            if (UserInterfaceManager.IsPaused) return;

            Player.CheckForMovementKeys();
            Player.CheckForInteractionKeys();
        }

        private static void Init()
        {
            // Initialize user interface
            UserInterfaceManager.Initialize();

            // Keep the dialog window in a global variable so we can check it in the game loop
            DialogWindow = UserInterfaceManager.Get<DialogWindow>();

            GameOverWindow = UserInterfaceManager.Get<GameOverWindow>();

            // Instantiate player in the middle of the map
            Player = EntityManager.Create<Player>(GridManager.Grid.GetFirstCell(a => a.LightProperties.Brightness > 0f && a.CellProperties.Walkable).Position);
            Player.Initialize();

            // Show a tutorial dialog window.
            DialogWindow.ShowDialog("Tutorial", new string[] { "Welcome to Emberpoint.", "To turn on your flashlight, press 'F'.", "Press 'Enter' to continue." });
           
        }
    }
}
