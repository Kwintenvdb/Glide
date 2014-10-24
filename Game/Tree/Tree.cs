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
    //Add new name of tree here
    public enum TreeName
    {
        Tree1,
        Tree2,
        Tree3
    }

    public abstract class Tree : GameModel
    {
        public Tree(Model model, Body body, TextureData textureData)
            : base(string.Empty)
        {
            base.Name = "Tree";
            base.Model = model;
            base.RigidBody = body;
            base.DiffuseTexture = textureData.Diffuse;
            base.NormalTexture = textureData.Normal;
            base.SpecularTexture = textureData.Specular;

            base.Depth = -55;
            Scale(2.5f, 2.5f, 1.5f);
        }
    }

    public class Tree1 : Tree
    {
        public Tree1(Model model, Body body, TextureData textureData)
            : base(model, body, textureData)
        {
            RigidBody.UserData = this;
        }
    }
}
