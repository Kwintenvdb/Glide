#region File Description
//-----------------------------------------------------------------------------
// SmokePlumeParticleSystem.cs
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
    public abstract class ParticleSystem:DrawableGameComponent
    {
        //Order of Drawing
        public const int AlphaBlendDrawOrder = 100;
        public const int AdditiveDrawOrder = 200;

        Game1 m_Game;
        GameModel m_Model;
        private Texture2D m_ParticleTexture;
        private Vector2 m_Origin = Vector2.Zero;
        public Rectangle? DrawRect;

        RenderContext m_RenderContext;

        private int m_NrParticles;
        Particle[] m_Particles;
        Queue<Particle> m_FreeParticles;
        public int FreeParticleCount
        {
            get { return m_FreeParticles.Count; }
        }
       

        #region constants to be set by subclasses

        protected int minNumParticles;
        protected int maxNumParticles;
       
      
        protected string textureFilename;

        /// <summary>
        /// minInitialSpeed and maxInitialSpeed are used to control the initial velocity
        /// of the particles. The particle's initial speed will be a random number 
        /// between these two. The direction is determined by the function 
        /// PickRandomDirection, which can be overriden.
        /// </summary>
        protected float minInitialSpeed;
        protected float maxInitialSpeed;

        /// <summary>
        /// minAcceleration and maxAcceleration are used to control the acceleration of
        /// the particles. The particle's acceleration will be a random number between
        /// these two. By default, the direction of acceleration is the same as the
        /// direction of the initial velocity.
        /// </summary>
        protected float minAcceleration;
        protected float maxAcceleration;

        /// <summary>
        /// minRotationSpeed and maxRotationSpeed control the particles' angular
        /// velocity: the speed at which particles will rotate. Each particle's rotation
        /// speed will be a random number between minRotationSpeed and maxRotationSpeed.
        /// Use smaller numbers to make particle systems look calm and wispy, and large 
        /// numbers for more violent effects.
        /// </summary>
        protected float minRotationSpeed;
        protected float maxRotationSpeed;

        /// <summary>
        /// minLifetime and maxLifetime are used to control the lifetime. Each
        /// particle's lifetime will be a random number between these two. Lifetime
        /// is used to determine how long a particle "lasts." Also, in the base
        /// implementation of Draw, lifetime is also used to calculate alpha and scale
        /// values to avoid particles suddenly "popping" into view
        /// </summary>
        protected float minLifetime;
        protected float maxLifetime;

        /// <summary>
        /// to get some additional variance in the appearance of the particles, we give
        /// them all random scales. the scale is a value between minScale and maxScale,
        /// and is additionally affected by the particle's lifetime to avoid particles
        /// "popping" into view.
        /// </summary>
        protected float minScale;
        protected float maxScale;

        /// <summary>
        /// different effects can use different blend states. fire and explosions work
        /// well with additive blending, for example.
        /// </summary>
		protected BlendState blendState;

        #endregion
        
        protected ParticleSystem(RenderContext context, int howManyEffects)
            : base(context.Game)
        {
            m_RenderContext = context;
            m_Game = context.Game;
            this.m_NrParticles = howManyEffects;

        }

        public override void Initialize()
        {
            InitializeConstants();
            
            // calculate the total number of particles we will ever need, using the
            // max number of effects and the max number of particles per effect.
            // once these particles are allocated, they will be reused, so that
            // we don't put any pressure on the garbage collector.
            m_Particles = new Particle[m_NrParticles * maxNumParticles];
            m_FreeParticles = new Queue<Particle>(m_NrParticles * maxNumParticles);
            for (int i = 0; i < m_Particles.Length; i++)
            {
                m_Particles[i] = new Particle();
                m_FreeParticles.Enqueue(m_Particles[i]);
            }
            base.Initialize();
        }

        protected abstract void InitializeConstants();

        protected override void LoadContent()
        {
            // make sure sub classes properly set textureFilename.
            if (string.IsNullOrEmpty(textureFilename))
            {
                string message = "textureFilename wasn't set properly, so the " +
                    "particle system doesn't know what texture to load. Make " +
                    "sure your particle system's InitializeConstants function " +
                    "properly sets textureFilename.";
                throw new InvalidOperationException(message);
            }
            // load the texture....
            m_ParticleTexture = m_Game.Content.Load<Texture2D>(textureFilename);

            // ... and calculate the center. this'll be used in the draw call, we
            // always want to rotate and scale around this point.
            m_Origin.X = m_ParticleTexture.Width / 2;
            m_Origin.Y = m_ParticleTexture.Height;


            base.LoadContent();
        }

        
        public void AddParticles(Vector2 where)
        {
            // the number of particles we want for this effect is a random number
            // somewhere between the two constants specified by the subclasses.
            Random random = new Random(Guid.NewGuid().GetHashCode());
            int numParticles = random.Next(minNumParticles, maxNumParticles);

            // create that many particles, if you can.
            for (int i = 0; i < numParticles && m_FreeParticles.Count > 0; i++)
            {
                // grab a particle from the freeParticles queue, and Initialize it.
                Particle p = m_FreeParticles.Dequeue();
                InitializeParticle(p, where);               
            }
        }

       
        protected virtual void InitializeParticle(Particle p, Vector2 where)
        {
            // first, call PickRandomDirection to figure out which way the particle
            // will be moving. velocity and acceleration's values will come from this.
            Vector2 direction = new Vector2(1,-1);

            // pick some random values for our particle
            float velocity = 
                MathHelp.RandomBetween(minInitialSpeed, maxInitialSpeed);
            float acceleration =
                MathHelp.RandomBetween(minAcceleration, maxAcceleration);
            float lifetime =
                MathHelp.RandomBetween(minLifetime, maxLifetime);
            float scale =
                MathHelp.RandomBetween(minScale, maxScale);
            float rotationSpeed =
                MathHelp.RandomBetween(minRotationSpeed, maxRotationSpeed);

            // then initialize it with those random values. initialize will save those,
            // and make sure it is marked as active.
            p.Initialize(
                where, velocity * direction, acceleration * direction,
                lifetime, scale, rotationSpeed);
        }

        /// <summary>
        /// PickRandomDirection is used by InitializeParticles to decide which direction
        /// particles will move. The default implementation is a random vector in a
        /// circular pattern.
        /// </summary>
        protected virtual Vector2 PickRandomDirection()
        {
            float angle = MathHelp.RandomBetween(0, MathHelper.TwoPi);
            return new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));
        }

        /// <summary>
        /// overriden from DrawableGameComponent, Update will update all of the active
        /// particles.
        /// </summary>
        /// 

        public void SetProjectionSettings(RenderContext context, GameModel model)
        {
            m_RenderContext = context;
            m_Model = model;
        }

        public override void Update(GameTime gameTime)
        {
            // calculate dt, the change in the since the last frame. the particle
            // updates will use this value.
            float dt = (float)gameTime.ElapsedGameTime.TotalSeconds;

            // go through all of the particles...
            foreach (Particle p in m_Particles)
            {
                
                if (p.Active)
                {
                    // ... and if they're active, update them.
                    p.Update(dt,m_RenderContext,m_Model);

                    // if that update finishes them, put them onto the free particles
                    // queue.
                    if (!p.Active)
                    {
                        m_FreeParticles.Enqueue(p);
                    }
                }   
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
           


            m_Game.SpriteBatch.Begin();

            if (m_Particles.Length > 0)
            {
                foreach (Particle p in m_Particles)
                {
                    // skip inactive particles
                    if (!p.Active)
                        continue;

                    float normalizedLifetime = p.TimeSinceStart / p.LifeTime;

                    float alpha = normalizedLifetime * (1 - normalizedLifetime);
                    Color color = Color.White * alpha;

                    float scale = p.Scale * (0.75f + .25f * normalizedLifetime);

                    m_Game.SpriteBatch.Draw(m_ParticleTexture, p.Position, DrawRect, color,
                        p.Rotation, m_Origin, scale, SpriteEffects.None, 1.0f);


                }
            }

            m_Game.SpriteBatch.End();

           

            base.Draw(gameTime);
        }
    }
}
