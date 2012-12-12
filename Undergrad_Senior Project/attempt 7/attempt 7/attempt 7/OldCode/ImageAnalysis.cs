////-----------------------------------------------------------------------
//// <copyright file="ImageAnalysis.cs" company="Anthony">
////     Preforms the image analysis compuation on the robot view.  
//// </copyright>
////-----------------------------------------------------------------------
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
    
//    /// <summary>
//    /// <para>
//    /// It was easier to redure 640*480 small triangles and view then from a distance than to put a texture on the GPU
//    /// So there is a camera to view the triangels. 
//    /// The class only analzes a small fraction of the pixels each time through the game loop inorder to keep the speed 
//    /// high. Many of the methods have loops that cause them to only look at every X pixel each time through and then
//    /// the next time through a different set. Basically it functions like a giant double "for" loop.
//    /// </para>    
//    /// The Image Analysis Class does the image processing of the robot view. 
//    /// </summary>
//    public class ImageAnalysis : Microsoft.Xna.Framework.GameComponent
//    {
//        /// <summary>
//        /// Number of lines the hough transform should find.
//        /// </summary>
//        private const short NumberofLinesToFind = 7;

//        /// <summary>
//        /// Size of the Acuumlator's lenght. Must be able to fit the largest posible value of rho. Max rho = Sqrt( ScreenHeight^2+ (ScreenWidth/2)^2), because the origin is the bottom center and the max rho is top left or right
//        /// </summary>        
//        private const int AccumLength = 600;

//        /// <summary>
//        /// How big the steps are for a potiential theta. In degreees. Large values  reduce computation but less acurate.  
//        /// </summary>
//        private const short ThetaIncrement = 5;

//        /// <summary>
//        /// How big the steps are for a potiential rho values. Large values  reduce computation but less acurate.  
//        /// </summary>
//        private const short RhoIncrement = 12;

//        /// <summary>
//        /// Inorder to make computations go faster, not every pixels is anylzed every time. 1 out of This value squared is analzed each pass
//        /// </summary>
//        private const short UpdateSquareDimForDrawing = 4;

//        /// <summary>
//        /// Inorder to make drawing go faster, not every pixels is updated every time. 1 out of This value squared is updated each pass
//        /// </summary>
//        private const short UpdateSquareDimForAnalysis = 4;

//        /// <summary>
//        /// Number of degrees around a maximum to clear around before searching the Accumulator again. 
//        /// </summary>
//        private const int ClearArroundMaxDegree = 10;

//        /// <summary>
//        /// Number of rho values around a maximum to clear around before searching the Accumulator again.
//        /// </summary>
//        private const int ClearArroundMaxRho = 30;

//        /// <summary>
//        /// Used by the smooth method. Dimension of Number of pixels  to look around for the smooth function. This (values*2)^2 = number of pixels checked.
//        /// </summary>
//        private const int SmoothSearchSize = 4;

//        /// <summary>
//        /// If true then steering desisions will be based off the theta's of the hough transform
//        /// </summary>       
//        private bool turnIndicatorisTheta = false;

        
//        /// <summary>
//        /// Texture object that represents the robot camera's current view
//        /// </summary>
//        private Texture2D robotCameraView;

//        /// <summary>
//        /// TrueFalse maps used for marking pixels either "good" or "bad"
//        /// </summary>
//        private bool[,] trueFalseMap, trueFalseMapB, trueFalseMapC;

//        /// <summary>
//        /// Screen Width of the image to analze
//        /// </summary>
//        private int screenWidth;

//        /// <summary>
//        /// Screen Height of the image to analze
//        /// </summary>
//        private int screenHeight;

//        /// <summary>
//        /// The turn indicator measures measures how much the analysis things the robot should go right or left. Right = positive. Left = negative
//        /// </summary>
//        private int turnIndication = 0;

//        /// <summary>
//        /// Stores information about lines from the hough transform. 
//        /// values 0-13 are the old hough info
//        /// each polarRho we want to find = 7 more values to store
//        /// 0=slope, 1= yInt, 2=Rho, 3=Theta, 4=Xvalue, 5=Yvalue, 6= size of the bin
//        /// 5  more ending values for the averages
//        /// </summary>
//        private double[] houghInfo;        

