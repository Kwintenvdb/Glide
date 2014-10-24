using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;

namespace gameProject.Prefab
{
    class Pickup : GameModel
    {
        public Pickup(string assetFile, string textureFile, RenderContext context) : base(assetFile, textureFile, context)
        {
            // Rigidbody is static, change this if you want moving pickups
            RigidBody = BodyFactory.CreateRectangle(context.World, ConvertUnits.ToSimUnits(20), ConvertUnits.ToSimUnits(20), 1.0f);
            RigidBody.BodyType = BodyType.Static;
            RigidBody.UserData = this;

            Depth = -100;
            Name = "Pickup";
        }
    }
}
