using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Attempt_7._2DDrawing;
using Attempt_7.Cameras;
using Attempt_7.Analysis;
using Attempt_7.DrawAbleSimulationObjects;
using Attempt_7.ViewPorts;
using Attempt_7;


namespace Attempt_7.DrawAbleSimulationObjects
{
    public class CourseLines :DrawAbleSimulationObject
    {

        /// <summary>
        /// The vertexPositionColor array holding the vertex information needed for drawing the white lines representing the course. 
        /// </summary>
        private VertexPositionColor[] whiteLinesVertexPositionColor;

        /// <summary>
        /// The Start Position of the Track
        /// </summary>
        public Vector3 startPosition { get; set; }

        /// <summary>
        /// Random Number Generator
        /// </summary>
        private Random randomGenerator { get; set; }


        /// <summary>
        /// track Array 
        /// </summary>
        private Vector3[] trackArray;

        /// <summary>
        /// Constructor. Generates the needed arrays for the course lines. 
        /// </summary>
        public CourseLines(Game game) : base(game)
        {
            int numberOfSpokes = ((SimulationMain)this.Game).config.coureLinesPointsPerCircle;
            this.trackArray = new Vector3[numberOfSpokes * 4];

            LoadLines();
             this.basicEffects = new BasicEffect(Game.GraphicsDevice); // Create a basic effects object so we the GPU knows how to render the vertex data
           
            
            
        }

        /// <summary>
        /// Creates the vertexPositionColor array "verts1" that is used to draw the white lines. 
        /// </summary>
        private void LoadLines()
        {

            // TODO: Add your initialization code here
            this.randomGenerator = new Random();
           

            this.whiteLinesVertexPositionColor = new VertexPositionColor[37 * 4]; // 2 innerlines and 2 outerlines
            int j = 0;
            float randomFirstX = 0, randomFirstY = 0;

            //Curve c = new Curve();
            //CurveKeyCollection cCollection = new CurveKeyCollection();
            //cCollection.Add(new CurveKey(

            // "i" is in radians going around the circle. 
            for (float i = 0; i < MathHelper.Pi * 2; i += MathHelper.Pi / 18)
            {
                float randomAdd1 = this.randomGenerator.Next(0, 2);
                float randomAdd2 = this.randomGenerator.Next(0, 2);

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


                this.trackArray[j + (37 * 0)] = new Vector3((float)((Math.Cos(i) * 10) + randomAdd1), (float)((Math.Sin(i) * 10) + randomAdd2), 0);
                this.trackArray[j + (37 * 1)] = new Vector3((float)((Math.Cos(i) * 15) + randomAdd1), (float)((Math.Sin(i) * 14) + randomAdd2), 0);
                this.trackArray[j + (37 * 2)] = new Vector3((float)((Math.Cos(i) * 10.2f) + randomAdd1), (float)((Math.Sin(i) * 10.2f) + randomAdd2), 0);
                this.trackArray[j + (37 * 3)] = new Vector3((float)((Math.Cos(i) * 15.2f) + randomAdd1), (float)((Math.Sin(i) * 14.2f) + randomAdd2), 0);



                // Create The Vertex PositionColor's of the lines. 
                this.whiteLinesVertexPositionColor[j + (37 * 0)] = new VertexPositionColor(this.trackArray[j + (37 * 0)], Color.White); // 1st inside line
                this.whiteLinesVertexPositionColor[j + (37 * 1)] = new VertexPositionColor(this.trackArray[j + (37 * 1)], Color.White); // 1st outside line
                this.whiteLinesVertexPositionColor[j + (37 * 2)] = new VertexPositionColor(this.trackArray[j + (37 * 2)], Color.White); // 2nd inside line -- make it thicker
                this.whiteLinesVertexPositionColor[j + (37 * 3)] = new VertexPositionColor(this.trackArray[j + (37 * 3)], Color.White); // 2nd outside line -- make it thicker

                j++;
            }

            this.startPosition = new Vector3( (float)((10.2 + 15) / 2),0, 0);
        }


        /// <summary>
        /// Draw the lines from the "camera"'s persepective. 
        /// </summary>
        /// <param name="camera">The camera to use when drawing.</param>
        public  void DrawCouseLines(Camera camera)
        {
            this.basicEffects.World = camera.World;
            this.basicEffects.View = camera.View;
            this.basicEffects.Projection = camera.Projection;
            this.basicEffects.TextureEnabled = false;
            this.basicEffects.VertexColorEnabled = true;

            foreach (EffectPass pass in this.basicEffects.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.Game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, this.whiteLinesVertexPositionColor, 0, 36); // Draw the 1st inner line
                this.Game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, this.whiteLinesVertexPositionColor, 37, 36); // Draw the 2nd inner line
                this.Game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, this.whiteLinesVertexPositionColor, 37 * 2, 36); // Draw the outerline
                this.Game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, this.whiteLinesVertexPositionColor, 37 * 3, 36); // Draw the 2nd outerline               
            }
        }

    }
}
