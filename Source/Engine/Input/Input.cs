using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Moonborne.Input
{
    public static class InputManager
    {
        static KeyboardState currentKeyboardState;
        static KeyboardState previousKeyboardState;

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
        /// Update input state given delta time
        /// </summary>
        /// <param name="dt"></param>
        public static void Update(float dt)
        {
            previousKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();
        }
    }
}