using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Attempt_7._2DDrawing;
using Attempt_7.Cameras;
using Attempt_7.Analysis;
using Attempt_7.DrawAbleSimulationObjects;
using Attempt_7.ViewPorts;
using Attempt_7;


namespace Attempt_7.DrawAbleSimulationObjects
{
    public class Grass : DrawAbleSimulationObject
    {
          /// <summary>
        /// The vertexPositionColor array holding the information for the grass/ground. 
        /// </summary>
        private VertexPositionTexture[] grassAndGroundVertexPositionColor;

          
        /// <summary>
        /// Represents the texture of the grass bitmap that is used to represent the grass. 
        /// </summary>
        private Texture2D grass;

          /// <summary>
        /// A large tiled version of the grass Texture. 
        /// </summary>
        private Texture2D tiledGrass;


        /// <summary>        
        /// Initializes a new instance of the DrawImageAnalysis class.
        /// </summary>      
        public Grass(Game game) : base(game)
        {
            // Great information for the GPU
           this.basicEffects = new BasicEffect(Game.GraphicsDevice);

           this.grass = Game.Content.Load<Texture2D>("grass11"); // Load the grass texture into the game content

            // Make a new texture that is a tiled version of the grass. 
           this.tiledGrass = this.CreateGrassTexture(this.grass); // Make a big grass texture that is a tile of the small grass texture. 
           this.LoadGround();          

        }


        /// <summary>
        /// Initialize the all non graphics resournces. 
        /// </summary>
        //protected override void Initialize()
        //{

        //     base.Initialize();
        //}
      
        /// <summary>
        /// Create vertex data for the grass. Verts2 is the VertexPositionTexture array
        /// </summary>
        private void LoadGround()
        {
            this.grassAndGroundVertexPositionColor = new VertexPositionTexture[4];
            int dimension = 15;
            float textureDim = 1.0f;
            this.grassAndGroundVertexPositionColor[0] = new VertexPositionTexture(new Vector3(-dimension, dimension, 0), new Vector2(0, 0));
            this.grassAndGroundVertexPositionColor[1] = new VertexPositionTexture(new Vector3(dimension, dimension, 0), new Vector2(textureDim, 0));
            this.grassAndGroundVertexPositionColor[2] = new VertexPositionTexture(new Vector3(-dimension, -dimension, 0), new Vector2(0, textureDim));
            this.grassAndGroundVertexPositionColor[3] = new VertexPositionTexture(new Vector3(dimension, -dimension, 0), new Vector2(textureDim, textureDim));
        }

        /// <summary>
        /// Takes a texture and returns a larger texture. Size of the new texture is 1024,1024. 
        /// </summary>
        /// <param name="texturetoUse">The smaller texture to tile</param>
        /// <returns>The larger texture with the smaller on tiled onto it. </returns>
        private Texture2D CreateGrassTexture(Texture2D texturetoUse)
        {
            Color[,] groundColors = this.TextureTo2DArray(texturetoUse);
            Vector2 newTextureSize = new Vector2(1024, 1024); // Size of the new texture. 
            Color[] foregroundColors = new Color[(int)newTextureSize.X * (int)newTextureSize.Y];

            for (int x = 0; x < (int)newTextureSize.X; x++)
            {
                for (int y = 0; y < (int)newTextureSize.Y; y++)
                {
                    foregroundColors[x + (y * (int)newTextureSize.X)] = groundColors[(x * 2) % texturetoUse.Width, (y * 2) % texturetoUse.Height];
                }
            }

            Texture2D newTexture = new Texture2D(this.Game.GraphicsDevice, (int)newTextureSize.X, (int)newTextureSize.Y, false, SurfaceFormat.Color);
            newTexture.SetData(foregroundColors);
            return newTexture;
        }
        
        /// <summary>
        /// Takes a texture converts it into a 2D color array
        /// </summary>
        /// <param name="texture">The texture to convert</param>
        /// <returns>The 2D color array </returns>
        private Color[,] TextureTo2DArray(Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height]; // Create a 1D array
            texture.GetData(colors1D); // Pull the color data out of the texture to the 1D arrray

            Color[,] colors2D = new Color[texture.Width, texture.Height]; // Create the 2D arrray
            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    colors2D[x, y] = colors1D[x + (y * texture.Width)]; // Populate the 2D array with the correct values.  
                }
            }

            return colors2D;
        }
                
        /// <summary>
        /// Draw the grass or ground from "camera"'s perspective
        /// </summary>
        /// <param name="camera">The camera to use when drawing.</param>
        public void DrawGrass(Camera camera)
        {
            this.basicEffects.World = camera.World;
            this.basicEffects.View = camera.View;
            this.basicEffects.Projection = camera.Projection;
            this.basicEffects.VertexColorEnabled = false;
            this.basicEffects.Texture = this.tiledGrass;
            this.basicEffects.TextureEnabled = true; // Because the ground is a texture object. 


            foreach (EffectPass pass in this.basicEffects.CurrentTechnique.Passes)
            {
                pass.Apply();
                Game.GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, this.grassAndGroundVertexPositionColor, 0, 2);
            }
        }       
    }
}
