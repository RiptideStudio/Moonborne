using System;
using Microsoft.Xna.Framework;
using Moonborne.Game.Objects;
using Moonborne.Graphics;
using Moonborne.Graphics.Window;

namespace Moonborne.Graphics.Camera
{
    public static class Camera
    {
        public static Vector2 Position = Vector2.Zero;
        public static float Zoom { get; private set; } = 2.0f; // Default zoom level
        public static float MaxZoom { get; private set; } = 10f; // Default zoom level
        public static float Rotation { get; private set; } = 0f; // Default rotation
        public static float InterpolationSpeed { get; private set; } = 0.2f; // How much the camera lags behind
        public static Matrix Transform { get; private set; } // Transformation matrix
        public static GameObject Target { get; private set; } // Target object to follow

        public static void Initialize()
        {
            UpdateTransform();
        }

        public static void SetTarget(GameObject newTarget)
        {
            Target = newTarget;
        }

        public static void FollowTarget()
        {
            if (Target != null)
            {
                SetPosition(Target.Position);
            }
        }

        public static void Update()
        {
            FollowTarget();
            UpdateTransform();
        }

        public static void SetPosition(Vector2 newPosition)
        {
            Position.X = MathHelper.Lerp(Position.X,newPosition.X+16,InterpolationSpeed);
            Position.Y = MathHelper.Lerp(Position.Y,newPosition.Y+16,InterpolationSpeed);
            UpdateTransform();
        }

        public static void SetZoom(float newZoom)
        {
            Zoom = MathHelper.Clamp(newZoom, 0.1f, MaxZoom);
            UpdateTransform();
        }

        public static void SetRotation(float newRotation)
        {
            Rotation = newRotation;
            UpdateTransform();
        }

        /// <summary>
        /// Update the camera's transform matrix
        /// </summary>
        public static void UpdateTransform()
        {
            // Create the camera transformation matrix
            Transform = Matrix.CreateTranslation(new Vector3(-Position, 0f)) *
                        Matrix.CreateRotationZ(Rotation) *
                        Matrix.CreateScale(WindowManager.ViewportScale*Zoom, WindowManager.ViewportScale*Zoom, 1f) *
                        Matrix.CreateTranslation(new Vector3(WindowManager.ViewportWidth / 2f, WindowManager.ViewportHeight / 2f, 0f));
        }
    }
}
