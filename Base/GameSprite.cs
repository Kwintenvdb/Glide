using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace gameProject
{
    public class GameSprite
    {
        //Transform variables
        public Vector2 Position = Vector2.Zero, PivotPoint = Vector2.Zero;
        public Vector2 ObjectScale = Vector2.One;
        public float Rotation = 0;
        public Matrix WorldMatrix;

        //Texture Variables
        string m_AssetFile;
        public Texture2D m_Texture;
        public int Width
        {
            get
            {
                return m_Texture.Width;
            }
        }
        public int Height
        {
            get
            {
                return m_Texture.Height;
            }
        }

        //Drawing Variables
        public Color Color = Color.White;
        public SpriteEffects Effect = SpriteEffects.None;
        public Rectangle? DrawRect;

       
        public GameSprite(string assetFile,RenderContext context)
        {
            //Load the texture
            m_AssetFile = assetFile;
            if(context !=null)
                 m_Texture = context.Content.Load<Texture2D>(m_AssetFile);
        }

        public void Update(RenderContext context)
        {

            //Create The World Matrix for positioning and stuff
            WorldMatrix =
                Matrix.CreateTranslation(new Vector3(-PivotPoint, 0)) *
                Matrix.CreateScale(new Vector3(ObjectScale, 1)) *
                Matrix.CreateRotationZ(MathHelper.ToRadians(Rotation)) *
                Matrix.CreateTranslation(new Vector3(Position, 0));            
        }

        public virtual void DrawAsGUI(RenderContext context, float alpha = 1.0f,bool inWorld = false)
        {
            context.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            context.SpriteBatch.Draw(m_Texture, Position, DrawRect, Color.White * alpha, MathHelper.ToRadians(Rotation), Vector2.Zero, ObjectScale, Effect,1);
            context.SpriteBatch.End();  

            context.Graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        public virtual void DrawOverModel(RenderContext context, GameModel model)
        {
            var camera = context.Camera;
            Vector3 center = context.GraphicsDevice.Viewport.Project(new Vector3(model.Position,0), camera.Projection, camera.View, Matrix.Identity);
            Position = new Vector2(center.X - Width/2, center.Y - Height/2);

            context.SpriteBatch.Begin();
            context.SpriteBatch.Draw(m_Texture, Position, DrawRect, new Color(Color.R, Color.G, Color.B, 1.0f), MathHelper.ToRadians(Rotation), Vector2.Zero, ObjectScale, Effect, 1);
            context.SpriteBatch.End();

            context.Graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;
        }

        public void Translate(Vector2 translation)
        {
            Position = translation;
        }

        public void Translate(float x, float y)
        {
            Position = new Vector2(x, y);
        }

        public void Rotate(float rotation)
        {
            Rotation = rotation;    
        }

        public void Scale(float scale)
        {
            ObjectScale = new Vector2(scale, scale) ;
        }

        public void Scale(float x, float y)
        {
            ObjectScale = new Vector2(x, y);
        }


    }
}
