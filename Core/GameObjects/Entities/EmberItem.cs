using Emberpoint.Core.GameObjects.Interfaces;
using Emberpoint.Core.GameObjects.Managers;
using Microsoft.Xna.Framework;
using SadConsole;
using SadConsole.Entities;

namespace Emberpoint.Core.GameObjects.Entities
{
    /// <summary>
    /// An item is also a renderable entity.
    /// </summary>
    public class EmberItem : Entity, IItem
    {
        public int ObjectId { get; }
        public int Amount { get; set; }
        public new string Name { get; }

        private Console _renderedConsole;

        public EmberItem(string name, int glyph, Color foregroundColor, int width = 1, int height = 1) : base(width, height)
        {
            ObjectId = ItemManager.GetUniqueId();
            ItemManager.Add(this);

            Name = name;
            Animation.CurrentFrame[0].Foreground = foregroundColor;
            Animation.CurrentFrame[0].Background = Color.Transparent;
            Animation.CurrentFrame[0].Glyph = glyph;
        }

        public void RenderObject(Console console)
        {
            _renderedConsole = console;
            console.Children.Add(this);
        }

        public void UnRenderObject()
        {
            if (_renderedConsole != null)
            {
                _renderedConsole.Children.Remove(this);
                _renderedConsole = null;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public bool Equals(IItem other)
        {
            // First two lines are just optimizations
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;

            return Name.Equals(other.Name);
        }

        public override bool Equals(object obj)
        {
            return Equals((IItem)obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        public int CompareTo(IItem other)
        {
            return string.Compare(Name, other.Name);
        }
    }
}