//        /// <summary>
//        /// Stores 4 vector3 locations of the beginning and end points of two lines on the screen. Was part of the old Hough system, but potientially still useful, so has not deleted.
//        /// 0 = start location of first line, 1 = end location of first line, 2 = start location of second line, 3 = end location of second line
//        /// </summary>
//        private List<Vector3> houghLineStartandStopVectors;       

//        /// <summary>
//        /// Color Array 2D from the robot camera that is analzed. 
//        /// </summary>
//        private Color[,] colorArray;

//        /// <summary>
//        /// Color Array 2D from the robot camera that is analzed. Can't extract informatino directly from the robot view Texture to 2D. But have to go through 1D array.
//        /// </summary>
//        private Color[] colorArray1D;       

//        /// <summary>
//        /// These values are the incremented numbers used in the giant double "for-loops". The "1" values are the for the first "for-loop"
//        /// </summary>
//        private short count1A = 0, count1B = 1, count1C = 0, count1D = 1, count1E = 0;

//        /// <summary>
//        /// These values are the incremented numbers used in the giant double "for-loops". The "2" values are the for the second "for-loop"
//        /// </summary>
//        private short count2A = 0, count2B = 0, count2C = 0, count2E = 0;

//        /// <summary>
//        /// An array of the middle clear pixels for each row. The length of the array is the number of rows = this.screenHeight.
//        /// </summary>
//        private int[] middleValues;

//        /// <summary>
//        /// Number of white pixels the "FindWhite" found
//        /// </summary>
//        private int totalWhiteCnt = 0;

//        /// <summary>
//        /// The accumlator for the hough values. Each position is a hough Bin. Each bin represents a line in Cartessian cordinates. The Accumlator is basically in polar cordinates. Theta,rho
//        /// </summary>
//        private short[,] accum2;
       
//        /// <summary>
//        /// Used by the smooth method. How many pixels must also be white in the area around a white pixel for it to be counted white. 
//        /// </summary>
//        private int cntThreshold = 15;

//        /// <summary>
//        /// On a scale of 0-255 how high does a pixel RGB value have to be before being declared white. 
//        /// </summary>
//        private int redGood, blueGood, greenGood;

//        /// <summary>
//        /// Sets red_good, blue_good, green_good to this value. 
//        /// </summary>
//        private int whiteParam = 150;

//        /// <summary>
//        /// The drawingImageAnalysis class handles the vertex information the shows what the robot is thinking
//        /// </summary>
//        DrawImageAnalysis drawAnalysis;        

//        /// <summary>
//        /// Initializes a new instance of the ImageAnalysis class.
//        /// </summary>
//        /// <param name="game">The game associated with the class</param>
//        /// <param name="screenSize">The size of the sceen to analze</param>
//        /// <param name="viewPortList1">A list of the view ports</param>
//        public ImageAnalysis(Game game, Vector2 screenSize, List<Viewport> viewPortList1)
//            : base(game)
//        {           
//            this.screenWidth = (int)screenSize.X;
//            this.screenHeight = (int)screenSize.Y;

//            //Creates the image drawing analysis object.
//            drawAnalysis = new DrawImageAnalysis(game, screenWidth, screenHeight, UpdateSquareDimForDrawing, UpdateSquareDimForAnalysis, NumberofLinesToFind, ThetaIncrement, RhoIncrement, viewPortList1,this );
//            drawAnalysis.DrawOrder = game.Components.Count;
//            Game.Components.Add(drawAnalysis);
//        }

//        /// <summary>
//        /// Called when the class is initialized. Creates many of the arrays and sets many of the values. 
//        /// </summary>
//        public override void Initialize()
//        {
//            // Create the arrays needed. Building them now will save CPU later. 
//            this.trueFalseMap = new bool[this.screenWidth, this.screenHeight];
//            this.trueFalseMapB = new bool[this.screenWidth, this.screenHeight];
//            this.trueFalseMapC = new bool[this.screenWidth, this.screenHeight];

//            this.accum2 = new short[180 / ThetaIncrement, AccumLength / RhoIncrement]; // Build the accumlator array. Make is smaller or shorter based on the size of the rho and theta increments
//            this.colorArray1D = new Color[this.screenWidth * this.screenHeight]; // Create a 1D array of color
//            this.colorArray = new Color[this.screenWidth, this.screenHeight]; // Create a 2D array of color

