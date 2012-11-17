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
using Attempt_7;


namespace UnitTestGame
{
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
       
        Vector2  screenSize = new Vector2 (640,480);        
        SpriteFont Arial;      

        List<Viewport> viewPortList;

        List<Texture2D> textureList;    
      
        ImageAnalysis imageAnalysis;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            IsMouseVisible = true;

            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferredBackBufferHeight = 700;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
           
            CreateViewPorts();           

            imageAnalysis = new ImageAnalysis(this, screenSize, viewPortList);
            this.Components.Add(imageAnalysis);
            base.Initialize();
        }

        private void CreateViewPorts()
        {
            viewPortList = new List<Viewport>();

            Viewport mainView = GraphicsDevice.Viewport;
            mainView.Width = (int)GraphicsDevice.Viewport.Width * 2 / 3;
            mainView.Height = (int)GraphicsDevice.Viewport.Height * 2 / 3;
            viewPortList.Add(mainView);
            for (int i = 0; i < 3; i++)
            {
                Viewport ViewSide0 = GraphicsDevice.Viewport;
                ViewSide0.X = (int)GraphicsDevice.Viewport.Width * 2 / 3;
                ViewSide0.Y = i * (int)GraphicsDevice.Viewport.Height * 1 / 3;//make 3 on teh right side of the screen going down. 
                ViewSide0.Width = (int)GraphicsDevice.Viewport.Width * 1 / 3;
                ViewSide0.Height = (int)GraphicsDevice.Viewport.Height * 1 / 3;
                viewPortList.Add(ViewSide0);
            }
            for (int i = 0; i < 2; i++)
            {
                Viewport ViewSide0 = GraphicsDevice.Viewport;
                ViewSide0.X = i * (int)GraphicsDevice.Viewport.Width * 1 / 3;
                ViewSide0.Y = (int)GraphicsDevice.Viewport.Height * 2 / 3;//make 3 on bottom of the screen going left. 
                ViewSide0.Width = (int)GraphicsDevice.Viewport.Width * 1 / 3;
                ViewSide0.Height = (int)GraphicsDevice.Viewport.Height * 1 / 3;
                viewPortList.Add(ViewSide0);
            }
            //main view = 0, topright = 1, center right = 2, bottom right = 3, bottom left = 4, bottom center = 5
        }
       

        protected override void LoadContent()
        {
            textureList = new List<Texture2D>();

            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Arial = Content.Load<SpriteFont>("Arial");


            textureList.Add(Content.Load<Texture2D>("LineTest"));
            textureList.Add(Content.Load<Texture2D>("LineTest2"));
            textureList.Add(Content.Load<Texture2D>("LineTest3"));
            textureList.Add(Content.Load<Texture2D>("LineTest4"));
            textureList.Add(Content.Load<Texture2D>("LineTest5"));
            textureList.Add(Content.Load<Texture2D>("LineTest6"));
            textureList.Add(Content.Load<Texture2D>("LineTest7"));
            textureList.Add(Content.Load<Texture2D>("LineTest8"));
            textureList.Add(Content.Load<Texture2D>("LineTest9"));

        }
        

        private void updateMouse()
        {
            ////Vector3 mouseVector = Vector3.Zero;
            ////MouseState mouse = Mouse.GetState();//create a mouse state object

            ////mouseVector.Y = mouse.X;//moving the mosue left and right is the camera position x axis
            ////mouseVector.X = mouse.Y;//moving the mouse up and down is the camera position y axis
            ////mouseVector.Z = (int)mouse.ScrollWheelValue / 10;//mouse scroll wheel total is the camera position z axis

            ////location2.X = mouseVector.X;
            ////location2.Y = mouseVector.Y;
            ////location2.Z = mouseVector.Z;
        }


        protected override void UnloadContent()
        {

        }
        

        protected override void Update(GameTime gameTime)
        {
            
            base.Update(gameTime);
            
            getKeyBoard();
            imageAnalysis.SetRobotCameraView(textureList[textureNumber]);
            imageAnalysis.Update1(gameTime);
           // c = TextureTo2DArray(textureList[textureNumber], colorArray1D, ColorArrayB);
            //ColorArray = TextureTo2DArray(robotCameraView, colorArray1D, ColorArrayB);


            //if (showTextureOrAnalysis == 0)
            //{
            //    FindWhite(c);
            //    Hough(TrueFalseMapFindWhite);
            //    //trueFalseMap = AddLerpBool(trueFalseMap);
            //    //colorArrayto3DRectangle(c, VertexArray, buffer1);
            //    UpdateBoolMapto3DRectangle(TrueFalseMapFindWhite, VertexArray, buffer1);
            //}
            //else
            //{
            //    FindWhite(c);
            //    colorArrayto3DRectangle(c, VertexArray, buffer1);
            //}
            //updateMouse();
            //Array.Clear(trueFalseMap, 0, trueFalseMap.Length);
            //Array.Clear(c, 0, c.Length);
            // camera.Update(gameTime );
        }
        
        int textureNumber = 1;
       
        private void getKeyBoard()
        {
            KeyboardState keyboardState = Keyboard.GetState();
            if(keyboardState .IsKeyDown ( Keys.Escape )){
                this.Exit();
            }           

            if (keyboardState.IsKeyDown(Keys.F1))
            {

                textureNumber = 0;
            }
            if (keyboardState.IsKeyDown(Keys.F2))
            {

                textureNumber = 1;
            } if (keyboardState.IsKeyDown(Keys.F3))
            {

                textureNumber = 2;
            } if (keyboardState.IsKeyDown(Keys.F4))
            {

                textureNumber = 3;
            } if (keyboardState.IsKeyDown(Keys.F5))
            {

                textureNumber = 4;
            } if (keyboardState.IsKeyDown(Keys.F6))
            {

                textureNumber = 5;
            } if (keyboardState.IsKeyDown(Keys.F7))
            {

                textureNumber = 6;
            } if (keyboardState.IsKeyDown(Keys.F8))
            {

                textureNumber = 7;
            } if (keyboardState.IsKeyDown(Keys.F9))
            {

                textureNumber = 8;
            }
        }

      
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            GraphicsDevice.Viewport = viewPortList[0];

            // Set the view port to the main view and draw the main view
            this.GraphicsDevice.Viewport = this.viewPortList[4]; // Main View  is 2/3 of the screen screen
            this.spriteBatch.Begin(); // Start the 2D drawing        
            this.spriteBatch.Draw(textureList[textureNumber], new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.White); // Draw the MainView
            this.spriteBatch.End(); // Stop drawing. 

            // Change viewports to the topLeft view port of the window. Draw the robot view. 
            //this.GraphicsDevice.Viewport = this.viewPortList[1];
            //this.spriteBatch.Begin(); // Start the 2D drawing      
            //this.spriteBatch.Draw(textureList[textureNumber], new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height), Color.Yellow   ); // Draw the robotView
            //this.spriteBatch.End(); // Stop drawing.
            ////drawAnalysis();
            //GraphicsDevice.Viewport = viewPortList[1];
            //drawText();
            //GraphicsDevice.Viewport = viewPortList[2];
            //drawImageToAnalze(viewPortList[2].Width, viewPortList[2].Height);
            base.Draw(gameTime);
        }
      
    }
}
