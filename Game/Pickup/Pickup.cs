using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics.Common;
using FarseerPhysics.Factories;
using FarseerPhysics.Collision;
using FarseerPhysics;
using FarseerPhysics.Dynamics;

using System.IO;
namespace gameProject
{
    //Add new name of pickup here
    public enum PickupName
    {
        Health,
        SpeedUp,
        SpeedDown,
        WindUp,
        WindDown,
        WindVulcano,
        Tree
    }

    public abstract class Pickup : GameModel
    {
        public bool isPickedUp = false;

        protected Player m_Receiver = null;

        public Pickup(Model model, Body body, TextureData textureData)
            : base(string.Empty)
        {
            base.Name = "Pickup";
            base.Model = model;
            base.RigidBody = body;
            base.RigidBody.OnCollision += RigidBody_OnCollision;
            base.DiffuseTexture = textureData.Diffuse;
            base.NormalTexture = textureData.Normal;
            base.SpecularTexture = textureData.Specular;

            Rotate(90, 0, 0);

        }
        private bool RigidBody_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            var collider = (GameModel)fixtureB.Body.UserData;

            if (collider != null && collider.Name == "Player")
            {
                m_Receiver = (Player)collider;
                isPickedUp = true;
            }

            return false;
        }

        abstract public void OnPickup();
        public abstract void Create(RenderContext context);
    }

    public class HealthPickup : Pickup
    {
        public HealthPickup(Model model, Body body, TextureData textureData)
            : base(model, body, textureData)
        {
            RigidBody.UserData = this;
        }
        public override void Create(RenderContext context)
        {
            throw new NotImplementedException();
        }

        public override void OnPickup()
        {
            m_Receiver.AddHealth();
            Console.WriteLine("Picked up Health");
            RigidBody.Dispose();

        }
    }

    public class SpeedUpPickup : Pickup
    {
        public SpeedUpPickup(Model model, Body body, TextureData textureData)
            : base(model, body, textureData)
        {
            RigidBody.UserData = this;
        }
        public override void Create(RenderContext context)
        {
            throw new NotImplementedException();
        }

        public override void OnPickup()
        {
            m_Receiver.IncreaseSpeed();
            Console.WriteLine("Picked up Speed");
            RigidBody.Dispose();
        }
    }

    public class SpeedDownPickup : Pickup
    {
        public SpeedDownPickup(Model model, Body body, TextureData textureData)
            : base(model, body, textureData)
        {
            RigidBody.UserData = this;
        }
        public override void Create(RenderContext context)
        {
            throw new NotImplementedException();
        }

        public override void OnPickup()
        {
            m_Receiver.DecreaseSpeed();
            Console.WriteLine("Picked up Decrease Speed");
            RigidBody.Dispose();
        }
    }

    public class WindDownPickup : Pickup
    {
        public WindDownPickup(Model model, Body body, TextureData textureData)
            : base(model, body, textureData)
        {
            RigidBody.UserData = this;
        }
        public override void Create(RenderContext context)
        {
            throw new NotImplementedException();
        }

        public override void OnPickup()
        {
            m_Receiver.ReduceWind();
            Console.WriteLine("Picked up Decrease Wind");
            RigidBody.Dispose();
        }
    }

    public class WindUpPickup : Pickup
    {
        public WindUpPickup(Model model, Body body, TextureData textureData)
            : base(model, body, textureData)
        {
            RigidBody.UserData = this;
        }
        public override void Create(RenderContext context)
        {
            throw new NotImplementedException();
        }

        public override void OnPickup()
        {
            m_Receiver.IncreaseWind();
            Console.WriteLine("Picked up Increase Wind");
            RigidBody.Dispose();
        }
    }

    public class WindVulcano : Pickup
    {
        WindParticleSystem m_windParticles;

        GameSprite m_WindIndicator;

        public WindVulcano(Model model, Body body, TextureData textureData)
            : base(model, body, textureData)
        {
            RigidBody.UserData = this;
            //base.Name = "WindVulcano";
        }

        public override void Create(RenderContext context)
        {
            m_windParticles = new WindParticleSystem(context, 3);
            m_windParticles.Initialize();
            context.Component.Add(m_windParticles);
            
            m_WindIndicator = new GameSprite("Textures/Particles/Smoke", context);
            //m_WindIndicator = new GameSprite("Textures/Models/D_Default", context);
        }

        public override void OnPickup()
        {
            m_Receiver.PickUpFromGround = true;
            m_Receiver.PullDirection = 1.0f;
            isPickedUp = false;
        }

        public override void Update(RenderContext context = null)
        {
            m_windParticles.SetProjectionSettings(context, this);
            m_windParticles.AddParticles(new Vector2(0,0));
            m_windParticles.Update(context.GameTime);
            m_WindIndicator.Update(context);

            base.Update(context);
        }

        public override void Draw(RenderContext context)
        {
            //Draw it as a sprite

            //m_WindIndicator.DrawOverModel(renderContext, this);
            m_windParticles.Draw(context.GameTime);

            context.Graphics.GraphicsDevice.DepthStencilState = DepthStencilState.Default;

            base.Draw(context);
        }
    }
}
