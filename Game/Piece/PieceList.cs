using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

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
    
    public class PiecePrefab
    {
       Model m_Model = null;
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
       public TextureData TextureData;

       public Vector2 Scale;
       public Vector2 Size;

       public PiecePrefab(RenderContext context, Collision name, string assetFile,float scaleX = 1.0f, float scaleY = 1.0f)
       {         
           //Set Name
            Name = name;
            Scale.X = scaleX;
            Scale.Y = scaleY;

           //Set Model
            if (m_Model == null)
            {
                if (!File.Exists("Content/Model/m_" + assetFile + ".xnb"))
                {
                    m_Model = context.Content.Load<Model>("Model/Default");
                    Console.WriteLine("Failed to load model " + assetFile);
                }
                else m_Model = context.Content.Load<Model>("Model/m_" + assetFile);
            }

           //Create RigidBody
           if (m_Body == null)
           {               
               var collisionTexture = context.Content.Load<Texture2D>("Textures/Collision/c_" + assetFile);
               BodyFromTexture.CreateBodyFromImage(ref m_Body, context.Game, context.World, collisionTexture, scaleX,scaleY);
               Size = new Vector2(collisionTexture.Width, collisionTexture.Height);
               m_Body.UserData = this;
               m_Body.BodyType = BodyType.Static;
               m_Body.Enabled = false;
           }

           if (!File.Exists("Content/Textures/Models/D_" + assetFile + ".xnb"))          
           {
               Console.WriteLine("GroundPrefab:: Missing Textures for " + assetFile);
           } 
           else
                TextureData.Diffuse = context.Content.Load<Texture2D>("Textures/Models/D_" + assetFile);
         }

    }

    //Stores all possible groundPrefabs so they only need to be loaded in once
    public static class PiecePrefabList
    {
        //Stores all possible GroundPrefabs as static so they only need to be loaded once
        static Dictionary<Collision, PiecePrefab> m_PrefabList = new Dictionary<Collision, PiecePrefab>();
        public static int Count = 0;
        
        public static void AddPrefab(PiecePrefab prefab)
        {
            if (prefab != null && !m_PrefabList.ContainsKey(prefab.Name))
            {
                m_PrefabList.Add(prefab.Name, prefab);
                Count++;
            }
        }

        public static Piece GetPrefab(Collision name,string typeOflevelName)
        {
            //See if Exists
            if(!m_PrefabList.ContainsKey(name))
                return null;                

            var data =  m_PrefabList[name];
            return new Piece(data.Model, data.Body,typeOflevelName, data.TextureData, data.Scale.X,data.Scale.Y);
        }

        public static Piece GetRandom(Random rnd,string type)
        {

            //Roll to generate a random enemy that is different from the previous generated enemy

            Piece piece = null;

            //Roll
            var roll = rnd.Next(0, m_PrefabList.Count - 1);

            //Add pickup here for each roll
            //roll = 0;
            piece = PiecePrefabList.GetPrefab((Collision)roll, type);

            return piece;
        }
        
    }    
   
}
