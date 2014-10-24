using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace gameProject
{
    class BackgroundManager
    {
        List<Background> m_Backgrounds = new List<Background>();

        public BackgroundManager(RenderContext context)
        {
        }

        public void AddBackgound(Background bg)
        {
            m_Backgrounds.Add(bg);
        }

        public void Update(RenderContext context,Vector2 dir)
        {
            foreach (Background bg in m_Backgrounds)
                bg.Update(context,dir);
        }

        public void DrawBefore(RenderContext context)
        {
            foreach (Background bg in m_Backgrounds)
            {
                if(bg.DrawBefore3D)
                     bg.DrawAsGUI(context);

            }
        }

        public void DrawAfter(RenderContext context)
        {
            foreach (Background bg in m_Backgrounds)
            {
                if (!bg.DrawBefore3D)
                    bg.DrawAsGUI(context);

            }
        }
    }
}
