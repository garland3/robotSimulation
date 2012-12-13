//-----------------------------------------------------------------------
// <copyright file="Camera.cs" company="Anthony">
//     Produces the camera for drawing. 
// </copyright>
//-----------------------------------------------------------------------
namespace Attempt_7.Cameras
{
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

    /// <summary>
    /// Represents the camera object/class that contains all the information needed about position and target for rendering. 
    /// </summary>
    public class Camera : Microsoft.Xna.Framework.GameComponent
    {

        /// <summary>
        /// Render Target 
        /// </summary>
        public RenderTarget2D renderTarget { get; set; }

       
        /// <summary>
        /// Matrix representation of the view determined by the position, target, and updirection.
        /// </summary>
        public Matrix View;

        /// <summary>
        /// Matrix representation of the view determined by the angle of the field of view (Pi/4), aspectRatio, nearest plane visible (1), and farthest plane visible (1200) 
        /// </summary>
        public Matrix Projection;

        /// <summary>
        /// Matrix representing how the real world cordinates differ from that of the rendering by the camera. 
        /// </summary>
        public Matrix World;

        /// <summary>
        /// The position of the camera. Defualt  = 0,0,0
        /// </summary>
        public Vector3 cameraPosition { get; set; }
        /// <summary>
        /// Target of the camera = a point in 3D space that the camera is pointed at. 
        /// </summary>
        public Vector3 target { get; set; }
        /// <summary>
        /// The 3D vector that defines which direction is up for the camera.
        /// </summary>
        protected Vector3 updirection;

        /// <summary>
        /// If the camera is mouse dependent than the position of the mouse determines the position of the camera in 3D space. 
        /// </summary>
        protected bool isMouseDependent;
       
        /// <summary>
        /// Initializes a new instance of the Camera class.
        /// </summary>
        /// <param name="game">The game object of the simulation</param>
        /// <param name="pos">The start position of the camera in 3D space</param>
        /// <param name="target">A point in 3D space that the camera will point at. </param>
        /// <param name="up">The 3D vector that determines which direction is up for the camera</param>
        /// <param name="isMouseDependent">Sets the camera's position based on the mouse if true</param>
        public  Camera(Game game, Vector3 pos, Vector3 target, Vector3 up, bool isMouseDependent)
            : base(game)
        {
           
            this.cameraPosition = pos;
            this.target = target;
            this.updirection = up;
            this.View = Matrix.CreateLookAt(pos, target, up);
            //this.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)Game.Window.ClientBounds.Width / (float)Game.Window.ClientBounds.Height, 1, 1200);
            this.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4,
                ((SimulationMain)Game).config.screenSize.X / ((SimulationMain)Game).config.screenSize.Y, 1.0f, 100);
           
            
            this.isMouseDependent = isMouseDependent;
            this.World = Matrix.Identity;

            
            // Add it as a game component. 
           
        }

        /// <summary>
        /// Initializes the camera
        /// </summary>
        //public virtual override void Initialize()
        //{           

           // base.Initialize();
        //}

        /// <summary>
        /// Updates the camera's view. If the camera is mouse dependent than Update gets the position of the mouse and keyboard inorder to update the camera position.
        /// </summary>
        /// <param name="gameTime">Clock Information</param>
        public override void Update(GameTime gameTime)
        {
            

            //this.View = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)Game.Window.ClientBounds.Width / (float)Game.Window.ClientBounds.Height, 1, 1200);

            // Recalculate the view during every update becauuse the camera position and target may have changed.
            this.View = Matrix.CreateLookAt(this.cameraPosition, this.target, this.updirection); // Update the veiw. 
          
            base.Update(gameTime);
        }

       

        
        
       
    }
}
