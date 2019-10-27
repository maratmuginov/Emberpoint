using Emberpoint.Core.Objects.Abstracts;
using Microsoft.Xna.Framework;

namespace Emberpoint.Core.Objects
{
    public class Player : EmberEntity
    {
        public Player(Point position) : base(Color.White, Color.Transparent, '@', 1, 1)
        {
            Position = position;
        }
    }
}
