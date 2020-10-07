using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Emberpoint.Core.GameObjects.Map;
using GoRogue;
using Microsoft.Xna.Framework;
using System;
using Tests.TestObjects.Grids;

namespace Tests.TestObjects.Entities
{
    public class BaseEntity : IEntity
    {
        public Point Position { get; set; }

        public EventHandler<SadConsole.Entities.Entity.EntityMovedEventArgs> Moved;

        public int FieldOfViewRadius { get; set; } = 0;

        private FOV _fieldOfView;
        public FOV FieldOfView
        {
            get
            {
                return _fieldOfView ?? (_fieldOfView = new FOV(_grid.FieldOfView));
            }
        }

        public int ObjectId { get; }

        private EmberGrid _grid;

        public BaseEntity()
        {
            ObjectId = EntityManager.GetUniqueId();
            Moved += OnMove;
        }

        public void LoadGrid(BaseGrid grid)
        {
            _grid = grid;
        }

        private void OnMove(object sender, SadConsole.Entities.Entity.EntityMovedEventArgs args)
        {
            if (FieldOfViewRadius > 0)
            {
                // Re-calculate the field of view
                FieldOfView.Calculate(Position, FieldOfViewRadius);
            }
        }

        public bool CanMoveTowards(Point position)
        {
            return _grid.InBounds(position) && _grid.GetCell(position).CellProperties.Walkable && !EntityManager.EntityExistsAt(position);
        }

        public void MoveTowards(Point position, bool checkCanMove = true)
        {
            if (checkCanMove && !CanMoveTowards(position)) return;
            var prevPos = Position;
            Position = position;
            Moved.Invoke(this, new SadConsole.Entities.Entity.EntityMovedEventArgs(null, prevPos));
        }

        public void ResetFieldOfView()
        {
            _fieldOfView = null;
        }

        public void RenderObject(SadConsole.Console console)
        {
            throw new NotImplementedException();
        }

        public void UnRenderObject()
        {
            throw new NotImplementedException();
        }
    }
}
