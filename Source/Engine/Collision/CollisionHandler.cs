using Moonborne.Game.Objects;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Moonborne.Game.Room;
using System.Runtime.InteropServices;
using System;
using Moonborne.Game.Gameplay;

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

                // Check against collidable tilemaps
                bool onStairs = false;
                int lowestLayer = 1;
                int highestLayer = 1;

                foreach (var layer in LayerManager.Layers)
                {
                    // Only check against tile layers
                    if (layer.Value.Type != LayerType.Tile)
                        continue;

                    // Get the first tilemap in the layer
                    Tilemap tilemap = layer.Value.Tilemaps[0];
                    Vector2 newPosition = obj.Position;

                    // Get current and next cell positions
                    int cellX = (int)((newPosition.X) / tilemap.tileSize);
                    int cellY = (int)((newPosition.Y) / tilemap.tileSize);

                    int nextX = cellX + (int)(obj.Velocity.X * dt);
                    int nextY = cellY + (int)(obj.Velocity.Y * dt);

                    // Clamp the positions to grid bounds
                    nextX = Math.Clamp(nextX, 0, tilemap.grid.GetLength(0) - 1);
                    nextY = Math.Clamp(nextY, 0, tilemap.grid.GetLength(1) - 1);
                    cellX = Math.Clamp(cellX, 0, tilemap.grid.GetLength(0) - 1);
                    cellY = Math.Clamp(cellY, 0, tilemap.grid.GetLength(1) - 1);

                    // Check for collisions
                    bool horizontalCollision = tilemap.grid[nextX, cellY] > 0;
                    bool verticalCollision = tilemap.grid[cellX, nextY] > 0;

                    // Process the current cell
                    bool collision = tilemap.grid[cellX, cellY] > 0;

                    if (collision)
                    {
                        // Compute the unique key for the tile
                        int tileKey = cellX + cellY * tilemap.grid.GetLength(0);

                        // Add the colliding tile to the TileList
                        if (tilemap.TileList.TryGetValue(tileKey, out var collidingTile))
                        {
                            if (!obj.TileList.Contains(collidingTile))
                            {
                                obj.TileList.Add(collidingTile);
                            }
                        }
                    }

                    // Process colliding tiles
                    foreach (Tile tile in obj.TileList)
                    {
                        if (tile.TileType == TileType.StairUp || tile.TileType == TileType.StairDown)
                        {
                            if (collision)
                            {
                                onStairs = true;
                            }
                        }
                        if (tile.Height > highestLayer)
                        {
                            highestLayer = tile.Height;
                        }
                        if (tile.Height < lowestLayer)
                        {
                            lowestLayer = tile.Height;
                        }
                    }
                }

                // Check for going up
                if (obj.Height != highestLayer)
                {
                    if (onStairs)
                    {
                        // Check direction of movement
                        if (obj.Velocity.Y < 0)
                        {
                            obj.Height = highestLayer;
                        }
                    }
                    else
                    {
                        obj.Position = obj.OldPosition;
                    }
                }

                // Check for going down
                if (obj.Height != lowestLayer)
                {
                    if (onStairs)
                    {
                        // Check direction of movement
                        if (obj.Velocity.Y > 0)
                        {
                            obj.Height = lowestLayer;
                        }
                    }
                }
            }

            Collisions.Clear();
        }
    }
}