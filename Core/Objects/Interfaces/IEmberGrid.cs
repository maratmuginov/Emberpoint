using Microsoft.Xna.Framework;

namespace Emberpoint.Core.Objects.Interfaces
{
    public interface IEmberGrid : IRenderable
    {
        int GridSize { get; }
        IEmberCell GetCell(Point position);
        IEmberCell GetCell(int x, int y);
        void SetCell(IEmberCell cell);
        IEmberCell[] GetNeighbors(IEmberCell cell);
    }
}
