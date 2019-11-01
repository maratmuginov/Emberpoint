using Emberpoint.Core.Extensions;
using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using GoRogue;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Emberpoint.Core.GameObjects.Map
{
    public class LightEngine<T> where T : EmberCell, ILightable
    {
        private bool _isCalibrated = false;
        public void Calibrate(T[] cells)
        {
            if (_isCalibrated) return;
            // Update light engine
            foreach (var cell in cells.Where(a => a.LightProperties.EmitsLight))
            {
                var oldCell = cell.Clone();
                oldCell.LightProperties.EmitsLight = false;
                AdjustLightLevels(cell, oldCell);
                GridManager.Grid.SetCell(cell);
            }
            _isCalibrated = true;
        }

        public void AdjustLightLevels(EmberCell newCell, EmberCell oldCell)
        {
            if (oldCell.LightProperties.EmitsLight == newCell.LightProperties.EmitsLight) return;

            var fov = new FOV(GridManager.Grid.FieldOfView);

            if (!newCell.LightProperties.EmitsLight)
            {
                fov.Calculate(newCell.Position, oldCell.LightProperties.LightRadius);

                var toChangeCells = new List<EmberCell>();
                for (int x = 0; x < GridManager.Grid.GridSizeX; x++)
                {
                    for (int y = 0; y < GridManager.Grid.GridSizeY; y++)
                    {
                        // If cell is in the field of view of the object
                        if (fov.BooleanFOV[x, y])
                        {
                            var pos = new Point(x, y);
                            var cellToAdd = newCell.Position == pos ? newCell : GridManager.Grid.GetCell(x, y);
                            cellToAdd.LightProperties.LightSources.Remove(newCell);
                            toChangeCells.Add(cellToAdd);
                        }
                    }
                }

                foreach (var cell in toChangeCells)
                {
                    if (cell.LightProperties.LightSources.Any()) continue;
                    cell.LightProperties.LightSources = null;
                    cell.LightProperties.Brightness = 0f;
                    cell.LightProperties.LightRadius = 0;
                    cell.LightProperties.LightColor = default;

                    GridManager.Grid.SetCellColors(cell, Game.Player, cell.CellProperties.NormalForeground, cell.CellProperties.ForegroundFov);
                    if (cell != newCell) // We don't need an infinite loop here :) It's passed by reference.
                        GridManager.Grid.SetCell(cell);
                }

                return;
            }

            var cells = new List<(EmberCell, float)>();
            fov.Calculate(newCell.Position, newCell.LightProperties.LightRadius);

            for (int x = 0; x < GridManager.Grid.GridSizeX; x++)
            {
                for (int y = 0; y < GridManager.Grid.GridSizeY; y++)
                {
                    // If cell is in the field of view of the object
                    if (fov.BooleanFOV[x, y])
                    {
                        var pos = new Point(x, y);
                        var distanceOfCenter = newCell.Position.SquaredDistance(pos);
                        var cellToAdd = newCell.Position == pos ? newCell : GridManager.Grid.GetCell(x, y);
                        if (cellToAdd.LightProperties.LightSources == null)
                        {
                            cellToAdd.LightProperties.LightSources = new List<EmberCell>();
                        }
                        cellToAdd.LightProperties.LightSources.Add(newCell);
                        cells.Add((cellToAdd, distanceOfCenter));
                    }
                }
            }

            var orderedCells = cells.OrderBy(a => a.Item2);
            var layers = orderedCells.Select(a => a.Item2).Distinct().ToList();
            var brightnessLayers = CalculateBrightnessLayers(newCell, layers);

            foreach (var lightedCell in orderedCells)
            {
                var brightness = brightnessLayers[lightedCell.Item2];
                if (lightedCell.Item1.LightProperties.Brightness < brightness)
                    lightedCell.Item1.LightProperties.Brightness = brightness;

                GridManager.Grid.SetCellColors(lightedCell.Item1, Game.Player, newCell.LightProperties.LightColor, Color.Lerp(newCell.LightProperties.LightColor, Color.Black, .5f));
                if (lightedCell.Item1 != newCell) // We don't need an infinite loop here :) It's passed by reference.
                    GridManager.Grid.SetCell(lightedCell.Item1);
            }
        }

        private Dictionary<float, float> CalculateBrightnessLayers(EmberCell centerCell, List<float> layers)
        {
            float deductAmount = centerCell.LightProperties.Brightness / layers.Count;
            float maxBrightness = centerCell.LightProperties.Brightness + deductAmount;
            return layers.ToDictionary(a => a, a => maxBrightness -= deductAmount);
        }
    }
}