//            this.houghInfo = new double[(14 + 7 + 5) * NumberofLinesToFind]; // Make the array to store hough information. Must be double so that slopes which are fractions can be stored

//            // Set the color thresholds
//            this.redGood = this.whiteParam;
//            this.blueGood = this.whiteParam;
//            this.greenGood = this.whiteParam;

//            this.houghLineStartandStopVectors = new List<Vector3>();
//            for (int i = 0; i < 4; i++)
//            {
//                this.houghLineStartandStopVectors.Add(Vector3.Zero);
//            }          

//            this.middleValues = new int[this.screenHeight]; // Steering desisions are based off the average middle clear value for each row

//            base.Initialize();
//        }

//        /// <summary> 
//        /// Stores the texture from the robot camera in a color array before the texture is disposed. Expicitly called in the SimulationMain Draw method
//        /// </summary>
//        /// <param name="gameTime1">Clock Information</param>
//        public void Update1(GameTime gameTime1)
//        {
//            if (this.robotCameraView != null)
//            {
//                this.colorArray = this.TextureTo2DArray(this.robotCameraView, this.colorArray1D, this.colorArray);
//            }
//        }

//        /// <summary>
//        /// Takes a  2D renderTarget/Texture and sets it as the image to analze. Explicitly called in the SimulationMain Draw Method.
//        /// </summary>
//        /// <param name="text">The texture to analze. </param>
//        public void SetRobotCameraView(Texture2D text)
//        {
//            this.robotCameraView = text;
//        }

//        /// <summary>
//        /// Calls the analysis methods that actually do all the work. Basically the Main function for the Image Analysis Class
//        /// </summary>
//        /// <param name="gameTime">Clock Info</param>
//        public override void Update(GameTime gameTime)
//        {
//            if (this.colorArray != null)
//            {
//                this.trueFalseMapC = this.FindWhite(this.colorArray); // Find White                
//                this.Hough(this.trueFalseMapC); // Run the hough  
//                if (this.turnIndicatorisTheta != true)
//                {
//                    this.trueFalseMap = this.ShowPath(this.trueFalseMapC, this.trueFalseMapB); // Find the path through the map   
//                }
               
//                ////UpdateColorArrayto3DRectangle(colorArray, vertexArray);
//            }
//            base.Update(gameTime);
//        }
//        public int GetWhiteCount()
//        {
//            return totalWhiteCnt;
//        }

//        public double[] GetHoughInfo()
//        {
//            return houghInfo;
//        }

//        public bool[,] GetTrueFalseMaptoDraw()
//        {
//            return trueFalseMapC;
//        }

//        /// <summary>
//        /// Allows the SimulationMain to get the turnIndicator
//        /// </summary>
//        /// <returns>The turn Indicator</returns>
//        public int GetTurnIndicator()
//        {
//            return this.turnIndication;
//        }
       
//        /// <summary>
//        /// Takes a texture and makes it into a 2D color array. Passing in the arrays is faster than trying to build it each time. 
//        /// </summary>
//        /// <param name="texture">Texture to extract color information</param>
//        /// <param name="colors1D">1D array needed for the extraction</param>
//        /// <param name="colors2D">2D array that will be returned</param>
//        /// <returns>The 2D color array of the texture color information</returns>
//        private Color[,] TextureTo2DArray(Texture2D texture, Color[] colors1D, Color[,] colors2D)
//        {
//            texture.GetData(colors1D);
//            for (int x = 0; x < texture.Width; x++)
//            {
//                for (int y = 0; y < texture.Height; y++)
//                {
//                    colors2D[x, y] = colors1D[x + (y * texture.Width)];
//                }
//            }

//            return colors2D;
//        }
        
//        /// <summary>
//        /// Takes a 2D color array and finds the pixels that are "white"
//        /// </summary>
//        /// <param name="colorArray1">The color array to find white in.</param>
//        /// <returns>The TrueFalse bool map of white pixels. True  = white, False = not white. </returns>
//        private bool[,] FindWhite(Color[,] colorArray1)
//        {
//            // True = bad, False = good.
//            this.totalWhiteCnt = 0;
//            int i, j;
//            bool[,] trueFalseMap = new bool[colorArray1.GetLength(0), colorArray1.GetLength(1)]; // Create the bool map.

