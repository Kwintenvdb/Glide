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
    public class MenuManager
    {
        Dictionary<string,GUIButton> m_MenuItems;

        Vector2 m_Position = Vector2.Zero;

        public GameSprite SplashScreen;

        //SplashScreen settings
        bool m_bIsStarting = true;
        float m_StartupTime = 25.0f; // time the splash screen stays
        float m_CurrentTime = 0.0f;
        float m_AlphaValue = 255.0f; // start at max opacity
        float m_FadeIncrement = 4f;
        double m_FadeDelay = .035f;
        
        public MenuManager(Vector2 position)
        {
            m_MenuItems = new Dictionary<string,GUIButton>();
            m_Position = position;                  
           
        }

        public void Initialize(RenderContext context)
        {
            //SplashScreen should always fill the screen of thw game correctly
            SplashScreen.Scale(context.ViewPortSize.X / SplashScreen.Width, context.ViewPortSize.Y / SplashScreen.Height);
        }

        public void AddItem(GUIButton item)
        {
            if (item != null)
                m_MenuItems.Add(item.Name,item);
        }

        public void Update(RenderContext context)
        {
            float deltaTime = (float)context.GameTime.ElapsedGameTime.Milliseconds / 100.0f;
            
            foreach (KeyValuePair<string, GUIButton> button in m_MenuItems)
            {
                button.Value.Update(context);                
            }
            ButtonBehaviour(context);

            m_CurrentTime += deltaTime;
            if (m_CurrentTime > m_StartupTime)
                 FadeOut(deltaTime);

        }


        //Add Button behaviour here
        void ButtonBehaviour(RenderContext context)
        {        
            if (m_MenuItems["Start"].IsClicked)
            {
                //see if a game is currently active
                if (!context.GameActive)
                    context.Game.RestartGame();

                context.Game.PausedGame = false;
            }
            else if (m_MenuItems["Exit"].IsClicked)
            {
                context.Game.Exit();
            }
            else if (m_MenuItems["Restart"].IsClicked)
            {
                context.Game.RestartGame();
                context.Game.PausedGame = false;
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


        }

        
        public void Draw(RenderContext context)
        {
            context.SpriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied);

            int i = 0;
            foreach (KeyValuePair<string, GUIButton> button in m_MenuItems)
            {

                 button.Value.Translate(m_Position.X - button.Value.Width / 2, m_Position.Y + i * (button.Value.Height * 1.25f) );
                 button.Value.Draw(context);
                 i++;
            }

            if (m_bIsStarting && m_AlphaValue > 0)                         
                 SplashScreen.Draw(context,m_AlphaValue/255.0f);

            context.SpriteBatch.End();
        }
    }
}
