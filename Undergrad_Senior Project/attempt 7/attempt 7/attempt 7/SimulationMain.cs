//-----------------------------------------------------------------------
// <copyright file="SimulationMain.cs" company="Anthony">
//     Produced to simulate the Bob Jones University Enginering Department's IGVC robot. 
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
    using Attempt_7._2DDrawing;
    using Attempt_7.Cameras;
    using Attempt_7.Analysis;
    using Attempt_7.DrawAbleSimulationObjects;
    using Attempt_7.ViewPorts;
    using Attempt_7;
   

    /// <summary>
    /// This is the main type for the Simulation
    /// </summary>
    public class SimulationMain : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// GPU manager
        /// </summary>
        private GraphicsDeviceManager graphics;

        /// <summary>
        /// Object With configuration information
        /// </summary>
        public ConfigurationInformation config { get; set; }

        /// <summary>
        /// World Camera
        /// </summary>
        public StationaryCamera worldViewCamera { get; set; }
        

        /// <summary>
        /// The robot object. See the robot class for more information.
        /// </summary>
        public Robot mainRobot{get;set;}
        
        /// <summary>
        /// Grass Object
        /// </summary>
        public Grass grassTurf { get; set; }

        /// <summary>
        /// Course Lines Object
        /// </summary>
        public CourseLines courseLines { get; set; }


        /// <summary>
        /// SimulationText
        /// </summary>
        public SimulationText simulationText { get; set; }

        /// <summary>
        /// The sprite batch object. Used to draw 2D graphics. 
        /// </summary>
        protected SpriteBatch spriteBatch;

        /// <summary>
        /// Time taken to finish the laps. 
        /// </summary>
        TimeSpan timePeneltyForRunningSlow;

         /// <summary>
        /// The time penelty amount that we will add to the totally time, if the game is running slow. 
        /// </summary>
        TimeSpan timePeneltyAmount;
        

        /// <summary>
        /// The Sprite Rectangle Manager that determines sectors of the screen. 
        /// </summary>
        public SpriteRectangleManager spriteRectangleManager { get; set; }

        /// <summary>
        /// List of camera's to draw the 3D world and render to a target in memory
        /// </summary>
        public List<Camera> cameraListDraw3DWorld { get; set; }

        /// <summary >     
        /// Initializes a new instance of the SimulationMain class.
        /// </summary>
        public SimulationMain()
        {
            this.config = new ConfigurationInformation();
            this.ReadParametersFromFile();

            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 50);

            // Initialize the camera list
            cameraListDraw3DWorld = new List<Camera>();

            this.graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            // Set GPU configuration information. 
            this.graphics.PreferredBackBufferWidth = (int)this.config.windowSize.Width; // Set the window size
            this.graphics.PreferredBackBufferHeight = (int)this.config.windowSize.Height;
            //this.graphics.PreferredBackBufferWidth = (int)this.config.screenSize.X; // Set the window size
            //this.graphics.PreferredBackBufferHeight = (int)this.config.screenSize.Y;
            this.graphics.IsFullScreen = false; // Not full screen
            this.graphics.ApplyChanges();

            // Set windows manager information
            Window.Title = "Robot simulation1";
            IsMouseVisible = true;

            this.spriteRectangleManager = new SpriteRectangleManager(this);
        }

        /// <summary>
        /// Initialize the all non graphics resournces. 
        /// </summary>
        protected override void Initialize()
        {
           // going to try to not use view ports and then see what happens. 
           // this.viewPortManager = new ViewPortManager
            this.timePeneltyForRunningSlow = TimeSpan.Zero;
            this.timePeneltyAmount = new TimeSpan(0, 0, 0, 0, 200);
            

            // Make the 3d world objects
            this.grassTurf = new Grass(this);
            this.Components.Add(this.grassTurf);
            this.courseLines = new CourseLines(this);
            this.Components.Add(this.courseLines);

            // Make a camera component that is controlled by mouse position - this is the main view
            this.worldViewCamera = new StationaryCamera(this, new Vector3(-40, -30, 20), Vector3.Zero, Vector3.UnitZ, true);
            this.cameraListDraw3DWorld.Add(worldViewCamera); // Add it to the list that draw 3D enviroment. 
            this.Components.Add(this.worldViewCamera);


            // Make a robot
            this.mainRobot = new Robot(this);
            this.Components.Add(this.mainRobot);

            // Put the robot in the start position
            this.mainRobot.position = this.courseLines.startPosition;

            // Make a new simulation Text object, and add it to the game componets. It shoud be drawabble
            this.simulationText = new SimulationText(this);
            this.Components.Add(this.simulationText);
            

            base.Initialize();
        }

        /// <summary>
        /// Called after initialize. Loads the textures and calls methods to create the simulated enviroment. 
        /// 3rd method called at start up
        /// </summary>
        protected override void LoadContent() 
        {
            this.spriteBatch = new SpriteBatch(GraphicsDevice); // Create a new SpriteBatch, which can be used to draw textures.
            this.GraphicsDevice.Viewport = new Viewport(this.config.windowSize);           
            //this.GraphicsDevice.Viewport = new Viewport(new Rectangle(0,0,(int)this.config.screenSize.X,(int)this.config.screenSize.Y));           


            base.LoadContent();
        }

        /// <summary>
        /// Unloads Information when the simulation is closed. Currently no information is saved when the simulation is closed. 
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }       

        /// <summary>
        /// Main update method for the entire simulation. Simulation logic is updated when this method is called. Part of the game loop.
        /// </summary>
        /// <param name="gameTime">Clock information</param>
        protected override void Update(GameTime gameTime)
        {           

            this.GetKeyBoard(gameTime); // Check the keyboard for commands      

            if (this.config.trackRobot == true)
            {
                this.worldViewCamera.target = this.mainRobot.position;                
            }

            // Penelty for running slow
            if (gameTime.IsRunningSlowly == true)
            {
                this.timePeneltyForRunningSlow += this.timePeneltyForRunningSlow;
            }

            base.Update(gameTime); // Call the update for all the game components (camera, robot, and imageAnalysis). -- this is where the real work is done           
        }

        /// <summary>
        /// Checks the keyboard for F3, and Escape
        /// </summary>
        /// <param name="gameTime1">Clock information</param>
        protected void GetKeyBoard(GameTime gameTime1)
        {
            KeyboardState keyboardState = Keyboard.GetState();

            // Force 500 millsecond between updates
            if (gameTime1.TotalGameTime.TotalMilliseconds - this.config.timePressedKey > 500)
            {
                if (keyboardState.IsKeyDown(Keys.F3))
                {
                    this.config.timePressedKey = (int)gameTime1.TotalGameTime.TotalMilliseconds;
                    if (this.config.trackRobot == false)
                    {
                        this.config.trackRobot = true; // Toggle on and off  
                    }
                    else
                    {
                        this.config.trackRobot = false; // Toggle on and off  
                    }
                }

                //toggle the hough mode. 
                if (keyboardState.IsKeyDown(Keys.F4))
                {
                    this.config.timePressedKey = (int)gameTime1.TotalGameTime.TotalMilliseconds;
                    if (this.config.currentHoughMode == 0)
                    {
                        config.currentHoughMode = 1; // Toggle on and off  
                    }
                    else
                    {
                        config.currentHoughMode = 0; // Toggle on and off  
                    }
                }
            }

            // Allows the game to exit when Escape is hit. 
            if (keyboardState.IsKeyDown(Keys.Escape) == true)
            {
                this.Exit();
            }
        }

       

        ///// <summary>
        ///// Returns the current Hough mode. If 0 then Old mode (top left origin), if 1 then New Hough mode( bottom Center). 
        ///// </summary>
        ///// <returns>The Mode 0 = Old, 1 = New</returns>
        //public int GetHoughMode()
        //{
        //    return currentHoughMode;
        //}
       

        /// <summary>
        /// Main draw method for the simulation. Draws all the world from both the mainview and the robots view. 
        /// <remarks >The render target must be built and disposed each time or else the a memory overload will occur</remarks>
        /// <remarks>Each time the Viewport is changed a new spriteBatch.Start method must be called.</remarks>
        /// </summary>
        /// <param name="gameTime">Clock Information</param>
        protected override void Draw(GameTime gameTime)
        {
            //bool simulationPause = ; // Find if the simulatin is paused from the robot. 
            if (this.mainRobot.paused != true)           
                this.config.frameDrawCount++;           

            this.BuildRenderTargets(); // Create the render targets to be in the computer memory instead of the screen.        

            // Go through each camera and draw the world from its persepective. 
            foreach (Camera camera in this.cameraListDraw3DWorld)
            {
                this.GraphicsDevice.SetRenderTarget(camera.renderTarget); // Set the render target for the particular Camera
                this.GraphicsDevice.Clear(Color.Black); // Clear the screen to Black

                // Draw the world from the perspective of each camera, 
                this.grassTurf.DrawGrass(camera);
                this.courseLines.DrawCouseLines(camera);               
                this.mainRobot.DrawRobot(camera);
                this.mainRobot.imageAnalysisRobotCamera.drawAnalysis.DrawImageAnalysisResults(camera);
               
            }

            // Set the renderTarget to the scree, Clear the screen, 
            this.GraphicsDevice.SetRenderTarget(null); // Set the rendering target back to the screen
            this.GraphicsDevice.Clear(Color.Gray); // Clear the screen to Gray

            // Send the imageAnalysis object the robot's view, 
            this.mainRobot.imageAnalysisRobotCamera.robotCameraViewTexture = this.mainRobot.robotCameraView.renderTarget; // Send the camera view from the robot camera to the analysis class
            this.mainRobot.imageAnalysisRobotCamera.preUpdate(gameTime); // Pull out the color data of the texture before the texture is disposed. 

            // Draw the render target, maks sure the source rectange size is the same as the texture to draw size,
            // (texture to draw, size of the part of the texture you what draw (the whole thing), Color, rotation, Origin of the texture,scale to 3/4 size, no effects, 0 is default)

            // Set the view port to the main view and draw the main view
            //this.GraphicsDevice.Viewport = this.viewPortList[0]; // Main View  is 2/3 of the screen screen
            
            this.spriteBatch.Begin(); // Start the 2D drawing 
            
            // Draw World View
            this.spriteBatch.Draw(this.worldViewCamera.renderTarget,this.spriteRectangleManager.topLeftVector,null,
                Color.White,0,Vector2.Zero,this.config.scaleFactorScreenSizeToWindow,SpriteEffects.None,0);

            // Draw Robot Camera View
            this.spriteBatch.Draw(this.mainRobot.robotCameraView.renderTarget, this.spriteRectangleManager.topRightVector, null,
                Color.White, 0, Vector2.Zero, this.config.scaleFactorScreenSizeToWindow, SpriteEffects.None, 0);

            //this.mainRobot.imageAnalysisRobotCamera.drawAnalysis.DrawTexture(this.spriteBatch);

            //this.DrawText(); // Draw a message about whether the simulation is paused or active. 
            //this.DrawTextDebugInfo();
            this.spriteBatch.End(); // Stop drawing. 

            base.Draw(gameTime); // Call all the drawableGameCmponets draw method (imageAnalysis.cs).
           

            // Dispose of the render target
            this.worldViewCamera.renderTarget.Dispose();
            this.mainRobot.robotCameraView.renderTarget.Dispose();
        }



        /// <summary>
        /// Set the renderTargets to a texture object in memory rather than rendering to the screen. Need target for both the Worldview and the robot CameraView.
        /// </summary>
        private void BuildRenderTargets()
        {
            // Create a target for rendering of the robot camera view
            this.mainRobot.robotCameraView.renderTarget =
                new RenderTarget2D(this.GraphicsDevice, (int)this.config.screenSize.X, (int)this.config.screenSize.Y); 

            // Create a render target for the world view.
            this.worldViewCamera.renderTarget =
                 new RenderTarget2D(this.GraphicsDevice, (int)this.config.screenSize.X, (int)this.config.screenSize.Y); 

        }


        /// <summary>
        /// Read parametes from File. 
        /// </summary>
        private void ReadParametersFromFile()
        {

            // Directory.GetCurrentDirectory()
            //string path = Directory.GetCurrentDirectory();
            //FileStream fileStream = new FileStream(@"c:/le.txt", FileMode.Open);
            string line;
            int counter = 0;
            try
            {
                // rho
                // theta
                // updateNforAnalysis

                if (File.Exists("test.txt"))
                {
                    //this.config.testString = File.ReadAllText("test.txt");
                    System.IO.StreamReader file = new System.IO.StreamReader(@"C:\Users\Anthony G\Documents\GitHub\robotSimulation\Undergrad_Senior Project\attempt 7\attempt 7\attempt 7\bin\x86\Release\test.txt");
                    
                    // First line
                    if((line = file.ReadLine()) != null)
                      this.config.robotSpeed= (float)Convert.ToDouble( line);
                    else
                        ExitWithMessage("Could not read 1 parameter of input file text.txt");

                    // SEcond LIne
                    if((line = file.ReadLine()) != null)
                      this.config.robotChangeDirectionThreshholdValue= (short)Convert.ToInt32( line);
                    else
                        ExitWithMessage("Could not read 2 parameter of input file text.txt");

                    // Third Line
                    if((line = file.ReadLine()) != null)
                        this.config.robotTurnRatio = (float)Convert.ToDouble(line);
                    else
                        ExitWithMessage("Could not read 3 parameter of input file text.txt");

                    // Fourth Line
                    if ((line = file.ReadLine()) != null)
                        this.config.UpdateSquareDimForAnalysis = (short)Convert.ToInt32(line);
                    else
                        ExitWithMessage("Could not read 4 parameter of input file text.txt");

                   

                    file.Close();


                }                
              
                // read from file or write to file
            }
            finally
            {
                //fileStream.Close();
            }
            // File name
            // open
            // parse, assign to parameters
            //close
        }

        /// <summary>
        /// Print Results of the simulation to a file. 
        /// </summary>
        public void PrintResultToFile(TimeSpan totalTime)
        {   
            System.IO.File.WriteAllText("results.txt", totalTime.TotalMilliseconds.ToString());
            Exit();
        }

         /// <summary>
        /// Print Results of the simulation to a file. 
        /// </summary>
        public void ExitWithMessage(String message)
        {
             System.IO.File.WriteAllText("results.txt", message.ToString());
              Exit();
        }

    }
}
