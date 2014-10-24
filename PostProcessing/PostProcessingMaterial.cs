using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace gameProject
{
    public abstract class PostProcessingMaterial : DrawableGameComponent
    {

        protected RenderTarget2D m_SceneTarget;
        protected RenderTarget2D m_RenderTarget1;
        protected RenderTarget2D m_RenderTarget2;

        protected SpriteBatch m_SpriteBatch;

        // Optionally displays one of the intermediate buffers used
        // by the bloom postprocess, so you can see exactly what is
        // being drawn into each rendertarget.
        public enum IntermediateBuffer
        {
            PreBloom,
            BlurredHorizontally,
            BlurredBothWays,
            FinalResult,
        }

        public IntermediateBuffer ShowBuffer
        {
            get { return showBuffer; }
            set { showBuffer = value; }
        }

        IntermediateBuffer showBuffer = IntermediateBuffer.FinalResult;

        public PostProcessingMaterial(Game game) :
            base(game)
        {

        }

        protected abstract override void LoadContent();
        protected abstract override void UnloadContent();
        public abstract override void Draw(GameTime gameTime);
        

        protected void DrawFullscreenQuad(Texture2D texture, RenderTarget2D renderTarget,
                                Effect effect, IntermediateBuffer currentBuffer)
        {
            GraphicsDevice.SetRenderTarget(renderTarget);

            DrawFullscreenQuad(texture,renderTarget.Width, renderTarget.Height,
                               effect, currentBuffer);
        }

        protected void DrawFullscreenQuad(Texture2D texture, int width, int height,
                                Effect effect, IntermediateBuffer currentBuffer)
        {
            // If the user has selected one of the show intermediate buffer options,
            // we still draw the quad to make sure the image will end up on the screen,
            // but might need to skip applying the custom pixel shader.
            if (showBuffer < currentBuffer)
            {
                effect = null;
            }

            m_SpriteBatch.Begin(0, BlendState.Opaque, null, null, null, effect);
            m_SpriteBatch.Draw(texture, new Rectangle(0, 0, width, height), Color.White);
            m_SpriteBatch.End();
        }
    }
}
