using Emberpoint.Core.Objects.Interfaces;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Emberpoint.Core.Objects
{
    public class EmberGrid : IEmberGrid
    {
        private IEmberCell[] Cells { get; }

        public int GridSize { get; }

        public EmberGrid(int gridSize, IEmberCell[] cells)
        {
            GridSize = gridSize;
            Cells = cells;
        }

        public IEmberCell GetCell(Point position)
        {
            return Cells[position.Y * GridSize + position.X];
        }

        public IEmberCell GetCell(int x, int y)
        {
            return Cells[y * GridSize + x];
        }

        public void RenderObject()
        {

        }

        public void SetCell(IEmberCell cell)
        {
            Cells[cell.Position.Y * GridSize + cell.Position.X] = cell;
        }

        public IEmberCell[] GetNeighbors(IEmberCell cell)
        {
            int x = cell.Position.X;
            int y = cell.Position.Y;

            var points = new List<Point>();
            if (!InBounds(x, y)) return Array.Empty<IEmberCell>();

            if (InBounds(x + 1, y)) points.Add(new Point(x + 1, y));
            if (InBounds(x - 1, y)) points.Add(new Point(x - 1, y));
            if (InBounds(x, y + 1)) points.Add(new Point(x, y + 1));
            if (InBounds(x, y - 1)) points.Add(new Point(x, y - 1));
            if (InBounds(x + 1, y + 1)) points.Add(new Point(x + 1, y + 1));
            if (InBounds(x - 1, y - 1)) points.Add(new Point(x - 1, y - 1));
            if (InBounds(x + 1, y - 1)) points.Add(new Point(x + 1, y - 1));
            if (InBounds(x - 1, y + 1)) points.Add(new Point(x - 1, y + 1));

            return points.Select(a => GetCell(a)).ToArray();
        }

        private bool InBounds(int x, int y)
        {
            return x >= 0 && y >= 0 && x < GridSize && y < GridSize;
        }
    }
}
