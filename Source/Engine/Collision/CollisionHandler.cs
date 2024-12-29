using Moonborne.Game.Objects;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Moonborne.Game.Room;
using System.Runtime.InteropServices;
using System;

namespace Moonborne.Engine.Collision
{
    public static class CollisionHandler
    {
        public static List<GameObject> Collisions = new List<GameObject>();
        public static HashSet<GameObject> Colliding = new HashSet<GameObject>();

        /// <summary>
        /// Check if two rectangles are colliding
        /// </summary>
        /// <param name="first"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        public static bool IsColliding(Rectangle first, Rectangle other)
        {
           return first.X < other.X + other.Width &&
           first.X + first.Width > other.X &&
           first.Y < other.Y + other.Height &&
           first.Y + first.Height > other.Y;
        }

        /// <summary>
        /// Handle collisions between game objects
        /// </summary>
        /// <param name="first"></param>
        /// <param name="other"></param>
        public static void HandleCollisions(float dt)
        {
            // Check for collisions
            foreach (var obj in Collisions)
            {
                // Track colliding pairs
                bool isColliding = false;

                foreach (var obj2 in Collisions)
                {
                    // Don't collide with ourself
                    if (obj == obj2)
                        continue;

                    // Check if two objects are colliding
                    if (IsColliding(obj.Hitbox, obj2.Hitbox))
                    {
                        obj.OnCollision(obj2);
                        obj2.OnCollision(obj);
                        isColliding = true;
                    }
                }

                // Handle collision end overlap events
                if (!isColliding && Colliding.Contains(obj))
                {
                    obj.OnCollisionEnd();
                    Colliding.Remove(obj);
                }
                else if (isColliding && !Colliding.Contains(obj))
                {
                    Colliding.Add(obj);
                }

                // Check against collidable tilemaps
                foreach (var layer in LayerManager.Layers)
                {
                    if (!layer.Value.Collideable)
                        continue;

                    // Get our position cellwise and check if we are in a cell with collision
                    Tilemap tilemap = layer.Value.Tilemaps[0];
                    Vector2 newPosition = obj.Position;

                    int cellX = (int)((newPosition.X) / tilemap.tileSize);
                    int cellY = (int)((newPosition.Y) / tilemap.tileSize);

                    int nextX = cellX + (int)(obj.Velocity.X * dt);
                    int nextY = cellY + (int)(obj.Velocity.Y * dt);

                    // Never index outside the grid
                    nextX = Math.Clamp(nextX, 0, tilemap.grid.GetLength(0)-1);
                    nextY = Math.Clamp(nextY, 0, tilemap.grid.GetLength(1)-1);
                    cellX = Math.Clamp(cellX, 0, tilemap.grid.GetLength(0)-1);
                    cellY = Math.Clamp(cellY, 0, tilemap.grid.GetLength(1)-1);

                    bool horizontalCollision = tilemap.grid[nextX, cellY] > 0;
                    bool verticalCollision = tilemap.grid[cellX, nextY] > 0;

                    // Horizontal collision
                    if (horizontalCollision)
                    {
                        obj.Velocity.X = 0;
                    }

                    // Vertical collision
                    if (verticalCollision)
                    {
                        obj.Velocity.Y = 0;
                    }
                }
            }

            Collisions.Clear();
        }
    }
}