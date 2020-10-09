﻿using Emberpoint.Core.GameObjects.Map;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Tests")]
namespace Emberpoint.Core.GameObjects.Abstracts
{
    public abstract class Blueprint<T> where T : EmberCell, new()
    {
        public int GridSizeX { get; private set; }
        public int GridSizeY { get; private set; }
        public string BlueprintPath { get; private set; }

        public Blueprint()
        {
            InitializeBlueprint(Constants.Blueprint.BlueprintsPath);
        }

        protected Blueprint(string customPath)
        {
            InitializeBlueprint(customPath);
        }

        private void InitializeBlueprint(string path)
        {
            BlueprintPath = path;
            var blueprintPath = Path.Combine(BlueprintPath, GetType().Name + ".txt");
            var blueprintConfigPath = Path.Combine(Constants.Blueprint.BlueprintsConfigPath, Constants.Blueprint.BlueprintTiles + ".json");
            var config = JsonConvert.DeserializeObject<BlueprintConfig>(File.ReadAllText(blueprintConfigPath));

            if (!File.Exists(blueprintConfigPath) || !File.Exists(blueprintPath))
                throw new Exception("Blueprint config file(s) were not found for " + Constants.Blueprint.BlueprintTiles);

            var blueprint = File.ReadAllText(blueprintPath).Replace("\r", "").Split('\n');

            GridSizeX = blueprint.Max(a => a.Length);
            GridSizeY = blueprint.Length;
        }

        /// <summary>
        /// Retrieves the cells from the blueprint.txt file and blueprint.json config file.
        /// Cells are not cached by default.
        /// </summary>
        /// <returns></returns>
        public T[] GetCells()
        {
            var name = GetType().Name;
            var blueprintPath = Path.Combine(BlueprintPath, name + ".txt");
            var blueprintConfigPath = 
                Path.Combine(Constants.Blueprint.BlueprintsConfigPath, Constants.Blueprint.BlueprintTiles + ".json");

            if (!File.Exists(blueprintPath) || !File.Exists(blueprintConfigPath) || !File.Exists(Constants.Blueprint.SpecialCharactersPath)) 
                return Array.Empty<T>();

            var specialConfig = JsonConvert.DeserializeObject<BlueprintConfig>(File.ReadAllText(Constants.Blueprint.SpecialCharactersPath));
            var specialChars = specialConfig.Tiles.ToDictionary(a => a.Glyph, a => a);

            var config = JsonConvert.DeserializeObject<BlueprintConfig>(File.ReadAllText(blueprintConfigPath));
            var tiles = config.Tiles.ToDictionary(a => (char?) a.Glyph, a => a);
            var nullTile = BlueprintTile.Null();

            foreach (var tile in tiles)
            {
                if (tile.Key == null) continue;
                if (specialChars.ContainsKey(tile.Key.Value))
                    throw new Exception("Glyph '" + tile.Key.Value + "': is reserved as a special character and cannot be used in " + name);
            }

            var blueprint = File.ReadAllText(blueprintPath).Replace("\r", "").Split('\n');

            var cells = new List<T>();
            for (int y=0; y < GridSizeY; y++)
            {
                for (int x = 0; x < GridSizeX; x++)
                {
                    char? charValue;

                    if (y >= blueprint.Length || x >= blueprint[y].Length)
                    {
                        charValue = null;
                    } else {
                        charValue = blueprint[y][x];
                    }
                    
                    var position = new Point(x, y);
                    BlueprintTile tile = nullTile;
                    if (charValue !=null && !tiles.TryGetValue(charValue, out tile)) 
                        throw new Exception("Glyph '" + charValue + "' was not present in the config file for blueprint: " + name);
                    var foregroundColor = GetColorByString(tile.Foreground);
                    var backgroundColor = tile.Background != null ? GetColorByString(tile.Background) : Color.Black;
                    var cell = new T()
                    {
                        Glyph = tile.Glyph,
                        Position = position,
                        Foreground = foregroundColor,
                        Background = backgroundColor,
                        CellProperties = new EmberCell.EmberCellProperties
                        {
                            NormalForeground = foregroundColor,
                            NormalBackground = backgroundColor,
                            ForegroundFov = Color.Lerp(foregroundColor, Color.Black, .5f),
                            BackgroundFov = Color.Lerp(backgroundColor, Color.Black, .5f),
                            Walkable = tile.Walkable,
                            Name = tile.Name,
                            BlocksFov = tile.BlocksFov,
                        },
                        LightProperties = new EmberCell.LightEngineProperties
                        {
                            EmitsLight = tile.EmitsLight,
                            LightRadius = tile.LightRadius,
                            Brightness = tile.Brightness
                        }
                    };

                    if (!string.IsNullOrWhiteSpace(tile.LightColor))
                    {
                        cell.LightProperties.LightColor = GetColorByString(tile.LightColor);
                    }

                    cells.Add(cell);
                }
            }
            return cells.ToArray();
        }

