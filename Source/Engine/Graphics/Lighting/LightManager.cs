using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Moonborne.Engine.Components;
using Moonborne.Graphics;
using Moonborne.Graphics.Camera;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Moonborne.Engine.Graphics.Lighting
{
    public class LightManager
    {
        public List<Vector2[]> shadowPolygons = new List<Vector2[]>();
        public GraphicsDevice graphicsDevice;
        public RenderTarget2D LightMap;

        /// <summary>
        /// Setup lighting manager
        /// </summary>
        /// <param name="graphicsDevice"></param>
        public LightManager(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            LightMap = new RenderTarget2D(graphicsDevice, 1920, 1080);
        }

        /// <summary>
        /// Calculate the shadows needed for lights
        /// </summary>
        /// <param name="obstacles"></param>
        /// <param name="lightPos"></param>
        public void CalculateShadows(List<Rectangle> obstacles, Vector2 lightPos)
        {
            shadowPolygons.Clear();

            foreach (var obstacle in obstacles)
            {
                        Vector2[] corners = {
                    new Vector2(obstacle.Left, obstacle.Top),
                    new Vector2(obstacle.Right, obstacle.Top),
                    new Vector2(obstacle.Right, obstacle.Bottom),
                    new Vector2(obstacle.Left, obstacle.Bottom)
                };

                List<Vector2> shadowVertices = new List<Vector2>();

                foreach (var corner in corners)
                {
                    Vector2 direction = Vector2.Normalize(corner - lightPos);
                    Vector2 extended = corner + (direction * 500); // Extend shadow

                    shadowVertices.Add(corner);
                    shadowVertices.Add(extended);
                }

                shadowPolygons.Add(shadowVertices.ToArray());
            }
        }

        /// <summary>
        /// Draws all of our lights in the scene
        /// </summary>
        public void RenderLights()
        {
            graphicsDevice.SetRenderTarget(LightMap);
            graphicsDevice.Clear(Color.Black);
            List<Light> lights = ObjectComponent.GetAllComponents<Light>();
            SpriteManager.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, null, null, null, Camera.TransformMatrix);

            // Draw Lights
            foreach (var light in lights)
            {
                Vector2 origin = new Vector2(light.Sprite.TextureWidth / 2, light.Sprite.TextureHeight / 2);
                float scale = light.Radius / (light.Sprite.TextureWidth / 2);

                SpriteManager.spriteBatch.Draw(
                    light.Sprite.Data,
                    light.Position,
                    null,
                    Color.White * light.Intensity,
                    0f,
                    origin,
                    scale,
                    SpriteEffects.None,
                    0f);
            }

            SpriteManager.spriteBatch.End();

            // Draw the actual shadows
            DrawShadows();
        }

        /// <summary>
        /// Render the shadows
        /// </summary>
        public void DrawShadows()
        {
            foreach (var polygon in shadowPolygons)
            {
                VertexPositionColor[] vertices = new VertexPositionColor[polygon.Length];

                for (int i = 0; i < polygon.Length; i++)
                {
                    vertices[i] = new VertexPositionColor(new Vector3(polygon[i], 0), Color.Black * 0.7f);
                }

                graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, polygon.Length - 2);
            }

            // Reset target render
            graphicsDevice.SetRenderTarget(null);
        }

    }
}
