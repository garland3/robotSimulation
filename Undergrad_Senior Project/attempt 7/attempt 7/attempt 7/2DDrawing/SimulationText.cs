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


namespace Attempt_7._2DDrawing
{
    /// <summary>
    /// This is a game component that shows 2D text that gives current information 
    /// about the simulation but is not used for dbugging
    /// </summary>
    public class SimulationText : _2DDrawingObject
    {

        /// <summary>
        /// The 2D font object needed to show text. Size = 20.
        /// </summary>
        private SpriteFont arialLarge;

        /// <summary>
        /// A smaller font of arial to show debug information. 
        /// </summary>
        private SpriteFont arialSmall;       

        public SimulationText(Game game)
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

            this.arialLarge = Game.Content.Load<SpriteFont>("ArialLarge"); // Load the font
            this.arialSmall = Game.Content.Load<SpriteFont>("Arial");
            // TODO: Add your initialization code here

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
            DrawText(gameTime);
        }



        /// <summary>
        /// Draw the Simulation Status as either PAUSED or Active 
        /// </summary>
        private void DrawText(GameTime gameTime)
        {
            this.spriteBatch.Begin(); // Start the 2D drawing 

            bool simulationPause =  ((SimulationMain)Game).mainRobot.paused; // Find if the simulatin is paused from the robot. 

            if (simulationPause == true)
            {
                this.spriteBatch.DrawString(this.arialLarge, "Simulation Status--- PAUSED", new Vector2(0, 0), Color.Yellow);
                // this.spriteBatch.DrawString(this.arialLarge, "DrawCnt" +frameDrawCount +"UpdateCnt"+updateCount , new Vector2(0, 300), Color.Yellow);
            }
            else
            {
                this.spriteBatch.DrawString(this.arialLarge, "Simulation Status--- Active", new Vector2(0, 0), Color.Green);
                //  this.spriteBatch.DrawString(this.arialLarge, "DrawCnt" + frameDrawCount + "UpdateCnt" + updateCount, new Vector2(0, 300), Color.Yellow);
            }

            if (gameTime.IsRunningSlowly == true)
            {
                // gameTime.
                TimeSpan diff =  gameTime.ElapsedGameTime.Subtract(TimeSpan.FromSeconds(1/60));
                this.spriteBatch.DrawString(this.arialLarge, diff.ToString(), new Vector2(0, 20), Color.Yellow);
                // this.spriteBatch.DrawString(this.arialLarge, "DrawCnt" +frameDrawCount +"UpdateCnt"+updateCount , new Vector2(0, 300), Color.Yellow);
            }

            this.spriteBatch.End(); // Stop drawing. 
        }

    }
}
