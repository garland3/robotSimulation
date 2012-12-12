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
    public class Obstacle : DrawAbleSimulationObject
    {
        /// <summary>
        /// Constructor
        /// </summary>       
        public  Obstacle(Game game) :base(game)
        {
        }
    }
}
