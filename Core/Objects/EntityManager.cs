using Emberpoint.Core.Objects.Abstracts;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Emberpoint.Core.Objects
{
    public static class EntityManager
    {
        /// <summary>
        /// Returns a new entity or null, if the position is already taken.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="position"></param>
        /// <returns></returns>
        public static T Create<T>(Point position) where T : EmberEntity, new()
        {
            if (EntityExistsAt(position))
            {
                return default;
            }

            var entity = new T()
            {
                Position = position
            };
            EmberEntityDatabase.Entities.Add(entity.ObjectId, entity);
            return entity;
        }

        public static int GetUniqueId()
        {
            return EmberEntityDatabase.GetUniqueId();
        }

        public static void Remove(EmberEntity entity)
        {
            EmberEntityDatabase.Entities.Remove(entity.ObjectId);
        }

        public static void Clear()
        {
            EmberEntityDatabase.Reset();
        }

        public static T GetEntityAt<T>(Point position) where T : EmberEntity
        {
            return (T)EmberEntityDatabase.Entities.Values.SingleOrDefault(a => a.Position == position);
        }

        public static T GetEntityAt<T>(int x, int y) where T : EmberEntity
        {
            return (T)EmberEntityDatabase.Entities.Values.SingleOrDefault(a => a.Position.X == x && a.Position.Y == y);
        }

        public static bool EntityExistsAt(int x, int y)
        {
            return GetEntityAt<EmberEntity>(x, y) != null;
        }

        public static bool EntityExistsAt(Point position)
        {
            return GetEntityAt<EmberEntity>(position) != null;
        }

        private static class EmberEntityDatabase
        {
            public static readonly Dictionary<int, EmberEntity> Entities = new Dictionary<int, EmberEntity>();

            private static int _currentId;
            public static int GetUniqueId()
            {
                return _currentId++;
            }

            public static void Reset()
            {
                Entities.Clear();
                _currentId = 0;
            }
        }
    }
}
