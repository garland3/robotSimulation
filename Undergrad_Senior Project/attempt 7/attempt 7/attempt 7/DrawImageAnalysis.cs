//namespace Attempt_7
//{
//    using System;
//    using System.Collections.Generic;
//    using System.Diagnostics;
//    using System.IO;
//    using System.Linq;
//    using System.Timers;
//    using Microsoft.Xna.Framework;
//    using Microsoft.Xna.Framework.Audio;
//    using Microsoft.Xna.Framework.Content;
//    using Microsoft.Xna.Framework.GamerServices;
//    using Microsoft.Xna.Framework.Graphics;
//    using Microsoft.Xna.Framework.Input;
//    using Microsoft.Xna.Framework.Media;

//     /// <summary>
//    /// This is a game component that implements IUpdateable. Draws the image analysis information. 
//    /// </summary>
//    public class DrawImageAnalysis : Microsoft.Xna.Framework.DrawableGameComponent
//    {
//        /// <summary>
//        /// These values are part of the double for loops that reduce the computational requirements. These are the values that are incremented.
//        /// </summary>
//        private int count1A, count2A, count1C, count2C;

//        /// <summary>
//        /// The screenSize.
//        /// </summary>
//        private int screenWidth, screenHeight;

//        /// <summary>
//        /// The dimensions of the double for loops
//        /// </summary>
//        private int UpdateSquareDimForDrawing, UpdateSquareDimForAnalysis;

//        /// <summary>
//        /// How many lines to find. 
//        /// </summary>
//        private int NumberofLinesToFind;

//        /// <summary>
//        /// How precise the hough transform is. How large between possible values. 
//        /// </summary>
//        private int ThetaIncrement, RhoIncrement;

//         /// <summary>
//        /// Camera used to draw the triangles of what the robot is "thinking"
//        /// </summary>
//        private Camera  camera;

//        /// <summary>
//        /// List holding the viewports used in the simulation. This list is created in the Main simulation and then passed to this class when it is initiaized. 
//        /// </summary>
//        private List<Viewport> viewPortList;        

//         /// <summary>
//        /// BasicEffects for how to draw the triangles.
//        /// </summary>
//        private BasicEffect basicEffects;

//        /// <summary>
//        /// Font to use for drawing the debug/hough information
//        /// </summary>
//        private SpriteFont arial;

//        /// <summary>
//        /// The spriteBatch is used to draw 2D graphics
//        /// </summary>
//        private SpriteBatch spriteBatch;        

//        /// <summary>
//        /// VertexPositionColory array that stores the triangles of what the robot is "thinking". Each triangle represents 1 pixel from the robot's view. So 640*480 triangles.
//        /// </summary>
//        private VertexPositionColor[] vertexArray;

//        /// <summary>
//        /// Stores information about the hough. 
//        /// </summary>
//        double[] houghInfo;

//        /// <summary>
//        /// The image analysis object associated with this class. 
//        /// </summary>
//        ImageAnalysis imageAnalysis;

//        public DrawImageAnalysis(Game game, int screenWidth, int screenHeight, int UpdateSquareDimForDrawing, int UpdateSquareDimForAnalysis, int NumberofLinesToFind, int ThetaIncrement, int RhoIncrement, List<Viewport> viewPortList, ImageAnalysis imaageanalysis)
//                    : base(game)
//        {
//            this.screenWidth = screenWidth;
//            this.screenHeight = screenHeight;
//            this.UpdateSquareDimForDrawing = UpdateSquareDimForDrawing;
//            this.UpdateSquareDimForAnalysis = UpdateSquareDimForAnalysis;
//            this.NumberofLinesToFind = NumberofLinesToFind;
//            this.ThetaIncrement = ThetaIncrement;
//            this.RhoIncrement = RhoIncrement;            
//            this.viewPortList = viewPortList;
//            this.imageAnalysis = imaageanalysis;
//        }

