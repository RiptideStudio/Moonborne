/*
 * Author: Callen Betts (RiptideDev) 
 * Breif: Defines a base action class for using UI and interfacing
 */


using Microsoft.Xna.Framework;

namespace Moonborne.Engine.UI
{
    public abstract class Action
    {
        public float Duration = 1f;
        public float TimeElapsed = 0f;
        public bool Loop = true;
    }

    public class MoveAction : Action
    {
        public Vector2 Position;
    }
}
