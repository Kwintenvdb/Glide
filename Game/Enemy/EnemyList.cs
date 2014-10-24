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
    //EnemyPrefab stores the model and the body of the pickup
    public class EnemyPrefab
    {
        Model m_Model = null;
        Body m_Body = null;

        public EnemyName Name;
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

        public EnemyPrefab(RenderContext context, EnemyName name, string assetFile)
        {
            Name = name;

            if (m_Model == null)
            {
                if (!File.Exists("Content/Model/m_" + assetFile + ".xnb"))
                {
                    m_Model = context.Content.Load<Model>("Model/Default");
                    Console.WriteLine("Failed to load model " + assetFile);
                }
                else m_Model = context.Content.Load<Model>("Model/m_" + assetFile); 
            }
            if (m_Body == null)
            {
                if (assetFile == "EnemyRotator")
                {
                    m_Body = BodyFactory.CreateRectangle(context.World, 250, 550, 1);
                    m_Body.BodyType = BodyType.Static;
                    m_Body.Enabled = false;
                    m_Body.IsSensor = true; //pickups are always triggers
                }
                else if (assetFile == "SmashingWall")
                {
                    m_Body = BodyFactory.CreateRectangle(context.World, 75, 250, 1);
                    m_Body.BodyType = BodyType.Static;
                    m_Body.Enabled = false;
                    //m_Body.IsSensor = true; //pickups are always triggers
                }
                else
                {
                    m_Body = BodyFactory.CreateRectangle(context.World, 100, 100, 1);
                    m_Body.BodyType = BodyType.Dynamic;
                    m_Body.Enabled = false;
                }
            }
            
            if (!File.Exists("Content/Textures/Models/D_" + assetFile + ".xnb")   )          
            {
                Console.WriteLine("EnemyPrefab:: Missing Texture for " + assetFile);
                return;
            }

            TextureData.Diffuse = context.Content.Load<Texture2D>("Textures/Models/D_" + assetFile);
         }
    }

    //Stores all possible EnemyPrefab so they only need to be loaded in once per game
    public static class EnemyPrefabList
    {
        public static int Count = 0;

        static Dictionary<EnemyName, EnemyPrefab> m_PrefabList = new Dictionary<EnemyName, EnemyPrefab>();

        public static void AddPrefab(EnemyPrefab prefab)
        {
            if (prefab != null && !m_PrefabList.ContainsKey(prefab.Name))
            {
                m_PrefabList.Add(prefab.Name, prefab);
                Count++;
            }
        }

        public static T GetPrefab<T>(EnemyName name)
        {
            //See if Exists
            if (!m_PrefabList.ContainsKey(name))
                return default(T);

            var data = m_PrefabList[name];
            T obj = (T)Activator.CreateInstance(typeof(T), data.Model, data.Body, data.TextureData);

            return obj;
        }


        public static Enemy GetRandom(Random rnd, int ScreenSize)
        {
            //Roll to generate a random enemy that is different from the previous generated enemy

            Enemy enemy = null;

            //Roll
            var roll = rnd.Next(0, Count);

            //Add pickup here for each roll
            if (roll == 0)
            {
                enemy = GetPrefab<Smash>(EnemyName.Smash);
            }
            else if (roll == 1)
                enemy = GetPrefab<FallDown>(EnemyName.FallDown);
            else if(roll == 2)
                enemy = GetPrefab<Wheel>(EnemyName.Wheel);
            else
                enemy = null;

            if (enemy != null)
            {
                //enemy.Create();
                float offsetPercent = 0.2f;
                enemy.Translate(rnd.Next(0, 1700), rnd.Next((int)(ScreenSize * -offsetPercent), (int)(ScreenSize * offsetPercent)));
                if (enemy.EnemyName == "Smash")
                {
                    enemy.Translate(0, -350);
                }
                else if (enemy.EnemyName == "FallDown")
                {
                    enemy.Translate(0, 250);
                }
            }

            return enemy;
        }
    }  
}
