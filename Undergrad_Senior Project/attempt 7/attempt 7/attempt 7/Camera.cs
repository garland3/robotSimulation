//-----------------------------------------------------------------------
// <copyright file="Camera.cs" company="Anthony">
//     Produces the camera for drawing. 
// </copyright>
//-----------------------------------------------------------------------
namespace Attempt_7
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
        private Vector3 cameraPosition = Vector3.Zero;
    
        /// <summary>
        /// Target of the camera = a point in 3D space that the camera is pointed at. 
        /// </summary>
        private Vector3 target = Vector3.Zero;

        /// <summary>
        /// The 3D vector that defines which direction is up for the camera.
        /// </summary>
        private Vector3 updirection;

        /// <summary>
        /// If the camera is mouse controlled, how much should the mouse cordinates differ from 3D cordinates in space. How much should they be translated in 3D space. 
        /// </summary>
        private Vector3 mouseOffset;

        /// <summary>
        /// If the camera is mouse dependent than the position of the mouse determines the position of the camera in 3D space. 
        /// </summary>
        private bool isMouseDependent;
       
        /// <summary>
        /// Initializes a new instance of the Camera class.
        /// </summary>
        /// <param name="game">The game object of the simulation</param>
        /// <param name="pos">The start position of the camera in 3D space</param>
        /// <param name="target">A point in 3D space that the camera will point at. </param>
        /// <param name="up">The 3D vector that determines which direction is up for the camera</param>
        /// <param name="isMouseDependent">Sets the camera's position based on the mouse if true</param>
        public Camera(Game game, Vector3 pos, Vector3 target, Vector3 up, bool isMouseDependent)
            : base(game)
        {
            this.cameraPosition = pos;
            this.target = target;
            this.updirection = up;
            this.View = Matrix.CreateLookAt(pos, target, up);
            this.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)Game.Window.ClientBounds.Width / (float)Game.Window.ClientBounds.Height, 1, 1200);
            this.isMouseDependent = isMouseDependent;
            this.World = Matrix.Identity;
            this.mouseOffset = pos;
        }

        /// <summary>
        /// Initializes the camera
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Updates the camera's view. If the camera is mouse dependent than Update gets the position of the mouse and keyboard inorder to update the camera position.
        /// </summary>
        /// <param name="gameTime">Clock Information</param>
        public override void Update(GameTime gameTime)
        {
            if (this.isMouseDependent == true)
            {
                this.UpdateMouse();
                this.UpdateKeyBoard();
            }

            //this.View = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)Game.Window.ClientBounds.Width / (float)Game.Window.ClientBounds.Height, 1, 1200);
            this.View = Matrix.CreateLookAt(this.cameraPosition, this.target, this.updirection); // Update the veiw. 
          
            base.Update(gameTime);
        }

        /// <summary>
        /// Returns the current camera position in 3D space.
        /// </summary>
        /// <returns>The camera position</returns>
        public Vector3 GetCameraPosition()
        {
            return this.cameraPosition;
        }

        /// <summary>
        /// Sets the camera position and target point to a specific position
        /// </summary>
        /// <param name="pos">The new position of the camera in 3D space</param>
        /// <param name="target">The new target point of the camera in 3D space</param>
        public void SetCameraPositionAndTarget(Vector3 pos, Vector3 target) 
        {
            this.cameraPosition = pos;
            this.target = target;
        }

        /// <summary>
        /// Updates the position of the camera based on the mouse's position. The Mouse origin (0,0) is the top left corner of the simulation Window.
        /// </summary>
        private void UpdateMouse()
        {
            Vector3 mouseVector = Vector3.Zero;
            MouseState mouse = Mouse.GetState(); // Create a mouse state object

            mouseVector.X = mouse.X; // Moving the mosue left and right is the camera position x axis
            mouseVector.Y = mouse.Y; // Moving the mouse up and down is the camera position y axis
            mouseVector.Z = (int)mouse.ScrollWheelValue; // Mouse scroll wheel total is the camera position z axis

            this.cameraPosition.X = mouseVector.X / 10;
            this.cameraPosition.Y = mouseVector.Y / 10;
            this.cameraPosition.Z = mouseVector.Z / 60;
            this.cameraPosition += this.mouseOffset;
        }
        
        /// <summary>
        /// Checks to see if the Arrow keys are pressed. Moves the camera position and target based on the arrow keys. 
        /// </summary>       
        private void UpdateKeyBoard()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                this.target += Vector3.Left / 3;
                this.mouseOffset += Vector3.Left / 3;
            }

            if (keyboardState.IsKeyDown(Keys.Right))
            {
                this.target += Vector3.Right / 3;
                this.mouseOffset += Vector3.Right / 3;
            }

            if (keyboardState.IsKeyDown(Keys.Up))
            {
                this.target += Vector3.Up / 3;
                this.mouseOffset += Vector3.Up / 3;
            }

            if (keyboardState.IsKeyDown(Keys.Down))
            {
                this.target += Vector3.Down / 3;
                this.mouseOffset += Vector3.Down / 3;
            }
        }
    }
}
