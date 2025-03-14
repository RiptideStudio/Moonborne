﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Moonborne.Engine.Collision;
using Moonborne.Game.Room;
using Moonborne.Graphics;
using Moonborne.Graphics.Camera;
using Moonborne.Graphics.Window;

namespace Moonborne.Input
{
    public static class InputManager
    {
        public static KeyboardState currentKeyboardState;
        public static KeyboardState previousKeyboardState;
        public static MouseState PreviousMouseState;
        public static MouseState MouseState;
        public static Vector2 MousePosition;
        public static Vector2 PreviousMousePosition;
        public static Vector2 MouseUIPosition;
        public static Vector2 PivotPoint;
        public static int PreviousScrollValue;
        public static int ScrollValue;
        public static Rectangle MouseHitbox = new Rectangle(0,0,6,6);
        public static Rectangle MouseWorldHitbox = new Rectangle(0,0,6,6);
        public static bool MouseIsLocked = false;
        public static Vector2 MouseDeltaPosition;
        public static MGame Game;

        /// <summary>
        /// Init input manager
        /// </summary>
        public static void Initialize(MGame game)
        {
            Game = game;
        }

        /// <summary>
        /// Check if a key is triggered by comparing previous and current state
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool KeyTriggered(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key) && previousKeyboardState.IsKeyUp(key);
        }        

        /// <summary>
        /// Locks the cursor and prevents the mouse from moving
        /// </summary>
        public static void SetMouseLocked(bool val)
        {
            // Set our pivot point if we decide to lock
            if (!MouseIsLocked)
            {
                PivotPoint = MousePosition;
            }

            Game.IsMouseVisible = !val;
            MouseIsLocked = val;
        }

        /// <summary>
        /// Returns the mouse's delta position (used for panning)
        /// </summary>
        /// <returns></returns>
        public static Vector2 GetMouseDeltaPosition()
        {
            return MouseDeltaPosition;
        }
        
        /// <summary>
        /// Check if a given key is held down
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool KeyDown(Keys key)
        {
            return currentKeyboardState.IsKeyDown(key);
        }        
        
        /// <summary>
        /// Check if a key was released
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static bool KeyReleased(Keys key)
        {
            return currentKeyboardState.IsKeyUp(key) && previousKeyboardState.IsKeyDown(key);
        }

        /// <summary>
        /// Check if left mouse pressed
        /// </summary>
        /// <returns></returns>
        public static bool MouseLeftPressed()
        {
            return MouseState.LeftButton == ButtonState.Pressed && PreviousMouseState.LeftButton == ButtonState.Released;
        }        
        
        /// <summary>
        /// Check if mouse left is held down
        /// </summary>
        /// <returns></returns>
        public static bool MouseLeftDown()
        {
            return MouseState.LeftButton == ButtonState.Pressed;
        }                
        
        /// <summary>
        /// If the mouse was released
        /// </summary>
        /// <returns></returns>
        public static bool MouseLeftReleased()
        {
            return MouseState.LeftButton == ButtonState.Released && PreviousMouseState.LeftButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Returns whether or not the right mouse button was released
        /// </summary>
        /// <returns></returns>
        public static bool MouseRightReleased()
        {
            return MouseState.RightButton == ButtonState.Released && PreviousMouseState.RightButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Middle mouse button down
        /// </summary>
        /// <returns></returns>
        public static bool MouseMiddleDown()
        {
            return MouseState.MiddleButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Middle released
        /// </summary>
        /// <returns></returns>
        public static bool MouseMiddleReleased()
        {
            return MouseState.MiddleButton == ButtonState.Released && PreviousMouseState.MiddleButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Check if mouse right is held down
        /// </summary>
        /// <returns></returns>
        public static bool MouseRightDown()
        {
            return MouseState.RightButton == ButtonState.Pressed;
        }

        /// <summary>
        /// Check if mouse right was pressed
        /// </summary>
        /// <returns></returns>
        public static bool MouseRightPressed()
        {
            return MouseState.RightButton == ButtonState.Pressed && PreviousMouseState.RightButton == ButtonState.Released;
        }

        /// <summary>
        /// If the mouse is wheeled up
        /// </summary>
        /// <returns></returns>
        public static bool MouseWheelUp()
        {
            return ScrollValue > PreviousScrollValue;
        }

        /// <summary>
        /// If the mouse is wheeled down
        /// </summary>
        /// <returns></returns>
        public static bool MouseWheelDown()
        {
            return ScrollValue < PreviousScrollValue;
        }

        /// <summary>
        /// Get the mouse's coordinates in world 
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <param name="viewportWidth"></param>
        /// <param name="viewportHeight"></param>
        /// <returns></returns>
        public static Vector2 MouseWorldCoords()
        {
            // Center of the screen (viewport center)
            Vector2 viewportCenter = new Vector2(WindowManager.ViewportWidth / WindowManager.ViewportScale/2, WindowManager.ViewportHeight / WindowManager.ViewportScale/2);

            // Adjust mouse position for camera zoom and position
            Vector2 worldPosition = ((MouseUIPosition - viewportCenter) / (1/Camera.Zoom)) + Camera.Position;

            return worldPosition;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public static Vector2 MouseDirection(Vector2 position)
        {
            return MouseWorldCoords() - position;
        }

        /// <summary>
        /// If our mouse is hovering over a target rectangle
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public static bool MouseIsHovering(Rectangle target)
        {
            return CollisionHandler.IsColliding(MouseHitbox, target);
        }

        /// <summary>
        /// Update input state given delta time
        /// </summary>
        /// <param name="dt"></param>
        public static void Update(float dt)
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            PreviousMouseState = MouseState;
            PreviousMousePosition = MouseState.Position.ToVector2();
            MouseState = Mouse.GetState();
            MousePosition = MouseState.Position.ToVector2();
            MouseUIPosition = MouseState.Position.ToVector2() / WindowManager.ViewportScale;
            PreviousScrollValue = ScrollValue;
            ScrollValue = MouseState.ScrollWheelValue;
            MouseHitbox.X = (int)MouseUIPosition.X-MouseHitbox.Width/2;
            MouseHitbox.Y = (int)MouseUIPosition.Y-MouseHitbox.Height/2;
            MouseWorldHitbox.X = (int)MousePosition.X- MouseWorldHitbox.Height/2;
            MouseWorldHitbox.Y = (int)MousePosition.Y- MouseWorldHitbox.Height/2;

            // Calculate the delta position of the mouse so we can detect how quickly it is moving
            MouseDeltaPosition = MousePosition - PreviousMousePosition;

            // Lock mouse position
            if (MouseIsLocked)
            {
                MouseDeltaPosition = PivotPoint - PreviousMousePosition;
                Mouse.SetPosition((int)PivotPoint.X, (int)PivotPoint.Y);
            }
        }
    }
}