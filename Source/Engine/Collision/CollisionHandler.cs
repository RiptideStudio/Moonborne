using Moonborne.Game.Objects;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Moonborne.Game.Room;
using System.Runtime.InteropServices;
using System;
using Moonborne.Game.Gameplay;
using MonoGame.Extended.Tiled;

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

                // Clear the TileList to avoid duplicates or stale collisions
                obj.TileList.Clear();

                foreach (var layer in LayerManager.Layers)
                {
                    // Only check against tile layers
                    if (layer.Value.Type != LayerType.Tile)
                        continue;

                    obj.NeedsLayerSort = true;
                }
            }

            Collisions.Clear();
        }


        /// <summary>
        /// Get a tile given a world position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool CollidingWithTile(float x, float y, Rectangle hitbox)
        {
            return CollidingWithTile((int)x, (int)y, hitbox);
        }

        /// <summary>
        /// Get a tile given a world position
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static bool CollidingWithTile(int x, int y, Rectangle hitbox)
        {
            Layer collidableLayer = null;
            foreach (var layer in LayerManager.Layers)
            {
                if (layer.Value.Collideable)
                {
                    collidableLayer = layer.Value;
                    break;
                }
            }

            if (collidableLayer == null)
            {
                return false;
            }

            int radius = 3;
            bool foundCollision = false;
            x /= 16;
            y /= 16;
            Tilemap tilemap = collidableLayer.Tilemaps[0];

            // Check adjacent tiles
            for (int i = 0; i < radius; i++)
            {
                for (int j = 0; j < radius; j++)
                {
                    int checkX = (x - (radius / 2) + i);
                    int checkY = (y - (radius / 2) + j);

                    // Clamp the positions to grid bounds
                    checkX = Math.Clamp(checkX, 0, tilemap.grid.GetLength(0) - 1);
                    checkY = Math.Clamp(checkY, 0, tilemap.grid.GetLength(1) - 1);

                    bool colliding = tilemap.grid[checkX, checkY] != 0;
                    Rectangle tileHitbox = new Rectangle(checkX * 16, checkY * 16, 16,16);

                    if (IsColliding(hitbox,tileHitbox) && colliding)
                    {
                        foundCollision = true;
                        break;
                    }
                }

                if (foundCollision)
                {
                    break;
                }
            }

            return foundCollision;
        }
    }
}