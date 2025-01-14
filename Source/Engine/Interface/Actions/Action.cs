/*
 * Author: Callen Betts (RiptideDev) 
 * Breif: Defines a base action class for using UI and interfacing
 */


using Microsoft.Xna.Framework;
using Moonborne.Game.Objects;

namespace Moonborne.Engine.UI
{
    /// <summary>
    /// Base action class
    /// </summary>
    public abstract class GameAction
    {
        public float Duration = 1f;
        public float TimeElapsed = 0f;
        public float InterpolationAmount = 0.2f;
        public bool Loop = false;
        public bool Playing = true;
        public GameObject Parent;

        /// <summary>
        /// Update the 
        /// </summary>
        /// <param name="dt"></param>
        public virtual void Update(float dt)
        {
            // If parent does not exist do not update
            if (Parent == null)
                return;

            TimeElapsed += dt;

            if (TimeElapsed > Duration)
            {
                TimeElapsed = 0f;

                if (Loop)
                {
                    // Insert code for looping...
                }
                else
                {
                    // Stop playing
                    Playing = false;
                    Parent.ActionsToDestroy.Add(this);
                }
            }
            else
            {
                if (Playing)
                {
                    Advance(dt);
                }
            }
        }

        /// <summary>
        /// Advance a frame if possible
        /// </summary>
        /// <param name="dt"></param>
        public virtual void Advance(float dt)
        {

        }
    }

    /// <summary>
    /// Translate action
    /// </summary>
    public class MoveAction : GameAction
    {
        public Vector2 TargetPosition;

        public MoveAction(Vector2 targetPosition)
        {
            TargetPosition = targetPosition;
        }

        public override void Advance(float dt)
        {
            Parent.Position.X = MathHelper.Lerp(Parent.Position.X, TargetPosition.X, InterpolationAmount);
            Parent.Position.Y = MathHelper.Lerp(Parent.Position.Y, TargetPosition.Y, InterpolationAmount);
        }
    }

    /// <summary>
    /// Fade alpha of object
    /// </summary>
    public class FadeAction : GameAction
    {
        public float TargetValue;

        public FadeAction(float targetValue)
        {
            TargetValue = targetValue;
        }

        public override void Advance(float dt)
        {
            Parent.Alpha = MathHelper.Lerp(Parent.Alpha, TargetValue, InterpolationAmount);
        }
    }
}
