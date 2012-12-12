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

namespace Attempt_7
{
    public class HoughLines: DrawAbleSimulationObject
    {

        public HoughLines(Game game)
            : base(game)
        {
            
        }

        /// <summary>
        /// Stores information about lines from the hough transform.    
        /// each polarRho we want to find = 7 more values to store
        /// 0=slope, 1= yInt, 2=Rho, 3=Theta, 4=Xvalue, 5=Yvalue, 6= size of the bin, 7= xTransformed value
        /// 8= yTransformedValue, 9 = distance to line Algorithm, 10= angle to line Algorithm
        /// 5  more ending values for the averages
        /// </summary>
      

        public double slope { get; set; }
        public double yIntercept { get; set; }
        public double rho { get; set; }
        public double theta { get; set; }
        public double xValue { get; set; }
        public double yValue { get; set; }
        public int sizeOfBin { get; set; }
        public double xTransformedValue { get; set; }
        public double yTransformedValue { get; set; }
        public double distanceToLine { get; set; }
        public double angleToLine { get; set; }

        public static double averageTheta { get; set; }
        public static double averageRho { get; set; }
        /// <summary>
        /// Stores  vector3 locations of the beginning and end points of two lines on the screen. Was part of the old Hough system,
        /// but potientially still useful, so has not deleted.
        /// 0 = start location of  line, 1 = end location ofline
        /// </summary>
       // private Vector3[] houghLineStartandStopVectors;
        public Vector3 houghStartVector { get; set; }
        public Vector3 houghEndVector { get; set; }
        /// <summary>
        /// Draw the lines from the "camera"'s persepective. 
        /// </summary>
        /// <param name="camera">The camera to use when drawing.</param>
        //public void DrawColorLines(Camera camera)
        //{
        //    this.basicEffects.World = camera.World;
        //    this.basicEffects.View = camera.View;
        //    this.basicEffects.Projection = camera.Projection;
        //    this.basicEffects.TextureEnabled = false;
        //    this.basicEffects.VertexColorEnabled = true;

        //    foreach (EffectPass pass in this.basicEffects.CurrentTechnique.Passes)
        //    {
        //        pass.Apply();
        //        this.Game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, this.whiteLinesVertexPositionColor, 0, 36); // Draw the 1st inner line
        //        this.Game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, this.whiteLinesVertexPositionColor, 37, 36); // Draw the 2nd inner line
        //        this.Game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, this.whiteLinesVertexPositionColor, 37 * 2, 36); // Draw the outerline
        //        this.Game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, this.whiteLinesVertexPositionColor, 37 * 3, 36); // Draw the 2nd outerline               
        //    }
        //}

         /// <summary>
        /// Initialize the all non graphics resournces. 
        /// </summary>
        public override void Initialize()
        {

        }

        public override void Update(GameTime gameTime)
        {

        }
    }
}
