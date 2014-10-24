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
    public enum Collision
    {
        Col1,
        Col2,
        Col3,
        Col4,
        Col5,
        Col6,
        Col7,
        Col8
    }
    
    public class GroundPrefab
    {

       Model m_Model = null;
       Texture2D m_Texture = null;
       Body m_Body = null;
       public Collision Name;

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
               //Send A clone of the body otherwise we change the original body
              
               return m_Body.DeepClone();
           }
       }

       public GroundPrefab(RenderContext context, Collision name, string modelFile, string collisionFile)
       {
            Name = name;

            if (m_Model == null)
            {
                m_Model = context.Content.Load<Model>(modelFile);
                Console.WriteLine("Load Model");
            }

            if (m_Body == null)
            {
                //Load the texture
                m_Texture = context.Content.Load<Texture2D>(collisionFile);
                BodyFromTexture.CreateBodyFromImage(ref m_Body, context.Game, context.World, m_Texture);
                m_Body.UserData = new Vector2(m_Texture.Width, m_Texture.Height);
                m_Body.BodyType = BodyType.Static;
                m_Body.Enabled = false;
                Console.WriteLine("Load Collision Body of Ground");
            }
        }

    }

    //Stores all possible groundPrefabs so they only need to be loaded in once
    public static class GroundPrefabList
    {
        //Stores all possible GroundPrefabs as static so they only need to be loaded once
        static Dictionary<Collision, GroundPrefab> m_PrefabList = new Dictionary<Collision, GroundPrefab>();
        
        public static void AddPrefab(GroundPrefab prefab)
        {
            if (prefab != null && !m_PrefabList.ContainsKey(prefab.Name))
            {
                m_PrefabList.Add(prefab.Name, prefab);
            }
        }

        public static Ground GetPrefab(Collision name)
        {
            //See if Exists
            if(!m_PrefabList.ContainsKey(name))
                return null;                

            var data =  m_PrefabList[name];
            return new Ground(data.Model, data.Body);
        }
        
    }
    
    //This is used to generate a model with collision from a texture    
    public class Ground : GameModel
    {
       public Ground(Model model, Body body)
            : base(string.Empty, string.Empty) //sen empty because models are already loaded
       {
           //Body gets enabled again in the main thread ( Issue with farseer world)          
            base.m_Model = model;
            base.RigidBody = body;



       }

    }
}
