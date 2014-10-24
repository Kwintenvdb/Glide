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
    class GUIButton : GameSprite
    {
        public bool IsClicked { get; set; }

        private MouseState m_MouseState, m_OldMouseState;
        private Rectangle m_HitRect;

        public GUIButton(string assetFile, RenderContext context)
            : base(assetFile, context)
        {
            IsClicked = false;
            m_HitRect = new Rectangle((int)Position.X, (int)Position.Y, Width, Height);
        }

        public void Update(RenderContext context)
        {
            // Reset the clicked boolean
            IsClicked = false;

            // Checks for one single left mouse button press
            m_MouseState = Mouse.GetState();            
            if (m_OldMouseState.LeftButton == ButtonState.Pressed
                && m_MouseState.LeftButton == ButtonState.Released)
            {
                var mousePoint = new Point(m_MouseState.X, m_MouseState.Y);
                // Fancy conversions
                m_HitRect.X = (int)ConvertUnits.ToDisplayUnits((float)Position.X);
                m_HitRect.Y = (int)ConvertUnits.ToDisplayUnits((float)Position.Y);

                if(m_HitRect.Contains(mousePoint))
                {
                    IsClicked = true;
                }
            }
            m_OldMouseState = m_MouseState;

            base.Update(context);
        }
    }
}

