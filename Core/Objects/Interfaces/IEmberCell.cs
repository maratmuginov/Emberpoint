using Microsoft.Xna.Framework;

namespace Emberpoint.Core.Objects.Interfaces
{
    public interface IEmberCell
    {
        Point Position { get; }
        int Character { get; }
        IEmberCell[] GetNeighbors(IEmberGrid grid);
    }
}
