#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Media;

using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
#endregion

namespace gameProject
{
    public class RenderContext
    {
        public SpriteBatch SpriteBatch { get; set; } //For sprite/Particle rendering
        public GraphicsDevice GraphicsDevice { get; set; } //for states setting
        public GraphicsDeviceManager Graphics; //For Viewport and stuff
        public GameTime GameTime { get; set; }
        public BaseCamera Camera; //for rendering 3D
        public World World; //for creating rigidBodies
        public Game1 Game; //for easy access to restarting 
        public ContentManager Content; //loading of content
        public Vector2 ViewPortSize;
       
        public bool GameActive = false;
        public Player Player;
        public GamePadState PadState;
        public GameComponentCollection Component;
    }

    public class Game1 : Game
    {
        // General
        GraphicsDeviceManager m_Graphics;
        public SpriteBatch SpriteBatch;
        RenderContext m_RenderContext;

        // Physics
        World m_FarseerWorld;
        Vector2 m_Gravity = new Vector2(0, -9.8f);

        //GameScene
        public GameScene MainScene;

        // Pause menu && startMenu
        public MenuManager m_MenuManager;
        MenuManager m_LostMenu;

        public bool PausedGame = true;
        KeyboardState m_OldState;

        //BloomEffect
        BloomPostProcessing m_BloomPP;
        
        WindParticleSystem m_Particles;

        //Testing of Sprites

        Texture2D m_Texture;
        GameSprite m_Sprite;
        
        public Game1()
            : base()
        {
            //Set Screen size baed on settings
            var screenData = XmlLoader.Load<WorldSettings>("WorldSettings");
            m_Graphics = new GraphicsDeviceManager(this);
          
            if (screenData.Fullscreen)
            {
                m_Graphics.ToggleFullScreen();
                m_Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                m_Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            }
            else
            {
                m_Graphics.PreferredBackBufferWidth = screenData.ScreenWidth;
                m_Graphics.PreferredBackBufferHeight = screenData.ScreenHeight;
            }

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here         

            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch = new SpriteBatch(GraphicsDevice);

            //Create a new FarseerWorld to allow physics calculations
            m_FarseerWorld = new World(m_Gravity);
            ConvertUnits.SetDisplayUnitToSimUnitRatio(1.0f);

            //Create a renderContext that holds important information that we can easily access
            m_RenderContext = new RenderContext();
            m_RenderContext.World = m_FarseerWorld;
            m_RenderContext.SpriteBatch = SpriteBatch;
            m_RenderContext.GraphicsDevice = m_Graphics.GraphicsDevice;
            m_RenderContext.Content = Content;
            m_RenderContext.Game = this;
            m_RenderContext.Camera = new BaseCamera(m_RenderContext);
            m_RenderContext.ViewPortSize = new Vector2((float)m_Graphics.PreferredBackBufferWidth, (float)m_Graphics.PreferredBackBufferHeight);
            m_RenderContext.Graphics = m_Graphics;
            m_RenderContext.Component = Components;

            //René's Controller problems
            if (System.Environment.UserName == "Rene") m_RenderContext.PadState = GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular);
            else m_RenderContext.PadState = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            //MENU
            //***************
            m_MenuManager = new MenuManager(m_RenderContext.ViewPortSize / 2);
            m_MenuManager.AddItem(new GUIButton("Textures/GUI/Buttons/StartNormal", "Textures/GUI/Buttons/StartOnHover", ButtonName.Start, m_RenderContext));
            m_MenuManager.AddItem(new GUIButton("Textures/GUI/Buttons/ExitNormal", "Textures/GUI/Buttons/ExitOnHover", ButtonName.Exit, m_RenderContext));
            m_MenuManager.AddItem(new GUIButton("Textures/GUI/Buttons/RestartNormal", "Textures/GUI/Buttons/RestartOnHover", ButtonName.Restart, m_RenderContext));
            m_MenuManager.AddItem(new GUIButton("Textures/GUI/Buttons/ToggleFullscreenNormal", "Textures/GUI/Buttons/ToggleFullscreenOnHover", ButtonName.Fullscreen, m_RenderContext));

            //Set the splash screen and hit screen
            m_MenuManager.BackgroundScreen = new GameSprite("Textures/GUI/BackgroundStartScreen", m_RenderContext);
            m_MenuManager.SplashScreen = new GameSprite("Textures/GUI/DAEScreen_GUI", m_RenderContext);
            m_MenuManager.Initialize(m_RenderContext);           

            m_LostMenu = new MenuManager(m_RenderContext.ViewPortSize / 2);
            m_LostMenu.AddItem(new GUIButton("Textures/GUI/Buttons/ExitNormal", "Textures/GUI/Buttons/ExitOnHover", ButtonName.Exit, m_RenderContext));
            m_LostMenu.AddItem(new GUIButton("Textures/GUI/Buttons/RestartNormal", "Textures/GUI/Buttons/RestartOnHover", ButtonName.Restart, m_RenderContext));
            m_LostMenu.Initialize(m_RenderContext);

