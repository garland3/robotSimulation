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
using Attempt_7;


namespace Attempt_7.ViewPorts
{
    public class ViewPortManager
    {

        /// <summary>
        /// List holding the viewports used in the simulation. 
        /// </summary>
        private List<Viewport> viewPortList;
       

        /// <summary>
        /// Constructor
        /// </summary>
        public  ViewPortManager()
        {
           // CreateViewPorts();
        }

        // Going to make a list, but in general I want to use the viewports by name
        // because that will be easier to remember. 
        // The list will let me make quick modifications to all the view ports by
        // looping through the list. 

     

    }
}
