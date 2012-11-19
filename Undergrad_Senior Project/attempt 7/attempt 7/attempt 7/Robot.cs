//-----------------------------------------------------------------------
// <copyright file="Robot.cs" company="Anthony">
//     Company copyright tag.
// </copyright>
//-----------------------------------------------------------------------

namespace Attempt_7
{
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
   
    /// <summary >
    /// Robot object that inherits from the XNA game component class. 
    /// </summary>   
    public class Robot : Microsoft.Xna.Framework.GameComponent
    {
        /// <summary>
        /// Position of the robot. 
        /// </summary>
        private Vector3 position;

        /// <summary>
        /// Where the robot starts. Needed so it can be reset. 
        /// </summary>        
        private Vector3 startPosition;

        /// <summary>
        /// Direction the robot is heading
        /// </summary>        
        private Vector3 direction;

        /// <summary>
        /// Speed the robot is moving forward on update. 
        /// /// </summary>
        private float speed;

        /// <summary>
        /// The distance in front of the robot that the camera is pointed at. 
        /// </summary>
        private float distanceToCameraTarget;

        /// <summary>
        /// Height of the camera above the robot. 
        /// </summary>
        private float cameraHeight;

        /// <summary>
        /// Camera associated with the robot.
        /// </summary>
        private Camera robotCameraView;

        /// <summary>
        /// Is the robot moving on each all to update? if paused = true then moving, if false then not moving. 
        /// </summary>
        private bool paused;

        /// <summary>
        /// How big does the turn indicator have to be, before the robot turns. 20 is default. 
        /// </summary>
        private int changeDirectionThreshholdValue = 10;

        /// <summary>
        /// Time in total milliseconds from the start of the game to the last time one of the keys was pressed. 
        /// </summary>
        private int timePressedKey;

        /// <summary>
        /// Initializes a new instance of the Robot class.
        /// </summary>
        /// <param name="game">The game associate with the robot. </param>
        /// <param name="position">Where the robot is to start</param>
        /// <param name="direction">What direction the robot is pointed at </param>
        /// <param name="speed">How fast the robot will go. </param>
        /// <param name="howFarToLookInFront">How far in front of the robot the camera is to point. </param>
        /// <param name="cameraHeight">How high above the ground the camera for the robot is.</param>
        public Robot(Game game, Vector3 position, Vector3 direction, float speed, float howFarToLookInFront, float cameraHeight)
            : base(game)
        {
            // Set the variables
            this.position = position;
            this.direction = direction;
            this.direction.Normalize();
            this.speed = speed;
            this.distanceToCameraTarget = howFarToLookInFront;
            this.cameraHeight = cameraHeight;

            // Create the camera for the robot based on the information just assigned to the robot. 
            this.robotCameraView = new Camera(Game, this.position + new Vector3(0, 0, this.cameraHeight), this.position + Vector3.Multiply(this.direction, this.distanceToCameraTarget), Vector3.Backward, false);
        }

        /// <summary>
        /// Gets the current position of the Robot
        /// </summary>
        /// <returns> Vector3 value of the robot position</returns>
        public Vector3 GetPosition()
        {
            return this.position;
        }

        /// <summary>
        /// Gets robot cammer hieght
        /// </summary>
        /// <returns> float value of the robot camera height</returns>
        public float  GetCameraHeight()
        {
            return this.cameraHeight;
        }

        /// <summary>
        /// Sets the position of the robot. 
        /// </summary>
        /// <param name="pos"> The new position of the robot.</param>
        public void SetPosition(Vector3 pos)
        {
            this.position = pos;
        }

        /// <summary>
        /// Called when the robot is first placed. Stores the value of the start position so that the robot can be reset to this location
        /// </summary>
        /// <param name="pos"> The position of the robot when it starts. </param>
        public void SetStartPosition(Vector3 pos)
        {
            this.position = pos;
            this.startPosition = pos; // Set the Start position so it can be reset later. 
        }

        /// <summary>
        /// Gets the camera/view that the robot's camera sees. 
        /// </summary>
        /// <returns> The camera associated with the robot. </returns>
        public Camera GetRobotCamera()
        {
            return this.robotCameraView;
        }

