using Emberpoint.Core.GameObjects.Map;
using Microsoft.Xna.Framework;

namespace Tests.TestObjects.Grids
{
    public class BaseGrid : EmberGrid
    {
        private BaseGrid(int gridSizeX, int gridSizeY, EmberCell[] cells) : base(gridSizeX, gridSizeY, cells)
        { }

        private BaseGrid(Emberpoint.Core.GameObjects.Abstracts.Blueprint<EmberCell> blueprint) : base(blueprint)
        { }

        public static BaseGrid Create(int width, int height)
        {
            // Build a custom cell grid
            var cells = new EmberCell[width * height];
            for (var x = 0; x < width; x++)
            {
                for (var y = 0; y < height; y++)
                {
                    cells[y * width + x] = new EmberCell
                    {
                        Background = Color.Black,
                        Foreground = Color.Gray,
                        Glyph = '.',

                        CellProperties = new EmberCell.EmberCellProperties
                        {
                            Name = "floor",
                            Walkable = true
                        },
                        Position = new Point(x, y),
                    };
                }
            }
            return new BaseGrid(width, height, cells);
        }

        public static BaseGrid Create(Emberpoint.Core.GameObjects.Abstracts.Blueprint<EmberCell> blueprint)
        {
            return new BaseGrid(blueprint);
        }
    }
}
