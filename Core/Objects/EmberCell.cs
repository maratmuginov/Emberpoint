using Emberpoint.Core.Objects.Interfaces;
using Microsoft.Xna.Framework;

namespace Emberpoint.Core.Objects
{
    public class EmberCell : IEmberCell
    {
        public Point Position { get; }

        public int Character { get; private set; }

        public EmberCell(Point position, int character)
        {
            Position = position;
            Character = character;
        }

        public IEmberCell[] GetNeighbors(IEmberGrid grid)
        {
            return grid.GetNeighbors(this);
        }
    }
}
