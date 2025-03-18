using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moonborne.Engine;
using Moonborne.Game.Gameplay;
using Moonborne.Game.Objects;
using Moonborne.Graphics;
using Moonborne.Graphics.Window;

namespace Moonborne.Graphics.Camera
{
    public class Camera : GameObject
    {
        public static Vector2 Position = Vector2.Zero;
        public static Vector2 TargetPosition = Vector2.Zero;
        public static float Zoom = 2.0f; // Default zoom level
        public static float CameraSize = 2.0f; // Our actual current camera size
        public static float GameCameraScale { get; set; } = 1; // The value we use in our game
        public static float MaxZoom = 100f; // How far we can zoom out
        public static float Rotation = 0f; // Default rotation
        public static float Smoothness { get; private set; } = 0.2f; // How much the camera lags behind
        public static Matrix TransformMatrix; // Transformation matrix
        public static GameObject Target { get; set; } // Target object to follow

        public static float ViewportScale = 1f;

        /// <summary>
        /// Object's create
        /// </summary>
        public override void Create()
        {
            base.Create();
            SpriteIndex.SetSpritesheet("Camera");
            SpriteIndex.VisibleInGame = false;
        }

        /// <summary>
        /// Follow the player
        /// </summary>
        /// <param name="dt"></param>
        public override void Update(float dt)
        {
            base.Update(dt);

            Transform.Position = Player.Instance.Transform.Position;
        }

        /// <summary>
        /// Draw the rectangle of our viewport
        /// </summary>
        /// <param name="spriteBatch"></param>
        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

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
                TargetPosition = Target.Transform.Position;
            }
        }

        /// <summary>
        /// Update the camera. Follows the target and updates the transform
        /// </summary>
        public static void Update()
        {
            FollowTarget();
            UpdateTransform();

            Zoom = MathHelper.Lerp(Zoom, CameraSize, 0.25f);
            Position.X = MathHelper.Lerp(Position.X, TargetPosition.X, Smoothness);
            Position.Y = MathHelper.Lerp(Position.Y, TargetPosition.Y + 8, Smoothness);

            Position.Y = (float)Math.Round(Position.Y,1);
            Position.X = (float)Math.Round(Position.X,1);
        }

        /// <summary>
        /// Set the position of the camera
        /// </summary>
        /// <param name="newPosition"></param>
        public static void SetPosition(Vector2 newPosition)
        {
            TargetPosition = newPosition;
            Position = newPosition;
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
            // Update our viewport scale
            ViewportScale = WindowManager.ViewportScale * (1 / Zoom);

            // Create the camera transformation matrix
            TransformMatrix = Matrix.CreateTranslation(new Vector3(-Position, 0f)) *
                        Matrix.CreateRotationZ(Rotation) *
                        Matrix.CreateScale(ViewportScale, ViewportScale, 1f) *
                        Matrix.CreateTranslation(new Vector3(WindowManager.ViewportWidth / 2f, WindowManager.ViewportHeight / 2f, 0f));
        }
    }
}