//            for (i = 0; i < this.screenWidth; ++i)
//            {
//                for (j = 0; j < this.screenHeight; ++j)
//                {
//                    // If above the thresholds. 
//                    if ((colorArray1[i, j].R > this.redGood)
//                        && (colorArray1[i, j].G > this.greenGood)
//                        && (colorArray1[i, j].B > this.blueGood))
//                    {
//                        trueFalseMap[i, j] = true;
//                        this.totalWhiteCnt++;
//                    }
//                    else
//                    {
//                        trueFalseMap[i, j] = false;
//                    }
//                }
//            }

//            return trueFalseMap;
//        }

//        /// <summary>
//        ///  Determines if white pixels meet the Width threshold to possibly be a line. 
//        ///  *******************NOT IN USE RIGHT NOW********************
//        /// </summary>
//        /// <param name="whitemap">The TrueFalse map of whate Pixels</param>
//        /// <returns> A truefalse map of white pixels that met the line width requirement</returns>
//        private bool[,] Whiteline(bool[,] whitemap)
//        {
//            int i, j, b, cnt;
//            bool[,] isLine = new bool[(int)this.screenWidth, (int)this.screenHeight]; // Make a new array
//            for (i = 0; i < this.screenWidth; ++i)
//            {
//                for (j = 0; j < this.screenHeight; ++j)
//                {
//                    if (whitemap[i, j] == true)
//                    {
//                        cnt = 0;
//                        for (b = 0; b <= 25; b++)
//                        {
//                            if (j + b < this.screenHeight && whitemap[i, j + b] == true)
//                            {
//                                cnt = cnt + 1;
//                            }
//                        }

//                        if (cnt > 3 && cnt < 15)
//                        {
//                            isLine[i, j] = true;
//                        }
//                        else
//                        {
//                            isLine[i, j] = false;
//                        }
//                    }
//                    else
//                    {
//                        isLine[i, j] = false;
//                    }
//                }
//            }

//            return isLine;
//        }

//        /// <summary>
//        /// Takes a truefalse map and for each pixel checks the other pixels around it to see if they are also white. If the number of pixels arround it that are also white is above a threshold then keep that pixel white. Meant to reduce noise in the picture.
//        /// *******************NOT IN USE RIGHT NOW********************
//        /// </summary>
//        /// <param name="original">The raw truefalse map of whie pixels.</param>
//        /// <param name="final">The trueFalse map to modify</param>
//        /// <returns>The truefalse map of pixels that met the threshold requirements. </returns>
//        private bool[,] Smooth(bool[,] original, bool[,] final)
//        {
//            int cnt;
//            for (int i = this.count1B; i < this.screenWidth; i += UpdateSquareDimForAnalysis)
//            {
//                for (int j = this.count2B; j < this.screenHeight; j += UpdateSquareDimForAnalysis)
//                {
//                    if (original[i, j] == true)
//                    {
//                        cnt = 0;
//                        for (int a = -SmoothSearchSize; a <= SmoothSearchSize; a++)
//                        {
//                            for (int b = -SmoothSearchSize; b <= SmoothSearchSize; b++)
//                            {
//                                if (i + a > -1 && i + a < this.screenWidth && j + b > -1 && j + b < this.screenHeight)
//                                {
//                                    if (original[i + a, j + b] == true)
//                                    {
//                                        cnt = cnt + 1;
//                                    }
//                                }
//                            }
//                        }

//                        if (cnt > this.cntThreshold)
//                        {
//                            final[i, j] = true;
//                        }
//                        else
//                        {
//                            final[i, j] = false;
//                        }
//                    }
//                    else
//                    {
//                        final[i, j] = false;
//                    }
//                }
//            }

//            this.count1B++;
//            if (this.count1B == UpdateSquareDimForDrawing)
//            {
//                this.count1B = 0;
//                this.count2B++;
//                if (this.count2B == UpdateSquareDimForDrawing)
//                {
//                    this.count2B = 0;
//                }
//            }

