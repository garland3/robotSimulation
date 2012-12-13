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
    using Attempt_7._2DDrawing;
    using Attempt_7.Cameras;
    using Attempt_7.Analysis;
    using Attempt_7;
    using Attempt_7.ViewPorts;
   
    /// <summary >
    /// Robot object
    /// </summary>   
    public class Robot : DrawAbleSimulationObject
    {

        /// <summary>
        /// The vertexPositionColor Array holding the information needed for the 3D representation of the robot. 
        /// </summary>
        private VertexPositionColor[] robotVertexPositionColor;

        
        /// <summary>
        /// Number of VertexPositionColor's that make up the robot in the virtual robot 
        /// </summary>
        private int numberOfVertexPositionColorRobot = 5;

          /// <summary>
        /// The offset for the VertexPositionColors Vector3 from the actual robot position
        /// </summary>
        private Vector3[] robotVertexPositionOffsetsFromRobot;


       

        /// <summary>
        /// Robot Lap Number
        /// </summary>
        public int robotLapNumber { get; set; }

        /// <summary>
        /// Number of Laps to complete
        /// </summary>
        public int robotNumberOfLapsToComplete { get; set; }

        /// <summary>
        /// Position of the robot. 
        /// </summary>
        public Vector3 position { get; set; }

        /// <summary>
        /// Where the robot starts. Needed so it can be reset. 
        /// </summary>        
        private Vector3 startPosition;

        /// <summary>
        /// Direction the robot is heading
        /// </summary>        
        public Vector3 direction { get; set; }

       

        /// <summary>
        /// Speed the robot is moving forward on update. 
        /// /// </summary>
        public float speed { get; set; }

       

        /// <summary>
        /// Camera associated with the robot.
        /// </summary>
        public Camera robotCameraView { get; set; }

        /// <summary>
        /// Make an image analysis associated with the robot. 
        /// </summary>
        public ImageAnalysis imageAnalysisRobotCamera { get; set; }

        /// <summary>
        /// Is the robot moving on each all to update? if paused = true then moving, if false then not moving. 
        /// </summary>
        public bool paused { get; set; }

        /// <summary>
        /// How big does the turn indicator have to be, before the robot turns. 20 is default. 
        /// </summary>
        public int changeDirectionThreshholdValue { get; set; }


        /// <summary>
        /// When Turning how much do we use the average value compared to how hard we turn. 
        /// </summary>
        public float turnRatio { get; set; }

        /// <summary>
        /// Time in total milliseconds from the start of the game to the last time one of the keys was pressed. 
        /// </summary>
        private int timePressedKey;


        

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
        /// Initializes a new instance of the Robot class.
        /// </summary>
        /// <param name="game">The game associate with the robot. </param>
        /// <param name="position">Where the robot is to start</param>
        /// <param name="direction">What direction the robot is pointed at </param>
        /// <param name="speed">How fast the robot will go. </param>
        /// <param name="howFarToLookInFront">How far in front of the robot the camera is to point. </param>
        /// <param name="cameraHeight">How high above the ground the camera for the robot is.</param>
        public Robot(Game game)
            : base(game)
        {
            this.robotLapNumber = 0;

            // Make a default configuration object
           

            // Set the variables in the robot based off the default configuration. 
            this.position = ((SimulationMain)game).config.robotPosition;
            this.direction = ((SimulationMain)game).config.robotDirection;
            this.speed = ((SimulationMain)game).config.robotSpeed;
            this.changeDirectionThreshholdValue = ((SimulationMain)game).config.robotChangeDirectionThreshholdValue;
            this.robotNumberOfLapsToComplete = ((SimulationMain)game).config.robotNumberOfLapsToComplete;
            this.turnRatio = ((SimulationMain)game).config.robotTurnRatio;
            

            this.basicEffects = new BasicEffect(this.Game.GraphicsDevice);


            // Normalize the direction. 
            this.direction.Normalize();

            // Create the camera for the robot based on the information just assigned to the robot. 
            // the camera height and distance to look infront are not stored in the robot, but just sent on to the camera. 
            // so we use the default configuration from robotconfig for cameraHeight and distanceToCameraTarget
            Vector3 cameraPosition = this.position + new Vector3(0, 0,  ((SimulationMain)game).config.robotCameraHeight);
            Vector3 cameraTarget = this.position + Vector3.Multiply(this.direction, ((SimulationMain)game).config.robotDistanceToCameraTarget);
            Vector3 cameraUp =  Vector3.Backward;
            bool isCameraMouseDependent = false;

            this.robotCameraView = new MovingCamera(Game, cameraPosition, cameraTarget, cameraUp, isCameraMouseDependent);
            ((SimulationMain)Game).cameraListDraw3DWorld.Add(robotCameraView);
            this.Game.Components.Add(this.robotCameraView);

            // Make an analysis object for the robot. 
            this.imageAnalysisRobotCamera = new ImageAnalysis(game);
            Game.Components.Add(this.imageAnalysisRobotCamera);

            // Make the robot triangles that will be drawn. 
            BuildVertexPositionColorArrayandOffSetArray();

            // Set the Robot as paused when first starting. 
            this.paused = true;
        }

        /// <summary>
        /// Called when the robot object is first made. 
        /// The robot starts paused.
        /// </summary>
        public override void Initialize()
        {       

            // call the base method. . 
            base.Initialize();
        }

        /// <summary>
        /// Initializes the vertexPositioncolors and the offset arrays. 
        /// </summary>
        private void BuildVertexPositionColorArrayandOffSetArray()
        {
            this.robotVertexPositionOffsetsFromRobot = new Vector3[numberOfVertexPositionColorRobot];

            this.robotVertexPositionOffsetsFromRobot[0] = new Vector3(-0.3f, 0.3f, 0);
             this.robotVertexPositionOffsetsFromRobot[1] = new Vector3(0.3f, 0.3f, 0);
             this.robotVertexPositionOffsetsFromRobot[2] = new Vector3(-0.0f, 0.0f, ((SimulationMain)Game).config.robotCameraHeight);
             this.robotVertexPositionOffsetsFromRobot[3] = new Vector3(-0.3f, -0.3f, 0);
             this.robotVertexPositionOffsetsFromRobot[4] = new Vector3(-0.3f, 0.3f, 0);


            this.robotVertexPositionColor = new VertexPositionColor[numberOfVertexPositionColorRobot];
           
            // Initialize the position and color of the vertexPositionColors.  
            this.robotVertexPositionColor[0] = new VertexPositionColor(robotVertexPositionOffsetsFromRobot[0] + this.position, Color.Aqua);
            this.robotVertexPositionColor[1] = new VertexPositionColor(robotVertexPositionOffsetsFromRobot[1] + this.position, Color.Green);
            this.robotVertexPositionColor[2] = new VertexPositionColor(robotVertexPositionOffsetsFromRobot[2] +this.position, Color.Red);
            this.robotVertexPositionColor[3] = new VertexPositionColor(robotVertexPositionOffsetsFromRobot[3] + this.position, Color.Black);
            this.robotVertexPositionColor[4] = new VertexPositionColor(robotVertexPositionOffsetsFromRobot[4] + this.position, Color.Orange);
           

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
                this.direction += (Vector3.Cross(this.direction, Vector3.UnitZ) / this.turnRatio) * turnIndicator; // 2500 is just an experimental value that works
                //this.direction += (this.direction*this.turnRatio * turnIndicator); // 2500 is just an experimental value that works
                
                this.direction.Normalize(); // Make a unit vector
            }
        }


        static int updateCount = 0;


        /// <summary>       
        /// Checks If robot has finished all the laps
        /// </summary>       
        public void checkIfRobotCompleteAllLaps(GameTime gameTime)
        {
            if (robotLapNumber == this.robotNumberOfLapsToComplete)
            {
                // this.totalTime = gameTime.TotalGameTime;
                ((SimulationMain)Game).PrintResultToFile(gameTime.TotalGameTime);
            }
        }

        /// <summary>       
        /// Checks If robot is on course
        /// </summary>       
        public void checkIfRobotOnCourse(GameTime gameTime)
        {
            if (this.position.Length() > 100)
            {
                // 32600,32700,34150
                ((SimulationMain)Game).ExitWithMessage("98765");
            }

        }


        /// <summary>
        /// Main Update method. 
        /// Calls all other methods for updating the robot. 
        /// Because the robot is a GameComponent, the update method is called by the SimulationMain.update method. 
        /// </summary>
        /// <param name="gameTime"> Clock information</param>
        public override void Update(GameTime gameTime)
        {
            if (this.paused != true)
                   updateCount++;

            // STart the robot 2 second after it is made
            if (gameTime.TotalGameTime.TotalSeconds >= 2)
                this.paused = false;
            
             bool potentialLap= false;
            // If we are in quadrant 4, then we have the potential of crossing start line. 
             if ((this.position.Y < 0) && (this.position.X >= 0))
                               potentialLap = true;
             
               
            this.GetKeyBoard(gameTime); // Get the key board values. 
            if (this.paused != true)
                this.position += Vector3.Multiply(this.direction, this.speed); // Move the robot forward by the speed of the robot

            // If we were in quandrant 4, but now the y>=0, so we are in quadrant 1, then we passed the finish line.
            // Incremenent the lap counter. 
            if (potentialLap == true && (this.position.Y >= 0) && this.paused != true)
                this.robotLapNumber += 1;


            this.checkIfRobotCompleteAllLaps(gameTime);
            this.checkIfRobotOnCourse(gameTime);  
            this.UpdateRobotVertexPositions();
            

            // Set the robot camera based on its new position
            this.robotCameraView.cameraPosition = this.position + new Vector3(0, 0, ((SimulationMain)Game).config.robotCameraHeight);
            this.robotCameraView.target = this.position + Vector3.Multiply(this.direction, ((SimulationMain)Game).config.robotDistanceToCameraTarget);
            
               
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
                         this.paused = true;
                 else
                    this.paused = false;
             }

            // Reset the robot. 
            if (keyboardState.IsKeyDown(Keys.F1) == true)
               this.position = this.startPosition;
           
            // Turn left
            if (keyboardState.IsKeyDown(Keys.S) == true)
            {
                this.direction -= Vector3.Cross(this.direction, Vector3.UnitZ) / 60;
                this.direction.Normalize(); // Make a unit vector
            }

            // Turn right
            if (keyboardState.IsKeyDown(Keys.F) == true)
                 this.direction += Vector3.Cross(this.direction, Vector3.UnitZ) / 60;
                this.direction.Normalize();
            

            // Change speed faster
            if (keyboardState.IsKeyDown(Keys.E) == true)
                this.speed += 0.0003f;
            
            // Change speed slower
            if (keyboardState.IsKeyDown(Keys.D) == true)
                this.speed -= 0.0003f;
        }

        /// <summary>
        /// Loads the vertex information about the robot and recreates it every game cycle to reflect the new position of the robot. 
        /// 'verts3' is the VertexPositionColor array
        /// </summary>
        private void UpdateRobotVertexPositions()
        {
            if (this.paused != true)
            {
                // Steering determination get from imageAnalysis and give to robot
                this.ChangeDirection(this.imageAnalysisRobotCamera.turnIndication);                
            }
                      
            // robotDirection.Normalize();                   

            // Pyramid that represents the robot.          
            this.robotVertexPositionColor[0].Position = this.robotVertexPositionOffsetsFromRobot[0]+ this.position;
            this.robotVertexPositionColor[1].Position = this.robotVertexPositionOffsetsFromRobot[1]+ this.position;
            this.robotVertexPositionColor[2].Position = this.robotVertexPositionOffsetsFromRobot[2]+ this.position;
            this.robotVertexPositionColor[3].Position = this.robotVertexPositionOffsetsFromRobot[3]+ this.position;
            this.robotVertexPositionColor[4].Position = this.robotVertexPositionOffsetsFromRobot[4]+ this.position;           
        }


        /// <summary>
        /// Draw the robot from the "camera"s perspective
        /// </summary>
        /// <param name="camera">The camera to use when drawing. </param>
        public void DrawRobot(Camera camera)
        {
            this.basicEffects.World = camera.World;
            this.basicEffects.View = camera.View;
            this.basicEffects.Projection = camera.Projection;
            this.basicEffects.TextureEnabled = false;
            this.basicEffects.VertexColorEnabled = true;

            foreach (EffectPass pass in this.basicEffects.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.Game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, this.robotVertexPositionColor, 0, numberOfVertexPositionColorRobot-2); // Verts3 is the robot.                
            }
        }
       
    }
}