        public T[] GetCellsRefactored()
        {
            var name = GetType().Name;
            var blueprintPath = Path.Combine(BlueprintPath, name + ".txt");
            var blueprintConfigPath = GetBlueprintConfigPath();

            if (!AllConfigPathsExist(blueprintPath, blueprintConfigPath, Constants.Blueprint.SpecialCharactersPath))
                return Array.Empty<T>();

            var specialConfig = JsonConvert.DeserializeObject<BlueprintConfig>(File.ReadAllText(Constants.Blueprint.SpecialCharactersPath));
            var specialChars = specialConfig.Tiles.ToDictionary(a => a.Glyph, a => a);

            var config = JsonConvert.DeserializeObject<BlueprintConfig>(File.ReadAllText(blueprintConfigPath));
            var tiles = config.Tiles.ToDictionary(a => a.Glyph, a => a);

            CheckForReservedChars(tiles, specialChars, name);

            var blueprint = File.ReadAllText(blueprintPath).Replace("\r", string.Empty).Split('\n');

            var nullTile = BlueprintTile.Null();
            var cells = GetCells(blueprint, nullTile, tiles, name);

            return cells.ToArray();
        }

        private static void CheckForReservedChars(Dictionary<char, BlueprintTile> tiles, IReadOnlyDictionary<char, BlueprintTile> specialChars, string name)
        {
            var containedKeys = tiles.Keys.Where(specialChars.ContainsKey);
            if (containedKeys.Any())
            {
                throw new Exception("Glyphs " + string.Join('\n', containedKeys.Select(key => $"'{key}'")) +
                                    "are reserved as a special character and cannot be used in " + name);
            }
        }

        private IEnumerable<T> GetCells(IReadOnlyList<string> blueprint, 
            BlueprintTile nullTile, 
            IReadOnlyDictionary<char, BlueprintTile> tiles, string name)
        {
            for (int y = 0; y < GridSizeY; y++)
            {
                for (int x = 0; x < GridSizeX; x++)
                {
                    var tile = nullTile;
                    if (TryGetCharValue(blueprint, x, y, out var charValue))
                        if (!tiles.TryGetValue(charValue, out tile))
                            throw new Exception("Glyph '" + charValue + 
                                                "' was not present in the config file for blueprint: " + name);

                    var position = new Point(x, y);
                    var cell = ConstructCell(tile, position);

                    if (string.IsNullOrWhiteSpace(tile.LightColor) == false)
                        cell.LightProperties.LightColor = GetColorByString(tile.LightColor);

                    yield return cell;
                }
            }
        }

        private static bool TryGetCharValue(IReadOnlyList<string> blueprint, int x, int y, out char charValue)
        {
            if (y >= blueprint.Count || x >= blueprint[y].Length)
            {
                charValue = ' ';
                return false;
            }
            charValue = blueprint[y][x];
            return true;
        }

        private static T ConstructCell(BlueprintTile tile, Point position)
        {
            var foregroundColor = GetColorByString(tile.Foreground);
            var backgroundColor = tile.Background != null ? GetColorByString(tile.Background) : Color.Black;
            return new T
            {
                Glyph = tile.Glyph,
                Position = position,
                Foreground = foregroundColor,
                Background = backgroundColor,
                CellProperties = new EmberCell.EmberCellProperties
                {
                    NormalForeground = foregroundColor,
                    NormalBackground = backgroundColor,
                    ForegroundFov = Color.Lerp(foregroundColor, Color.Black, .5f),
                    BackgroundFov = Color.Lerp(backgroundColor, Color.Black, .5f),
                    Walkable = tile.Walkable,
                    Name = tile.Name,
                    BlocksFov = tile.BlocksFov,
                },
                LightProperties = new EmberCell.LightEngineProperties
                {
                    EmitsLight = tile.EmitsLight,
                    LightRadius = tile.LightRadius,
                    Brightness = tile.Brightness
                }
            };
        }

        private bool AllConfigPathsExist(params string[] configPaths) => configPaths.All(File.Exists);

        private string GetBlueprintConfigPath()
        {
            return Path.Combine(Constants.Blueprint.BlueprintsConfigPath, Constants.Blueprint.BlueprintTiles + ".json");
        }

        private static Color GetColorByString(string value)
        {
            var prop = typeof(Color).GetProperty(value);
            if (prop != null)
                return (Color)prop.GetValue(null, null);
            return default;
        }
    }

    [Serializable]
    internal class BlueprintConfig
    {
#pragma warning disable 0649
        public BlueprintTile[] Tiles;
#pragma warning restore 0649
    }

    [Serializable]
    internal class BlueprintTile
    {
#pragma warning disable 0649
        public char Glyph;
        public string Name;
        public bool Walkable;
        public string Foreground;
        public string Background;
        public bool BlocksFov;
        public bool EmitsLight;
        public string LightColor;
        public int LightRadius;
        public float Brightness;
#pragma warning restore 0649
        public static BlueprintTile Null()
        {
            var nullTile = new BlueprintTile
            {
                Glyph = ' ',
                Foreground = "BurlyWood",
                Background = "Black",
                Name = null,
                Walkable = false
            };

            return nullTile;
        }
    }
}
