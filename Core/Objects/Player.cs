using Emberpoint.Core.Extensions;
using Emberpoint.Core.Objects.Abstracts;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using SadConsole;

namespace Emberpoint.Core.Objects
{
    public class Player : EmberEntity
    {
        public Player() : base(Constants.Player.Foreground, Color.Transparent, Constants.Player.Character, 1, 1)
        {
            FieldOfViewRadius = Constants.Player.FieldOfViewRadius;
        }

        public void CheckForMovement()
        {
            if (Global.KeyboardState.IsKeyPressed(Keys.Z))
            {
                MoveTowards(Position.Translate(0, -1));
            }
            else if (Global.KeyboardState.IsKeyPressed(Keys.S))
            {
                MoveTowards(Position.Translate(0, 1));
            }
            else if (Global.KeyboardState.IsKeyPressed(Keys.Q))
            {
                MoveTowards(Position.Translate(-1, 0));
            }
            else if (Global.KeyboardState.IsKeyPressed(Keys.D))
            {
                MoveTowards(Position.Translate(1, 0));
            }
        }
    }
}
