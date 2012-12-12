
using System;
using System.Collections.Generic;
using System.Linq;
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
using Attempt_7;
using Attempt_7.ViewPorts;


namespace Attempt_7
{
    class DefaultRobotConfiguration
    {
        
        public Vector3 position { get; set; }              
        public Vector3 direction { get; set; }  
        public float speed { get; set; }        
        public float distanceToCameraTarget { get; set; }       
        public float cameraHeight { get; set; }
        public int changeDirectionThreshholdValue { get; set; }

        public DefaultRobotConfiguration()
        {
            position = new Vector3(0, 10.5f, 0);
            direction = Vector3.UnitY;
            speed = 0.03f;
            distanceToCameraTarget = 1.8f;
            cameraHeight = 1;
            changeDirectionThreshholdValue = 10; // default should be around 10-20
        }
    }
}
