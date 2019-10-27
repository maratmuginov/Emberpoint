using Emberpoint.Core.Objects.Interfaces;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Entities;

namespace Emberpoint.Core.Objects.Abstracts
{
    public abstract class EmberEntity : Entity, IEntity
    {
        public int ObjectId { get; }

        public EmberEntity(Color foreground, Color background, int glyph, int width = 1, int height = 1) : base(width, height)
        {
            ObjectId = EmberEntityDatabase.GetUniqueId();

            Animation.CurrentFrame[0].Foreground = foreground;
            Animation.CurrentFrame[0].Background = background;
            Animation.CurrentFrame[0].Glyph = glyph;
        }

        public void MoveTowards(Point position)
        {
            Position = position;
        }

        public void RenderObject(Console console)
        {
            console.Children.Add(this);
        }

        private static class EmberEntityDatabase
        {
            private static int _currentId;
            public static int GetUniqueId()
            {
                return _currentId++;
            }
        }
    }
}
