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
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class _2DDrawingObject : Microsoft.Xna.Framework.DrawableGameComponent
    {

        /// <summary>
        /// The sprite batch object. Used to draw 2D graphics. 
        /// </summary>
        protected SpriteBatch spriteBatch;

        /// <summary>
        /// Reference to the simulation main. 
        /// </summary>        
        protected SimulationMain simulationMain;


        public _2DDrawingObject(Game game)
            : base(game)
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

            base.Initialize();
        }

        /// <summary>
        /// Called after initialize. Loads the textures and calls methods to create the simulated enviroment. 
        /// </summary>
        protected override void LoadContent() // 3rd method called at start up
        {
            this.spriteBatch = new SpriteBatch(GraphicsDevice); // Create a new SpriteBatch, which can be used to draw textures.   

            base.LoadContent();
        }

        /// <summary>
        /// Called by the LoadContent. This allows for specific objects to add to the main Load Content Without the need to 
        /// override it. They can over ride this function instad. 
        /// </summary>
        protected void LoadSpecificContent() // 3rd method called at start up
        {
            
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
        }
    }
}
