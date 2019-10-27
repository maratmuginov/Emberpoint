using Microsoft.Xna.Framework;

namespace Emberpoint.Core.Objects.Interfaces
{
    public interface IEntity : IRenderable
    {
        Point Position { get; set; }
        void MoveTowards(Point position);
    }
}
