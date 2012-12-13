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


    
namespace Attempt_7
{
    /// <summary>
    /// Contains configuration information about the Simulation. When it is initialized, it sets
    /// a bunch of default values. 
    /// </summary>
    public class ConfigurationInformation
    {
        
        const int OLD_HOUGH_MODE = 0;
        const int New_HOUGH_MODE = 1;

        /// <summary>
        /// How many lines to find. 
        /// </summary>
        public int numberofLinesToFind { get; set; }

        /// <summary>
        /// 2D vector representing the resolution of the cameras used in the simulation. 
        /// </summary>
        public Vector2 screenSize { get; set; }

        /// <summary>
        /// Rectangle object that is the size of the camera resolution (screenSize).
        /// </summary>
        public Rectangle screenRectangle { get; set; }       

        
        /// <summary>
        /// 2D vector represeting the size of the Window's Window that the simulation will run in. 
        /// </summary>
        public Rectangle windowSize {  get; set; }

        /// <summary>
        /// Scale Factor to make the screenSize fit 1/4 the windowSize
        /// Scaling is less than one. 
        /// </summary>
        public Vector2 scaleFactorScreenSizeToWindow { get; set; }  

        /// <summary>
        /// Bool representing if the main Camera moves its view to follow the robot. 
        /// </summary>
        public bool trackRobot { get; set; }
        

        /// <summary>
        /// Time in totalmillseconds when F1, F2, or F3 were pushed. 
        /// </summary>
        public int timePressedKey { get; set; }
       


        /// <summary>
        /// If 0 then Old mode (top left origin), if 1 then New Hough mode( bottom Center). 
        /// </summary>
        public int currentHoughMode { get; set; }       


        public int frameDrawCount { get; set; }
       

        /// <summary>
        /// Size of the Acuumlator's lenght. Must be able to fit the largest posible value of rho. Max rho = Sqrt( ScreenHeight^2+ (ScreenWidth/2)^2), because the origin is the bottom center and the max rho is top left or right
        /// </summary>        
        public int AccumLength {get;set;}

        /// <summary>
        /// lenght of the hold hough accumulator
        /// </summary>
        public  int AccumLengthOld {get;set;}


        /// <summary>
        /// How big the steps are for a potiential theta. In degreees. Large values  reduce computation but less acurate.  
        /// </summary>
        public  short ThetaIncrement {get;set;}


        /// <summary>
        /// How big the steps are for a potiential rho values. Large values  reduce computation but less acurate.  
        /// </summary>
        public  short RhoIncrement {get;set;}

        /// <summary>
        /// Inorder to make computations go faster, not every pixels is anylzed every time. 1 out of This value squared is analzed each pass
        /// </summary>
        public  short UpdateSquareDimForDrawing {get;set;}

        /// <summary>
        /// Inorder to make drawing go faster, not every pixels is updated every time. 1 out of This value squared is updated each pass
        /// </summary>
        public  short UpdateSquareDimForAnalysis {get;set;}

        /// <summary>
        /// Number of degrees around a maximum to clear around before searching the Accumulator again. 
        /// </summary>
        public  int ClearArroundMaxDegree {get;set;}

        /// <summary>
        /// Number of rho values around a maximum to clear around before searching the Accumulator again.
        /// </summary>
        public  int ClearArroundMaxRho {get;set;}

        /// <summary>
        /// Used by the smooth method. Dimension of Number of pixels  to look around for the smooth function. This (values*2)^2 = number of pixels checked.
        /// </summary>
        public  int SmoothSearchSize {get;set;}

       

        /// <summary>
        /// If true then steering desisions will be based off the theta's of the hough transform
        /// </summary>       
        public bool turnIndicatorisTheta {get;set;}

        /// <summary>
        /// Sets red_good, blue_good, green_good to this value. 
        /// </summary>
        public int whiteParam { get; set; }

        /// <summary>
        /// Used by the smooth method. How many pixels must also be white in the area around a white pixel for it to be counted white. 
        /// </summary>
        public int cntThreshold { get; set; }

        /// <summary>
        /// A test String for Reading and Writing. 
        /// </summary>
        public string testString { get; set; }


        /// <summary>
        /// Spokes on the course when making it. ie. when making the couse we pick trackPointsPerCircle to make the whole 
        /// circular track so that the angle between each point
        /// </summary>
        public int coureLinesPointsPerCircle { get; set; }

        // Robot Parameters
        public Vector3 robotPosition { get; set; }
        public Vector3 robotDirection { get; set; }
        public float robotSpeed { get; set; }
        public int robotChangeDirectionThreshholdValue { get; set; }
        public int robotNumberOfLapsToComplete { get; set; }
        public float robotTurnRatio { get; set; }
        
        // Robot Camera
        public float robotDistanceToCameraTarget { get; set; }
        public float robotCameraHeight { get; set; }
       
        
       
       

        /// <summary>
        /// Initialize and set the defaults of everything. 
        /// 
        /// </summary>
        public  ConfigurationInformation()
        {
            SetDefaults();
        }

        /// <summary>
        /// Sets the Defaults
        /// </summary>
        public void SetDefaults()
        {
            this.numberofLinesToFind = 3;
            this.screenSize = new Vector2(640, 480);
            this.screenRectangle = new Rectangle(0, 0, 640, 480);
            this.windowSize = new Rectangle(0,0,1000, 780);
            // calculate the scaling
            
            // Method 1 which is a true scalling, but the x and y are not scaled the same, so you get distortion
            this.scaleFactorScreenSizeToWindow     = new Vector2(this.windowSize.Width/(2*this.screenSize.X),this.windowSize.Height/(2*this.screenSize.Y));
            
            // 0.78125,0.8125

            // or the second method that just takes the smaller of the two, so that there will be even scaling
            //this.scaleFactorScreenSizeToWindow = new Vector2((float)0.78125, (float)0.78125);
               


            this.trackRobot = false;
            this.timePressedKey = 0;
            currentHoughMode = 1;
            this.frameDrawCount = 0;
            this.UpdateSquareDimForDrawing = 3;
            this.UpdateSquareDimForAnalysis = 3;
            this.AccumLength = 600;
            this.AccumLengthOld = 810;
            this.ThetaIncrement = 4;
            this.RhoIncrement = 7;
            this.ClearArroundMaxDegree = 5;
            this.ClearArroundMaxRho = 8;
            this.SmoothSearchSize = 4;
            
            // Course Lines
            this.coureLinesPointsPerCircle = 37;

            // Image Analysis
            this.whiteParam = 200;
            this.cntThreshold = 15;


            // 
            robotPosition = new Vector3(0, 10.5f, 0);
            robotDirection = Vector3.UnitY;
            robotSpeed = 0.20f;
            //distanceToCameraTarget = 1.8f;
            //cameraHeight = 1;
            robotDistanceToCameraTarget = 1.8f * 2;
            robotCameraHeight = 2f;
            robotChangeDirectionThreshholdValue = 10; // default should be around 10-20
            robotNumberOfLapsToComplete = 1;
            robotTurnRatio = 2500;
           
        }

    }
}
