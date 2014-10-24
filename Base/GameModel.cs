using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

using System.IO;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gameProject
{
    public struct TextureData
    {
        public Texture2D Diffuse;
        public Texture2D Normal;
        public Texture2D Specular;
    }


    public class GameModel
    {
        //Used for Collision
        public Body RigidBody;
        public string Name = "";
        public Vector2 Offset = Vector2.Zero;

        //Distance drawn from Camera
        protected float Depth = 0.0f;

        //Transform Variables
        public Vector2 Position
        {
            get { return RigidBody.Position; }
            set { 
                RigidBody.Position = new Vector2(value.X,value.Y); }
        }
        public Vector3 ObjectScale = Vector3.One;
        public Quaternion Rotation;
        protected Matrix WorldMatrix;
        
        //Asset Variables
        protected string m_AssetFile;

        protected Texture2D DiffuseTexture;
        protected Texture2D NormalTexture;
        protected Texture2D SpecularTexture;

        protected Model Model;

        protected float Alpha = 1.0f;



        public GameModel(string assetFile = "Default")
        {
            if (assetFile == "")
                assetFile = "Default";
            m_AssetFile = assetFile;
            
        }

        public void Delete()
        {
            Model = null;
            RigidBody = null;
        }

       public virtual void Initialize(RenderContext context)
        {

            //m_Effect = context.Content.Load<Effect>("Effect/BasicShader");
           //Create an empty rigidBody if one doesn't exist
            if(RigidBody == null)
            {
                RigidBody = BodyFactory.CreateBody(context.World);
            }

           //Load Content if needed
            LoadContent(context.Content);
        }

        void LoadContent(ContentManager contentManager)
        {
            //if model doesn't exist load it
            if (Model == null)
            {               
                if (!File.Exists("Content/Model/m_" + m_AssetFile + ".xnb"))
                {
                    Model = contentManager.Load<Model>("Model/Default");
                }
                else Model = contentManager.Load<Model>("Model/m_" +m_AssetFile);
            }           

            //if(texture Doesn't exist load it
            if (DiffuseTexture == null)
                DiffuseTexture = contentManager.Load<Texture2D>("Textures/Models/D_" + m_AssetFile);
            

            if (NormalTexture == null)
                NormalTexture = contentManager.Load<Texture2D>("Textures/Models/D_Default");

            if (SpecularTexture == null)
                SpecularTexture = contentManager.Load<Texture2D>("Textures/Models/D_Default");
        }

        public virtual void Update(RenderContext context = null)
        {
            Vector3 pos = new Vector3(Position + Offset, Depth);
            WorldMatrix = Matrix.CreateFromQuaternion(Rotation) *
                          Matrix.CreateScale(ObjectScale) *
                          Matrix.CreateTranslation(pos);
        }

        public virtual void Draw(RenderContext renderContext)
        {
            var transforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(transforms);
                
            foreach (ModelMesh mesh in Model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.View = renderContext.Camera.View;
                    effect.Projection = renderContext.Camera.Projection;
                    effect.World = transforms[mesh.ParentBone.Index] * WorldMatrix;
                    effect.GraphicsDevice.BlendState = BlendState.NonPremultiplied;
                    effect.PreferPerPixelLighting = true;
                    effect.TextureEnabled = true;
                    effect.Texture = DiffuseTexture;
                    effect.SpecularColor = new Vector3(0);
                    effect.SpecularPower = 100;
                    effect.Alpha = Alpha;
                }

                mesh.Draw();
            }

        }

        public void Rotate(float pitch, float yaw, float roll)
        {
            Rotation = Quaternion.CreateFromYawPitchRoll(MathHelper.ToRadians(yaw), MathHelper.ToRadians(pitch), MathHelper.ToRadians(roll));
            RotateBody(roll);
        }
        public void RotateBody(float roll)
        {
            RigidBody.Rotation = MathHelper.ToRadians(roll);
        }

        public void Scale(float scale)
        {
            ObjectScale = new Vector3(scale, scale, scale);
        }

        public void Scale(float x,float y, float z)
        {
            ObjectScale = new Vector3(x, y, z);
        }

        public void Translate(float x, float y)
        {
            Position = new Vector2(x,y);
        }

        public void Translate(Vector2 translation)
        {
            Position = translation;
        }

    }
}
