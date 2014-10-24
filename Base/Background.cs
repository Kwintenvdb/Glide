using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace gameProject
{
    //Parallaxing Background
   public class Background :GameSprite
    {
        private Vector2 m_Offset; //offset to start drawing our image
        private Vector2 m_Speed; //speed of parallaxing effect
        Viewport m_ViewPort;
       public bool DrawBefore3D;

        public float Zoom;

        private Rectangle? m_Rect
        {
            get { return new Rectangle((int)m_Offset.X, (int)m_Offset.Y, (int)(m_ViewPort.Width / Zoom), (int)(m_ViewPort.Height / Zoom)); }
        }


        public Background(string t, Vector2 s, float z,bool bFront, RenderContext context):
            base("Textures/Background/"  + t,context)
        {
            //PivotPoint.Y -= Height/2;
            m_Speed = s;
            Zoom = z;
            m_ViewPort = context.Graphics.GraphicsDevice.Viewport; ;
            DrawBefore3D = bFront;
        }

        public void Update(RenderContext context, Vector2 playerDir)
        {
            //calculate distance to move image
            Vector2 dist = playerDir * m_Speed * (float)context.GameTime.ElapsedGameTime.TotalSeconds;
            m_Offset += dist;
            m_ViewPort = context.Graphics.GraphicsDevice.Viewport;
            //DrawRect = m_Rect;
            base.Update(context);
        }

        public override void DrawAsGUI(RenderContext context, float alpha = 1.0f,bool inWorld = false)
        {
            context.SpriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.LinearWrap, null, null, null, WorldMatrix);
            //context.SpriteBatch.Begin();
            context.SpriteBatch.Draw(m_Texture, new Vector2(m_ViewPort.X,m_ViewPort.Y), m_Rect, Color.White, 0, PivotPoint, Zoom, SpriteEffects.None, 1);
            context.SpriteBatch.End();
            
        }
          

    }
}