//            return final;
//        }

//        /// <summary>
//        /// Find a path through the map to go through and return it as a bool map, also set the turn indicator.
//        /// Based off the reactiveNavigation from the old robot code. 
//        /// </summary>
//        /// <param name="blocked">The trueFalse map of pixels that are blocked.</param>
//        /// <param name="clearPath">Blank. This map will be turned.</param>
//        /// <returns>The trueFalse map of a clear path.</returns>
//        private bool[,] ShowPath(bool[,] blocked, bool[,] clearPath)
//        {
//            int lastRightX = this.screenWidth, lastLeftX = 0;
//            int leftX, rightX;
//            int sum = 0;

//            for (int rowNumber = this.screenHeight - this.count1D - 1; rowNumber > 0; rowNumber -= UpdateSquareDimForAnalysis)
//            {
//                // Start in the middle
//                leftX = (int)((lastLeftX + lastRightX) / 2);

//                // Search starting in the middle and bottom of picture and go left (i.e. subtract) for a blocked pixel
//                while (leftX > lastLeftX && blocked[leftX, rowNumber] == false)
//                {
//                    clearPath[leftX, rowNumber] = true;
//                    leftX--;
//                }

//                // Start in the middle and bottome go right 
//                rightX = (int)((lastLeftX + lastRightX) / 2);

//                while (rightX < lastRightX && blocked[rightX, rowNumber] == false)
//                {
//                    clearPath[rightX, rowNumber] = true;
//                    rightX++;
//                }

//                lastLeftX = leftX;
//                lastRightX = rightX;
//                sum += (int)((lastLeftX + lastRightX) / 2);
//            }

//            this.count1D++;
//            if (this.count1D == UpdateSquareDimForAnalysis)
//            {
//                this.count1D = 0;
//            }

//            // Find the average middle pixels for each row and set the turn indicator based on this value. 
//            this.turnIndication = ((sum * UpdateSquareDimForAnalysis) / this.screenHeight) - (this.screenWidth / 2);
//            return clearPath;
//        }

//        /// <summary>
//        /// Finds Max value in Hough. Store information about that max.
//        /// </summary>
//        /// <param name="accumToAnalze">The accumlator of bins we want to search</param>
//        /// <param name="thetaIncrement">How large is the quantitization of the theta values. </param>
//        /// <param name="startIndexOfStoringHoughInfoList">What value in the Array 'HoughInfo' should we start storing information.</param>
//        private void FindMaxInAccumArrayOfHough(short[,] accumToAnalze, short thetaIncrement, short startIndexOfStoringHoughInfoList)
//        {
//            int maxTheta = 1;
//            int maxRho = 1;
//            int maxAccum = 1;
//            int accumDim1 = accumToAnalze.GetLength(0);
//            int accumDim2 = accumToAnalze.GetLength(1);

//            // Run through to find cell with most votes
//            for (int s = 0; s < accumDim1; s++)
//            {
//                for (int t = 0; t < accumDim2; t++)
//                {
//                    if (accumToAnalze[s, t] > maxAccum)
//                    {
//                        maxTheta = s;
//                        maxRho = t;
//                        maxAccum = accumToAnalze[s, t];
//                    }
//                }
//            }

//            maxTheta *= thetaIncrement; // Scale the Theta back to real size.
//            maxRho = maxRho * RhoIncrement; // Scale the Rho back to real size.
//            double x1 = (int)(maxRho * Math.Cos(MathHelper.ToRadians(maxTheta))); // Find the x Point corresponding the theta, rho
//            double y1 = (int)(maxRho * Math.Sin(MathHelper.ToRadians(maxTheta))); // Find the y Point corresponding the theta, rho

//            double slope1 = 1;
//            int yintercept1 = 0;

//            if (maxTheta != 270 && maxTheta != 90)
//            {
//                slope1 = -1 / Math.Tan(MathHelper.ToRadians(maxTheta)); // Calculating the slope
//            }

//            slope1 = Math.Round(slope1, 2);
//            yintercept1 = (int)(y1 - (slope1 * x1));

