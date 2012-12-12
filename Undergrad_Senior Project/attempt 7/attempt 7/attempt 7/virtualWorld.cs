using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Attempt_7
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class virtualWorld
    {
        /// <summary>
        /// The basicEffects for rendering in 3D.
        /// </summary>
        private BasicEffect basicEffects;

        

        /// <summary>
        /// Represents the texture of the grass bitmap that is used to represent the grass. 
        /// </summary>
        private Texture2D grass;

        /// <summary>
        /// Represents a random object so that random numbers can be generated. 
        /// </summary>
        private Random rand;


        /// <summary>
        /// The vertexPositionColor array holding the vertex information needed for drawing the white lines representing the course. 
        /// </summary>
        private VertexPositionColor[] whiteLinesVertexPositionColor;

        /// <summary>
        /// The vertexPositionColor array holding the information for the grass/ground. 
        /// </summary>
        private VertexPositionTexture[] grassAndGroundVertexPositionColor;

        public virtualWorld(BasicEffect basicEffects, Texture2D grass)
        {
            this.basicEffects = basicEffects;
            // TODO: Construct any child components here
       
            // TODO: Add your initialization code here
            this.rand = new Random();

            this.largeGrass = this.CreateGrassTexture(this.grass); // Make a big grass texture that is a tile of the small grass texture. 



          
            // Load the vertex arrays for the lines, ground, and robot. 
            this.LoadLines();
            this.LoadGround();
           

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }
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
        /// Creates the vertexPositionColor array "verts1" that is used to draw the white lines. 
        /// </summary>
        private void LoadLines()
        {
            this.whiteLinesVertexPositionColor = new VertexPositionColor[37 * 4]; // 2 innerlines and 2 outerlines
            int j = 0;
            float randomFirstX = 0, randomFirstY = 0;

            //Curve c = new Curve();
            //CurveKeyCollection cCollection = new CurveKeyCollection();
            //cCollection.Add(new CurveKey(

            // "i" is in radians going around the circle. 
            for (float i = 0; i < MathHelper.Pi * 2; i += MathHelper.Pi / 18)
            {
                float randomAdd1 = this.rand.Next(0, 2);
                float randomAdd2 = this.rand.Next(0, 2);

                if (j == 0)
                {
                    randomFirstX = randomAdd1; // Store the random changes for the first set of points
                    randomFirstY = randomAdd2;
                }

                if (j == 36)
                {
                    randomAdd1 = randomFirstX; // Apply the same random changes to the last that you did the first. 
                    randomAdd2 = randomFirstY;
                }
                this.trackArray[j] = new Vector2((float)((Math.Cos(i) * 10) + randomAdd1), (float)((Math.Sin(i) * 10) + randomAdd2));

                this.whiteLinesVertexPositionColor[j] = new VertexPositionColor(new Vector3(trackArray[j].X, trackArray[j].Y, 0), Color.WhiteSmoke); // 1st inside line
                this.whiteLinesVertexPositionColor[j + 37] = new VertexPositionColor(new Vector3((float)((Math.Cos(i) * 15) + randomAdd1), (float)((Math.Sin(i) * 14) + randomAdd2), 0), Color.White); // 1st outside line
                this.whiteLinesVertexPositionColor[j + (37 * 2)] = new VertexPositionColor(new Vector3((float)((Math.Cos(i) * 10.2f) + randomAdd1), (float)((Math.Sin(i) * 10.2f) + randomAdd2), 0), Color.White); // 2nd inside line -- make it thicker
                this.whiteLinesVertexPositionColor[j + (37 * 3)] = new VertexPositionColor(new Vector3((float)((Math.Cos(i) * 15.2f) + randomAdd1), (float)((Math.Sin(i) * 14.2f) + randomAdd2), 0), Color.White); // 2nd outside line -- make it thicker

                j++;
            }

            // Put the robot's starting position between the lines and store the robot's start position
            this.robot1.SetStartPosition(new Vector3((float)(10.5f + randomFirstX), (float)(0 + (randomFirstY / 2)), 0));
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

            Texture2D newTexture = new Texture2D(this.GraphicsDevice, (int)newTextureSize.X, (int)newTextureSize.Y, false, SurfaceFormat.Color);
            newTexture.SetData(foregroundColors);
            return newTexture;
        }


        /// <summary>
        /// Draw the lines from the "camera"'s persepective. 
        /// </summary>
        /// <param name="camera">The camera to use when drawing.</param>
        public  void DrawColorLines(Camera camera)
        {
            this.basicEffects.World = camera.World;
            this.basicEffects.View = camera.View;
            this.basicEffects.Projection = camera.Projection;
            this.basicEffects.TextureEnabled = false;
            this.basicEffects.VertexColorEnabled = true;

            foreach (EffectPass pass in this.basicEffects.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, this.whiteLinesVertexPositionColor, 0, 36); // Draw the 1st inner line
                this.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, this.whiteLinesVertexPositionColor, 37, 36); // Draw the 2nd inner line
                this.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, this.whiteLinesVertexPositionColor, 37 * 2, 36); // Draw the outerline
                this.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, this.whiteLinesVertexPositionColor, 37 * 3, 36); // Draw the 2nd outerline               
            }
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
            this.basicEffects.Texture = this.largeGrass;
            this.basicEffects.TextureEnabled = true; // Because the ground is a texture object. 


            foreach (EffectPass pass in this.basicEffects.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, this.grassAndGroundVertexPositionColor, 0, 2);
            }
        }
        

            public virtual void Draw (GameTime gameTime)
            {

            }
    }
}
