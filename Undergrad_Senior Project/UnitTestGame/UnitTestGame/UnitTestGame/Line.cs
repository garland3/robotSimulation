
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

      //s public Vector3 ;

        //public void Line()
       // {

      //  }
    }
}
