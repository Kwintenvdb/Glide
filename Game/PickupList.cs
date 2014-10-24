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


namespace gameProject
{
    //Add new name of pickup here
    public enum PickupName
    {
        Health
    }

    //PickupPrefab stores the model and the body of the pickup
    public class PickupPrefab
    {
        Model m_Model = null;
        Body m_Body = null;

        public PickupName Name;
        public Model Model
        {
            get
            {
                return m_Model;
            }
        }

        public Body Body
        {
            get
            {
                return m_Body.DeepClone();
            }
        }

        public PickupPrefab(RenderContext context, PickupName name, string modelFile)
        {
            if (m_Model == null)
            {
                m_Model = context.Content.Load<Model>(modelFile);

                if (m_Body == null)
                {
                    m_Body = BodyFactory.CreateRectangle(context.World, 100, 100, 1);
                    m_Body.BodyType = BodyType.Static;
                    m_Body.Enabled = false;
                    m_Body.IsSensor = true; //pickups are always triggers
                }

            }
        }
    }

    //Stores all possible PickupPrefabs so they only need to be loaded in once
    public static class PickupPrefabList
    {
        static Dictionary<PickupName, PickupPrefab> m_PrefabList = new Dictionary<PickupName, PickupPrefab>();

        public static void AddPrefab(PickupPrefab prefab)
        {
            if (prefab != null && !m_PrefabList.ContainsKey(prefab.Name))
            {
                m_PrefabList.Add(prefab.Name, prefab);
            }
        }

        public static T GetPrefab<T>(PickupName name)
        {
            //See if Exists
            if (!m_PrefabList.ContainsKey(name))
                return default(T);

            var data = m_PrefabList[name];
            T obj = (T)Activator.CreateInstance(typeof(T),data.Model,data.Body);

            
            return obj;
        }
    }

    public abstract class Pickup : GameModel
    {
       public bool isPickedUp = false;

       protected Player m_Receiver = null;

        public Pickup(Model model, Body body)
            : base(string.Empty, string.Empty)
        {
            base.Name = "Pickup";
            base.m_Model = model;
            base.RigidBody = body;
            base.RigidBody.OnCollision +=RigidBody_OnCollision;
        }

        private bool RigidBody_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            var collider = (GameModel)fixtureB.Body.UserData;

            if (collider.Name == "Player")
            {
                m_Receiver = (Player)collider;

                isPickedUp = true;
            }

            return false;
        }

        abstract public void OnPickup();
       
       
    }

    public class HealthPickup : Pickup
    {
        public HealthPickup(Model model, Body body)
            : base(model, body)
        {
            RigidBody.UserData = this;
        }

        public override void OnPickup()
        {
            m_Receiver.HealthBar.AddHealth();
            Console.WriteLine("Picked up Health");
        }
    }
  
}
