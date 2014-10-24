#region Using Statements
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
#endregion
namespace gameProject
{
    public class WindParticleSystem:ParticleSystem
    {
        public float Direction = 1;

        public WindParticleSystem(RenderContext context, int amount):
            base(context,amount)
        {


        }

        /// <summary>
        /// Set up the constants that will give this particle system its behavior and
        /// properties.
        /// </summary>
        protected override void InitializeConstants()
        {
            textureFilename = "Textures/Particles/WindParticle";

            minInitialSpeed = 1;
            maxInitialSpeed = 20;

            // we don't want the particles to accelerate at all, aside from what we
            // do in our overriden InitializeParticle.
            minAcceleration = 2;
            maxAcceleration = 5;

            // long lifetime, this can be changed to create thinner or thicker smoke.
            // tweak minNumParticles and maxNumParticles to complement the effect.
            minLifetime = 2.0f;
            maxLifetime = 1.0f;

            minScale = .25f;
            maxScale = 1.0f;

            minNumParticles = 1;
            maxNumParticles = 3;

            // rotate slowly, we want a fairly relaxed effect
            //minRotationSpeed = -MathHelper.PiOver4 / 2.0f;
            //maxRotationSpeed = MathHelper.PiOver4 / 2.0f;

            blendState = null;

            //DrawOrder = AlphaBlendDrawOrder;
        }

        protected override Vector2 PickRandomDirection()
        {
            // Point the particles somewhere between 80 and 100 degrees.
            // tweak this to make the smoke have more or less spread.
            float radians = MathHelp.RandomBetween(
                MathHelper.ToRadians(80), MathHelper.ToRadians(100));

            Vector2 direction = Vector2.Zero;
            // from the unit circle, cosine is the x coordinate and sine is the
            // y coordinate. We're negating y because on the screen increasing y moves
            // down the monitor.
           // direction.X = (float)Math.Cos(radians);
            direction.Y = Direction;
            return direction;
        }

        protected override void InitializeParticle(Particle p, Vector2 where)
        {
            base.InitializeParticle(p, where);

            // the base is mostly good, but we want to simulate a little bit of wind
            // heading to the right.
            p.Acceleration.X += MathHelp.RandomBetween(10, 50);
        }
    }

}
