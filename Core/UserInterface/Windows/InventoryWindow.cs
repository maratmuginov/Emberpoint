using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Entities;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Microsoft.Xna.Framework;
using SadConsole;
using System.Collections.Generic;
using System.Linq;

namespace Emberpoint.Core.UserInterface.Windows
{
    public class InventoryWindow : Console, IUserInterface
    {
        private readonly Console _textConsole;

        private int _maxLineRows;
        private readonly Dictionary<IItem, int> _inventoryDict;

        public InventoryWindow(int width, int height) : base(width, height)
        {
            this.DrawBorders(width, height, "O", "|", "-", Color.Gray);
            Print(3, 0, "Inventory", Color.Purple);

            _inventoryDict = new Dictionary<IItem, int>();
            _maxLineRows = Height - 2;
            _textConsole = new Console(Width - 2, Height - 2)
            {
                Position = new Point(2, 1),
            };

            Position = new Point(Constants.Map.Width + 7, 3);

            Children.Add(_textConsole);
            Global.CurrentScreen.Children.Add(this);
        }

        public void Initialize()
        {
            // Adding default Items to Inventory
            var sanityPotion = new EmberItem("Potion of Sanity", 'S', Color.Green) { Amount = 3 };
            AddInventoryItem(sanityPotion);
            AddInventoryItem(new EmberItem("Potion of Health", 'H', Color.Red) { Amount = 1 });

            sanityPotion.Amount = 2;
            RemoveInventoryItem(sanityPotion);
        }

        public void Update()
        {
            // Tell's sadconsole to redraw this console
            _textConsole.IsDirty = true;
            IsDirty = true;
        }

        public void AddInventoryItem(IItem item)
        {
            if (!_inventoryDict.ContainsKey(item))
            {
                _inventoryDict.Add(item, item.Amount);
                return;
            }
            _inventoryDict[item] += item.Amount;
            UpdateInventoryText();
        }

        public void RemoveInventoryItem(IItem item)
        {
            if (_inventoryDict.ContainsKey(item))
            {
                _inventoryDict[item] -= item.Amount;
                if (_inventoryDict[item] < 1)
                {
                    _inventoryDict.Remove(item);
                    ItemManager.Remove(item);
                }
            }
            UpdateInventoryText();
        }

        private void UpdateInventoryText()
        {
            _textConsole.Clear();
            _textConsole.Cursor.Position = new Point(0, 0);

            if (_inventoryDict.Count > _maxLineRows)
            {
                foreach (var item in _inventoryDict.OrderBy(x => x.Key).Take(_maxLineRows - 1))
                {
                    _textConsole.Cursor.Print(string.Format("{0} : {1}\r\n", item.Value, item.Key));
                }
                _textConsole.Cursor.Print("<More Items..>");
            }
            else
            {
                foreach (var item in _inventoryDict.OrderBy(x => x.Key))
                {
                    _textConsole.Cursor.Print(string.Format("{0} : {1}\r\n", item.Value, item.Key));
                }
            }
        }
    }
}