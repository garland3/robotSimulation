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

namespace Attempt_7._2DDrawing
{
    /// <summary>
    /// This is a game component that implements is meant to show debugging text. 
    /// </summary>
    public class DebugText : _2DDrawingObject
    {

        /// <summary>
        /// Font to use for drawing the debug/hough information
        /// </summary>
        private SpriteFont arial;

        /// <summary>
        /// Draw Analysis we are using. 
        /// </summary>
        public ImageAnalysis imageAnalysisLinked { get; set; }

        public DebugText(Game game)
            : base(game )
        {

           
            // TODO: Construct any child components here
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            this.arial = Game.Content.Load<SpriteFont>("Arial"); // Load the font
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
        /// Main draw method 
        /// </summary>
        /// <param name="gameTime">Clock Information</param>
        public override void Draw(GameTime gameTime)
        {
            DrawText();
        }

        /// <summary>
        /// Draws the Text giving important information about the status of the analysis.
        /// </summary>
        /// <param name="turnIndication">Indicator of how much to turn</param>
        /// <param name="totalWhiteCnt">Number of white pixels found</param>
        /// <param name="houghInfo">Hough information array</param>
        private void DrawText()
        {
            int spacing = 15;

            // Begin Drawing. 
            this.spriteBatch.Begin();
            
            this.spriteBatch.DrawString(this.arial, "slope", new Vector2(0, 0), Color.White);
            this.spriteBatch.DrawString(this.arial, "yintercept", new Vector2(0, spacing * 1), Color.White);
            this.spriteBatch.DrawString(this.arial, "rho", new Vector2(0, spacing * 2), Color.White);
            this.spriteBatch.DrawString(this.arial, "theta", new Vector2(0, spacing * 3), Color.White);
            this.spriteBatch.DrawString(this.arial, "x1", new Vector2(0, spacing * 4), Color.White);
            this.spriteBatch.DrawString(this.arial, "y1", new Vector2(0, spacing * 5), Color.White);
            this.spriteBatch.DrawString(this.arial, "BinSize", new Vector2(0, spacing * 6), Color.White);
            this.spriteBatch.DrawString(this.arial, "xTrans = ", new Vector2(0, spacing * 7), Color.White);
            this.spriteBatch.DrawString(this.arial, "yTrans = ", new Vector2(0, spacing * 8), Color.White);
            this.spriteBatch.DrawString(this.arial, "Distance = ", new Vector2(0, spacing * 9), Color.White);
            this.spriteBatch.DrawString(this.arial, "Angle = ", new Vector2(0, spacing * 10), Color.White);
            ////this.spriteBatch.DrawString(this.arial, "update speed for drawing = " + Math.Pow(UpdateSquareDimForDrawing, 2).ToString() + " update speed for analysis = " + Math.Pow(UpdateSquareDimForAnalysis, 2).ToString(), new Vector2(0, spacing * 9), Color.White);
           // this.spriteBatch.DrawString(this.arial, "Turn Indicator = " + turnIndication.ToString(), new Vector2(0, spacing * 11), Color.White);
           // this.spriteBatch.DrawString(this.arial, "White Count = " + totalWhiteCnt.ToString() + " Vertexs = " + this.count1D.ToString(), new Vector2(0, spacing * 12), Color.White);
          //  this.spriteBatch.DrawString(this.arial, "Number Of lines To find = " + this.numberofLinesToFind.ToString() + " -  theta Average = " + this.houghInfo[(this.numberofLinesToFind * 11)].ToString() + "  - rho Average = " + this.houghInfo[(this.numberofLinesToFind * 11) + 1].ToString(), new Vector2(0, spacing * 13), Color.White);
          //  this.spriteBatch.DrawString(this.arial, "Singualarites Old:" + this.imageAnalysis.GetSingularitesOldHough() + " New = " + this.imageAnalysis.GetSingularitiesNewHough(), new Vector2(0, spacing * 14), Color.White);
          //  this.spriteBatch.DrawString(this.arial, "Hough Mode " + this.imageAnalysis.GetHoughMode(), new Vector2(0, spacing * 15), Color.White);



            int numberOfLinesToFind = ((SimulationMain)Game).config.numberofLinesToFind;

            int spacingForText = (int)((((SimulationMain)Game).config.screenSize.X) - 50) / numberOfLinesToFind;
          
            // Insert the hough data for each line. 
            for (int j = 0; j < numberOfLinesToFind; j++)
            {
                //for (int i = 0; i < 11; i++)
                //{
                this.spriteBatch.DrawString(this.arial, this.imageAnalysisLinked.houghLineList[j].slope.ToString(), new Vector2(50 + (spacingForText * j),1*spacing),Color.White);
                this.spriteBatch.DrawString(this.arial, this.imageAnalysisLinked.houghLineList[j].yIntercept.ToString(), new Vector2(50 + (spacingForText * j), 2 * spacing), Color.White);
                this.spriteBatch.DrawString(this.arial, this.imageAnalysisLinked.houghLineList[j].rho.ToString(), new Vector2(50 + (spacingForText * j), 3 * spacing), Color.White);
                this.spriteBatch.DrawString(this.arial, this.imageAnalysisLinked.houghLineList[j].theta.ToString(), new Vector2(50 + (spacingForText * j), 4 * spacing), Color.White);
                this.spriteBatch.DrawString(this.arial, this.imageAnalysisLinked.houghLineList[j].xValue.ToString(), new Vector2(50 + (spacingForText * j), 5 * spacing), Color.White);
                this.spriteBatch.DrawString(this.arial, this.imageAnalysisLinked.houghLineList[j].yValue.ToString(), new Vector2(50 + (spacingForText * j), 6 * spacing), Color.White);
                this.spriteBatch.DrawString(this.arial, this.imageAnalysisLinked.houghLineList[j].sizeOfBin.ToString(), new Vector2(50 + (spacingForText * j), 7 * spacing), Color.White);
                this.spriteBatch.DrawString(this.arial, this.imageAnalysisLinked.houghLineList[j].xTransformedValue.ToString(), new Vector2(50 + (spacingForText * j), 8 * spacing), Color.White);
                this.spriteBatch.DrawString(this.arial, this.imageAnalysisLinked.houghLineList[j].yTransformedValue.ToString(), new Vector2(50 + (spacingForText * j), 9 * spacing), Color.White);
                this.spriteBatch.DrawString(this.arial, this.imageAnalysisLinked.houghLineList[j].distanceToLine.ToString(), new Vector2(50 + (spacingForText * j), 10 * spacing), Color.White);
                this.spriteBatch.DrawString(this.arial, this.imageAnalysisLinked.houghLineList[j].angleToLine.ToString(), new Vector2(50 + (spacingForText * j), 11 * spacing), Color.White);
                //this.spriteBatch.DrawString(this.arial, this.imageAnalysisLinked.houghLineList[j].slope.ToString(), new Vector2(50 + (spacingForText * j), 12 * spacing), Color.White);
                 
                //}
            }

            this.spriteBatch.DrawString(this.arial, "Count1D =" + this.imageAnalysisLinked.drawAnalysis.count1D.ToString(), new Vector2(0, 12 * spacing), Color.White);
            this.spriteBatch.DrawString(this.arial, 
                "White Count = " + this.imageAnalysisLinked.drawAnalysis.imageAnalysis.totalWhiteCnt.ToString(), new Vector2(0, 13 * spacing), Color.White);
            this.spriteBatch.DrawString(this.arial,
                "Robot Direction = " + ((SimulationMain)Game).mainRobot.direction.ToString(), new Vector2(0, 14 * spacing), Color.White);

            this.spriteBatch.DrawString(this.arial,
               "Robot Dir Theta = " +  MathHelper.ToDegrees((float)this.imageAnalysisLinked.drawAnalysis.thetaRobotDir).ToString(), new Vector2(0, 15 * spacing), Color.White);


         
           
            this.spriteBatch.DrawString(this.arial,
               "robot Lap " + ((SimulationMain)Game).mainRobot.robotLapNumber.ToString(), new Vector2(0, 16 * spacing), Color.White);


            // output information about the current settings. 
            this.spriteBatch.DrawString(this.arial,
               "Speed seetting " + ((SimulationMain)Game).config.robotSpeed.ToString(), new Vector2(0, 17 * spacing), Color.White);

            this.spriteBatch.DrawString(this.arial,
               "Theta Turning Threshold " + ((SimulationMain)Game).config.robotChangeDirectionThreshholdValue.ToString(), new Vector2(0, 18 * spacing), Color.White);

            this.spriteBatch.DrawString(this.arial,
               "Robot Turn Ratio " + ((SimulationMain)Game).config.robotTurnRatio.ToString(), new Vector2(0, 19 * spacing), Color.White);

            this.spriteBatch.DrawString(this.arial,
               "Do Analysis on every N pixels " + ((SimulationMain)Game).config.UpdateSquareDimForAnalysis.ToString(), new Vector2(0, 20 * spacing), Color.White);

            this.spriteBatch.DrawString(this.arial,
              "Error " + ((SimulationMain)Game).config.errorToShow, new Vector2(0, 21 * spacing), Color.White);


            this.spriteBatch.End();
        }
    }
}
