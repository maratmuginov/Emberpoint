using Emberpoint.Core.Objects.Interfaces;
using Microsoft.Xna.Framework;

namespace Emberpoint.Core.Objects.Abstracts
{
    public abstract class EmberEntity : IEntity
    {
        public Point Position { get; set; }
        public int ObjectId { get; }

        public EmberEntity()
        {
            ObjectId = EmberEntityDatabase.GetUniqueId();
        }

        public void MoveTowards(Point position)
        {
            Position = position;
        }

        public void RenderObject()
        {

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
