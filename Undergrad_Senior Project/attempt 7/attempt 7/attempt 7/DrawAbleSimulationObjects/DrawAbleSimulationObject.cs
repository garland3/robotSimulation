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
    /// This is a game component that implements. We want to manually call the Draw Functions, so we will not make it a drawable
    /// Componet.
    /// to load content we will use the constructor. 
    /// </summary>
    public class DrawAbleSimulationObject : Microsoft.Xna.Framework.GameComponent
    {
        /// <summary>
        /// The basicEffects for rendering in 3D.
        /// </summary>
        protected BasicEffect basicEffects { get; set; }


        

        public DrawAbleSimulationObject(Game game)
            : base(game)
        {
            // game.Content.Load<>();
            this.basicEffects = new BasicEffect(Game.GraphicsDevice); // Create a basic effects object so we the GPU knows how to render the vertex data
           
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
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }
    }
}
