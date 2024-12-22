using Microsoft.Xna.Framework;
using System;

namespace Moonborne.Utils.Math
{
    public static class MoonMath
    {
        /// <summary>
        /// Return the distance between two points
        /// </summary>
        /// <param name="pointOne"></param>
        /// <param name="pointTwo"></param>
        /// <returns></returns>
        public static float Distance(Vector2 pointOne, Vector2 pointTwo)
        {
            return (pointTwo - pointOne).Length();
        }

        /// <summary>
        /// Get a direction vector between two points
        /// </summary>
        /// <param name="pointOne"></param>
        /// <param name="pointTwo"></param>
        /// <returns></returns>
        public static Vector2 Direction(Vector2 start, Vector2 end)
        {
            return (end - start);
        }

        /// <summary>
        /// Returns a random integer between min and max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int RandomRange(int min, int max)
        {
            System.Random random = new System.Random();

            return random.Next(min, max);
        }        
        
        /// <summary>
        /// Returns a random float between min and max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static float RandomRange(float min, float max)
        {
            System.Random random = new System.Random();
            float t = (float)random.NextDouble();

            // Scale to the desired range
            return t * (max - min) + min;
        }

        /// <summary>
        /// Lerp a vector
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="interpolation"></param>
        /// <returns></returns>
        public static Vector2 Lerp(Vector2 start, Vector2 end, float interpolation)
        {
            Vector2 newValue = Vector2.One;

            newValue.X = MathHelper.Lerp(start.X, end.X, interpolation);
            newValue.Y = MathHelper.Lerp(start.Y, end.Y, interpolation);

            return newValue;
        }
    }
}