        /// <summary>
        /// Accessor method that tells if the robot is moving forward
        ///  </summary>
        /// <returns> True if paused, false if robot is moving. </returns>
        public bool GetIsRobotPaused()
        {
            return this.paused;
        }

        /// <summary>
        /// Accessor method that tells the direction the robot is going. 
        /// </summary>
        /// <returns> Vector3 of the direction the robot is pointed</returns>
        public Vector3 GetDirection()
        {
            return this.direction;
        }

        /// <summary>
        /// Called when the robot object is first made. 
        /// The robot starts paused.
        /// </summary>
        public override void Initialize()
        {
            this.paused = true;
            base.Initialize();
        }

        /// <summary><para >
        /// Turns the robot based on the value of the turnIndicator
        /// Turn indicator must be greater or less than the threshold turn value before the robot will turn. 
        /// Positive cross product of positive z with the dirrection the robot is going = "right"
        /// Negative cross product of positive z with the dirrection the robot is going = "left"
        /// </para>
        /// </summary>
        /// <param name="turnIndicator"> The TurnIndicator value must be passed to this method</param>
        public void ChangeDirection(float turnIndicator)
        {
            if (turnIndicator > this.changeDirectionThreshholdValue || turnIndicator < -this.changeDirectionThreshholdValue)
            {
                this.direction += (Vector3.Cross(this.direction, Vector3.UnitZ) / 2500) * turnIndicator; // 2500 is just an experimental value that works
                this.direction.Normalize(); // Make a unit vector
            }
        }

        /// <summary>
        /// Main Update method. 
        /// Calls all other methods for updating the robot. 
        /// Because the robot is a GameComponent, the update method is called by the Game1.update method. 
        /// </summary>
        /// <param name="gameTime"> Clock information</param>
        public override void Update(GameTime gameTime)
        {
            this.GetKeyBoard(gameTime); // Get the key board values. 
            if (this.paused != true)
            {
                this.position += Vector3.Multiply(this.direction, this.speed); // Move the robot forward by the speed of the robot
            }

            // Set the robot camera based on its new position
            this.robotCameraView.SetCameraPositionAndTarget(this.position + new Vector3(0, 0, this.cameraHeight), this.position + Vector3.Multiply(this.direction, this.distanceToCameraTarget)); // Update the camera position
            this.robotCameraView.Update(gameTime); // Update the camera, by calling the camera's update method
            base.Update(gameTime);
        }

        /// <summary>
        /// Checks to see if a key was pressed. F1,F2,Numpad4,NumPad6,NumPad8,NumPad2 are all checked.
        /// Called by update.
        /// </summary>
        /// <param name="gameTime1"> Clock information</param>
        protected void GetKeyBoard(GameTime gameTime1)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            // Toggle pausing the game. Force 500 milliseconds between toggles. 
            if (keyboardState.IsKeyDown(Keys.F2) && gameTime1.TotalGameTime.TotalMilliseconds - this.timePressedKey > 500)
            {
                this.timePressedKey = (int)gameTime1.TotalGameTime.TotalMilliseconds;
                if (this.paused == false)
                {
                    this.paused = true;
                }
                else
                {
                    this.paused = false;
                }
            }

            // Reset the robot. 
            if (keyboardState.IsKeyDown(Keys.F1) == true)
            {
                this.position = this.startPosition;
            }

            // Turn left
            if (keyboardState.IsKeyDown(Keys.S) == true)
            {
                this.direction -= Vector3.Cross(this.direction, Vector3.UnitZ) / 60;
                this.direction.Normalize(); // Make a unit vector
            }

            // Turn right
            if (keyboardState.IsKeyDown(Keys.F) == true)
            {
                this.direction += Vector3.Cross(this.direction, Vector3.UnitZ) / 60;
                this.direction.Normalize();
            }

            // Change speed faster
            if (keyboardState.IsKeyDown(Keys.E) == true)
            {
                this.speed += 0.0001f;
            }

            // Change speed slower
            if (keyboardState.IsKeyDown(Keys.D) == true)
            {
                this.speed -= 0.0001f;
            }
        }
    }
}