//            // Store the information found about the line in the 'this.houghInfo' array starting at the value 'StartIndexOfStoringHoughInfoList'
//            this.houghInfo[startIndexOfStoringHoughInfoList + 0] = slope1;
//            this.houghInfo[startIndexOfStoringHoughInfoList + 1] = yintercept1;
//            this.houghInfo[startIndexOfStoringHoughInfoList + 2] = maxRho;
//            this.houghInfo[startIndexOfStoringHoughInfoList + 3] = maxTheta;
//            this.houghInfo[startIndexOfStoringHoughInfoList + 4] = x1;
//            this.houghInfo[startIndexOfStoringHoughInfoList + 5] = y1;
//            this.houghInfo[startIndexOfStoringHoughInfoList + 6] = maxAccum;
//        }

//        /// <summary>
//        /// Part of the old Hough system. Finds the edge values on the screen of the lines based on the slope and yInt.
//        /// </summary>
//        /// <param name="slope1">Slope of the line</param>
//        /// <param name="yintercept1">YIntercept of the line</param>
//        /// <param name="startIndexforStorageArray">Where to store the information in the storage array</param>
//        private void CalculateStartandStopofLine(double slope1, short yintercept1, int startIndexforStorageArray)
//        {
//            int startX = 0; 
//            int startY = 0; 
//            int endX = 0; 
//            int endY = 0;

//            // Now switch all the y to -y for the start and end values
//            // Left Side
//            if (yintercept1 >= 0)
//            {
//                startX = (int)(-yintercept1 / slope1);
//                startY = 0;
//            }

//            if (yintercept1 <= 0 && yintercept1 > -this.screenHeight)
//            {
//                startX = 0;
//                startY = -(int)yintercept1;
//            }

//            if (yintercept1 < -this.screenHeight)
//            {
//                startX = (int)((-this.screenHeight - yintercept1) / slope1);
//                startY = this.screenHeight;
//            }

//            // Find the end cordinates of the line.
//            // Right Side          
//            int yright = (int)((slope1 * this.screenWidth) + yintercept1);
//            if (yright > 0)
//            {
//                endX = (int)(-yintercept1 / slope1);
//                endY = 0;
//            }

//            if (yright < 0 && yright > -this.screenHeight)
//            {
//                endX = this.screenWidth;
//                endY = -yright;
//            }

//            if (yright < -this.screenHeight)
//            {
//                endX = (int)((-this.screenHeight - yintercept1) / slope1);
//                endY = this.screenHeight;
//            }

//            // Store the Line information in the array 'houghLineStartandStopVectors' starting at the value 'startIndexforStorageArray'
//            this.houghLineStartandStopVectors[startIndexforStorageArray + 0] = new Vector3(startX, startY, 0);
//            this.houghLineStartandStopVectors[startIndexforStorageArray + 1] = new Vector3(endX, endY, 0);
//        }

//        /// <summary>
//        /// For each white pixel that might be part of a line, Find all the potiential lines going through it and store each vote for that line in the accumlator's bins.
//        /// Calls the methods to search through the accumlator to find the bins with the largest values. 
//        /// </summary>
//        /// <param name="isLine">The trueFalse array of pixels that might be part of a line. </param>
//        private void Hough(bool[,] isLine)
//        {
//            // Find all possible lines through each point and put into array bin.
//            for (short x = this.count1E; x < this.screenWidth; x += UpdateSquareDimForAnalysis)
//            {
//                for (short y = this.count2E; y < this.screenHeight; y += UpdateSquareDimForAnalysis)
//                {
//                    if (isLine[x, y] == true)
//                    {
//                        for (short theta = 0; theta < 180; theta += ThetaIncrement)
//                        {
//                            // Bottom center = origin. 
//                            short xprime = (short)(x - (this.screenWidth / 2));
//                            short yprime = (short)(-y + this.screenHeight);
//                            short rhoprime = (short)(((xprime * Math.Cos(MathHelper.ToRadians(theta))) + (yprime * Math.Sin(MathHelper.ToRadians(theta)))) / RhoIncrement);
//                            if (rhoprime >= 0)
//                            {
//                                this.accum2[theta / ThetaIncrement, rhoprime]++;
//                            }
//                            else
//                            {
//                                this.accum2[theta / ThetaIncrement, -rhoprime]++;
//                            }
//                        }
//                    }
//                }
//            }

