
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

namespace Attempt_7.DrawAbleSimulationObjects
{

    /// <summary>
    /// This is a game component that implements IUpdateable. Draws the image analysis information. 
    /// </summary>
    public class DrawImageAnalysis : DrawAbleSimulationObject
    {
        /// <summary>
        /// These values are part of the double for loops that reduce the computational requirements. These are the values that are incremented.
        /// </summary>
        private int count1A, count2A, count1C, count2C;



        /// <summary>
        /// The screenSize.
        /// </summary>
        private int screenWidth, screenHeight;

        /// <summary>
        /// The dimensions of the double for loops
        /// </summary>
        private int updateSquareDimForDrawing;

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
        // private List<Viewport> viewPortList;

        /// <summary>
        /// BasicEffects for how to draw the triangles.
        /// </summary>
        private BasicEffect basicEffects;

        /// <summary>
        /// Texture of what the Camera found as white. 
        /// </summary>
        public Texture2D textureFindWhite { get; set; }

        /// <summary>
        /// The sprite batch object. Used to draw 2D graphics. 
        /// </summary>
        protected SpriteBatch spriteBatch;

        /// <summary>
        /// Texture of what HoughLines
        /// </summary>
        public Texture2D textureHoughLines { get; set; }

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
        // private double[] houghInfo;

        /// <summary>
        /// The image analysis object associated with this class. 
        /// </summary>
        public ImageAnalysis imageAnalysis { get; set; }

        /// <summary>
        /// A list of the vertexs to draw.
        /// </summary>
        private int[] vertexIndex;

        /// <summary>
        /// Other count usage. 1D = the number of vertexs to draw. So the number of triangels will be 1/3 this number. 
        /// </summary>
        public int count1D { get; set; }



        public Color[] colorMap { get; set; }

        /// <summary>        
        /// Initializes a new instance of the DrawImageAnalysis class.
        /// </summary>      
        public DrawImageAnalysis(Game game)
            : base(game)
        {
            this.screenWidth = (int)((SimulationMain)game).config.screenSize.X;
            this.screenHeight = (int)((SimulationMain)game).config.screenSize.Y;

            this.updateSquareDimForDrawing = (int)((SimulationMain)game).config.UpdateSquareDimForDrawing;

            this.numberofLinesToFind = (int)((SimulationMain)game).config.numberofLinesToFind;
            this.thetaIncrement = (int)((SimulationMain)game).config.ThetaIncrement;
            this.rhoIncrement = (int)((SimulationMain)game).config.RhoIncrement;
            //this.viewPortList = viewPortList;
            this.imageAnalysis = ((SimulationMain)game).mainRobot.imageAnalysisRobotCamera;


        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // Create a new SpriteBatch, which can be used to draw 2D textures.
            this.spriteBatch = new SpriteBatch(Game.GraphicsDevice);

            this.basicEffects = new BasicEffect(Game.GraphicsDevice); // Create a basic effects object so we the GPU knows how to render the vertex data


            this.textureFindWhite = new Texture2D(Game.GraphicsDevice, screenWidth, screenHeight);
            this.colorMap = new Color[screenWidth * screenHeight];

            for (int i = 0; i < colorMap.Length; i++)
            {
                this.colorMap[i] = Color.AliceBlue;
            }




            // // Camera needed for the analysis picture 
            // int xposition = this.screenHeight * 50 / 100;
            // int yposition = this.screenWidth * 50 / 100;

            // Vector3 cameraPosition = new Vector3(yposition, xposition, -590);
            // Vector3 cameraTarget = new Vector3(yposition, xposition, 0);
            // Vector3 camerUp = -Vector3.UnitY;

            // this.camera = new Camera(Game, cameraPosition, cameraTarget, camerUp, false); // Unit x for updirection
            // this.Game.Components.Add(this.camera);

            // this.LoadVertexArray(); // Loads the vertexs needed to draw the analysis triangles

            this.vertexArray2 = new VertexPositionColor[65535]; // The vertex array for the analysis triangles , the largest this could be is 65535 = 16 bit
            for (int i = 0; i < 65535; i++)
            {
                this.vertexArray2[i] = new VertexPositionColor(Vector3.Zero, Color.Blue);
            }


            // this.vertexArray2 = new VertexPositionColor[65535]; // The vertex array for the analysis triangles , the largest this could be is 65535 = 16 bit
            // for (int i = 0; i < 65535; i++)
            // {
            //     this.vertexArray2[i] = new VertexPositionColor(Vector3.UnitX, Color.Blue);
            // }

            base.Initialize();
        }

