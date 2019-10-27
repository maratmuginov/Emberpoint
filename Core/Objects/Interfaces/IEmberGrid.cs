using Microsoft.Xna.Framework;

namespace Emberpoint.Core.Objects.Interfaces
{
    public interface IEmberGrid : IRenderable
    {
        int GridSizeX { get; }
        int GridSizeY { get; }
        EmberCell GetCell(Point position);
        EmberCell GetCell(int x, int y);
        void SetCell(EmberCell cell);
        EmberCell[] GetNeighbors(EmberCell cell);
        bool InBounds(int x, int y);
        bool InBounds(Point position);
    }
}
