using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;


namespace gameProject
{
    public class Player:GameModel
    {
        #region Variable Declaration

        //Movement
        float m_Rotation = 0f
             , m_TurnAmount = 3.0f
             , m_MaxXVelocity = 70.0f
             , m_CurrentVelo = 0f
             , m_Timer = 0.0f
             , m_FrictionMultiplier = 0.1f
             , m_DeltaTime = 0.0f
             , m_Time = 0.0f
             , m_PickupDuration = 20.0f;


        Vector2 m_Direction = Vector2.Zero
               , m_OrginalPos = Vector2.Zero
               , m_PosOnHit = Vector2.Zero
               , m_FrictionForce = Vector2.Zero;

        Vector2 m_Velocity
        {
            get { return RigidBody.LinearVelocity; }
            set { RigidBody.LinearVelocity = value; }
        }
        

        //Wind
        enum WindState { NoWind, BuildUp, BuildOff }
        WindState m_WindState = WindState.NoWind;

        float m_TwirlTime = 0f
             , m_MaxTwirlTime = 20f
             , m_PickupFromGroundTime = 0f
             , m_PickupFromGroundMaxTime = 30f
             , m_TwirlDevider = 2.5f
             , m_FinalWindForce = 30f
             , m_CurrentWindForce = 0f
             , m_Multplier = 1f
             , m_CurrentIncrement = 0.05f
             , m_MinHorizontalWind = 0f
             , m_VulcanoPower = 40.0f;

        bool m_CameraCanRotate = true;
        bool m_GameIsReallyStarted = false;

        public float PullDirection = 1.0f;
        
        //Chain of Leaves
        List<Chain> m_Chain = new List<Chain>();
        List<Vector2> m_ChainPositions = new List<Vector2>();
        List<float> m_ChainRotations = new List<float>();
        int m_NrOfChainPos = 60;/* 400;*/

        public bool AllowInput = true;
        bool m_bPickedup = false
            , m_TwirlEffect = true
            , m_Twirl = false
            ,m_bMove = true;
        public bool PickUpFromGround = false;


        //Health
        int m_MaxHealth = 4;
        public int Health = 4;
        int m_ChainCount = 4; //nr of leaves
        bool m_bInvincible = false;

        Model m_Flat;
        Model m_Curved;

        RenderContext m_Context;

        #endregion Variable Declaration

        public Player(RenderContext context, Vector2 originalPos)
            : base("PlayerV2")
        {
            m_Context = context;

            base.Name = "Player";
            base.m_AssetFile = "PlayerV2";
            m_OrginalPos = originalPos;
            
            //Create rigidBody      
            RigidBody = BodyFactory.CreateRectangle(context.World, ConvertUnits.ToSimUnits(100), ConvertUnits.ToSimUnits(10), 1.0f);
            RigidBody.BodyType = BodyType.Dynamic;
            RigidBody.OnCollision += RigidBody_OnCollision;
            RigidBody.OnSeparation += RigidBody_OnSeparation;
            RigidBody.UserData = this; //Add this class as data for easy access in collision
            RigidBody.Restitution = 0.1f;
            RigidBody.FixedRotation = true;
            Depth = 0;
            
            //Chain Creation
            float scale = 1f;
            for (int i = 0; i < m_ChainCount; i++)
            {
                Chain piece = new Chain(context, scale);
                piece.Translate(originalPos);
                piece.Initialize(context);
                m_Chain.Add(piece);
            }
            for (int i = 0; i < m_NrOfChainPos ; i++)
            {
                m_ChainPositions.Add(Vector2.Zero);
                m_ChainRotations.Add(0f);
            }


            //Xml
            /*var xmlPlayerData = XmlLoader.Load<PlayerSettings>("PlayerSettings");
            m_Multplier = xmlPlayerData.multiplier;
            m_TurnAmount = xmlPlayerData.turnAmount;
            m_CurrentIncrement = xmlPlayerData.windIncrement;
            m_TwirlEffect = xmlPlayerData.toggleTwirl;
            if (xmlPlayerData.minWindSpeedX != -1) m_MinHorizontalWind = xmlPlayerData.minWindSpeedX;*/

            m_Velocity = Vector2.Zero;

            m_Flat = context.Content.Load<Model>("Model/m_PlayerV2");
            m_Curved = context.Content.Load<Model>("Model/m_PlayerV2");
            base.DiffuseTexture = context.Content.Load<Texture2D>("Textures/Models/D_PlayerV2");

            Scale(0.6f);
           
        }

