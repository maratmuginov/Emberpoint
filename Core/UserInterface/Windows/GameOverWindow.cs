
using System.Collections.Generic;
using System.Text;
using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Microsoft.Xna.Framework;
using SadConsole;

namespace Emberpoint.Core.UserInterface.Windows
{
    public class GameOverWindow : Console, IUserInterface
    {
        private static MapWindow _mapWindow;
        private static DialogWindow _dialogWindow;
        private static InventoryWindow _inventoryWindow;

        private readonly Console _gameOverConsole;
        private readonly Console _textConsole;

        public GameOverWindow(int width, int height) : base(width, height)
        {
            _textConsole = new Console(Width - 2, Height - 2)
            {
                Position = new Point(20, 20),
                DefaultBackground = Color.Black
            };

            _gameOverConsole = new Console(Width - 2, Height - 2)
            {
                Position = new Point(23, 16),
                DefaultBackground = Color.Black
            };

           
            Children.Add(_gameOverConsole);
            _gameOverConsole.Children.Add(_textConsole);
            IsVisible = false;

            Global.CurrentScreen.Children.Add(this);

            DrawGameOverTitle();
            DrawText();
        }

        public void ShowGameOverWindow()
        {
            // Get reference on Map window
            _mapWindow = UserInterfaceManager.Get<MapWindow>();
            // Get reference on Dialog window
            _dialogWindow = UserInterfaceManager.Get<DialogWindow>();
            // Get reference on Inventory window
            _inventoryWindow = UserInterfaceManager.Get<InventoryWindow>();

            // Show only Game Over window
            _mapWindow.IsVisible = false;
            _dialogWindow.IsVisible = false;
            _inventoryWindow.IsVisible = false;
            this.IsVisible = true;
        }

        public void Update()
        {
            IsDirty = true;
        }

        private void DrawText()
        {
            _textConsole.Cursor.Print("Press 'Enter' to return to main menu" + System.Environment.NewLine);
            _textConsole.Cursor.Print("Press 'Esc' to exit the application");
        }

        private void DrawGameOverTitle()
        {
            _gameOverConsole.Cursor.Print(@" _______  _______  _______  _______    _______           _______  _______ " + System.Environment.NewLine);
            _gameOverConsole.Cursor.Print(@"(  ____ \(  ___  )(       )(  ____ \  (  ___  )|\     /|(  ____ \(  ____ )" + System.Environment.NewLine);
            _gameOverConsole.Cursor.Print(@"| (    \/| (   ) || () () || (    \/  | (   ) || )   ( || (    \/| (    )|" + System.Environment.NewLine);
            _gameOverConsole.Cursor.Print(@"| |      | (___) || || || || (__      | |   | || |   | || (__    | (____)|" + System.Environment.NewLine);
            _gameOverConsole.Cursor.Print(@"| | ____ |  ___  || |(_)| ||  __)     | |   | |( (   ) )|  __)   |     __)" + System.Environment.NewLine);
            _gameOverConsole.Cursor.Print(@"| | \_  )| (   ) || |   | || (        | |   | | \ \_/ / | (      | (\ (   " + System.Environment.NewLine);
            _gameOverConsole.Cursor.Print(@"| (___) || )   ( || )   ( || (____/\  | (___) |  \   /  | (____/\| ) \ \__" + System.Environment.NewLine);
            _gameOverConsole.Cursor.Print(@"(_______)|/     \||/     \|(_______/  (_______)   \_/   (_______/|/   \__/" + System.Environment.NewLine);
        }
    }
}

// Game over title source
// _______  _______  _______  _______    _______           _______  _______ 
//(  ____ \(  ___  )(       )(  ____ \  (  ___  )|\     /|(  ____ \(  ____ )
//| (    \/| (   ) || () () || (    \/  | (   ) || )   ( || (    \/| (    )|
//| |      | (___) || || || || (__      | |   | || |   | || (__    | (____)|
//| | ____ |  ___  || |(_)| ||  __)     | |   | |( (   ) )|  __)   |     __)
//| | \_  )| (   ) || |   | || (        | |   | | \ \_/ / | (      | (\ (   
//| (___) || )   ( || )   ( || (____/\  | (___) |  \   /  | (____/\| ) \ \__
//(_______)|/     \||/     \|(_______/  (_______)   \_/   (_______/|/   \__/ 
                                                                          

