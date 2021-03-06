﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics;

namespace gameProject
{
    class Player:GameModel
    {
        //Movement
        float m_Rotation = 0, m_TurnAmount = 3.0f;
        Vector2 m_Direction = Vector2.Zero
              , m_OrginalPos = Vector2.Zero;

        Vector2 m_Velocity
        {
            get { return RigidBody.LinearVelocity; }
            set { RigidBody.LinearVelocity = value; }
        }    
        
        public HealthBar HealthBar;

        //Wind
        enum WindState { NoWind, BuildUp, BuildOff }
        WindState m_WindState = WindState.NoWind;
        float m_WindTime = 0,
              m_WindTimePause = 5,
              m_FinalWindForce = 0,
              m_CurrentWindForce = 0,
              m_Multplier =  1f, 
              m_CurrentIncrement = 0.05f;
        Vector2 m_WindDirection = Vector2.Zero;


        bool m_bInput = true;

        public Player(string assetFile, RenderContext context, Vector2 originalPos)
            : base(assetFile, "Textures/Collision/c_Piece1", context)
        {
            m_OrginalPos = originalPos;
            //Create rigidBody        
            RigidBody = BodyFactory.CreateRectangle(context.World, ConvertUnits.ToSimUnits(20), ConvertUnits.ToSimUnits(10), 1.0f);
            RigidBody.BodyType = BodyType.Dynamic;
            RigidBody.OnCollision += RigidBody_OnCollision;
            RigidBody.OnSeparation +=RigidBody_OnSeparation;
            RigidBody.UserData = this;  
            RigidBody.Restitution = 0.1f;
            Depth = -200;

            //Xml
            var xmlPlayerData = XmlLoader.Load<PlayerSettings>("PlayerSettings");
            m_Multplier = xmlPlayerData.multiplier;
            m_TurnAmount = xmlPlayerData.turnAmount;
            m_CurrentIncrement = xmlPlayerData.windIncrement;

            m_Velocity = Vector2.Zero;
           
        }

        private void RigidBody_OnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
        }

        bool RigidBody_OnCollision(Fixture fixtureA, Fixture fixtureB, FarseerPhysics.Dynamics.Contacts.Contact contact)
        {
            HealthBar.RemoveHealth();


            return true;
        }

        public void Update(RenderContext context)
        {
            float deltaTime = (float)context.GameTime.ElapsedGameTime.Milliseconds / 100;
            HandleInput();

            if (m_bInput)
            {
                RandomWind(deltaTime);
                Movement(deltaTime);
            }
            
            Rotate(0, 0, m_Rotation);
            base.Update();
        }

        void Movement(float deltaTime)
        {
            var windVec = Wind();
            windVec -= RigidBody.LinearVelocity;

            var dot = windVec.X * (float)Math.Cos(RigidBody.Rotation + Math.PI / 2);
            var sin = (float)Math.Sin(RigidBody.Rotation + Math.PI / 2);
            dot += windVec.Y * (float)Math.Sin(RigidBody.Rotation + Math.PI / 2);

            m_Velocity += m_Multplier *  new Vector2(dot * (float)Math.Cos(RigidBody.Rotation + Math.PI / 2), dot * (float)Math.Sin(RigidBody.Rotation + Math.PI / 2));

        }

        void HandleInput()
        {
            var keyState = Keyboard.GetState();
            var padState = GamePad.GetState(PlayerIndex.One,GamePadDeadZone.Circular);

            // See if GamePad is connected and use that

            if (padState.IsConnected && (padState.ThumbSticks.Right.Y != 0f && padState.ThumbSticks.Right.X != 0f)) 
            {
                    m_Rotation = (float)Math.Atan2(padState.ThumbSticks.Right.Y, padState.ThumbSticks.Right.X);
                    m_bInput = true;
            }
            else
            {
                if (keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.A))
                {
                    m_Rotation += m_TurnAmount;
                    m_bInput = true;
                }

                else if (keyState.IsKeyDown(Microsoft.Xna.Framework.Input.Keys.D))
                {
                    m_Rotation -= m_TurnAmount;
                    m_bInput = true;
                }
            }

        }

        void RandomWind(float delta)
        {
            var xmlPlayerData = XmlLoader.Load<PlayerSettings>("PlayerSettings");
            switch (m_WindState)
            {
                case WindState.NoWind:
                    m_WindTime += delta;
                    if (m_WindTime >= m_WindTimePause)
                    {
                        m_WindTime = 0f;
                        Random rnd = new Random();
                        if (xmlPlayerData.timeBetweenWindBursts == -1) m_WindTimePause = rnd.Next(1, 15);
                        else m_WindTimePause = xmlPlayerData.timeBetweenWindBursts;
                        m_WindState = WindState.BuildUp; //initialise wind burst
                        m_FinalWindForce = (float)(rnd.NextDouble() * 10.0); 
                        m_WindDirection.X = (float)(rnd.Next(2)) - 1;
                        m_WindDirection.Y = 0f;
                    }
                    break;
                case WindState.BuildUp:
                    if (m_CurrentWindForce < m_FinalWindForce)
                    {
                        m_CurrentWindForce += m_CurrentIncrement;
                       // m_Rotation += m_TurnAmount;
                    }
                    else
                        m_WindState = WindState.BuildOff;

                    break;
                case WindState.BuildOff:
                    if (m_CurrentWindForce > 0.1f) //0.1 so there's always some wind
                        m_CurrentWindForce -= m_CurrentIncrement;
                    else
                    {
                        m_CurrentWindForce = 0.1f; 
                        m_WindState = WindState.NoWind;
                    }
                    break;
            }

        }

        Vector2 Wind()
        {
            Vector2 wind = new Vector2( m_Multplier * m_FinalWindForce * m_WindDirection.X, 2f * (Position.Y / (m_OrginalPos.Y / 2))); //was -5
            var xmlPlayerData = XmlLoader.Load<PlayerSettings>("PlayerSettings");
            if (xmlPlayerData.windX != -1f) wind = new Vector2(xmlPlayerData.windX, wind.Y);
            if (xmlPlayerData.windY != -1f) wind = new Vector2(wind.X, xmlPlayerData.windY);
            return wind;
        }


        public void DisplaySettings(RenderContext context)
        {

            TextRenderer.DrawText("Mulitplier: " + m_Multplier + 
                                    "\nTurn Amount: " + m_TurnAmount + 
                                    "\nIncrement: " + m_CurrentIncrement +
                                    "\nVelocity: " + m_Velocity +
                                    "\nWindDirection: " + m_WindDirection                                    
                                    ,context.ViewPortSize.X * 0.75f, context.ViewPortSize.Y * 0.05f, Color.White, context);
        }


        
    }
};
