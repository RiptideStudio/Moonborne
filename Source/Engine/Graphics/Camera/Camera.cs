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
        public static float Zoom = 2.0f; // Default zoom level
        public static float TargetZoom = 2.0f; // Default zoom level
        public static float MaxZoom { get; private set; } = 10f; // Default zoom level
        public static float Rotation { get; private set; } = 0f; // Default rotation
        public static float InterpolationSpeed { get; private set; } = 0.2f; // How much the camera lags behind
        public static Matrix Transform; // Transformation matrix
        public static GameObject Target { get; private set; } // Target object to follow

        /// <summary>
        /// Initialize the camera's transform
        /// </summary>
        public static void Initialize()
        {
            UpdateTransform();
        }

        /// <summary>
        /// Set the target object
        /// </summary>
        /// <param name="newTarget"></param>
        public static void SetTarget(GameObject newTarget)
        {
            Target = newTarget;
        }

        /// <summary>
        /// Follow the target entity
        /// </summary>
        public static void FollowTarget()
        {
            if (Target != null)
            {
                SetPosition(Target.Position);
            }
        }

        /// <summary>
        /// Update the camera. Follows the target and updates the transform
        /// </summary>
        public static void Update()
        {
            FollowTarget();
            UpdateTransform();
            Zoom = MathHelper.Lerp(Zoom, TargetZoom, 0.25f);
        }

        /// <summary>
        /// Set the position of the camera
        /// </summary>
        /// <param name="newPosition"></param>
        public static void SetPosition(Vector2 newPosition)
        {
            Position.X = MathHelper.Lerp(Position.X,newPosition.X,InterpolationSpeed);
            Position.Y = MathHelper.Lerp(Position.Y,newPosition.Y+8,InterpolationSpeed);
            UpdateTransform();
        }

        /// <summary>
        /// Set the zoom of our camera
        /// </summary>
        /// <param name="newZoom"></param>
        public static void SetZoom(float newZoom)
        {
            Zoom = MathHelper.Clamp(newZoom, 0.1f, MaxZoom);
            UpdateTransform();
        }

        /// <summary>
        /// Set the rotation of our camera. Used for screen shake
        /// </summary>
        /// <param name="newRotation"></param>
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
