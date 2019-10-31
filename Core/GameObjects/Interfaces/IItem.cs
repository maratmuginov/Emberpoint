using Microsoft.Xna.Framework;
using System;

namespace Emberpoint.Core.GameObjects.Interfaces
{
    public interface IItem : IRenderable, IEquatable<IItem>, IComparable<IItem>
    {
        int ObjectId { get; }
        string Name { get; }
        int Amount { get; set; }
        Point Position { get; set; }
    }
}