//        /// <summary>
//        /// Allows the game component to perform any initialization it needs to before starting
//        /// to run.  This is where it can query for any required services and load content.
//        /// </summary>
//        public override void Initialize()
//        {
//            // Create a new SpriteBatch, which can be used to draw 2D textures.
//            this.spriteBatch = new SpriteBatch(Game.GraphicsDevice);
//            this.houghInfo = new double[(14 + 7 + 5) * NumberofLinesToFind]; // Make the array to store hough information. Must be double so that slopes which are fractions can be stored
                   
//            this.arial = Game.Content.Load<SpriteFont>("Arial"); // Load the font               

//            int scale = 1; // Scales the analysis screenSize. 

//            // Camera needed for the analysis picture 
//            this.camera = new Camera(Game, new Vector3(this.screenHeight / (scale * 2), this.screenWidth / (scale * 2), -550), new Vector3(this.screenHeight / (scale * 2), this.screenWidth / (scale * 2), 0), -Vector3.UnitX, false);
//            //// Game.Components.Add(camera);

//            this.LoadVertexArray(); // Loads the vertexs needed to draw the analysis triangles

         
//            base.Initialize();
//        }


//        /// <summary>
//        /// Allows the game component to update itself.
//        /// </summary>
//        /// <param name="gameTime">Provides a snapshot of timing values.</param>
//        public override  void Update(GameTime gameTime)
//        {
//            this.houghInfo = imageAnalysis.GetHoughInfo();
//            UpdateBoolMapto3DRectangle(imageAnalysis.GetTrueFalseMaptoDraw(), houghInfo);
//            base.Update(gameTime);
//        }
        
//        /// <summary>
//        /// Takes a 2D color array and colors the triangles. Also adds the hough lines. 
//        /// </summary>
//        /// <param name="c">Color Array to up on the triangles</param>
//        /// <param name="vertexArray"> Vertex array of the triangles</param>        
//        public void UpdateColorArrayto3DRectangle(Color[,] c, double[] houghInfo)
//        {
//            for (int x = this.count1C; x < this.screenWidth; x += UpdateSquareDimForDrawing)
//            {
//                for (int y = this.count2C; y < this.screenHeight; y += UpdateSquareDimForDrawing)
//                {
//                    this.vertexArray[x + (y * this.screenWidth)].Color = c[x, y];
//                }
//            }

//            this.count1C++;
//            if (this.count1C == UpdateSquareDimForDrawing)
//            {
//                this.count1C = 0;
//                this.count2C++;
//                this.AddHoughLinesColor(houghInfo); // Add the hough lines. 

//                if (this.count2C == UpdateSquareDimForDrawing)
//                {
//                    this.count2C = 0;
//                }
//            }
//        }

//        /// <summary>
//        /// Loads the SpriteBatch Content.
//        /// </summary>
//        protected override void LoadContent()
//        {
//            base.LoadContent();
//        }       

//        /// <summary>
//        /// Calls methods to draw the analysis triangles and the Debug text. 
//        /// </summary>
//        /// <param name="gameTime">Clock Information</param>
//        public override void Draw(GameTime gameTime)
//        {
//            Game.GraphicsDevice.Viewport = this.viewPortList[2]; // Right center view port
//            this.DrawAnalysis(); // Draw the analysis triangles
//            Game.GraphicsDevice.Viewport = this.viewPortList[3]; // Right bottom
//            this.DrawText(imageAnalysis.GetTurnIndicator(), imageAnalysis.GetWhiteCount(), houghInfo); // Draw the hough text info
//            base.Draw(gameTime);
//        }

//        /// <summary>
//        /// Colors the analysis triangles based a trueFalse bool map. Also adds the Hough information
//        /// If true then Blue. If false then Transparent.
//        /// </summary>
//        /// <param name="c">TrueFalse bool map</param>
//        ///
//       public  void UpdateBoolMapto3DRectangle(bool[,] c, double[] houghInfo)
//        {
//            for (int x = this.count1A; x < this.screenWidth; x += UpdateSquareDimForDrawing)
//            {
//                for (int y = this.count2A; y < this.screenHeight; y += UpdateSquareDimForDrawing)
//                {
//                    if (c[x, y] == true)
//                    {
//                        this.vertexArray[x + (y * this.screenWidth)].Color = Color.Blue;
//                    }
//                    else
//                    {
//                        this.vertexArray[x + (y * this.screenWidth)].Color = Color.DarkOrange;
//                    }
//                }
//            }

