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
    //This is used to generate a model with collision from a texture    
    public class Piece : GameModel
    {
        public float Width
        {
            set{}
            get{ return ObjectScale.X *  m_Width;}
        }

         public float Height
        {
            set{}
            get{ return ObjectScale.Y *  m_Height;}
        }
         float m_Width, m_Height;

        public Piece(Model model, Body body,string name, 
            TextureData textureData,float scalex = 1.0f, float scaleY = 1.0f)
            : base() //send empty because models are already loaded
        {

            Name = name;    
            
            //Transfer model from prefab to this
            base.Model = model;            
            base.RigidBody = body;       
            
            //Scale the object with the provided size
            Scale(scalex * 1.2f,scaleY,0.7f);

            //Set the width and height
            PiecePrefab piecePrefab = (PiecePrefab)body.UserData;
            Vector2 size = piecePrefab.Size;
            m_Width = (int)size.X;
            m_Height = (int)size.Y;

            base.RigidBody.UserData = this; //Add this as userdata
            
            //Set Texture
            base.DiffuseTexture = textureData.Diffuse;
            base.NormalTexture = textureData.Normal;
            base.SpecularTexture = textureData.Specular;

            //Rotate/Translate based on location of the object
            if (Name == "Top")
            {
                Offset.Y = 260;
                Rotate(0, 0, 180);
                Translate(0, 0);
            }
            else if (Name == "Ground")
            {
                Offset.Y = -260;
                Translate(0, 100);
            }

        }
    }
}
