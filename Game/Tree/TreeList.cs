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
    public class TreePrefab
    {
        Model m_Model = null;
        Body m_Body = null;

        public TreeName Name;
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

        public TreePrefab(RenderContext context, TreeName name, string assetFile)
        {
            Name = name;

            if (m_Model == null)
            {
                m_Model = context.Content.Load<Model>("Model/m_" + assetFile);
            }

            if (m_Body == null)
            {
                m_Body = BodyFactory.CreateBody(context.World); // empty body for tree

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
    public static class TreePrefabList
    {
        public static int Count = 0;

        static Dictionary<TreeName, TreePrefab> m_PrefabList = new Dictionary<TreeName, TreePrefab>();

        public static void AddPrefab(TreePrefab prefab)
        {
            if (prefab != null && !m_PrefabList.ContainsKey(prefab.Name))
            {
                m_PrefabList.Add(prefab.Name, prefab);
                Count++;
            }
        }

        public static T GetPrefab<T>(TreeName name)
        {
            //See if Exists
            if (!m_PrefabList.ContainsKey(name))
                return default(T);

            var data = m_PrefabList[name];
            T obj = (T)Activator.CreateInstance(typeof(T), data.Model, data.Body, data.TextureData);

            return obj;
        }

        public static Tree GetRandom(Random rnd, int ScreenSize)
        {
            //Roll to generate a random tree that is different from the previous generated enemy
            Tree tree = null;

            //Roll
            var roll = rnd.Next(0, Count);
            //Add pickup here for each roll
            if (roll == 0)
                tree = TreePrefabList.GetPrefab<Tree1>(TreeName.Tree1);
            if (roll == 1)
                tree = TreePrefabList.GetPrefab<Tree1>(TreeName.Tree2);
            if (roll == 2)
                tree = TreePrefabList.GetPrefab<Tree1>(TreeName.Tree3);

            return tree;
        }
    }

}