//            this.count1A++;
//            if (this.count1A == UpdateSquareDimForDrawing)
//            {
//                this.count1A = 0;
//                this.count2A++;
//                this.AddHoughLinesColor(houghInfo); // Add the hough lines. 
//                if (this.count2A == UpdateSquareDimForDrawing)
//                {
//                    this.count2A = 0;
//                }
//            }
//        }

//        /// <summary>
//        /// Updates the vertex information of the triangels for a line of a particular color based off its starting and ending position. 
//        /// </summary>
//        /// <param name="startLocation"> Start position of the line. </param>
//        /// <param name="endLocation">End position of the line </param>
//        /// <param name="c">Color to draw the line. </param>
//        private void InsertLine(Vector3 startLocation, Vector3 endLocation, Color c)
//        {
//            // Fifty points the line make a line.
//            for (int i = 0; i < 50; i++)
//            {
//                // Find each points location 
//                Vector3 lineLocation1 = Vector3.Lerp(startLocation, endLocation, (i / 50f));
//                {
//                    // If it is on the screen
//                    if (lineLocation1.X < this.screenWidth && lineLocation1.X > 0 && lineLocation1.Y < this.screenHeight && lineLocation1.Y > 0)
//                    {
//                        // Make the point 5 wide
//                        for (int j = 0; j < 5; j++)
//                        {
//                            // Make the point 5 height
//                            for (int k = 0; k < 5; k++)
//                            {
//                                // If the point is on the screen
//                                if ((int)(lineLocation1.X + k) + ((int)(lineLocation1.Y + j) * this.screenWidth) < vertexArray.Length &&
//                                         ((int)(lineLocation1.X + k) + ((int)(lineLocation1.Y + j) * this.screenWidth)) > 0)
//                                {
//                                    this.vertexArray[(int)(lineLocation1.X + k) + ((int)(lineLocation1.Y + j) * this.screenWidth)].Color = c; // Change the point to the color
//                                }
//                            }
//                        }
//                    }
//                }
//            }
//        }

//        /// <summary>
//        /// Puts the hough information about the lines in the vertex array. 
//        /// </summary>
//        private void AddHoughLinesColor(double[] houghInfo)
//        {
//            ////   insertLine(HoughLineStartandStopVectors[0], HoughLineStartandStopVectors[1], Color.Red);//red= strongest
//            ////   insertLine(HoughLineStartandStopVectors[2], HoughLineStartandStopVectors[3], Color.Green);     

//            // The first line found will be the darkest. The last line found will be the lightest
//            int colorInc = (255 - 0) / NumberofLinesToFind;

//            for (int i = 0; i < NumberofLinesToFind; i++)
//            {
//                int xprime = (int)(houghInfo[(14 + 4) + (i * 7)] + (this.screenWidth / 2)); // In screen cordinates with origin in the top left
//                int yprime = (int)(-houghInfo[(14 + 5) + (i * 7)] + this.screenHeight);
//                this.InsertLine(new Vector3((int)this.screenWidth / 2, (int)this.screenHeight, 0), new Vector3(xprime, yprime, 0), new Color(0, 0 + (i * colorInc), 0 + (i * colorInc)));
//            }
//        }


//        /// <summary>
//        /// Draws the Analysis Triangles stored in the 'vertexArray'
//        /// </summary>
//        public void DrawAnalysis()
//        {
//            foreach (EffectPass pass in basicEffects.CurrentTechnique.Passes)
//            {
//                pass.Apply();
//                GraphicsDevice . DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, this.vertexArray, 0, (this.screenHeight * this.screenWidth) - 2);
//            }
//        }

