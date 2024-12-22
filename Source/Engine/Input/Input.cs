using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
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
        public static MouseState MouseState;
        public static Vector2 MousePosition;

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
        /// Check if 
        /// </summary>
        /// <returns></returns>
        public static bool MouseLeftPressed()
        {
            return MouseState.LeftButton == ButtonState.Pressed;
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
            Vector2 viewportCenter = new Vector2(WindowManager.ViewportWidth / 2f, WindowManager.ViewportHeight / 2f);

            // Adjust mouse position for camera zoom and position
            Vector2 worldPosition = ((MousePosition - viewportCenter) / Camera.Zoom) + Camera.Position;

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
        /// Update input state given delta time
        /// </summary>
        /// <param name="dt"></param>
        public static void Update(float dt)
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
            MouseState = Mouse.GetState();
            MousePosition = MouseState.Position.ToVector2();
        }
    }
}