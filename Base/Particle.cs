#region File Description
//-----------------------------------------------------------------------------
// Particle.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace gameProject
{
   public class Particle
    {
        //Variables
        public Vector2 Position,
                       Velocity,
                       Acceleration;

        public float LifeTime,
                     TimeSinceStart,
                     Scale,
                     Rotation,
                     RotSpeed;

        public bool Active
        {
            get { return TimeSinceStart < LifeTime; }
        }

        public void Initialize(Vector2 pos, Vector2 velo, Vector2 acel, float lifeTime, float scale, float rotSpeed)
        {
           Position = pos;
           Velocity = velo;
           Acceleration = acel;
           LifeTime = lifeTime;
           Scale = scale;
           RotSpeed = rotSpeed;

           TimeSinceStart = 0.0f;

           //this.Rotation = MathHelp.RandomBetween(0, MathHelper.TwoPi);

        }

        public void Update(float dt,RenderContext context,GameModel model)
        {
            //Project onto a model
            var camera = context.Camera;
            Vector3 center = context.GraphicsDevice.Viewport.Project(new Vector3(model.Position, 0), camera.Projection, camera.View, Matrix.Identity);
            Position = new Vector2(center.X, center.Y);

            Velocity += Acceleration * dt;
            Position += Velocity * dt;

            //Position.Y += 120 * dt;

            Rotation += RotSpeed * dt;
            TimeSinceStart += dt;

           
        }




    }
}
