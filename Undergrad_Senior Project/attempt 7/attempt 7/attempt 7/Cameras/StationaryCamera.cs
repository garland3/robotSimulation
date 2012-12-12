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
    public class StationaryCamera : Camera
    {
        /// <summary>
        /// If the camera is mouse controlled, how much should the mouse cordinates differ from 3D cordinates in space. How much should they be translated in 3D space. 
        /// </summary>
        private Vector3 mouseOffset;

        /// <summary>
        /// Calls the base Camera constructor. 
        /// </summary>
        /// <param name="game">The game object of the simulation</param>
        /// <param name="pos">The start position of the camera in 3D space</param>
        /// <param name="target">A point in 3D space that the camera will point at. </param>
        /// <param name="up">The 3D vector that determines which direction is up for the camera</param>
        /// <param name="isMouseDependent">Sets the camera's position based on the mouse if true</param>
        public  StationaryCamera(Game game, Vector3 pos, Vector3 target, Vector3 up, bool isMouseDependent)
            : base(game, pos, target, up, isMouseDependent)
        {            
        }


        /// <summary>
        /// Initializes the camera
        /// </summary>
        public override void Initialize()
        {
            this.isMouseDependent = true;
            this.mouseOffset = base.cameraPosition;
            base.Initialize();
        }


        public override void Update(GameTime gameTime)
        {
            if (this.isMouseDependent == true)
            {
                this.UpdateMouse();
                this.UpdateKeyBoard();
            }

            //this.View = Matrix.CreatePerspectiveFieldOfView(MathHelper.PiOver4, (float)Game.Window.ClientBounds.Width / (float)Game.Window.ClientBounds.Height, 1, 1200);

            // Recalculate the view during every update becauuse the camera position and target may have changed.
            // this is done the base.Update //this.View = Matrix.CreateLookAt(this.cameraPosition, this.target, this.updirection); // Update the veiw. 

            base.Update(gameTime);
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

            this.cameraPosition = new Vector3(mouseVector.X / 10, mouseVector.Y / 10, mouseVector.Z / 60);
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
