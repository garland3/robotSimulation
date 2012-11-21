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
        /// 2D vector representing the resolution of the cameras used in the simulation. 
        /// </summary>
        private Vector2 screenSize = new Vector2(640, 480);

        /// <summary>
        /// Rectangle object that is the size of the camera resolution (screenSize).
        /// </summary>
        private Rectangle screenRectangle = new Rectangle(0, 0, 640, 480);

        /// <summary>
        /// 2D vector represeting the size of the Window's Window that the simulation will run in. 
        /// </summary>
        private Vector2 windowSize = new Vector2(1000, 780);

        /// <summary>
        /// The sprite batch object. Used to draw 2D graphics. 
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// Represents the texture of the grass bitmap that is used to represent the grass. 
        /// </summary>
        private Texture2D grass;

        /// <summary>
        /// A large tiled version of the grass Texture. 
        /// </summary>
        private Texture2D largeGrass;

        /// <summary>
        /// The basicEffects for rendering in 3D.
        /// </summary>
        private BasicEffect basicEffects;

        /// <summary>
        /// The vertexPositionColor array holding the vertex information needed for drawing the white lines representing the course. 
        /// </summary>
        private VertexPositionColor[] verts1;

        /// <summary>
        /// The vertexPositionColor array holding the information for the grass/ground. 
        /// </summary>
        private VertexPositionTexture[] verts2;

        /// <summary>
        /// The vertexPositionColor Array holding the information needed for the 3D representation of the robot. 
        /// </summary>
        private VertexPositionColor[] verts3;

        /// <summary>
        /// track Array 
        /// </summary>
        private Vector2[] trackArray;

        /// <summary>
        /// List holding the viewports used in the simulation. 
        /// </summary>
        private List<Viewport> viewPortList;

        /// <summary>
        /// Bool representing if the main Camera moves its view to follow the robot. 
        /// </summary>
        private bool trackRobot = false;

        /// <summary>
        /// Time in totalmillseconds when F1, F2, or F3 were pushed. 
        /// </summary>
        private int timePressedKey = 0;

        /// <summary>
        /// The 2D font object needed to show text. Size = 20.
        /// </summary>
        private SpriteFont arialLarge;

        /// <summary>
        /// A smaller font of arial to show debug information. 
        /// </summary>
        private SpriteFont arialSmall;       

        /// <summary>
        /// Represents a random object so that random numbers can be generated. 
        /// </summary>
        private Random rand;

        /// <summary>
        /// The robot object. See the robot class for more information.
        /// </summary>
        private Robot robot1;


        /// <summary>
        /// Robot Lap Number
        /// </summary>
        private int robotLapNumber;

        /// <summary>
        /// Number of Laps to complete
        /// </summary>
        private int robotNumberOfLapsToComplete;

        /// <summary>
        /// total errors in the line detection. 
        /// </summary>
        private int totalErrorsInLineDetection;

        /// <summary>
        /// A list of the cameras. There are 2, The mainview and the robot view. 
        /// <list type="Camera">
        /// <item > WorldView Camera =  0 (The main camera)</item>
        /// <item> Robot View Camera = 1 </item>
        /// </list>
        /// </summary>        
        private List<Camera> cameraList;

        /// <summary> 
        /// <para >
        /// <item>A list of the rendertargets. Rather than rendering to the screen, the image produced by the GPU is saved to a rendertarget. </item>
        /// <item>The rendertargets correspond directly with the camera views. </item>
        /// </para>
        /// <list type=" RenderTargets">
        /// <item >World View Target = 0 </item>
        /// <item >Robot View Target = 1</item>
        /// </list>
        /// </summary>
        private List<RenderTarget2D> renderTargets;

        /// <summary>
        /// The image analysis object/class does the image processing. 
        /// </summary>
        private ImageAnalysis imageAnalysis;

        /// <summary >     
        /// Initializes a new instance of the SimulationMain class.
        /// </summary>
        public SimulationMain()
        {
            this.graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Initialize the Simulation Object.  
        /// </summary>
        protected override void Initialize()
        {
            this .trackArray = new Vector2[37*4];
            this.ReadParametersFromFile();
            this.graphics.PreferredBackBufferWidth = (int)this.windowSize.X; // Set the window size
            this.graphics.PreferredBackBufferHeight = (int)this.windowSize.Y;
            this.graphics.IsFullScreen = false  ; // Not full screen
            this.graphics.ApplyChanges();
            Window.Title = "Robot simulation1";

            IsMouseVisible = true;
            this.CreateViewPorts(); // Creates the viewports at startup.

            this.cameraList = new List<Camera>(); // List to insert the cameras. 

            // Make a camera component that is controlled by mouse position - this is the main view
            Camera cameraWorldView = new Camera(this, new Vector3(-40, -30, 20), Vector3.Zero, Vector3.UnitZ, true);
            Components.Add(cameraWorldView); // Updates will now sync with the game and the component
            this.cameraList.Add(cameraWorldView); // Add it to our list of cameras to render from;

            // Make camera for the robot. 
            this.robot1 = new Robot(this, new Vector3(0, 10.5f, 0), Vector3.UnitY, 0.03f, 1.8f, 1);
            Components.Add(this.robot1); // Updates will now sync with the game and the component
            this.cameraList.Add(this.robot1.GetRobotCamera()); // Add the robot camera to a list of cameras to draw from

            // Make an image analysis object so we can do calculations on the robot's view. 
            this.imageAnalysis = new ImageAnalysis(this, this.screenSize, this.viewPortList);
            Components.Add(this.imageAnalysis); // Updates will now sync with the game and the component

            this.rand = new Random();
            base.Initialize();
        }

        /// <summary>
        /// Called after initialize. Loads the textures and calls methods to create the simulated enviroment. 
        /// </summary>
        protected override void LoadContent() // 3rd method called at start up
        {
            this.spriteBatch = new SpriteBatch(GraphicsDevice); // Create a new SpriteBatch, which can be used to draw textures.

            this.arialLarge = Content.Load<SpriteFont>("ArialLarge"); // Load the font
            this.arialSmall = Content.Load<SpriteFont>("Arial");
            this.grass = Content.Load<Texture2D>("grass11"); // Load the grass texture
            this.largeGrass = this.CreateGrassTexture(this.grass); // Make a big grass texture that is a tile of the small grass texture. 

            // Load the vertex arrays for the lines, ground, and robot. 
            this.LoadLines();
            this.LoadGround();
            this.UpDateRobotPosition();

            this.basicEffects = new BasicEffect(GraphicsDevice); // Create a basic effects object so we the GPU knows how to render the vertex data

            base.LoadContent();
        }

        /// <summary>
        /// Unloads Information when the simulation is closed. Currently no information is saved when the simulation is closed. 
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }
        static int updateCount = 0;

        /// <summary>
        /// Main update method for the entire simulation. Simulation logic is updated when this method is called. Part of the game loop.
        /// </summary>
        /// <param name="gameTime">Clock information</param>
        protected override void Update(GameTime gameTime)
        {
            if (this.robot1.GetIsRobotPaused() != true)
            {
                updateCount++;
            }
            
            this.GetKeyBoard(gameTime); // Check the keyboard for commands
            this.UpDateRobotPosition(); // Update the position of the robot.                 

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
            if (gameTime1.TotalGameTime.TotalMilliseconds - this.timePressedKey > 500)
            {
                if (keyboardState.IsKeyDown(Keys.F3))
                {
                    this.timePressedKey = (int)gameTime1.TotalGameTime.TotalMilliseconds;
                    if (this.trackRobot == false)
                    {
                        this.trackRobot = true; // Toggle on and off  
                    }
                    else
                    {
                        this.trackRobot = false; // Toggle on and off  
                    }
                }

                //toggle the hough mode. 
                if (keyboardState.IsKeyDown(Keys.F4))
                {
                    this.timePressedKey = (int)gameTime1.TotalGameTime.TotalMilliseconds;
                    if (currentHoughMode  == 0)
                    {
                        currentHoughMode = 1; // Toggle on and off  
                    }
                    else
                    {
                        currentHoughMode = 0; // Toggle on and off  
                    }
                }
            }

            // Allows the game to exit when Escape is hit. 
            if (keyboardState.IsKeyDown(Keys.Escape) == true)
            {
                this.Exit();
            }
        }

        /// <summary>
        /// If 0 then Old mode (top left origin), if 1 then New Hough mode( bottom Center). 
        /// </summary>
        public  static int currentHoughMode = 1;

        ///// <summary>
        ///// Returns the current Hough mode. If 0 then Old mode (top left origin), if 1 then New Hough mode( bottom Center). 
        ///// </summary>
        ///// <returns>The Mode 0 = Old, 1 = New</returns>
        //public int GetHoughMode()
        //{
        //    return currentHoughMode;
        //}
        static int frameDrawCount = 0;
        /// <summary>
        /// Main draw method for the simulation. Draws all the world from both the mainview and the robots view. 
        /// <remarks >The render target must be built and disposed each time or else the a memory overload will occur</remarks>
        /// <remarks>Each time the Viewport is changed a new spriteBatch.Start method must be called.</remarks>
        /// </summary>
        /// <param name="gameTime">Clock Information</param>
        protected override void Draw(GameTime gameTime)
        {
            //bool simulationPause = ; // Find if the simulatin is paused from the robot. 
            if (this.robot1.GetIsRobotPaused() != true)
            {
                frameDrawCount++;
            }
            this.BuildRenderTargets(); // Create the render targets to be in the computer memory instead of the screen. 

            int h = 0;

            // Go through each camera and draw the world from its persepective. 
            foreach (Camera cam in this.cameraList) 
            {
                this.GraphicsDevice.SetRenderTarget(this.renderTargets[h]); // Set the render target, worldview then robot view
                this.GraphicsDevice.Clear(Color.Black); // Clear the screen to Black

                // Draw the world from the perspective of each camera
                this.DrawGrass(cam); // i.e grass
                this.DrawColorLines(cam);
                this.DrawRobot(cam);
                h++; // Go to the next render target
            }

            // Set the renderTarget to the scree, Clear the screen, 
            this.GraphicsDevice.SetRenderTarget(null); // Set the rendering target back to the screen
            this.GraphicsDevice.Clear(Color.Gray); // Clear the screen to Gray

            // Send the imageAnalysis object the robot's view, 
            this.imageAnalysis.SetRobotCameraView(this.renderTargets[1]); // Send the camera view from the robot camera to the analysis class
            this.imageAnalysis.Update1(gameTime); // Pull out the color data of the texture before the texture is disposed. 

            // Draw the render target, maks sure the source rectange size is the same as the texture to draw size,
            // (texture to draw, size of the part of the texture you what draw (the whole thing), Color, rotation, Origin of the texture,scale to 3/4 size, no effects, 0 is default)

            // Set the view port to the main view and draw the main view
            this.GraphicsDevice.Viewport = this.viewPortList[0]; // Main View  is 2/3 of the screen screen
            this.spriteBatch.Begin(); // Start the 2D drawing        
            this.spriteBatch.Draw(this.renderTargets[0], new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White); // Draw the MainView
            this.DrawText(); // Draw a message about whether the simulation is paused or active. 
            this.spriteBatch.End(); // Stop drawing. 

            // Change viewports to the topLeft view port of the window. Draw the robot view. 
            this.GraphicsDevice.Viewport = this.viewPortList[1];
            this.spriteBatch.Begin(); // Start the 2D drawing      
            this.spriteBatch.Draw(this.renderTargets[1], new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White); // Draw the robotView
            this.spriteBatch.End(); // Stop drawing.


            //// Change viewports to the bottomriht view port of the window. GiveDebug information. 
            //this.GraphicsDevice.Viewport = this.viewPortList[4];
            //this.spriteBatch.Begin(); // Start the 2D drawing     
            //this.DrawTextDebugInfo();
            //this.spriteBatch.End(); // Stop drawing.
           
            base.Draw(gameTime); // Call all the drawableGameCmponets draw method (imageAnalysis.cs).
            foreach (RenderTarget2D r in this.renderTargets)
            {
                r.Dispose(); // Free up the memory resources 
            }
        }

        private void DrawTextDebugInfo()
        {
           //// spriteBatch.DrawString(arialSmall  , "hi", new Vector2(0, 30), Color.White );
        }
        /// <summary>
        /// Creates the 5 view ports. View ports are like windows inside of a Window's window.
        /// <list type=" ViewPorts">
        /// <item > MainPort = 0 </item>
        /// <item > TopRight = 1</item>
        /// <item > CenterRight = 2</item>
        /// <item > BottomRight = 3</item>
        /// <item > BottomLeft = 4</item>
        /// <item > BottomCenter = 5</item>
        /// </list>
        /// </summary>        
        private void CreateViewPorts()
        {
            this.viewPortList = new List<Viewport>(); // Make a list for the viewports

            Viewport mainView = GraphicsDevice.Viewport; // Default view port is the window size. 
            mainView.Width = (int)GraphicsDevice.Viewport.Width * 2 / 3; // Takes up 2/3 of the x and y distances
            mainView.Height = (int)GraphicsDevice.Viewport.Height * 2 / 3;
            this.viewPortList.Add(mainView);

            for (int i = 0; i < 3; i++)
            {
                Viewport viewSide0 = GraphicsDevice.Viewport;
                viewSide0.X = (int)GraphicsDevice.Viewport.Width * 2 / 3;
                viewSide0.Y = i * (int)GraphicsDevice.Viewport.Height * 1 / 3; // Make 3 on the right side of the screen going down. 
                viewSide0.Width = (int)GraphicsDevice.Viewport.Width * 1 / 3;
                viewSide0.Height = (int)GraphicsDevice.Viewport.Height * 1 / 3;
                this.viewPortList.Add(viewSide0);
            }

            for (int i = 0; i < 2; i++)
            {
                Viewport viewSide0 = GraphicsDevice.Viewport;
                viewSide0.X = i * (int)GraphicsDevice.Viewport.Width * 1 / 3;
                viewSide0.Y = (int)GraphicsDevice.Viewport.Height * 2 / 3; // Make 3 on bottom of the screen going left. 
                viewSide0.Width = (int)GraphicsDevice.Viewport.Width * 1 / 3;
                viewSide0.Height = (int)GraphicsDevice.Viewport.Height * 1 / 3;
                this.viewPortList.Add(viewSide0);
            }
        }       

        /// <summary>
        /// Takes a texture and returns a larger texture. Size of the new texture is 1024,1024. 
        /// </summary>
        /// <param name="texturetoUse">The smaller texture to tile</param>
        /// <returns>The larger texture with the smaller on tiled onto it. </returns>
        private Texture2D CreateGrassTexture(Texture2D texturetoUse)
        {
            Color[,] groundColors = this.TextureTo2DArray(texturetoUse);
            Vector2 newTextureSize = new Vector2(1024, 1024); // Size of the new texture. 
            Color[] foregroundColors = new Color[(int)newTextureSize.X * (int)newTextureSize.Y];

            for (int x = 0; x < (int)newTextureSize.X; x++)
            {
                for (int y = 0; y < (int)newTextureSize.Y; y++)
                {
                    foregroundColors[x + (y * (int)newTextureSize.X)] = groundColors[(x * 2) % texturetoUse.Width, (y * 2) % texturetoUse.Height];
                }
            }

            Texture2D newTexture = new Texture2D(this.GraphicsDevice, (int)newTextureSize.X, (int)newTextureSize.Y, false, SurfaceFormat.Color);
            newTexture.SetData(foregroundColors);
            return newTexture;
        }

        /// <summary>
        /// Takes a texture converts it into a 2D color array
        /// </summary>
        /// <param name="texture">The texture to convert</param>
        /// <returns>The 2D color array </returns>
        private Color[,] TextureTo2DArray(Texture2D texture)
        {
            Color[] colors1D = new Color[texture.Width * texture.Height]; // Create a 1D array
            texture.GetData(colors1D); // Pull the color data out of the texture to the 1D arrray

            Color[,] colors2D = new Color[texture.Width, texture.Height]; // Create the 2D arrray
            for (int x = 0; x < texture.Width; x++)
            {
                for (int y = 0; y < texture.Height; y++)
                {
                    colors2D[x, y] = colors1D[x + (y * texture.Width)]; // Populate the 2D array with the correct values.  
                }
            }

            return colors2D;
        }

        /// <summary>
        /// Create vertex data for the grass. Verts2 is the VertexPositionTexture array
        /// </summary>
        private void LoadGround()
        {
            this.verts2 = new VertexPositionTexture[4];
            int dimension = 15;
            float  textureDim = 1.0f;
            this.verts2[0] = new VertexPositionTexture(new Vector3(-dimension, dimension, 0), new Vector2(0, 0));
            this.verts2[1] = new VertexPositionTexture(new Vector3(dimension, dimension, 0), new Vector2(textureDim, 0));
            this.verts2[2] = new VertexPositionTexture(new Vector3(-dimension, -dimension, 0), new Vector2(0, textureDim));
            this.verts2[3] = new VertexPositionTexture(new Vector3(dimension, -dimension, 0), new Vector2(textureDim, textureDim));
        }

        /// <summary>
        /// Creates the vertexPositionColor array "verts1" that is used to draw the white lines. 
        /// </summary>
        private void LoadLines()
        {
            this.verts1 = new VertexPositionColor[37 * 4]; // 2 innerlines and 2 outerlines
            int j = 0;
            float randomFirstX = 0, randomFirstY = 0;

            //Curve c = new Curve();
            //CurveKeyCollection cCollection = new CurveKeyCollection();
            //cCollection.Add(new CurveKey(

            // "i" is in radians going around the circle. 
            for (float i = 0; i < MathHelper.Pi * 2; i += MathHelper.Pi / 18)
            {
                float randomAdd1 = this.rand.Next(0, 2);
                float randomAdd2 = this.rand.Next(0, 2);

                if (j == 0)
                {
                    randomFirstX = randomAdd1; // Store the random changes for the first set of points
                    randomFirstY = randomAdd2;
                }

                if (j == 36)
                {
                    randomAdd1 = randomFirstX; // Apply the same random changes to the last that you did the first. 
                    randomAdd2 = randomFirstY;
                }
                this.trackArray[j] = new Vector2((float)((Math.Cos(i) * 10) + randomAdd1), (float)((Math.Sin(i) * 10) + randomAdd2));

                this.verts1[j] = new VertexPositionColor(new Vector3(trackArray[j].X,trackArray[j].Y, 0), Color.WhiteSmoke); // 1st inside line
                this.verts1[j + 37] = new VertexPositionColor(new Vector3((float)((Math.Cos(i) * 15) + randomAdd1), (float)((Math.Sin(i) * 14) + randomAdd2), 0), Color.White); // 1st outside line
                this.verts1[j + (37 * 2)] = new VertexPositionColor(new Vector3((float)((Math.Cos(i) * 10.2f) + randomAdd1), (float)((Math.Sin(i) * 10.2f) + randomAdd2), 0), Color.White); // 2nd inside line -- make it thicker
                this.verts1[j + (37 * 3)] = new VertexPositionColor(new Vector3((float)((Math.Cos(i) * 15.2f) + randomAdd1), (float)((Math.Sin(i) * 14.2f) + randomAdd2), 0), Color.White); // 2nd outside line -- make it thicker

                j++;
            }

            // Put the robot's starting position between the lines and store the robot's start position
            this.robot1.SetStartPosition(new Vector3((float)(10.5f + randomFirstX), (float)(0 + (randomFirstY / 2)), 0));
        }

        /// <summary>
        /// Loads the vertex information about the robot and recreates it every game cycle to reflect the new position of the robot. 'verts3' is the VertexPositionColor array
        /// </summary>
        private void UpDateRobotPosition()
        {
            if (this.robot1.GetIsRobotPaused() != true)
            {
                this.robot1.ChangeDirection(this.imageAnalysis.GetTurnIndicator()); // Steering determination get from imageAnalysis and give to robot
            }

            this.verts3 = new VertexPositionColor[16];  
            Vector3 robotPosition = this.robot1.GetPosition();
            Vector3 robotDirection = this.robot1.GetDirection();
            robotDirection.Normalize();
            Vector3 robotHeight = new Vector3(0, 0,1);

            // two rectangle robot representation. 

            // bottom
            //this.verts3[0] = new VertexPositionColor(new Vector3(0.3f, 0, 0) + robotPosition + robotDirection, Color.Aqua);
            //this.verts3[1] = new VertexPositionColor(new Vector3(-0.3f, 0, 0) + robotPosition + robotDirection, Color.AliceBlue);
            //this.verts3[2] = new VertexPositionColor(new Vector3(-0.3f, 0, 0) - robotPosition + robotDirection, Color.Aqua);
            //this.verts3[3] = new VertexPositionColor(new Vector3(0.3f, 0, 0) - robotPosition + robotDirection, Color.Azure  );

            //// top of bottom box. 
            //this.verts3[4] = new VertexPositionColor(new Vector3(0.3f, 0, 0) + robotPosition + robotDirection + robotHeight/2, Color.Aqua);
            //this.verts3[5] = new VertexPositionColor(new Vector3(-0.3f, 0, 0) + robotPosition + robotDirection + robotHeight/2, Color.AliceBlue);
            //this.verts3[6] = new VertexPositionColor(new Vector3(-0.3f, 0, 0) - robotPosition + robotDirection + robotHeight/2, Color.Aqua);
            //this.verts3[7] = new VertexPositionColor(new Vector3(0.3f, 0, 0) - robotPosition + robotDirection + robotHeight/2, Color.Azure);

            //// bottom  of top box. 
            //this.verts3[8] = new VertexPositionColor(new Vector3(0.3f, 0, 0) + robotPosition/2 + robotDirection + robotHeight/2, Color.Aqua);
            //this.verts3[9] = new VertexPositionColor(new Vector3(-0.3f, 0, 0) + robotPosition/2 + robotDirection + robotHeight/2, Color.AliceBlue);
            //this.verts3[10] = new VertexPositionColor(new Vector3(-0.3f, 0, 0) - robotPosition/2 + robotDirection + robotHeight/2, Color.Aqua);
            //this.verts3[11] = new VertexPositionColor(new Vector3(0.3f, 0, 0) - robotPosition/2 + robotDirection + robotHeight/2, Color.Azure);

            //// top  of top box. 
            //this.verts3[12] = new VertexPositionColor(new Vector3(0.3f, 0, 0) + robotPosition / 2 + robotDirection + robotHeight , Color.Aqua);
            //this.verts3[13] = new VertexPositionColor(new Vector3(-0.3f, 0, 0) + robotPosition / 2 + robotDirection + robotHeight , Color.AliceBlue);
            //this.verts3[14] = new VertexPositionColor(new Vector3(-0.3f, 0, 0) - robotPosition / 2 + robotDirection + robotHeight , Color.Aqua);
            //this.verts3[15] = new VertexPositionColor(new Vector3(0.3f, 0, 0) - robotPosition / 2 + robotDirection + robotHeight, Color.Azure);


            // Pyramid that represents the robot.
            this.verts3[0] = new VertexPositionColor(new Vector3(-0.3f, 0.3f, 0) + robotPosition, Color.Aqua);
            this.verts3[1] = new VertexPositionColor(new Vector3(0.3f, 0.3f, 0) + robotPosition, Color.Green);
            this.verts3[2] = new VertexPositionColor(new Vector3(-0.0f, 0.0f, 1.0f) + robotPosition, Color.Red);
            this.verts3[3] = new VertexPositionColor(new Vector3(-0.3f, -0.3f, 0) + robotPosition, Color.Black);
            this.verts3[4] = new VertexPositionColor(new Vector3(-0.3f, 0.3f, 0) + robotPosition, Color.Orange);

            if (this.trackRobot == true)
            {
                this.cameraList[0].SetCameraPositionAndTarget(robotPosition + (Vector3.UnitZ * 8), robotPosition); // Set the main camera to follow if follow is on
            }

            this.cameraList[1] = this.robot1.GetRobotCamera(); // Update the robot camera as well.
        }
                
        /// <summary>
        /// Set the renderTargets to a texture object in memory rather than rendering to the screen. Need target for both the Worldview and the robot CameraView.
        /// </summary>
        private void BuildRenderTargets()
        {
            this.renderTargets = new List<RenderTarget2D>(); // This list will correspond to the camera list

            // Main view target. 
            RenderTarget2D targetWorldView = new RenderTarget2D(this.GraphicsDevice, (int)this.screenSize.X, (int)this.screenSize.Y); // Create a target for rendering of the mouse countroled camera view
            this.renderTargets.Add(targetWorldView); // Add to the list of targets

            // The render target for the robot's camera view.  
            RenderTarget2D targetRobotView = new RenderTarget2D(GraphicsDevice, (int)this.screenSize.X, (int)this.screenSize.Y); // Create a target for the render of the robot camera view
            this.renderTargets.Add(targetRobotView); // Add to the list               
        }

        /// <summary>
        /// Draw the robot from the "camera"s perspective
        /// </summary>
        /// <param name="camera">The camera to use when drawing. </param>
        private void DrawRobot(Camera camera)
        {
            this.basicEffects.World = camera.World;
            this.basicEffects.View = camera.View;
            this.basicEffects.Projection = camera.Projection;
            this.basicEffects.TextureEnabled = false;
            this.basicEffects.VertexColorEnabled = true;

            foreach (EffectPass pass in this.basicEffects.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, this.verts3, 0, 3); // Verts3 is the robot.                
            }
        }

        /// <summary>
        /// Draw the lines from the "camera"'s persepective. 
        /// </summary>
        /// <param name="camera">The camera to use when drawing.</param>
        private void DrawColorLines(Camera camera)
        {
            this.basicEffects.World = camera.World;
            this.basicEffects.View = camera.View;
            this.basicEffects.Projection = camera.Projection;
            this.basicEffects.TextureEnabled = false;
            this.basicEffects.VertexColorEnabled = true;

            foreach (EffectPass pass in this.basicEffects.CurrentTechnique.Passes)
            {
                pass.Apply();
                this.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, this.verts1, 0, 36); // Draw the 1st inner line
                this.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, this.verts1, 37, 36); // Draw the 2nd inner line
                this.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, this.verts1, 37 * 2, 36); // Draw the outerline
                this.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.LineStrip, this.verts1, 37 * 3, 36); // Draw the 2nd outerline               
            }
        }

        /// <summary>
        /// Draw the grass or ground from "camera"'s perspective
        /// </summary>
        /// <param name="camera">The camera to use when drawing.</param>
        private void DrawGrass(Camera camera)
        {
            this.basicEffects.World = camera.World;
            this.basicEffects.View = camera.View;
            this.basicEffects.Projection = camera.Projection;
            this.basicEffects.VertexColorEnabled = false;
            this.basicEffects.Texture = this.largeGrass  ;
            this.basicEffects.TextureEnabled = true; // Because the ground is a texture object. 
           

            foreach (EffectPass pass in this.basicEffects.CurrentTechnique.Passes)
            {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, this.verts2, 0, 2);
            }
        }

        /// <summary>
        /// Draw the Simulation Status as either PAUSED or Active 
        /// </summary>
        private void DrawText()
        {
            bool simulationPause = this.robot1.GetIsRobotPaused(); // Find if the simulatin is paused from the robot. 

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
        }


        /// <summary>
        /// Read parametes from File. 
        /// </summary>
                private void ReadParametersFromFile()
                {
          

            FileStream fileStream = new FileStream(@"c:\file.txt", FileMode.Open);
            try
            {
          // read from file or write to file
        }
        finally
        {
          fileStream.Close();
        }
            // File name
            // open
            // parse, assign to parameters
            //close
        }

        /// <summary>
        /// Print Results of the simulation to a file. 
        /// </summary>
        private void PrintResultToFile()
        {
            // File name
            // open
            // print parameters to file so that they are easy to parse by optimizer. 
            //close
        }
    }
}
