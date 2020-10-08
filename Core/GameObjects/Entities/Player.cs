using System;
using System.Collections.Generic;
using System.Linq;
using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Items;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.UserInterface.Windows;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Components;

namespace Emberpoint.Core.GameObjects.Entities
{
    public class Player : EmberEntity
    {
        private InventoryWindow _inventory;
        public InventoryWindow Inventory
        {
            get => _inventory ??= UserInterfaceManager.Get<InventoryWindow>();
        }

        private MapWindow _mapWindow;
        public MapWindow MapWindow
        {
            get => _mapWindow ??= UserInterfaceManager.Get<MapWindow>();
        }

        private FovWindow _fovObjectsWindow;

        public Player() : base(Constants.Player.Foreground, Color.Transparent, Constants.Player.Character, 1, 1)
        {
            FieldOfViewRadius = 0;
            Components.Add(new EntityViewSyncComponent());
        }

        public override void OnMove(object sender, EntityMovedEventArgs args)
        {
            // Handles flashlight lights
            GridManager.Grid.LightEngine.HandleFlashlight(this);

            // OnMove will redraw fov, and flashlight needs to be handled before that
            base.OnMove(sender, args);

            // Update visible objects in fov window
            UpdateFovWindow();
        }

        private void UpdateFovWindow()
        {
            _fovObjectsWindow ??= UserInterfaceManager.Get<FovWindow>();
            _fovObjectsWindow.Update(this);
        }

        private IEnumerable<char> GetBrightCellsInFov(int fovRadius)
        {
            return GridManager.Grid.GetExploredCellsInFov(this, fovRadius)
                .Where(a => a.LightProperties.Brightness > 0f)
                .Select(a => (char) a.Glyph);
        }

        public override void Update(TimeSpan timeElapsed)
        {
            // Check movement keys for the player
            CheckForMovementKeys();

            // Check any interaction keys for the player
            CheckForInteractionKeys();

            // Call base to update correctly
            base.Update(timeElapsed);
        }

        public void Initialize()
        {
            // Draw player on the map
            RenderObject(MapWindow);

            // Center viewport on player
            MapWindow.CenterOnEntity(this);

            UpdateFovWindow();
        }

        public void CheckForInteractionKeys()
        {
            //If this will grow in the future, we may want to add a Dictionary<Keybinding, EmberItem>
            // to efficiently retrieve and activate items.
            if (Global.KeyboardState.IsKeyPressed(KeybindingsManager.GetKeybinding(Keybindings.Flashlight)))
            {
                var flashLight = Game.Player.Inventory.GetItemOfType<Flashlight>();
                flashLight?.Switch();
            }
        }

        public void CheckForMovementKeys()
        {
            foreach (var movementKey in _playerMovements.Keys
                .Where(key => Global.KeyboardState.IsKeyPressed(KeybindingsManager.GetKeybinding(key))))
            {
                var (x, y) = _playerMovements[movementKey];
                MoveTowards(Position.Translate(x, y));
            }
        }

        private readonly Dictionary<Keybindings, (int x, int y)> _playerMovements = 
        new Dictionary<Keybindings, (int x, int y)>
        {
            {Keybindings.Movement_Up, (0, -1)},
            {Keybindings.Movement_Down, (0, 1)},
            {Keybindings.Movement_Left, (-1, 0)},
            {Keybindings.Movement_Right, (1, 0)}
        };
    }
}