        ///// <summary>
        ///// Loads the position of each triangle into the VertexArray
        ///// </summary>
        //private void LoadVertexArray()
        //{
        //    this.vertexIndex = new int[this.screenHeight * this.screenWidth * 2];

        //    this.vertexArray = new VertexPositionColor[(this.screenWidth * this.screenHeight) + 5]; // The vertex array for the analysis triangles

        //    for (int x = this.screenHeight - 1; x > 0; x--)
        //    {
        //        for (int y = this.screenWidth - 1; y > 0; y -= 2)
        //        {
        //            this.vertexArray[(x * this.screenWidth) - y] = new VertexPositionColor(new Vector3(x, y, 0), Color.White); // Put a vertex at the position.
        //            this.vertexArray[(x * this.screenWidth) - (y + 1)] = new VertexPositionColor(new Vector3(x + 1, y, 0), Color.White); // Put another vertex a the position but +1 in the X direction
        //        }
        //    }

        //    // Create the basic effect. 
        //    //this.basicEffects = new BasicEffect(Game.GraphicsDevice);
        //    //this.basicEffects.TextureEnabled = false;
        //    //this.basicEffects.VertexColorEnabled = true;
        //    //this.basicEffects.World = this.camera.World;
        //    //this.basicEffects.View = this.camera.View;
        //    //this.basicEffects.Projection = this.camera.Projection;
        //}


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            this.count1D = 0;
            //this.textureFindWhite = GenerateTexturesFromBoolArray(imageAnalysis.findWhiteTrueFalseMap, this.colorMap);

           //this.ProjectBoolMapOnGroundJason(imageAnalysis.findWhiteTrueFalseMap);
            // this.ProjectBoolMapOnGroundAnthony(imageAnalysis.findWhiteTrueFalseMap);
            this.ProjectBoolMapOnGroundAnthony2(imageAnalysis.findWhiteTrueFalseMap);

            //for (int i = 0; i < imageAnalysis.houghLineList.Count; i++)
            //{
            //    this.InsertLine(imageAnalysis.houghLineList[i].houghStartVector, imageAnalysis.houghLineList[i].houghEndVector, Color.Red);
            //}


            InsertRobotPositionDirectionLines();

            base.Update(gameTime);
        }


        /// <summary>
        /// Main draw method 
        /// </summary>
        /// <param name="gameTime">Clock Information</param>
        public void DrawTexture(SpriteBatch spriteBatchSent)
        {
            Vector2 topRightVector = ((SimulationMain)Game).spriteRectangleManager.topRightVector;
            Vector2 scaleFactor = ((SimulationMain)Game).config.scaleFactorScreenSizeToWindow;

            //this.spriteBatch.Begin(); // Start the 2D drawing 
            spriteBatchSent.Draw(this.textureFindWhite, topRightVector, null,
               Color.White, 0, Vector2.Zero, scaleFactor, SpriteEffects.None, 0);
            //this.spriteBatch.End(); // Stop drawing. 

            // Game.GraphicsDevice.Viewport = this.viewPortList[1]; // Right center view port           
            // this.DrawVertexArray2(); // Draw the triangles showing the analysis

            // Game.GraphicsDevice.Viewport = this.viewPortList[2]; // Right bottom
            //this.DrawText(this.imageAnalysis.GetTurnIndicator(), this.imageAnalysis.GetWhiteCount(), this.houghInfo); // Draw the hough text info


            //Game.GraphicsDevice.Textures[0] = null;
        }

