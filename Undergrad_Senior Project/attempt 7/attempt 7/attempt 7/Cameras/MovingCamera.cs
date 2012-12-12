using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Attempt_7.Cameras
{
    class MovingCamera : Camera
    {

      
       /// <summary>
        /// Calls the base Camera constructor. 
        /// </summary>
        /// <param name="game">The game object of the simulation</param>
        /// <param name="pos">The start position of the camera in 3D space</param>
        /// <param name="target">A point in 3D space that the camera will point at. </param>
        /// <param name="up">The 3D vector that determines which direction is up for the camera</param>
        /// <param name="isMouseDependent">Sets the camera's position based on the mouse if true</param>
        public MovingCamera(Game game, Vector3 pos, Vector3 target, Vector3 up, bool isMouseDependent)
            : base(game, pos, target, up, isMouseDependent)
        {            
        }

        /// <summary>
        /// Initializes the camera
        /// </summary>
        public  override void Initialize()
        {
            this.isMouseDependent = false;
            
            base.Initialize();
        }


         /// <summary>
        /// Updates the camera's view. If the camera is mouse dependent than Update gets the position of the mouse and keyboard inorder to update the camera position.
        /// </summary>
        /// <param name="gameTime">Clock Information</param>
        public override void Update(GameTime gameTime)
        {
          

            //this.View = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)Game.Window.ClientBounds.Width / (float)Game.Window.ClientBounds.Height, 1, 1200);
            this.View = Matrix.CreateLookAt(this.cameraPosition, this.target, this.updirection); // Update the veiw. 

            base.Update(gameTime);
        }

        
    }
}
