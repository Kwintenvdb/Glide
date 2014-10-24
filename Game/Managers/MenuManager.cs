using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace gameProject
{
    public enum ButtonName : int
    {
        Start = 0,
        Exit,
        Restart,
        Fullscreen
    };

    public class MenuManager
    {
        Dictionary<ButtonName,GUIButton> m_MenuItems;

        Vector2 m_Position = Vector2.Zero;

        public GameSprite SplashScreen;
        public GameSprite BackgroundScreen;
        

        //SplashScreen settings
        bool m_bIsStarting = false;
        float m_StartupTime = 25.0f; // time the splash screen stays
        float m_CurrentTime = 0.0f;
        float m_AlphaValue = 255.0f; // start at max opacity
        float m_FadeIncrement = 4f;
        double m_FadeDelay = .035f;
        private Vector2 m_ViewportDimensions;

        GamePadState m_PadOld;
        //Button selection
        private int m_SelectedBtn = 0;       

        
        public MenuManager(Vector2 position)
        {
            m_MenuItems = new Dictionary<ButtonName,GUIButton>();
            m_Position = position;                            
        }

        public void Initialize(RenderContext context)
        {
            //SplashScreen should always fill the screen of thw game correctly
            if (SplashScreen != null )
            {
                m_bIsStarting = true;
               
                SplashScreen.Scale(context.ViewPortSize.X / SplashScreen.Width, context.ViewPortSize.Y / SplashScreen.Height);
            }
            if(BackgroundScreen != null)
            {
                BackgroundScreen.Scale(context.ViewPortSize.X / BackgroundScreen.Width, context.ViewPortSize.Y / BackgroundScreen.Height);
            }
        }

        public void AddItem(GUIButton item)
        {
            if (item != null)
                m_MenuItems.Add(item.BtnName, item);
        }

        public void Update(RenderContext context)
        {
            m_ViewportDimensions.X = context.GraphicsDevice.Viewport.Width;
            m_ViewportDimensions.Y = context.GraphicsDevice.Viewport.Height;

            float deltaTime = (float)context.GameTime.ElapsedGameTime.Milliseconds / 100.0f;

            //update the buttons
            if (m_CurrentTime > m_StartupTime)
            {
                foreach (KeyValuePair<ButtonName, GUIButton> button in m_MenuItems)
                {
                    button.Value.Update(context);
                }

                ButtonBehaviour(context);
            }

            if (SplashScreen != null)
            {
                m_CurrentTime += deltaTime;
                if (m_CurrentTime > m_StartupTime)
                    FadeOut(deltaTime);
            }
        }

        //Add Button behaviour here
        void ButtonBehaviour(RenderContext context)
        {
            //With keyboard and GamePad
            foreach (KeyValuePair<ButtonName, GUIButton> button in m_MenuItems)
            {
                button.Value.SelectButton(false);
            }

            //Only happens when gamePad is connected
            if (context.PadState.IsConnected)
            {
                if (context.PadState.IsButtonDown(Buttons.DPadDown) && !m_PadOld.IsButtonDown(Buttons.DPadDown))
                {
                    m_SelectedBtn++;
                    if (m_SelectedBtn >= 4) m_SelectedBtn = 0;
                }
                else if (context.PadState.IsButtonDown(Buttons.DPadUp) && !m_PadOld.IsButtonDown(Buttons.DPadUp))
                {
                    m_SelectedBtn--;
                    if (m_SelectedBtn < 0) m_SelectedBtn = 3;
                }

                m_PadOld = context.PadState;

                if (m_MenuItems.ContainsKey((ButtonName)m_SelectedBtn))
                    m_MenuItems[(ButtonName)m_SelectedBtn].SelectButton(true);
            }

         

            //Button behaviors wen clicked
            if (m_MenuItems.ContainsKey(ButtonName.Start) && m_MenuItems[ButtonName.Start].IsClicked)
            {
                //see if a game is currently active
                if (!context.GameActive)
                    context.Game.RestartGame();

                context.Game.PausedGame = false;
            }
            else if (m_MenuItems.ContainsKey(ButtonName.Exit)&&m_MenuItems[ButtonName.Exit].IsClicked)
            {
                context.Game.MainScene.SaveScore();
                context.Game.Exit();
            }
            else if (m_MenuItems.ContainsKey(ButtonName.Restart) && m_MenuItems[ButtonName.Restart].IsClicked)
            {
                context.Game.MainScene.SaveScore();
                context.Game.RestartGame();
                context.Game.PausedGame = false;
            }
            else if (m_MenuItems.ContainsKey(ButtonName.Fullscreen) && m_MenuItems[ButtonName.Fullscreen].IsClicked)
            {
                BackgroundScreen.Scale(1);
                context.Graphics.ToggleFullScreen();

                if (!context.Graphics.IsFullScreen)
                {
                    //context.Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
                    //context.Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
                    context.Graphics.PreferredBackBufferWidth = 1280;
                    context.Graphics.PreferredBackBufferHeight = 720;
                    BackgroundScreen.Scale(context.ViewPortSize.X / BackgroundScreen.Width, context.ViewPortSize.Y / BackgroundScreen.Height);
                    
                }
                context.Graphics.ApplyChanges();
            }
        }

        void FadeOut(float deltaTime)
        {
            m_FadeDelay -= deltaTime;

            if (m_FadeDelay <= 0)
            {
                m_FadeDelay = .035f;
                m_AlphaValue -= m_FadeIncrement;
            }

            if (m_AlphaValue <= 0)
            {
                m_bIsStarting = false;
            }
        }

        public void Draw(RenderContext context)
        {        
            if(BackgroundScreen!= null)
                BackgroundScreen.DrawAsGUI(context);

            int i = 0;
            if (!m_bIsStarting)
            {
                m_CurrentTime = m_StartupTime * 2;
                foreach (KeyValuePair<ButtonName, GUIButton> button in m_MenuItems)
                {                    
                    float xPos = context.GraphicsDevice.Viewport.Width / 2 - button.Value.NewWidth / 2;
                    float yPos = context.GraphicsDevice.Viewport.Height / 2 + i * button.Value.NewHeight - 100;                
                    button.Value.Translate(xPos, yPos);
                    button.Value.DrawAsGUI(context);
                    i++;
                }
            }

            if (m_bIsStarting && m_AlphaValue > 0)
            {
                if (SplashScreen != null)
                    SplashScreen.DrawAsGUI(context, m_AlphaValue / 255.0f);
            }
        }
    }
}
