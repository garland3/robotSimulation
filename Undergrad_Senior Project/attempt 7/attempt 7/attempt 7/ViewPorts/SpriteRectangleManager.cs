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

namespace Attempt_7.ViewPorts
{
    public class SpriteRectangleManager
    {
        /// <summary>
        /// the game that we are managing the rectangles for
        /// </summary>
        private Game game;

        /// <summary>
        /// Rectangle in the top left. 
        /// </summary>
        public Rectangle topLeft { get; set; }
        public Vector2  topLeftVector { get; set; }


        /// <summary>
        /// Rectangle in the top right. 
        /// </summary>
        public Rectangle topRight { get; set; }
        public Vector2 topRightVector { get; set; }

        /// <summary>
        /// Rectangle in the bottom left. 
        /// </summary>
        public Rectangle bottomLeft { get; set; }
        public Vector2 bottomLeftVector { get; set; }

        /// <summary>
        /// Rectangle in the bottom right. 
        /// </summary>
        public Rectangle bottomRight { get; set; }
        public Vector2 bottomRightVector { get; set; }

        

        /// <summary>
        /// Rectangle in the top right. 
        /// </summary>
        public SpriteRectangleManager(Game game)
        {
            this.game = game;
            this.topLeftVector = new Vector2(0, 0);
            this.topRightVector = new Vector2(((SimulationMain)game).GraphicsDevice.Viewport.Width / 2, 0);
            this.bottomLeftVector = new Vector2(0, ((SimulationMain)game).GraphicsDevice.Viewport.Height / 2);
            this.bottomRightVector = new Vector2(((SimulationMain)game).GraphicsDevice.Viewport.Width / 2, ((SimulationMain)game).GraphicsDevice.Viewport.Height / 2);
            

            this.topLeft = new Rectangle(0, 0,  ((SimulationMain)game).GraphicsDevice.Viewport.Width / 2, ((SimulationMain)game).GraphicsDevice.Viewport.Height / 2);
            this.topRight = new Rectangle(((SimulationMain)game).GraphicsDevice.Viewport.Width / 2, 0 ,((SimulationMain)game).GraphicsDevice.Viewport.Width , ((SimulationMain)game).GraphicsDevice.Viewport.Height / 2);
            this.bottomLeft = new Rectangle(0, ((SimulationMain)game).GraphicsDevice.Viewport.Height / 2, ((SimulationMain)game).GraphicsDevice.Viewport.Width / 2, ((SimulationMain)game).GraphicsDevice.Viewport.Height);
            this.bottomRight = new Rectangle(((SimulationMain)game).GraphicsDevice.Viewport.Width / 2,
                                             ((SimulationMain)game).GraphicsDevice.Viewport.Height / 2,
                                             ((SimulationMain)game).GraphicsDevice.Viewport.Width,
                                              ((SimulationMain)game).GraphicsDevice.Viewport.Height);                
                
        }



        /// <summary>
        /// Creates the 5 view ports. View ports are like windows inside of a Window's window.
        /// <list type=" ViewPorts">
        /// <item > MainPort = 0 </item>
        /// <item > TopRight = 1</item>
        /// <item > CenterRight = 2</item>
        /// <item > BottomRight = 3</item>
        /// <item > BottomLeft = 4</item>
        /// <item > BottomCenter = 5</item>
        /// </list>
        /// </summary>        
        //private void CreateViewPorts()
        //{
        //    this.viewPortList = new List<Viewport>(); // Make a list for the viewports

        //    Viewport mainView = GraphicsDevice.Viewport; // Default view port is the window size. 
        //    mainView.Width = (int)GraphicsDevice.Viewport.Width * 2 / 3; // Takes up 2/3 of the x and y distances
        //    mainView.Height = (int)GraphicsDevice.Viewport.Height * 2 / 3;

        //    this.viewPortList.Add(mainView);

        //    for (int i = 0; i < 3; i++)
        //    {
        //        Viewport viewSide0 = GraphicsDevice.Viewport;
        //        viewSide0.X = (int)GraphicsDevice.Viewport.Width * 2 / 3;
        //        viewSide0.Y = i * (int)GraphicsDevice.Viewport.Height * 1 / 3; // Make 3 on the right side of the screen going down. 
        //        viewSide0.Width = (int)GraphicsDevice.Viewport.Width * 1 / 3;
        //        viewSide0.Height = (int)GraphicsDevice.Viewport.Height * 1 / 3;
        //        this.viewPortList.Add(viewSide0);
        //    }

        //    for (int i = 0; i < 2; i++)
        //    {
        //        Viewport viewSide0 = GraphicsDevice.Viewport;
        //        viewSide0.X = i * (int)GraphicsDevice.Viewport.Width * 1 / 3;
        //        viewSide0.Y = (int)GraphicsDevice.Viewport.Height * 2 / 3; // Make 3 on bottom of the screen going left. 
        //        viewSide0.Width = (int)GraphicsDevice.Viewport.Width * 1 / 3;
        //        viewSide0.Height = (int)GraphicsDevice.Viewport.Height * 1 / 3;
        //        this.viewPortList.Add(viewSide0);
        //    }
        //}       
    }
}