        /// <summary>
        /// Draw the analysis from the "camera"'s persepective. 
        /// </summary>
        /// <param name="camera">The camera to use when drawing.</param>
        public void DrawImageAnalysisResults(Camera camera)
        {
            if (this.count1D != 0)
            {
                this.basicEffects.World = camera.World;
                this.basicEffects.View = camera.View;
                this.basicEffects.Projection = camera.Projection;
                this.basicEffects.TextureEnabled = false;
                this.basicEffects.VertexColorEnabled = true;

                foreach (EffectPass pass in this.basicEffects.CurrentTechnique.Passes)
                {
                    pass.Apply();
                    // divide count1D by 3 because three verts make a triangle. 
                    Game.GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, this.vertexArray2, 0, this.count1D / 3);
                }
            }


        }

        /// <summary>
        /// Takes a 2D color array and colors the triangles.
        /// </summary>
        /// <param name="c">Color array to make into triangles</param>   
        // public void UpdateColorArrayto3DRectangle(Color[,] c)
        // {
        //     for (int x = this.count1C; x < this.screenWidth; x += this.updateSquareDimForDrawing)
        //     {
        //         for (int y = this.count2C; y < this.screenHeight; y += this.updateSquareDimForDrawing)
        //         {
        //             this.vertexArray[x + (y * this.screenWidth)].Color = c[x, y];
        //         }
        //     }

        //     this.count1C++;
        //     if (this.count1C == this.updateSquareDimForDrawing)
        //     {
        //         this.count1C = 0;
        //         this.count2C++;
        //         this.AddHoughLinesColor(); // Add the hough lines. 

        //         if (this.count2C == this.updateSquareDimForDrawing)
        //         {
        //             this.count2C = 0;
        //         }
        //     }
        // }


        /// <summary>
        ///  Colors the analysis triangles based a trueFalse bool map. Also adds the Hough information
        /// If true then Blue. If false then Transparent.
        /// </summary>
        /// <param name="c">True False map</param>
        /// <param name="houghInfo">Hough Information</param>
        public void UpdateBoolMapto3DVerts(bool[,] c)
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

            //this.AddHoughLinesColor(); // Add the hough lines. 
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






        private Texture2D GenerateTexturesFromBoolArray(bool[,] boolArray, Color[] colorMap)
        {
            Texture2D textureToModify = new Texture2D(Game.GraphicsDevice, screenWidth, screenHeight);

            for (int i = 0; i < screenWidth; i++)
            {
                for (int j = 0; j < screenHeight; j++)
                {

                    if (boolArray[i, j] == true)
                    {
                        colorMap[i + (j * screenWidth)] = Color.Red;
                    }
                    else
                    {
                        colorMap[i + (j * screenWidth)] = Color.Transparent;
                    }
                }
            }

            textureToModify.SetData<Color>(colorMap);

            return textureToModify;
        }


        ///// <summary>
        ///// Takes a bool map of and makes vertex positions based on the map. 
        ///// </summary>
        ///// <param name="c"> The bool map</param>       
        //private void BuildVertexArrayforDrawingSmallerNumberofTriangles(bool[,] c)
        //{
        //    this.count1D = 0;
        //    for (int x = 0; x < this.screenWidth; x++)
        //    {
        //        for (int y = 0; y < this.screenHeight; )
        //        {
        //            if (c[x, y] == true)
        //            {
        //                int j = 1;

        //                // If there are multiple pixels in a row, then make them all into on larger triangle.
        //                // Find how many pixels are in  a row.
        //                while ((y + j) < this.screenHeight && c[x, y + j] == true)
        //                {
        //                    j++;
        //                }

        //                this.vertexArray2[this.count1D + 2].Position.X = (float)x;
        //                this.vertexArray2[this.count1D + 2].Position.Y = (float)y;

        //                this.vertexArray2[this.count1D + 2].Color = Color.Red;

        //                // Put another vertex a the position but +1 in the X direction
        //                this.vertexArray2[this.count1D + 1].Position.X = (float)x + 3;
        //                this.vertexArray2[this.count1D + 1].Position.Y = (float)y + j;

        //                this.vertexArray2[this.count1D + 1].Color = Color.Red;

        //                // Put another vertex a the position but +1 in the X direction
        //                this.vertexArray2[this.count1D + 0].Position.X = (float)x;
        //                this.vertexArray2[this.count1D + 0].Position.Y = (float)y + 3 + j;
        //                this.vertexArray2[this.count1D + 0].Color = Color.Red;
        //                this.count1D += 3;

        //                y += j;
        //            }
        //            else
        //            {
        //                y++;
        //            }
        //        }
        //    }
        //}


        /// <summary>
        /// Takes a bool map of and makes vertex positions based on the map. 
        /// </summary>
        /// <param name="c"> The bool map</param>       
        private void ProjectBoolMapOnGroundJason(bool[,] c)
        {
            float DistanceToPoint;
            float distanceFromCenterLine;

            float triangleSize = 0.02f;

           // this.count1D = 0;

            // 1.8 = distance to center point of were camera is looking (i.e.) how far to look in front
            double disToCenterCamLooking = 1.8;
            // 1 = height of robot/camera
            int height = 1;
            // alpha is the angle from the robot camera to where it is looking in the center. 
            double alpha = Math.Atan(disToCenterCamLooking / height);

            double radian = MathHelper.ToRadians(16.875f);

            Vector3 robotPos = ((SimulationMain)Game).mainRobot.position;
            Vector3 robotDir = ((SimulationMain)Game).mainRobot.direction;



            for (int x = 0; x < this.screenWidth; x++)
            {
                for (int y = 0; y < this.screenHeight; )
                {
                    if (c[x, y] == true && this.count1D < 62000)
                    {
                        int j = 1;

                        // If there are multiple pixels in a row, then make them all into on larger triangle.
                        // Find how many pixels are in  a row.
                        while ((y + j) < this.screenHeight && c[x, y + j] == true)
                        {
                            j++;
                        }



                        if (y >= 240)
                            DistanceToPoint = (float)(disToCenterCamLooking - (height * Math.Sin((double)(y - 240) / 240) * radian) /
                                (Math.Cos(alpha) * Math.Sin(MathHelper.PiOver2 - ((y - 240) / 240) * radian) - alpha));
                        else
                            DistanceToPoint = (float)(disToCenterCamLooking - (height * Math.Sin((double)(240 - y) / 240) * radian) /
                                (Math.Cos(alpha) * Math.Sin(MathHelper.PiOver2 - ((240 - y) / 240) * radian) - alpha));


                        float DD = Vector2.DistanceSquared(Vector2.Zero, new Vector2((float)DistanceToPoint, (float)height));
                        //Math.Sqrt((double)(DistanceToPoint ^ 2 + height ^ 2));

                        if (x < 320)
                            distanceFromCenterLine = (int)(DD -
                                Math.Tan(((320 - x) / 320) * MathHelper.PiOver4 / 2));
                        else
                            distanceFromCenterLine = (int)(DD -
                                Math.Tan(((x - 320) / 320) * MathHelper.PiOver4 / 2));


                        // Vector3 pointLocationOnGround = new Vector3(DistanceToPoint,distanceFromCenterLine,0);
                        //pointLocationOnGround+= ((SimulationMain)Game).mainRobot.position; // Add the robot Position


                        Vector2 pointOnGroud = Vector2.Zero;

                        // Left
                        if (x < 320)
                        {
                            pointOnGroud.Y = robotPos.Y + (DistanceToPoint * robotDir.Y) / (robotDir.Length()) + distanceFromCenterLine * robotDir.Y / robotDir.Length();
                            pointOnGroud.X = robotPos.X + (DistanceToPoint * robotDir.X) / (robotDir.Length()) - distanceFromCenterLine * robotDir.Y / robotDir.Length();
                        }
                        // right
                        else
                        {
                            pointOnGroud.Y = robotPos.Y + (DistanceToPoint * robotDir.Y) / (robotDir.Length()) - distanceFromCenterLine * robotDir.Y / robotDir.Length();
                            pointOnGroud.X = robotPos.X + (DistanceToPoint * robotDir.X) / (robotDir.Length()) + distanceFromCenterLine * robotDir.Y / robotDir.Length();
                        }


                        // Vector3.
                        //x = DistanceToPoint;
                        //y = distanceFromCenterLine;                       


                        //this.vertexArray2[this.count1D + 2].Position.X = (float)x;
                        //this.vertexArray2[this.count1D + 2].Position.Y = (float)y;
                        this.vertexArray2[this.count1D + 0].Position.X = pointOnGroud.X - triangleSize;
                        this.vertexArray2[this.count1D + 0].Position.Y = pointOnGroud.Y - triangleSize * j;

                        this.vertexArray2[this.count1D + 0].Color = Color.Red;

                        // Put another vertex a the position but +1 in the X direction triangleSize
                        //this.vertexArray2[this.count1D + 1].Position.X = pointOnGroud.X + 3;
                        //this.vertexArray2[this.count1D + 1].Position.Y = pointOnGroud.Y + j;
                        this.vertexArray2[this.count1D + 1].Position.X = pointOnGroud.X;
                        this.vertexArray2[this.count1D + 1].Position.Y = pointOnGroud.Y + triangleSize * j;

                        this.vertexArray2[this.count1D + 1].Color = Color.Red;

                        // Put another vertex a the position but +1 in the X direction
                        //this.vertexArray2[this.count1D + 0].Position.X = pointOnGroud.X;
                        //this.vertexArray2[this.count1D + 0].Position.Y = pointOnGroud.Y + 3 + j;
                        this.vertexArray2[this.count1D + 2].Position.X = pointOnGroud.X + triangleSize;
                        this.vertexArray2[this.count1D + 2].Position.Y = pointOnGroud.Y - triangleSize * j;
                        this.vertexArray2[this.count1D + 2].Color = Color.Orange;
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
        /// Robot Direction in radians
        /// </summary>       
        public double thetaRobotDir { get; set; }

        /// <summary>
        /// Takes a bool map of and makes vertex positions based on the map. 
        /// </summary>
        /// <param name="c"> The bool map</param>       
        private void ProjectBoolMapOnGroundAnthony(bool[,] c)
        {

            float triangleSize = 0.02f;

           // this.count1D = 0;

            // 1.8 = distance to center point of were camera is looking (i.e.) how far to look in front
            double disToCenterCamLooking = 1.8;
            // 1 = height of robot/camera
            int height = 1;
            // alpha is the angle from the robot camera to where it is looking in the center. 
            double alpha = Math.Atan(disToCenterCamLooking / height);



            Vector3 robotPos = ((SimulationMain)Game).mainRobot.position;
            Vector3 robotDir = ((SimulationMain)Game).mainRobot.direction;

            // Point of interest in Robot Cordinate system R
            Vector3 pointOfInterest_R = Vector3.Zero;
            // Point of interest in  World W cordinate system. 
            Vector3 pointOfInterest_W = Vector3.Zero;


            for (int x = 0; x < this.screenWidth; x++)
            {
                for (int y = 0; y < this.screenHeight; )
                {
                    if (c[x, y] == true && this.count1D < 62000)
                    {
                        int j = 1;

                        // If there are multiple pixels in a row, then make them all into on larger triangle.
                        // Find how many pixels are in  a row.
                        while ((y + j) < this.screenHeight && c[x, y + j] == true)
                        {
                            j++;
                        }


                        // See my sheet of paper. (m,n) is the point of interest in the robot's cordinate system
                        float beta = (float)(-((y - 240) * MathHelper.PiOver4 / 480) + alpha);

                        float gamma = (float)Math.Atan((x - 320) * MathHelper.PiOver4 / 640);


                        // float centerPoint = (float)Math.Tan(alpha);
                        pointOfInterest_R.Y = (float)Math.Tan(beta);
                        pointOfInterest_R.X = (float)Math.Tan(gamma) * pointOfInterest_R.Y;

                        this.thetaRobotDir = Math.Atan(robotDir.Y / robotDir.X);
                        //if (robotDir.X <= 0)
                        //    theta += MathHelper.Pi;


                        pointOfInterest_W.X = (float)(robotPos.X + pointOfInterest_R.X * Math.Cos(this.thetaRobotDir) - pointOfInterest_R.Y * Math.Sin(this.thetaRobotDir));
                        pointOfInterest_W.Y = (float)(robotPos.Y + pointOfInterest_R.X * Math.Sin(this.thetaRobotDir) - pointOfInterest_R.Y * Math.Sin(this.thetaRobotDir));

                        pointOfInterest_R += robotPos;
                        // Vector3.
                        //x = DistanceToPoint;
                        //y = distanceFromCenterLine;                       


                        //this.vertexArray2[this.count1D + 2].Position.X = (float)x;
                        //this.vertexArray2[this.count1D + 2].Position.Y = (float)y;
                        this.vertexArray2[this.count1D + 0].Position.X = pointOfInterest_R.X - triangleSize;
                        this.vertexArray2[this.count1D + 0].Position.Y = pointOfInterest_R.Y - triangleSize * j;

                        this.vertexArray2[this.count1D + 0].Color = Color.Red;

                        // Put another vertex a the position but +1 in the X direction triangleSize
                        //this.vertexArray2[this.count1D + 1].Position.X = pointOnGroud.X + 3;
                        //this.vertexArray2[this.count1D + 1].Position.Y = pointOnGroud.Y + j;
                        this.vertexArray2[this.count1D + 1].Position.X = pointOfInterest_R.X;
                        this.vertexArray2[this.count1D + 1].Position.Y = pointOfInterest_R.Y + triangleSize * j;

                        this.vertexArray2[this.count1D + 1].Color = Color.Red;

                        // Put another vertex a the position but +1 in the X direction
                        //this.vertexArray2[this.count1D + 0].Position.X = pointOnGroud.X;
                        //this.vertexArray2[this.count1D + 0].Position.Y = pointOnGroud.Y + 3 + j;
                        this.vertexArray2[this.count1D + 2].Position.X = pointOfInterest_R.X + triangleSize;
                        this.vertexArray2[this.count1D + 2].Position.Y = pointOfInterest_R.Y - triangleSize * j;
                        this.vertexArray2[this.count1D + 2].Color = Color.Orange;
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
        /// Takes a bool map of and makes vertex positions based on the map. 
        /// </summary>
        /// <param name="c"> The bool map</param>       
        private void ProjectBoolMapOnGroundAnthony2(bool[,] c)
        {
            float triangleSize = 0.05f;            

            // Point of interest in  World W cordinate system. 
            Vector3 pointOfInterest_W = Vector3.Zero;
            // Point of interest in Robot Cordinate system R
            Vector3 pointOfInterest_R = Vector3.Zero;
            // alpha is the angle from the robot camera to where it is looking in the center. 
            //double alpha = Math.Atan(1.8f / 1);


            /// Matrix representation of the view determined by the position, target, and updirection.
            Matrix View = ((SimulationMain)Game).mainRobot.robotCameraView.View;

            /// Matrix representation of the view determined by the angle of the field of view (Pi/4), aspectRatio, nearest plane visible (1), and farthest plane visible (1200) 
            Matrix Projection = ((SimulationMain)Game).mainRobot.robotCameraView.Projection;

            /// Matrix representing how the real world cordinates differ from that of the rendering by the camera. 
            Matrix World = ((SimulationMain)Game).mainRobot.robotCameraView.World;

            Plane groundPlan = new Plane(Vector3.UnitZ, 0.0f);            

            for (int x = 0; x < this.screenWidth; x++)
            {
                for (int y = 0; y < this.screenHeight; )
                {
                    if (c[x, y] == true && this.count1D < 62000)
                    {
                        int j = 1;

                        float x1 = x * this.Game.GraphicsDevice.Viewport.Width / this.screenWidth;
                        float y1 = y * this.Game.GraphicsDevice.Viewport.Height / this.screenHeight;
                        

                        Vector3 nearPlanePoint = Game.GraphicsDevice.Viewport.Unproject(new Vector3(x1, y1, 0), Projection, View, World);
                        Vector3 farPlanePoint = Game.GraphicsDevice.Viewport.Unproject(new Vector3(x1, y1, 1), Projection, View, World);

                        //Vector3 Diff = nearPlanePoint - farPlanePoint;
                        //Diff.Normalize();
                        //float nearPlainDistance = 1.0f;
                        //Vector3 robotLoc = Diff * nearPlainDistance + nearPlanePoint;
                            
                            //Vector3 pointOfInterest_W = Vector3.in
                        Ray ray = new Ray(nearPlanePoint, farPlanePoint);
                      
                        float? interceptionAtPlane  = ray.Intersects(groundPlan);

                        if (interceptionAtPlane != null)
                            pointOfInterest_W = ray.Position + ray.Direction * (float)interceptionAtPlane;
                        else
                            break;
                        

                        this.vertexArray2[this.count1D + 0].Position.X = pointOfInterest_W.X - triangleSize;
                        this.vertexArray2[this.count1D + 0].Position.Y = pointOfInterest_W.Y - triangleSize * j;
                        

                        this.vertexArray2[this.count1D + 0].Color = Color.DarkOrange;

                        // Put another vertex a the position but +1 in the X direction triangleSize
                        //this.vertexArray2[this.count1D + 1].Position.X = pointOnGroud.X + 3;
                        //this.vertexArray2[this.count1D + 1].Position.Y = pointOnGroud.Y + j;
                        this.vertexArray2[this.count1D + 1].Position.X = pointOfInterest_W.X;
                        this.vertexArray2[this.count1D + 1].Position.Y = pointOfInterest_W.Y + triangleSize * j;

                        this.vertexArray2[this.count1D + 1].Color = Color.Red;

                        // Put another vertex a the position but +1 in the X direction
                        //this.vertexArray2[this.count1D + 0].Position.X = pointOnGroud.X;
                        //this.vertexArray2[this.count1D + 0].Position.Y = pointOnGroud.Y + 3 + j;
                        this.vertexArray2[this.count1D + 2].Position.X = pointOfInterest_W.X + triangleSize;
                        this.vertexArray2[this.count1D + 2].Position.Y = pointOfInterest_W.Y - triangleSize * j;

                        this.vertexArray2[this.count1D + 2].Color = Color.Orange;
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



        ///// <summary>
        ///// Takes a bool map of and makes vertex positions based on the map. 
        ///// </summary>
        ///// <param name="c"> The bool map</param>       
        //private  Texture2D BuildTextureBasedOnBoolMap(bool[,] c, Color[,] coloMap)
        //{           

        //    texture.GetData(colors1D);
        //    for (int x = 0; x < texture.Width; x++)
        //    {
        //        for (int y = 0; y < texture.Height; y++)
        //        {
        //            colors2D[x, y] = colors1D[x + (y * texture.Width)];
        //        }
        //    }

        //    Texture2D newTExture = new Texture2D(Game.GraphicsDevice,screenWidth,screenHeight);



        //    this.count1D = 0;
        //    for (int x = 0; x < this.screenWidth; x++)
        //    {
        //        for (int y = 0; y < this.screenHeight;)
        //        {
        //            if (c[x, y] == true)
        //            {
        //               coloMap[i
        //            }
        //            else
        //            {
        //                y++;
        //            }
        //        }
        //    }


        //}

        /// <summary>
        /// Updates the vertex information of the triangels for a line of a particular color based off its starting and ending position. 
        /// </summary>
        /// <param name="startLocation"> Start position of the line. </param>
        /// <param name="endLocation">End position of the line </param>
        /// <param name="c">Color to draw the line. </param>
        private void InsertLine(Vector3 startLocation, Vector3 endLocation, Color c)
        {
            float size = 0.04f;
            // Make a line, but make the number of points dependent on the length of the line. 
            // Put 25 per unit length. Because the size of the triangles are 0.02 which is 50 per lines to make it whole. 
            int incrementNumber = (int)(Vector3.Distance(startLocation, endLocation) * 25);

            for (int i = 0; i < incrementNumber; i++)
            {
                // Find each points location 
                Vector3 lineLocation1 = Vector3.Lerp(startLocation, endLocation, (float)(i * 1f / incrementNumber));
                {
                    if (this.count1D < 65000)
                    {

                        // Put a vertex at the position.
                        this.vertexArray2[this.count1D + 0].Position.X = lineLocation1.X - size;
                        this.vertexArray2[this.count1D + 0].Position.Y = lineLocation1.Y - size;
                        this.vertexArray2[this.count1D + 0].Color = c;

                        this.vertexArray2[this.count1D + 1].Position.X = lineLocation1.X;
                        this.vertexArray2[this.count1D + 1].Position.Y = lineLocation1.Y + size; // Put another vertex a the position but +1 in the X direction
                        this.vertexArray2[this.count1D + 1].Color = c;

                        this.vertexArray2[this.count1D + 2].Position.X = lineLocation1.X + size;
                        this.vertexArray2[this.count1D + 2].Position.Y = lineLocation1.Y - size; // Put another vertex a the position but +1 in the X direction
                        this.vertexArray2[this.count1D + 2].Color = c;

                        this.count1D += 3;
                    }
                }
            }
        }


        /// <summary>
        /// Insertes aa line showing the position and direction of hte robot. 
        /// </summary>        
        private void InsertRobotPositionDirectionLines()
        {
            Vector3 robotPos = ((SimulationMain)Game).mainRobot.position;
            Vector3 robotDir = ((SimulationMain)Game).mainRobot.direction;

            InsertLine(robotPos, Vector3.Zero, Color.Blue);
            InsertLine(robotPos, robotDir * 2 + robotPos, Color.Black);
        }

        /// <summary>
        /// Puts the hough information about the lines in the vertex array. 
        /// </summary>
        //private void AddHoughLinesColor()
        //{
        //    // The first line found will be the darkest. The last line found will be the lightest
        //    int colorInc = (255 - 0) / this.numberofLinesToFind;

        //    for (int i = 0; i < this.numberofLinesToFind; i++)
        //    {
        //        // Insert a hough line to from the start and stop positions calculated before. 
        //        this.InsertLine(this.houghLineStartandStopVectors[i * 4], this.houghLineStartandStopVectors[1 + (i * 4)], new Color(255 - (i * colorInc), 0, 0));

        //        // Insert a line from 0,0 to closest point on hough line. This line will be perpendicular to hough line. 
        //        this.InsertLine(this.houghLineStartandStopVectors[(i * 4) + 2], Vector3.Zero, new Color(150, 255 - (i * colorInc), 150));

        //        // Insert a line from bottom middle to the closest point on hough line. This line will be perpendicular to hough line. 
        //        //this.InsertLine(this.houghLineStartandStopVectors[(i * 4) + 3], new Vector3(this.screenWidth / 2, this.screenHeight, 0), new Color(0, 255, 255 - (i * colorInc)));
        //    }
        //}       

        ///// <summary>
        ///// Draws the analysis rectangles that are in the "vertexArray2" and only draws the first count1D/3 in the array.
        ///// </summary>
        //private void DrawVertexArray2()
        //{
        //    if (this.count1D != 0)
        //    {
        //        foreach (EffectPass pass in this.basicEffects.CurrentTechnique.Passes)
        //        {
        //            pass.Apply();
        //            GraphicsDevice.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleList, this.vertexArray2, 0, this.count1D / 3);
        //        }
        //    }
        //}

        // /// <summary>
        // /// Loads the SpriteBatch Content.
        // /// </summary>
        // protected override void LoadContent()
        // {
        //     base.LoadContent();
        // }


    }
}