        void RigidBody_OnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            var collider = (GameModel)fixtureB.Body.UserData;
            if (collider != null && collider.Name == "Smash")
            {

                RigidBody.AngularVelocity = 0;
                m_bMove = true;
            }
        }

        bool RigidBody_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            var collider = (GameModel)fixtureB.Body.UserData;
            if (collider != null)
            {
                if (collider.Name == "Ground")
                {
                    PickUpFromGround = true;
                    PullDirection = 1.0f;
                    TakeHealth();
                    return true;
                }
                else if (collider.Name == "Top")
                {
                    PickUpFromGround = false;
                    PullDirection = -1.0f;
                    TakeHealth();
                    return true;
                }
                else if (collider.Name == "Smash")
                {
                    TakeHealth();
                    return true;
                }

            }

            return true;
        }

        //Main update loop
        public override void Update(RenderContext context)
        {
            //Camera zooming
            CameraZoom(context);

            //Chains Pos
            m_ChainPositions.Add(Position);
            m_ChainRotations.Add(m_Rotation);
            m_ChainPositions.RemoveAt(0);
            m_ChainRotations.RemoveAt(0);
            
            

            m_DeltaTime = (float)context.GameTime.ElapsedGameTime.Milliseconds / 100;
            
            //So the player doesn;t lose all health on a hit, player is invincible in debug
            //#if(!DEBUG)
            if (m_bInvincible)
            {
                m_Time += m_DeltaTime;
                if (m_Time > 4.5f)
                {
                    m_bInvincible = false;
                    m_Time = 0;
                }
            }
            if (m_GameIsReallyStarted == false) m_bInvincible = true;

            //Allow input
            if(AllowInput)
                HandleInput(context);

            HandlePickupTime();

            if(m_bMove)
                Movement(m_DeltaTime);
            
            //still working on the friction
            if (m_Velocity != Vector2.Zero)
            {
                Vector2 velocityNormalized = m_Velocity;
                velocityNormalized.Normalize();
                m_FrictionForce = m_FrictionMultiplier * (-velocityNormalized);
                m_Velocity += m_FrictionForce;
            }

            RandomWind(m_DeltaTime);
            if (m_TwirlEffect == true && m_Twirl == true) Twirl(m_DeltaTime, context);
            else if (m_CameraCanRotate)
            {
                Vector3 up = Vector3.Up;
                up.X = MathHelper.Lerp(up.X, 0, 0.1f);
                context.Camera.SetUpVector(up);
            }

            Rotate(-m_Rotation, 90, 0);
            RotateBody(m_Rotation);

            //Clamp velocity to 60 by default
            if(!m_bPickedup)
                m_CurrentVelo = m_Velocity.X;
           MathHelp.Clamp(ref m_CurrentVelo, m_MaxXVelocity, -m_MaxXVelocity);
            m_Velocity = new Vector2(m_CurrentVelo, m_Velocity.Y);

            base.Update();

            UpdateChains();
        }

        public void UpdateChains()
        {
/*
            if (m_Velocity.X > 0f)
            {
                Vector2 idealPos = Vector2.Zero;
                Vector2 PrevPos = Position;
                float idealRotation = 0.0f;
                float maxDistanceBetweenChainPieces = 100.0f;
                float idealDistance = 0.0f;
                int chainNr = 0;
                foreach (Chain chain in m_Chain)
                {
                    chainNr++;
                    idealDistance = 0;
                    for (int i = 0; i < m_NrOfChainPos; i++)
                    {
                        float distance = Vector2.Distance(PrevPos, m_ChainPositions[i]);
                        if (distance > idealDistance && distance < maxDistanceBetweenChainPieces && m_ChainPositions[i].X < PrevPos.X)
                        {
                            idealDistance = distance;
                            idealPos = m_ChainPositions[i];
                            idealRotation = m_ChainRotations[i];
                        }
                    }
                    PrevPos = idealPos;
                    chain.Update(idealRotation, idealPos);
                }
            }
            else
            {*/
                int i = m_NrOfChainPos;
                 foreach (Chain chain in m_Chain)
                 {
                     i -= m_NrOfChainPos / m_ChainCount;
                     chain.Update(m_ChainRotations[i], m_ChainPositions[i]);
                 }
            /*}*/
        }

        #region Pickup Boosts

        void HandlePickupTime()
        {
            if (m_bPickedup ==true)
            {
                m_Timer += m_DeltaTime;
                if (m_Timer > m_PickupDuration)
                {
                    m_Timer = 0;
                    m_MaxXVelocity = 100;
                    m_bPickedup = false;
                    Console.WriteLine("Stop increase");
                }
            }

        }

        public void IncreaseSpeed()
        {
            
            m_CurrentVelo = m_MaxXVelocity;
            m_bPickedup = true;

          
        }

        public void DecreaseSpeed()
        {
            m_CurrentVelo = 40;
            m_bPickedup = true;
        }

        public void IncreaseWind()
        {
            Random rnd = new Random();
            m_FinalWindForce = (float)(rnd.NextDouble() * 25) + 250f; //gives a good wind boost
            m_WindState = WindState.BuildUp;

        }

        public float ReduceWind()
        {
            Random rnd = new Random();
            float val = (float)(rnd.NextDouble() * 25.0) +25f;
            m_FinalWindForce -= val; //slows player down
            m_Twirl = true;
            return val;
        }

        #endregion Pickup Boosts    

        #region Input
        void HandleInput(RenderContext context)
        {
            var keyState = Keyboard.GetState();
            

            // See if GamePad is connected and use that
            if (context.PadState.IsConnected)
            {
                if( context.PadState.ThumbSticks.Left.X != 0f)
                {
                    m_Rotation -= context.PadState.ThumbSticks.Left.X *m_TurnAmount;
                }
                else if (context.PadState.IsButtonDown(Buttons.DPadLeft))
                {
                    m_Rotation += m_TurnAmount;
                    m_GameIsReallyStarted = true;
                    base.Model = m_Curved;
                }
                else if (context.PadState.IsButtonDown(Buttons.DPadRight))
                {
                    m_Rotation -= m_TurnAmount;
                    m_GameIsReallyStarted = true;
                    base.Model = m_Curved;
                }
            }
            else
            {
                if (keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A) || keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Left))
                {
                    m_Rotation += m_TurnAmount;
                    m_GameIsReallyStarted = true;
                    base.Model = m_Curved;
                }

                else if (keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D) || keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.Right))
                {
                    m_Rotation -= m_TurnAmount;
                    m_GameIsReallyStarted = true;
                    base.Model = m_Curved;
                }
                else
                    base.Model = m_Flat;
            }

        }
        #endregion

        #region Movement
        void Movement(float deltaTime)
        {
            if (m_GameIsReallyStarted == true)
            {
                var windVec = Wind(deltaTime);
                windVec -= RigidBody.LinearVelocity;
                var dot = windVec.X * (float)Math.Cos(RigidBody.Rotation + Math.PI / 2);
                dot += windVec.Y * (float)Math.Sin(RigidBody.Rotation + Math.PI / 2);

                m_Velocity += m_Multplier * new Vector2(dot * (float)Math.Cos(RigidBody.Rotation + Math.PI / 2)
                                                       , dot * (float)Math.Sin(RigidBody.Rotation + Math.PI / 2));
            }
            else
            {
                Position = m_OrginalPos;
                m_Velocity = Vector2.Zero;
            }
        }

        float camRot = 0f;
        void Twirl(float delta, RenderContext context)
        {
            m_TwirlTime += delta;
            if (m_TwirlTime > m_MaxTwirlTime)
            {
                m_Twirl = false;
                m_TwirlTime = 0f;
            }
            else
            {
                //makes the leaf twirl less and less 
                float twirlAmount = (m_MaxTwirlTime - (m_MaxTwirlTime * (m_TwirlTime / m_MaxTwirlTime))) / m_TwirlDevider; 
                m_Rotation += twirlAmount;

                if (m_CameraCanRotate)
                {
                    camRot += 0.025f;
                    Vector3 up = Vector3.Up;
                    float lerp = MathHelper.Lerp(up.X, up.X + (float)Math.Sin(System.Convert.ToDouble(camRot)), 0.1f);
                    up.X = lerp;
                    context.Camera.SetUpVector(up);
                }
            }
        }

        void RandomWind(float delta)
        {
            var xmlPlayerData = XmlLoader.Load<PlayerSettings>("PlayerSettings");
            switch (m_WindState)
            {
                case WindState.NoWind:
                    break;
                case WindState.BuildUp:
                    if (m_CurrentWindForce < m_FinalWindForce)
                    {
                        m_CurrentWindForce += m_CurrentIncrement;
                    }
                    else
                        m_WindState = WindState.BuildOff;

                    break;
                case WindState.BuildOff:
                    if (m_CurrentWindForce > m_MinHorizontalWind) 
                        m_CurrentWindForce -= m_CurrentIncrement;
                    else
                    {
                        m_CurrentWindForce = m_MinHorizontalWind; 
                        m_WindState = WindState.NoWind;
                    }
                    break;
            }

        }

        Vector2 Wind(float delta)
        {
            
            Vector2 wind;
            if (!PickUpFromGround)
            {
                wind = new Vector2(m_CurrentWindForce, -0.4f - 0.02f * (Position.Y / 25));
                if (wind.Y < 4f) wind = new Vector2(wind.X, 4f);
            }
            else //Get Sucked in or pushed out
            {
                wind = new Vector2(0f, m_VulcanoPower * PullDirection);
                m_PickupFromGroundTime += delta;
                if (m_PickupFromGroundTime > m_PickupFromGroundMaxTime)
                {
                    m_PickupFromGroundTime = 0;
                    PickUpFromGround = false;
                }
            }

            var xmlPlayerData = XmlLoader.Load<PlayerSettings>("PlayerSettings");
            if (xmlPlayerData.windX != -1f) wind = new Vector2(xmlPlayerData.windX, wind.Y);
            if (xmlPlayerData.windY != -1f) wind = new Vector2(wind.X, xmlPlayerData.windY);

            return wind;
        }

        #endregion


        public void TakeHealth()
        {
            m_Context.Game.MainScene.PlayerHit = true;

            if (!m_bInvincible && Health > 0)
            {
                Health--;
                m_Chain[Health].Active = false;
              
                m_bInvincible = true;
            }

        }

        public void AddHealth()
        {
            if (Health < m_MaxHealth)
            {
                Health++;
                m_Chain[Health - 1].Active = true;
            }
        }

        public void DisplaySettings(RenderContext context)
        {
            TextRenderer.DrawText("Mulitplier: " + m_Multplier + 
                                    "\nTurn Amount: " + m_TurnAmount + 
                                    "\nIncrement: " + m_CurrentIncrement +
                                    "\nVelocity: " + m_Velocity +
                                    "\nPosition: " + Position
                                    ,context.ViewPortSize.X * 0.75f, context.ViewPortSize.Y * 0.05f, Color.White, context);
        }

        public override void Draw(RenderContext renderContext)
        {

            foreach (Chain chain in m_Chain)
            {
                if (chain.Active)
                    chain.Draw(renderContext);
            }
            base.Draw(renderContext);
        }

        public void DrawChains(RenderContext renderContext)
        {
            foreach (Chain chain in m_Chain)
            {
                if(chain.Active)
                    chain.Draw(renderContext);
            }
        }

        void CameraZoom(RenderContext context)
        {
            float currentZ = context.Camera.Position.Z;
            float lerpAmount = 0.04f;
            float newZ = 500 + Math.Abs(m_Velocity.Length()) * 2;

            context.Camera.Position.Z = MathHelper.Lerp(currentZ, newZ, lerpAmount);
        }     
    }
};
