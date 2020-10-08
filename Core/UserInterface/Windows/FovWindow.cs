using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Abstracts;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using SadConsole;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Console = SadConsole.Console;

namespace Emberpoint.Core.UserInterface.Windows
{
    public class FovWindow : Console, IUserInterface
    {
        public Console Console => this;

        private readonly int _maxLineRows;
        private readonly Console _textConsole;
        //Recreated with ReinitializeCharObjects()
        private Dictionary<char, CharObj> _charObjects;
        private readonly Dictionary<char, BlueprintTile> _blueprintTiles;

        public FovWindow(int width, int height) : base(width, height)
        {
            this.DrawBorders(width, height, "O", "|", "-", Color.Gray);
            Print(3, 0, "Objects", Color.Orange);

            _charObjects = new Dictionary<char, CharObj>();
            _blueprintTiles = GetTilesFromConfig();
            _maxLineRows = Height - 2;

            _textConsole = new Console(Width - 2, Height - 2)
            {
                Position = new Point(2, 1),
            };

            Position = new Point(Constants.Map.Width + 7, 3 + 25);

            Children.Add(_textConsole);
            Global.CurrentScreen.Children.Add(this);
        }

        private Dictionary<char, BlueprintTile> GetTilesFromConfig()
        {
            var name = GridManager.Grid.Blueprint.GetType().Name;
            var blueprintPath = Path.Combine(GridManager.Grid.Blueprint.BlueprintPath, name + ".txt");
            var blueprintConfigPath = Path.Combine(Constants.Blueprint.BlueprintsConfigPath, Constants.Blueprint.BlueprintTiles + ".json");

            if (!File.Exists(blueprintPath) || !File.Exists(blueprintConfigPath) || !File.Exists(Constants.Blueprint.SpecialCharactersPath))
                return new Dictionary<char, BlueprintTile>();

            var specialConfig = JsonConvert.DeserializeObject<BlueprintConfig>(File.ReadAllText(Constants.Blueprint.SpecialCharactersPath));
            var specialChars = specialConfig.Tiles.ToDictionary(a => a.Glyph, a => a);

            var config = JsonConvert.DeserializeObject<BlueprintConfig>(File.ReadAllText(blueprintConfigPath));
            var tiles = config.Tiles.ToDictionary(a => a.Glyph, a => a);

            foreach (var (key, value) in specialChars)
                tiles.Add(key, value);

            return tiles;
        }

        public void ReinitializeCharObjects(IEnumerable<char> characters, bool updateText = true)
        {
            _charObjects = new Dictionary<char, CharObj>(GetCharObjectPairs(characters));

            if (updateText)
                UpdateText();
        }

        private IEnumerable<KeyValuePair<char, CharObj>> GetCharObjectPairs(IEnumerable<char> characters)
        {
            foreach (var character in characters)
            {
                if (!_blueprintTiles.TryGetValue(character, out var tile) || tile.Name == null) continue;
                var glyphColor = GetColorByString(tile.Foreground);
                yield return new KeyValuePair<char, CharObj>(character, new CharObj(tile.Glyph, glyphColor, tile.Name));
            }
        }

        private Color GetColorByString(string value)
        {
            var prop = typeof(Color).GetProperty(value);
            if (prop != null)
                return (Color)prop.GetValue(null, null);
            return default;
        }

        public void UpdateText()
        {
            _textConsole.Clear();
            _textConsole.Cursor.Position = new Point(0, 0);

            var orderedValues = _charObjects.OrderBy(x => x.Key).Select(pair => pair.Value);

            foreach (var charObj in orderedValues.Take(_maxLineRows - 1))
                DrawCharObj(charObj);

            if (_charObjects.Count > _maxLineRows)
                _textConsole.Cursor.Print("<More Objects..>");
        }
        private void DrawCharObj(CharObj charObj)
        {
            _textConsole.Cursor.Print(new ColoredString('[' + charObj.Glyph + "]:", charObj.GlyphColor, Color.Transparent));
            _textConsole.Cursor.Print(' ' + charObj.Name);
            _textConsole.Cursor.CarriageReturn();
            _textConsole.Cursor.LineFeed();
        }

        public void Update(IEntity entity)
        {
            var farBrightCells = GetBrightCellsInFov(entity, Constants.Player.FieldOfViewRadius + 3);

            // Gets cells player can see after FOV refresh.
            var cells = GridManager.Grid.GetExploredCellsInFov(entity, Constants.Player.FieldOfViewRadius)
                .Select(a => (char)a.Glyph)
                //Merge in bright cells before FOV refresh.
                .Union(farBrightCells)
                //Take only unique cells as an array.
                .Distinct();

            // Draw visible cells to the FOV window
            ReinitializeCharObjects(characters: cells, updateText: false);
            UpdateText();
        }

        private IEnumerable<char> GetBrightCellsInFov(IEntity entity, int fovRadius)
        {
            return GridManager.Grid.GetExploredCellsInFov(entity, fovRadius)
                .Where(a => a.LightProperties.Brightness > 0f)
                .Select(a => (char)a.Glyph);
        }

        private readonly struct CharObj
        {
            public readonly char Glyph;
            public readonly Color GlyphColor;
            public readonly string Name;

            public CharObj(char glyph, Color glyphColor, string name)
            {
                Glyph = glyph;
                GlyphColor = glyphColor;
                Name = name;
            }
        }

    }
}
