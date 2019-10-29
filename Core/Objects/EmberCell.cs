using Microsoft.Xna.Framework;
using SadConsole;

namespace Emberpoint.Core.Objects
{
    public class EmberCell : Cell
    {
        public Point Position { get; set; }
        public bool Walkable { get; set; }
        public string Name { get; set; }
        public Color NormalForeground { get; set; }
        public Color ForegroundFov { get; set; }

        public EmberCell() 
        {
            NormalForeground = Color.White;
            Foreground = Color.White;
            ForegroundFov = Color.Gray;
            Background = Color.Black;
            Walkable = true;
            Glyph = '.';
        }

        public EmberCell(Point position, int glyph, Color foreground, Color fov)
        {
            Position = position;
            Glyph = glyph;
            NormalForeground = foreground;
            Foreground = foreground;
            ForegroundFov = fov;
            Background = Color.Black;
            Walkable = true;
        }

        public EmberCell(Point position, int glyph, Color foreground, Color fov, Color background)
        {
            Position = position;
            Glyph = glyph;
            NormalForeground = foreground;
            Foreground = foreground;
            ForegroundFov = fov;
            Background = background;
            Walkable = true;
        }

        public void CopyFrom(EmberCell cell)
        {
            // Does foreground, background, glyph, mirror, decorators
            CopyAppearanceFrom(cell);

            ForegroundFov = cell.ForegroundFov;
            IsVisible = cell.IsVisible;
            Name = cell.Name;
            NormalForeground = cell.NormalForeground;
            Position = cell.Position;
            Walkable = cell.Walkable;
        }

        public new EmberCell Clone()
        {
            var cell = new EmberCell()
            {
                ForegroundFov = this.ForegroundFov,
                IsVisible = this.IsVisible,
                Name = this.Name,
                NormalForeground = this.NormalForeground,
                Position = this.Position,
                Walkable = this.Walkable
            };
            // Does foreground, background, glyph, mirror, decorators
            CopyAppearanceTo(cell);
            return cell;
        }
    }
}
