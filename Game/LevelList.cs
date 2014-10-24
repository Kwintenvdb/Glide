using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Graphics;


namespace gameProject
{

    //Create a new class that inherits from this and add all possible things to it
    // ex: a Level can contain the ground, trees, pickup,..
    class Level
    {
        public Vector2 Position;

        protected Random m_RandomGenerator;

        List<GameModel> m_Child = new List<GameModel>(); // all the different parts of a level

        public bool IsInitialized = false;

        public int Width = 0;

        public Level(RenderContext renderContext)
        {
            m_RandomGenerator = new Random();
        }

        public virtual void Initialize(RenderContext context)
        {
            IsInitialized = true;

            //Load All Possible Pickups here
            PickupPrefabList.AddPrefab(new PickupPrefab(context,PickupName.Health, "Model/Dynamic/Player_Model"));
        }

        protected void AddChild(GameModel child)
        {
            if (child != null)
                m_Child.Add(child);
        }

        protected void RemoveChild(GameModel child)
        {
            m_Child.Remove(child);
            child = null;

        }

        public virtual void Draw(RenderContext context)
        {
            foreach (GameModel model in m_Child)
                model.Draw(context);
        }

        public virtual void Update(RenderContext context, bool bThreading)
        {
            //update all models in the level
            foreach (GameModel model in m_Child.ToList<GameModel>())
            {

                if (!bThreading && !model.RigidBody.Enabled)
                {
                    model.RigidBody.Enabled = true;
                }

                //if it is a pick up, typecast it to a Pickup
                if(model.Name == "Pickup")
                {
                    var pickup = (Pickup)model;
                    if (pickup.isPickedUp)
                    {
                        //Do Pick up behaviour
                        pickup.OnPickup();

                        RemoveChild(pickup);
                        pickup = null;
                    }

                }

                model.Update();
            }

        }

        public void Translate(Vector2 translation)
        {
            //Translates the position o the models
            foreach (GameModel model in m_Child)
                model.Translate(model.Position + translation);


            Position = translation;
        }

        

    }
        
    //This file contains all possible levels, to avoid redundant cluttering
    class Level_0:Level
    {
        public Level_0(RenderContext context)
            : base(context)
        {
            
        }

        public override void Initialize(RenderContext context)
        {
            Ground side = GroundPrefabList.GetPrefab(Collision.Col1);;
            side.Rotate(0, 0, -90);
            side.Translate(500, 750);
            AddChild(side);

            var size = (Vector2)side.RigidBody.UserData;
            Width = (int)size.Y;
            base.Initialize(context);
        }
      

    }

    class Level_1 : Level
    {
        public Level_1(RenderContext context)
            : base(context)
        {

        }

        public override void Initialize(RenderContext context)
        {

            Ground ground = GroundPrefabList.GetPrefab(Collision.Col1);
            ground.Translate(0, 0);
            AddChild(ground);

            Ground Top = GroundPrefabList.GetPrefab(Collision.Col1);
            Top.Rotate(0, 0, 180);
            Top.Translate(1200, 300);
            AddChild(Top);

            Pickup pickup = PickupPrefabList.GetPrefab<HealthPickup>(PickupName.Health);

            pickup.Translate(1200, 150);
            AddChild(pickup);

            var size = (Vector2)ground.RigidBody.UserData;
            Width = (int)size.X;
            base.Initialize(context);
        }

    }

    class Level_2 : Level
    {
        public Level_2(RenderContext context)
            : base(context)
        {

        }

        public override void Initialize(RenderContext context)
        {

            Ground ground = GroundPrefabList.GetPrefab(Collision.Col4);
            ground.Translate(0, 0);
            AddChild(ground);

            Ground Top = GroundPrefabList.GetPrefab(Collision.Col2);
            Top.Rotate(0, 0, 180);
            Top.Translate(1200, 300);
            AddChild(Top);

            var size = (Vector2)ground.RigidBody.UserData;
            Width = (int)size.X;
            base.Initialize(context);
        }

    }

    class Level_3 : Level
    {
        public Level_3(RenderContext context)
            : base(context)
        {

        }

        public override void Initialize(RenderContext context)
        {

            Ground ground = GroundPrefabList.GetPrefab(Collision.Col7);
            ground.Translate(0, 0);
            AddChild(ground);

            Ground Top = GroundPrefabList.GetPrefab(Collision.Col6);
            Top.Rotate(0, 0, 180);
            Top.Translate(1200, 300);
            AddChild(Top);

            var size = (Vector2)ground.RigidBody.UserData;
            Width = (int)size.X;
            base.Initialize(context);
        }

    }
}
