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
        public TextureData TextureData;

        public PickupPrefab(RenderContext context, PickupName name, string assetFile)
        {
            Name = name;

            if (m_Model == null)
            {
                if (!File.Exists("Content/Model/m_Pickup.xnb"))
                {
                    m_Model = context.Content.Load<Model>("Model/Default");
                    Console.WriteLine("Failed to load model " + assetFile);
                }
                if (assetFile == "WindVulcano")
                    m_Model = context.Content.Load<Model>("Model/m_" + assetFile);
                else
                {
                    //m_Model = context.Content.Load<Model>("Model/m_" + assetFile);
                    m_Model = context.Content.Load<Model>("Model/m_PickupPlane");
                }
            }

            if (m_Body == null)
            {
                if (assetFile != "WindVulcano") m_Body = BodyFactory.CreateRectangle(context.World, 100, 100, 1);
                else m_Body = BodyFactory.CreateRectangle(context.World, 200, 1000, 1);
                
                m_Body.BodyType = BodyType.Static;
                m_Body.Enabled = false;
                m_Body.IsSensor = true; //pickups are always triggers
            }

            if (!File.Exists("Content/Textures/Models/D_" + assetFile + ".xnb"))
            {
                TextureData.Diffuse = context.Content.Load<Texture2D>("Textures/Models/D_Default");
                Console.WriteLine("Failed  to load Texture " + assetFile);
            }
            else
            {
                TextureData.Diffuse = context.Content.Load<Texture2D>("Textures/Models/D_" + assetFile);
            }

           

        }
    }

    //Stores all possible PickupPrefabs so they only need to be loaded in once per game
    public static class PickupPrefabList
    {
        public static int Count = 0;

        static Dictionary<PickupName, PickupPrefab> m_PrefabList = new Dictionary<PickupName, PickupPrefab>();

        public static void AddPrefab(PickupPrefab prefab)
        {
            if (prefab != null && !m_PrefabList.ContainsKey(prefab.Name))
            {
                m_PrefabList.Add(prefab.Name, prefab);
                Count++;
            }
        }

        public static T GetPrefab<T>(PickupName name)
        {
            //See if Exists
            if (!m_PrefabList.ContainsKey(name))
                return default(T);

            var data = m_PrefabList[name];
            T obj = (T)Activator.CreateInstance(typeof(T), data.Model, data.Body, data.TextureData);

            return obj;
        }

        public static Pickup GetRandom(Random rnd,int ScreenSize)
        {
            //Roll to generate a random enemy that is different from the previous generated enemy
          
            Pickup pickup = null;

            //Roll
            var roll = rnd.Next(0, Count);
            //Add pickup here for each roll
            if (roll == 0)
                pickup = 
                    PickupPrefabList.GetPrefab
                    <HealthPickup>(PickupName.Health);
            else if (roll == 1)
                pickup = 
                    PickupPrefabList.GetPrefab
                    <SpeedUpPickup>(PickupName.SpeedUp);
            else if (roll == 2)
                pickup = 
                    PickupPrefabList.GetPrefab
                    <SpeedDownPickup>(PickupName.SpeedDown);
            else if (roll == 3)
                pickup = PickupPrefabList.GetPrefab
                    <WindUpPickup>(PickupName.WindUp);
            else if (roll == 4)
                pickup = PickupPrefabList.GetPrefab
                    <WindDownPickup>(PickupName.WindDown);
            else
                pickup = null;            

            if (pickup != null)
            {
                //For Planes
                pickup.Scale(-15.0f);
                pickup.Rotate(0, 0, 180);

                //For models

                //pickup.Scale(3.0f);
               // pickup.Rotate(0, 0, 0);
                pickup.Translate(rnd.Next(0, 1700), 
                    rnd.Next((int)(ScreenSize * -0.4f), 
                    (int)(ScreenSize * 0.4f)));
            }
                
            return pickup;
        }
    }

}