//        /// <summary>
//        /// Draws the Text giving important information about the status of the analysis.
//        /// </summary>
//        private void DrawText(int turnIndication, int totalWhiteCnt, double[] houghInfo)
//        {
//            int spacing = 15;
//            this.spriteBatch.Begin();
//            this.spriteBatch.DrawString(this.arial, "slope", new Vector2(0, 0), Color.White);
//            this.spriteBatch.DrawString(this.arial, "yintercept", new Vector2(0, spacing * 1), Color.White);
//            this.spriteBatch.DrawString(this.arial, "rho", new Vector2(0, spacing * 2), Color.White);
//            this.spriteBatch.DrawString(this.arial, "theta", new Vector2(0, spacing * 3), Color.White);
//            this.spriteBatch.DrawString(this.arial, "x1", new Vector2(0, spacing * 4), Color.White);
//            this.spriteBatch.DrawString(this.arial, "y1", new Vector2(0, spacing * 5), Color.White);
//            this.spriteBatch.DrawString(this.arial, "maxAccum", new Vector2(0, spacing * 6), Color.White);
//            this.spriteBatch.DrawString(this.arial, "Theta Increment = " + ThetaIncrement.ToString(), new Vector2(0, spacing * 7), Color.White);
//            this.spriteBatch.DrawString(this.arial, "rho Increment = " + RhoIncrement.ToString(), new Vector2(0, spacing * 8), Color.White);
//            this.spriteBatch.DrawString(this.arial, "update speed for drawing = " + Math.Pow(UpdateSquareDimForDrawing, 2).ToString() + " update speed for analysis = " + Math.Pow(UpdateSquareDimForAnalysis, 2).ToString(), new Vector2(0, spacing * 9), Color.White);
//            this.spriteBatch.DrawString(this.arial, "Turn Indicator = " + turnIndication.ToString(), new Vector2(0, spacing * 10), Color.White);
//            this.spriteBatch.DrawString(this.arial, "White Count = " + totalWhiteCnt.ToString(), new Vector2(0, spacing * 11), Color.White);
//            this.spriteBatch.DrawString(this.arial, "Number Of lines To find = " + NumberofLinesToFind.ToString() + " -  theta Average = " + houghInfo[(14 + (NumberofLinesToFind * 7))].ToString() + "  - rho Average = " + houghInfo[(14 + (NumberofLinesToFind * 7)) + 1].ToString(), new Vector2(0, spacing * 12), Color.White);

//            int spacingForText = (GraphicsDevice.Viewport.Width - 50) / NumberofLinesToFind;

//            // Insert the hough data for each line. 
//            for (int j = 0; j < NumberofLinesToFind; j++)
//            {
//                for (int i = 0; i < 7; i++)
//                {
//                    this.spriteBatch.DrawString(this.arial, houghInfo[14 + (j * 7) + i].ToString(), new Vector2(50 + (spacingForText * j), i * spacing), Color.White);
//                }
//            }

//            this.spriteBatch.End();
//        }

//        /// <summary>
//        /// Loads the position of each triangle into the VertexArray
//        /// </summary>
//        private void LoadVertexArray()
//        {
//            this.vertexArray = new VertexPositionColor[(this.screenWidth * this.screenHeight) + 5]; // The vertex array for the analysis triangles
//            int scale = 1;

//            for (int x = this.screenHeight - 1; x > 0; x--)
//            {
//                for (int y = this.screenWidth - 1; y > 0; y -= 2)
//                {
//                    this.vertexArray[(x * this.screenWidth) - y] = new VertexPositionColor(new Vector3(x / scale, y / scale, 0), Color.White); // Put a vertex at the position.
//                    this.vertexArray[(x * this.screenWidth) - (y + 1)] = new VertexPositionColor(new Vector3((x + 1) / scale, y / scale, 0), Color.White); // Put another vertex a the position but +1 in the X direction
//                }
//            }

//            // Create the basic effect. 
//            this.basicEffects = new BasicEffect(Game.GraphicsDevice);
//            this.basicEffects.TextureEnabled = false;
//            this.basicEffects.VertexColorEnabled = true;
//            this.basicEffects.World = this.camera.World;
//            this.basicEffects.View = this.camera.View;
//            this.basicEffects.Projection = this.camera.Projection;
//        }
       
//    }
//}

   