//            this.count1E++;
//            if (this.count1E == UpdateSquareDimForAnalysis)
//            {
//                this.count1E = 0;
//                this.count2E++;
//                if (this.count2E == UpdateSquareDimForAnalysis)
//                {
//                    // Find the largest values. 
//                    for (int i = 0; i < NumberofLinesToFind; i++)
//                    {
//                        this.FindMaxInAccumArrayOfHough(this.accum2, ThetaIncrement, (short)(14 + (i * 7)));
//                        this.ClearMaxInAccum(this.accum2, ThetaIncrement, 14 + (i * 7));
//                    }

//                    this.FindAverages(); // Find the average rho and theta
//                    Array.Clear(this.accum2, 0, this.accum2.Length); // Clear the accumulator
//                    this.count2E = 0;
//                }
//            }
//        }

//        /// <summary>
//        /// Find the average theta values. Weigh them according to the rho value of each one. Make a turning desision based off the weighted thetas if turnBytheta is on. 
//        /// </summary>
//        private void FindAverages()
//        {
//            int thetaSum = 0;
//            int rhoSum = 0;
//            int count = 0;
//            int rho = -1;
//            int theta = -1;
//            int binSize = 0;
//            int turn = -1;

//            for (int i = 0; i < NumberofLinesToFind; i++)
//            {
//                // If the rho or theta are the same as the last one, then don't count it. 
//                if (this.houghInfo[(14 + (i * 7) + 4)] != rho && this.houghInfo[(14 + (i * 7) + 5)] != theta && this.houghInfo[(14 + (i * 7) + 6)] * 1.5 > binSize)
//                {
//                    rho = (int)this.houghInfo[(14 + (i * 7) + 2)]; // Sum the rhos
//                    theta = (int)this.houghInfo[(14 + (i * 7) + 3)]; // Sum the thetas
//                    binSize = (int)this.houghInfo[(14 + (i * 7) + 4)]; // Get bin size
//                    rhoSum += rho;
//                    thetaSum += theta;
//                    turn += (theta - 90) * (577 - rho) / 500;
//                    count++;
//                }
//            }

//            int averageTheta = thetaSum / count;
//            this.houghInfo[(14 + (NumberofLinesToFind * 7))] = averageTheta; // Compute and store the averages
//            this.houghInfo[(14 + (NumberofLinesToFind * 7)) + 1] = rhoSum / count;
//            if (this.turnIndicatorisTheta == true)
//            {
//                this.turnIndication = turn / count;
//            }
//        }

//        /// <summary>
//        /// Clears the array arround the maximum. Gets information about the line to clear around from the 'this.houghInfo' array
//        /// </summary>
//        /// <param name="accumToChange">The Accumulator to clear around</param>
//        /// <param name="degreeIncrement">The theta quantitization</param>
//        /// <param name="indexOffset">How far into the 'this.houghInfo' array is the data for the line we want to clear around</param>
//        /// <returns> The accumulator with the values around the max cleared.</returns>
//        private short[,] ClearMaxInAccum(short[,] accumToChange, short degreeIncrement, int indexOffset)
//        {
//            int rho = (int)(this.houghInfo[indexOffset + 2] / RhoIncrement);
//            int theta = (int)this.houghInfo[indexOffset + 3] / ThetaIncrement;
//            int accumDim1 = accumToChange.GetLength(0); // Get the size accum Array
//            int accumDim2 = accumToChange.GetLength(1);

//            // Clear the one with the most votes
//            for (int degree = -ClearArroundMaxDegree / ThetaIncrement; degree < ClearArroundMaxDegree / ThetaIncrement; degree++)
//            {
//                for (int phi = -ClearArroundMaxRho / RhoIncrement; phi < ClearArroundMaxRho / RhoIncrement; phi++)
//                {
//                    int thetaprime = (theta + degree) % accumDim1;
//                    int rhoprime = (rho + phi) % accumDim2;
//                    if (thetaprime > 0 && thetaprime < accumDim1 && rhoprime > 0 && rhoprime < accumDim2)
//                    {
//                        accumToChange[thetaprime, rhoprime] = 0;
//                    }
//                }
//            }

//            return accumToChange;
//        }       
//    }
//}