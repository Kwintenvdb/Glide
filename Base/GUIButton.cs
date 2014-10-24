using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace gameProject
{
    public class GUIButton : GameSprite
    {
        public bool IsClicked { get; set; }
        public int NewWidth;    // cannot edit the sprites width and height values, use these instead
        public int NewHeight;

        private float m_Scalefactor = 0.5f;

        private MouseState m_MouseState, m_OldMouseState;
        private KeyboardState m_OldState;
        private Rectangle m_HitRect;
        public ButtonName BtnName;
        private string m_NormalTextureFile;
        private string m_HoverTextureFile;
        private Texture2D m_NormalTexture;
        private Texture2D m_HoverTexture;

        private bool m_IsSelected = false;

        public GUIButton(string assetFile, string hoverTextureFile, ButtonName name, RenderContext context)
            : base(assetFile, context)
        {
            Scale(m_Scalefactor);

            m_NormalTextureFile = assetFile;
            m_HoverTextureFile = hoverTextureFile;
            m_NormalTexture = m_Texture;
            m_HoverTexture = context.Content.Load<Texture2D>(m_HoverTextureFile);

            BtnName = name;

            float w = (float)Width * m_Scalefactor;
            float h = (float)Height * m_Scalefactor;
            NewWidth = (int)w;
            NewHeight = (int)h;

            IsClicked = false;
            m_HitRect = new Rectangle((int)Position.X, (int)Position.Y, NewWidth, NewHeight);

        }

        public new void Update(RenderContext context)
        {
            // Reset the clicked boolean
            IsClicked = false;

            //Get States
            m_MouseState = Mouse.GetState();
            KeyboardState keystate = Keyboard.GetState();
            var padState = context.PadState;

            // Fancy conversions
            var mousePoint = new Point(m_MouseState.X, m_MouseState.Y);
            m_HitRect.X = (int)ConvertUnits.ToDisplayUnits((float)Position.X);
            m_HitRect.Y = (int)ConvertUnits.ToDisplayUnits((float)Position.Y);

            //With Mouse( when no PadState is connected
            if (!padState.IsConnected && m_HitRect.Contains(mousePoint))
            {
                //On Mouse Click
                if (m_OldMouseState.LeftButton == ButtonState.Pressed && m_MouseState.LeftButton == ButtonState.Released)
                {
                    IsClicked = true;
                    SoundManager.Play("ButtonClick");
                }
               

                SelectButton(true);
            }
  
            
            //Changing of texture of selected button
            if(m_IsSelected)
            {
                if (padState.IsButtonDown(Buttons.Start) || keystate.IsKeyDown(Keys.Enter) && m_OldState.IsKeyUp(Keys.Enter))
                    IsClicked = true;  
                m_Texture = m_HoverTexture;
            }
            else if (!m_HitRect.Contains(mousePoint) || !m_IsSelected)
            {
                m_Texture = m_NormalTexture;
            }
         
            //Set Old states
            m_OldMouseState = m_MouseState;
            m_OldState = keystate;

            //SoundManager.Stop("ButtonClick");

            base.Update(context);
        }

        public void SelectButton(bool select)
        {
           
                
            m_IsSelected = select;
           
        }
    }
}

