using Microsoft.Xna.Framework;

namespace Emberpoint.Core.Extensions
{
    public static class MonoGameExtensions
    {
        public static Point Translate(this Point point, int x, int y)
        {
            return new Point(point.X + x, point.Y + y);
        }

        public static Point Translate(this Point point, Point other)
        {
            return new Point(point.X + other.X, point.Y + other.Y);
        }
    }
}
