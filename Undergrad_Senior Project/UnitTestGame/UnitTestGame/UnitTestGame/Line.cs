
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



   public class Line
    {
        /// Stores information about lines from the hough transform.    
        /// each polarRho we want to find = 7 more values to store
        /// 0=slope, 1= yInt, 2=Rho, 3=Theta, 4=Xvalue, 5=Yvalue, 6= size of the bin, 7= xTransformed value 8= yTransformedValue, 9 = distance to line Algorithm, 10= angle to line Algorithm
        /// 5  more ending values for the averages
        public double slope;
        public double yInt;
        public double Rho;
        public double Theta;
        public double Xvalue;
        public double Yvalue;
        public double sizeOfBin;
        
       


        public double xTrans;
        public double yTrans;
        public double distance;
        public double angleTransformed;

       /// <summary>
       /// The position that is the original (cos(theta)*rho, sin(theta)*rho)
       /// </summary>
        public Vector3 originalFoundPos;

       public Vector3 

        public Line()
        {

        }
    }
}