            //SOUND
            //*****
            SoundManager.AddSound("Background", m_RenderContext);
           // SoundManager.AddSound("ButtonClick", m_RenderContext);

            //POST PROCESSING
            //***************
            m_BloomPP = new BloomPostProcessing(this);
            m_RenderContext.Component.Add(m_BloomPP);

            m_Particles = new WindParticleSystem(m_RenderContext, 7);
           // m_RenderContext.Component.Add(m_Particles);

            //GAMESCENE
            //*********

            MainScene = new GameScene();            

            m_Texture = Content.Load<Texture2D>("Textures/Models/D_Default");
            m_Sprite = new GameSprite("Textures/Models/D_Default", m_RenderContext);
            m_Sprite.Scale(0.5f);
        }

        protected override void UnloadContent()
        {
            m_RenderContext.Content.Unload();
        }

        protected override void Update(GameTime gameTime)
        {
            //SET GAMEPAD STATE
            //*****************
            if (System.Environment.UserName == "Rene") m_RenderContext.PadState = GamePad.GetState(PlayerIndex.Two, GamePadDeadZone.Circular);
            else m_RenderContext.PadState = GamePad.GetState(PlayerIndex.One, GamePadDeadZone.Circular);            

            m_RenderContext.GameTime = gameTime;
            m_RenderContext.GameActive = MainScene.isActive;
            m_RenderContext.ViewPortSize = new Vector2(m_Graphics.GraphicsDevice.Viewport.Width, m_Graphics.GraphicsDevice.Viewport.Height);

            //update our scene    

            PauseGame();
            if (!PausedGame && MainScene.isActive)
            {
                 //GAMESCENE UPDATE
                 //****************
                 MainScene.Update(m_RenderContext);
                //update the farseer world
                m_FarseerWorld.Step((float)m_RenderContext.GameTime.ElapsedGameTime.Milliseconds / 100);

                Vector2 Where = m_RenderContext.Player.Position;
                m_Particles.AddParticles(Where);

                //LOSE SCREEN UPDATE
                if (MainScene.hasLost)
                    m_LostMenu.Update(m_RenderContext);
            }
            else
                m_MenuManager.Update(m_RenderContext);

            m_Sprite.Update(m_RenderContext);

            base.Update(gameTime);
        }

        public Rectangle? DrawRect;
        protected override void Draw(GameTime gameTime)
        {
            
            GraphicsDevice.Clear(Color.CornflowerBlue);
            
            //DRAW MAIN SCENE AND DRAW POSTPROCESSING
            //***************************************
            if (!PausedGame && MainScene.isActive)
            {
                m_BloomPP.BeginDraw();
                m_BloomPP.Visible = true;

                MainScene.Draw(m_RenderContext);

                if (MainScene.hasLost)
                {
                    m_LostMenu.Draw(m_RenderContext);
                    TextRenderer.DrawText("You Died! Press Escape to go back to the menu", m_RenderContext.ViewPortSize.X * 0.30f, m_RenderContext.ViewPortSize.Y * 0.1f, Color.White, m_RenderContext);
                }
            }                
            else
            {
                //No post processing in the menu
                m_BloomPP.Visible = false;
                m_MenuManager.Draw(m_RenderContext);

            }

            //if (m_RenderContext.Player != null)
            //{

            //    Vector2 translation = -m_RenderContext.Player.Position + m_RenderContext.ViewPortSize / 2;
            //    Matrix View = Matrix.CreateTranslation(new Vector3(translation, 0));
            //    SpriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, View);
            //    // SpriteBatch.Draw(m_Texture, new Vector2(150, 450), Color.Orange);


            //    m_Sprite.Color = Color.Green;
            //    //m_Sprite.Draw(m_RenderContext);

            //    m_Sprite.Position = new Vector2(150, 150);
            //    m_Sprite.Color = Color.Red;
            //    m_RenderContext.SpriteBatch.Draw(m_Sprite.m_Texture, m_Sprite.Position, null, Color.White, MathHelper.ToRadians(0), Vector2.Zero, Vector2.One, SpriteEffects.None, 1);

            //    m_Sprite.Position = new Vector2(660, 150);
            //    //m_Sprite.Position = ConvertUnits.ToDisplayUnits(m_Sprite.Position);
            //    m_Sprite.Draw(m_RenderContext, 1.0f, true);
            //}

            
            base.Draw(gameTime);
        }

        void PauseGame()
        {
            // Pause menu
            var newState = Keyboard.GetState();
            if (newState.IsKeyDown(Keys.Escape) && !m_OldState.IsKeyDown(Keys.Escape))
            {

                if (!PausedGame)
                    SoundManager.Pause("Background");
                else
                    SoundManager.Play("Background", true);
                PausedGame = !PausedGame;
            }
            m_OldState = newState;
        }

        public void RestartGame()
        {
            //Clean up previous level
            MainScene.CleanUp();

            //Reinitialize scene
            MainScene.Initialize(m_RenderContext);

            PausedGame = false;
        }

        }
}
