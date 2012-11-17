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
    /// This is a game component that implements IUpdateable. Draws the image analysis information. 
    /// </summary>
    public class DrawImageAnalysis : Microsoft.Xna.Framework.DrawableGameComponent
    {
        /// <summary>
        /// These values are part of the double for loops that reduce the computational requirements. These are the values that are incremented.
        /// </summary>
        private int count1A, count2A, count1C, count2C;

        /// <summary>
        /// Other count usage. 1D = the number of vertexs to draw. So the number of triangels will be 1/3 this number. 
        /// </summary>
        private int count1D = 1;

        /// <summary>
        /// The screenSize.
        /// </summary>
        private int screenWidth, screenHeight;

        /// <summary>
        /// The dimensions of the double for loops
        /// </summary>
        private int updateSquareDimForDrawing, updateSquareDimForAnalysis;

        /// <summary>
        /// How many lines to find. 
        /// </summary>
        private int numberofLinesToFind;

        /// <summary>
        /// How precise the hough transform is. How large between possible values. 
        /// </summary>
        private int thetaIncrement, rhoIncrement;

        /// <summary>
        /// Camera used to draw the triangles of what the robot is "thinking"
        /// </summary>
        private Camera camera;

        /// <summary>
        /// List holding the viewports used in the simulation. This list is created in the Main simulation and then passed to this class when it is initiaized. 
        /// </summary>
        private List<Viewport> viewPortList;

        /// <summary>
        /// BasicEffects for how to draw the triangles.
        /// </summary>
        private BasicEffect basicEffects;

        /// <summary>
        /// Font to use for drawing the debug/hough information
        /// </summary>
        private SpriteFont arial;

        /// <summary>
        /// The spriteBatch is used to draw 2D graphics
        /// </summary>
        private SpriteBatch spriteBatch;

        /// <summary>
        /// VertexPositionColory array that stores the triangles of what the robot is "thinking". Each triangle represents 1 pixel from the robot's view. So 640*480 triangles.
        /// </summary>
        private VertexPositionColor[] vertexArray;

        /// <summary>
        /// Stores vertex information about the houglines and white pixels. Each time a vertex is added count1D should be incremented. 
        /// </summary>
        private VertexPositionColor[] vertexArray2;

        /// <summary>
        /// Stores information about the hough. 
        /// </summary>
        private double[] houghInfo;

        /// <summary>
        /// The image analysis object associated with this class. 
        /// </summary>
        private ImageAnalysis imageAnalysis;

        /// <summary>
        /// A list of the vertexs to draw.
        /// </summary>
        private int[] vertexIndex;

        /// <summary>
        /// Stores info about where houghlines start and stop. Values 0=start of houghline Vector3, 1= End of houghLine Vector3
        /// </summary>
        private Vector3[] houghLineStartandStopVectors;

        /// <summary>        
        /// Initializes a new instance of the DrawImageAnalysis class.
        /// </summary>
        /// <param name="game">the game associated with the calss</param>
        /// <param name="screenWidth">ScreenWidth</param>
        /// <param name="screenHeight">ScreenHeight</param>
        /// <param name="updateSquareDimForDrawing">How fast to update drawing. Obsolete</param>
        /// <param name="updateSquareDimForAnalysis">How fast to update analysis. This is used mostly for hough</param>
        /// <param name="numberofLinesToFind">How many lines to find in the picture</param>
        /// <param name="thetaIncrement">Quantitization of angle size, in degrees</param>
        /// <param name="rhoIncrement">Quantitization of rho size</param>
        /// <param name="viewPortList">A list of view ports</param>
        /// <param name="imageAnalysis">Associated imageAnalysis object.</param>
        public DrawImageAnalysis(Game game, int screenWidth, int screenHeight, int updateSquareDimForDrawing, int updateSquareDimForAnalysis, int numberofLinesToFind, int thetaIncrement, int rhoIncrement, List<Viewport> viewPortList, ImageAnalysis imageAnalysis)
            : base(game)
        {
            this.screenWidth = screenWidth;
            this.screenHeight = screenHeight;
            this.updateSquareDimForDrawing = updateSquareDimForDrawing;
            this.updateSquareDimForAnalysis = updateSquareDimForAnalysis;
            this.numberofLinesToFind = numberofLinesToFind;
            this.thetaIncrement = thetaIncrement;
            this.rhoIncrement = rhoIncrement;
            this.viewPortList = viewPortList;
            this.imageAnalysis = imageAnalysis;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // Create a new SpriteBatch, which can be used to draw 2D textures.
            this.spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            this.houghInfo = this.imageAnalysis.GetHoughInfo();           
            this.arial = Game.Content.Load<SpriteFont>("Arial"); // Load the font

            // Camera needed for the analysis picture 
            int xposition = this.screenHeight * 50 / 100;
            int yposition = this.screenWidth * 50 / 100;
            this.camera = new Camera(Game, new Vector3(yposition, xposition, -590), new Vector3(yposition, xposition, 0), -Vector3.UnitY, true); // Unit x for updirection
            this.Game.Components.Add(this.camera);

            this.vertexIndex = new int[this.screenHeight * this.screenWidth * 2];            

            this.LoadVertexArray(); // Loads the vertexs needed to draw the analysis triangles
            this.vertexArray2 = new VertexPositionColor[65535]; // The vertex array for the analysis triangles , the largest this could be is 65535 = 16 bit
            for (int i = 0; i < 65535; i++)
            {
                this.vertexArray2[i] = new VertexPositionColor(Vector3.UnitX, Color.Blue);
            }

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            this.houghInfo = this.imageAnalysis.GetHoughInfo();
            this.houghLineStartandStopVectors = this.imageAnalysis.GetHoughStartandStopVectors();
            this.BuildVertexArrayforDrawingSmallerNumberofTriangles(this.imageAnalysis.GetTrueFalseMaptoDraw());
            this.AddHoughLinesColor(); // Add the hough lines. 

            base.Update(gameTime);
        }

       /// <summary>
        /// Takes a 2D color array and colors the triangles.
       /// </summary>
       /// <param name="c">Color array to make into triangles</param>   
        public void UpdateColorArrayto3DRectangle(Color[,] c)
        {
            for (int x = this.count1C; x < this.screenWidth; x += this.updateSquareDimForDrawing)
            {
                for (int y = this.count2C; y < this.screenHeight; y += this.updateSquareDimForDrawing)
                {
                    this.vertexArray[x + (y * this.screenWidth)].Color = c[x, y];
                }
            }

            this.count1C++;
            if (this.count1C == this.updateSquareDimForDrawing)
            {
                this.count1C = 0;
                this.count2C++;
                this.AddHoughLinesColor(); // Add the hough lines. 

                if (this.count2C == this.updateSquareDimForDrawing)
                {
                    this.count2C = 0;
                }
            }
        }

        /// <summary>
        /// Calls methods to draw the analysis triangles and the Debug text. 
        /// </summary>
        /// <param name="gameTime">Clock Information</param>
        public override void Draw(GameTime gameTime)
        {
            Game.GraphicsDevice.Viewport = this.viewPortList[1]; // Right center view port           
            this.DrawVertexArray2(); // Draw the triangles showing the analysis

            Game.GraphicsDevice.Viewport = this.viewPortList[2]; // Right bottom
            this.DrawText(this.imageAnalysis.GetTurnIndicator(), this.imageAnalysis.GetWhiteCount(), this.houghInfo); // Draw the hough text info
            base.Draw(gameTime);
        }

       /// <summary>
       ///  Colors the analysis triangles based a trueFalse bool map. Also adds the Hough information
       /// If true then Blue. If false then Transparent.
       /// </summary>
       /// <param name="c">True False map</param>
       /// <param name="houghInfo">Hough Information</param>
        public void UpdateBoolMapto3DRectangle(bool[,] c, double[] houghInfo)
        {
            for (int x = this.count1A; x < this.screenWidth; x += this.updateSquareDimForDrawing)
            {
                for (int y = this.count2A; y < this.screenHeight; y += this.updateSquareDimForDrawing)
                {
                    if (c[x, y] == true)
                    {
                        this.vertexArray[x + (y * this.screenWidth)].Color = Color.White;
                    }
                    else
                    {
                        this.vertexArray[x + (y * this.screenWidth)].Color = Color.Transparent;
                    }
                }
            }

            this.AddHoughLinesColor(); // Add the hough lines. 
            this.count1A++;
            if (this.count1A == this.updateSquareDimForDrawing)
            {
                this.count1A = 0;
                this.count2A++;

                if (this.count2A == this.updateSquareDimForDrawing)
                {
                    this.count2A = 0;
                }
            }
        }
        
        /// <summary>
        /// Loads the SpriteBatch Content.
        /// </summary>
        protected override void LoadContent()
        {
            base.LoadContent();
        }

        /// <summary>
        /// Takes a bool map of and makes vertex positions based on the map. 
        /// </summary>
        /// <param name="c"> The bool map</param>       
        private void BuildVertexArrayforDrawingSmallerNumberofTriangles(bool[,] c)
        {            
            this.count1D = 0;
            for (int x = 0; x < this.screenWidth; x++)
            {
                for (int y = 0; y < this.screenHeight;)
                {
                    if (c[x, y] == true)
                    {
                        int j = 1;

                        // If there are multiple pixels in a row, then make them all into on larger triangle.
                        // Find how many pixels are in  a row.
                        while ((y + j) < this.screenHeight && c[x, y + j] == true)
                        {
                            j++;
                        }

                        this.vertexArray2[this.count1D + 0].Position = new Vector3(x, y, 0); // Put a vertex at the position.
                        this.vertexArray2[this.count1D + 0].Color = Color.Blue;
                        this.vertexArray2[this.count1D + 1].Position = new Vector3(x + 3, y + j, 0); // Put another vertex a the position but +1 in the X direction
                        this.vertexArray2[this.count1D + 1].Color = Color.Blue;
                        this.vertexArray2[this.count1D + 2].Position = new Vector3(x, y + 3 + j, 0); // Put another vertex a the position but +1 in the X direction
                        this.vertexArray2[this.count1D + 2].Color = Color.Blue;
                        this.count1D += 3;

                        y += j;
                    }
                    else
                    {
                        y++;
                    }
                }
            }
        }

        /// <summary>
        /// Updates the vertex information of the triangels for a line of a particular color based off its starting and ending position. 
        /// </summary>
        /// <param name="startLocation"> Start position of the line. </param>
        /// <param name="endLocation">End position of the line </param>
        /// <param name="c">Color to draw the line. </param>
        private void InsertLine(Vector3 startLocation, Vector3 endLocation, Color c)
        {           
            // Make a line, but make the number of points dependent on the length of the line. 
            int incrementNumber = (int)Vector3.Distance(startLocation, endLocation) / 10;
            for (int i = 0; i < incrementNumber; i++)
            {
                // Find each points location 
                Vector3 lineLocation1 = Vector3.Lerp(startLocation, endLocation, (float)(i * 1f / incrementNumber));
                {
                    // Make the point 5 wide
                    this.vertexArray2[this.count1D + 0].Position = new Vector3(lineLocation1.X, lineLocation1.Y, 0); // Put a vertex at the position.
                    this.vertexArray2[this.count1D + 0].Color = c;
                    this.vertexArray2[this.count1D + 1].Position = new Vector3(lineLocation1.X + 10, lineLocation1.Y, 0); // Put another vertex a the position but +1 in the X direction
                    this.vertexArray2[this.count1D + 1].Color = c;
                    this.vertexArray2[this.count1D + 2].Position = new Vector3(lineLocation1.X, lineLocation1.Y + 10, 0); // Put another vertex a the position but +1 in the X direction
                    this.vertexArray2[this.count1D + 2].Color = c;
                    this.count1D += 3;                    
                }
            }
        }

        /// <summary>
        /// Puts the hough information about the lines in the vertex array. 
        /// </summary>
        private void AddHoughLinesColor()
        {
            // The first line found will be the darkest. The last line found will be the lightest
            int colorInc = (255 - 0) / this.numberofLinesToFind;

            for (int i = 0; i < this.numberofLinesToFind; i++)
            {
                // Insert a hough line to from the start and stop positions calculated before. 
                this.InsertLine(this.houghLineStartandStopVectors[i * 4], this.houghLineStartandStopVectors[1 + (i * 4)], new Color(255 - (i * colorInc), 0, 0));

                // Insert a line from 0,0 to closest point on hough line. This line will be perpendicular to hough line. 
                this.InsertLine(this.houghLineStartandStopVectors[(i * 4) + 2], Vector3.Zero, new Color(150, 255 - (i * colorInc), 150));

                // Insert a line from bottom middle to the closest point on hough line. This line will be perpendicular to hough line. 
                //this.InsertLine(this.houghLineStartandStopVectors[(i * 4) + 3], new Vector3(this.screenWidth / 2, this.screenHeight, 0), new Color(0, 255, 255 - (i * colorInc)));
            }
        }       

        /// <summary>
        /// Draws the analysis rectangles that are in the "vertexArray2" and only draws the first count1D/3 in the array.
        /// </summary>
        private void DrawVertexArray2()
        {
            if (this.count1D != 0)
            {
                foreach (EffectPass pass in this.basicEffects.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, this.vertexArray2, 0, this.count1D / 3);
                }
            }
        }

       /// <summary>
        /// Draws the Text giving important information about the status of the analysis.
       /// </summary>
       /// <param name="turnIndication">Indicator of how much to turn</param>
       /// <param name="totalWhiteCnt">Number of white pixels found</param>
       /// <param name="houghInfo">Hough information array</param>
        private void DrawText(int turnIndication, int totalWhiteCnt, double[] houghInfo)
        {
            int spacing = 15;
            this.spriteBatch.Begin();
            this.spriteBatch.DrawString(this.arial, "slope", new Vector2(0, 0), Color.White);
            this.spriteBatch.DrawString(this.arial, "yintercept", new Vector2(0, spacing * 1), Color.White);
            this.spriteBatch.DrawString(this.arial, "rho", new Vector2(0, spacing * 2), Color.White);
            this.spriteBatch.DrawString(this.arial, "theta", new Vector2(0, spacing * 3), Color.White);
            this.spriteBatch.DrawString(this.arial, "x1", new Vector2(0, spacing * 4), Color.White);
            this.spriteBatch.DrawString(this.arial, "y1", new Vector2(0, spacing * 5), Color.White);
            this.spriteBatch.DrawString(this.arial, "BinSize", new Vector2(0, spacing * 6), Color.White);
            this.spriteBatch.DrawString(this.arial, "xTrans = ", new Vector2(0, spacing * 7), Color.White);
            this.spriteBatch.DrawString(this.arial, "yTrans = ", new Vector2(0, spacing * 8), Color.White);
            this.spriteBatch.DrawString(this.arial, "Distance = ", new Vector2(0, spacing * 9), Color.White);
            this.spriteBatch.DrawString(this.arial, "Angle = ", new Vector2(0, spacing * 10), Color.White);
            ////this.spriteBatch.DrawString(this.arial, "update speed for drawing = " + Math.Pow(UpdateSquareDimForDrawing, 2).ToString() + " update speed for analysis = " + Math.Pow(UpdateSquareDimForAnalysis, 2).ToString(), new Vector2(0, spacing * 9), Color.White);
            this.spriteBatch.DrawString(this.arial, "Turn Indicator = " + turnIndication.ToString(), new Vector2(0, spacing * 11), Color.White);
            this.spriteBatch.DrawString(this.arial, "White Count = " + totalWhiteCnt.ToString() + " Vertexs = " + this.count1D.ToString(), new Vector2(0, spacing * 12), Color.White);
            this.spriteBatch.DrawString(this.arial, "Number Of lines To find = " + this.numberofLinesToFind.ToString() + " -  theta Average = " + this.houghInfo[(this.numberofLinesToFind * 11)].ToString() + "  - rho Average = " + this.houghInfo[(this.numberofLinesToFind * 11) + 1].ToString(), new Vector2(0, spacing * 13), Color.White);
            this.spriteBatch.DrawString(this.arial, "Singualarites Old:" + this.imageAnalysis.GetSingularitesOldHough() + " New = " + this.imageAnalysis.GetSingularitiesNewHough(), new Vector2(0, spacing * 14), Color.White);
            this.spriteBatch.DrawString(this.arial, "Hough Mode " + this.imageAnalysis.GetHoughMode(), new Vector2(0, spacing * 15), Color.White);

            int spacingForText = (GraphicsDevice.Viewport.Width - 50) / this.numberofLinesToFind;

            // Insert the hough data for each line. 
            for (int j = 0; j < this.numberofLinesToFind; j++)
            {
                for (int i = 0; i < 11; i++)
                {
                    this.spriteBatch.DrawString(this.arial, houghInfo[(j * 11) + i].ToString(), new Vector2(50 + (spacingForText * j), i * spacing), Color.White);
                }
            }

            this.spriteBatch.End();
        }

        /// <summary>
        /// Loads the position of each triangle into the VertexArray
        /// </summary>
        private void LoadVertexArray()
        {
            this.vertexArray = new VertexPositionColor[(this.screenWidth * this.screenHeight) + 5]; // The vertex array for the analysis triangles

            for (int x = this.screenHeight - 1; x > 0; x--)
            {
                for (int y = this.screenWidth - 1; y > 0; y -= 2)
                {
                    this.vertexArray[(x * this.screenWidth) - y] = new VertexPositionColor(new Vector3(x, y, 0), Color.White); // Put a vertex at the position.
                    this.vertexArray[(x * this.screenWidth) - (y + 1)] = new VertexPositionColor(new Vector3(x + 1, y, 0), Color.White); // Put another vertex a the position but +1 in the X direction
                }
            }

            // Create the basic effect. 
            this.basicEffects = new BasicEffect(Game.GraphicsDevice);
            this.basicEffects.TextureEnabled = false;
            this.basicEffects.VertexColorEnabled = true;
            this.basicEffects.World = this.camera.World;
            this.basicEffects.View = this.camera.View;
            this.basicEffects.Projection = this.camera.Projection;
        }
    }
}