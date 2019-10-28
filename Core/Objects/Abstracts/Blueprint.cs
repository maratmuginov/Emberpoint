using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Emberpoint.Core.Objects.Abstracts
{
    public abstract class Blueprint<T> where T : EmberCell, new()
    {
        public T[] GetCells()
        {
            var name = GetType().Name;
            var root = GetApplicationRoot();
            var specialCharactersPath = Path.Combine(root, "Core", "Objects", "Blueprints", "SpecialCharactersConfig.json");
            var blueprintPath = Path.Combine(root, "Core", "Objects", "Blueprints", name + ".txt");
            var blueprintConfigPath = Path.Combine(root, "Core", "Objects", "Blueprints", "Config", name + ".json");

            if (!File.Exists(blueprintPath) || !File.Exists(blueprintConfigPath) || !File.Exists(specialCharactersPath)) 
                return Array.Empty<T>();

            var specialConfig = JsonConvert.DeserializeObject<BlueprintConfig>(File.ReadAllText(specialCharactersPath));
            var specialChars = specialConfig.Tiles.ToDictionary(a => a.Glyph, a => a);

            var config = JsonConvert.DeserializeObject<BlueprintConfig>(File.ReadAllText(blueprintConfigPath));
            var tiles = config.Tiles.ToDictionary(a => a.Glyph, a => a);

            foreach (var tile in tiles)
            {
                if (specialChars.ContainsKey(tile.Key))
                    throw new Exception("Glyph '" + tile.Key + "': is reserved as a special character and cannot be used in " + name);
            }

            var blueprint = File.ReadAllText(blueprintPath).Replace("\r", "").Split('\n');

            var cells = new List<T>();
            for (int y=0; y < config.GridSizeY; y++)
            {
                for (int x = 0; x < config.GridSizeX; x++)
                {
                    var charValue = blueprint[y][x];
                    var position = new Point(x, y);
                    BlueprintTile tile;
                    if (!tiles.TryGetValue(charValue, out tile)) 
                        throw new Exception("Glyph '" + charValue + "' was not present in the config file for blueprint: " + name);
                    var foregroundColor = GetColorByString(tile.Foreground);
                    cells.Add(new T() 
                    { 
                        Glyph = tile.Glyph, 
                        Position = position, 
                        Foreground = foregroundColor, 
                        Walkable = tile.Walkable,
                        Name = tile.Name
                    });
                }
            }
            return cells.ToArray();
        }

        private Color GetColorByString(string value)
        {
            var prop = typeof(Color).GetProperty(value);
            if (prop != null)
                return (Color)prop.GetValue(null, null);
            return default;
        }

        private string GetApplicationRoot()
        {
            var appRoot = AppContext.BaseDirectory.Substring(0, AppContext.BaseDirectory.LastIndexOf("\\bin"));
            return appRoot.Substring(0, appRoot.LastIndexOf("\\") + 1);
        }

        [Serializable]
        private class BlueprintConfig
        {
#pragma warning disable 0649
            public int GridSizeX;
            public int GridSizeY;
            public BlueprintTile[] Tiles;
#pragma warning restore 0649
        }

        [Serializable]
        private class BlueprintTile
        {
#pragma warning disable 0649
            public char Glyph;
            public string Name;
            public bool Walkable;
            public string Foreground;
#pragma warning restore 0649
        }
    }
}
