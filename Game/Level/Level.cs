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
        Random randVal = new Random(Guid.NewGuid().GetHashCode());

        List<GameModel> m_Child = new List<GameModel>(); // all the different parts of a level
        public bool IsInitialized = false;

        //The volcanos spawns are reduced in later generated levels
        static int NrLevels = 0;

        protected Vector2 ScreenSize;

        public float Width = 0;

        public Level()
        {
        }

        public virtual void Initialize(RenderContext context)
        {
            ScreenSize = context.ViewPortSize;            

            //Increase the number of levels we have generated so we can see where we are to make it more difficult
            NrLevels++;

            //Each level has a ground and top collision
            var t = PiecePrefabList.GetRandom(new Random(Guid.NewGuid().GetHashCode()), "Top");
            AddChild(t);

            //Change the Width of the level so they tile nicely
            var g = PiecePrefabList.GetRandom(new Random(Guid.NewGuid().GetHashCode()), "Ground");
            Width = g.Width * 0.85f;
            AddChild(g);

            int nrEnemies = 0;
            int nrPickups = 1;
            //Med add Every 3 levels
            if (NrLevels > 5 && NrLevels < 10)
            {
                nrEnemies = randVal.Next(0, 3);
                nrPickups = randVal.Next(0, 3);
            }
            else if(NrLevels > 10)
            {
                nrEnemies = randVal.Next(0, 3);
                nrPickups = randVal.Next(0, 3);
            }
            if (NrLevels < 15)
            {
                AddVolcano(g.Height, g.Position.Y, context);
            }
            else if (NrLevels >= 15 && NrLevels < 50)
            {
                if (NrLevels % 3 == 0)
                    AddVolcano(g.Height, g.Position.Y, context);
            }
            else if (NrLevels > 50)
            {
                if (NrLevels % 5 == 0)
                    AddVolcano(g.Height, g.Position.Y, context);
            }

            //Add A pickup
            for (int i = 0; i < nrPickups; ++i)
            {
                //Can return null
                var pickup = PickupPrefabList.GetRandom(new Random(Guid.NewGuid().GetHashCode()), (int)ScreenSize.Y);
                if (pickup != null)
                    AddChild(pickup);
            }
               

            //Add trees
            for (int i = 0; i < 3; ++i)
            {
                var tree = TreePrefabList.GetRandom(new Random(Guid.NewGuid().GetHashCode()), (int)ScreenSize.Y);
                if (tree != null)
                {
                    var rnd = new Random(Guid.NewGuid().GetHashCode());
                    tree.Translate(tree.Position.X + rnd.Next(0, 1700), tree.Position.Y - 460.0f);
                    AddChild(tree);
                }
            }
 
            for (int i = 0; i < nrEnemies; ++i)
            {
                var enemy = EnemyPrefabList.GetRandom(new Random(Guid.NewGuid().GetHashCode()), (int)ScreenSize.Y);
                if (enemy != null)
                {
                    //if enemy is a smash we need 2 of them in opposite directions
                    if (enemy.GetType() == typeof(Smash))
                    {
                        Smash smashTop = EnemyPrefabList.GetPrefab<Smash>(EnemyName.Smash);
                        smashTop.Translate(enemy.Position.X, enemy.Position.Y + smashTop.m_Distance * 2);
                        //Set the directions
                        enemy.Create(1);
                        smashTop.Create(-1);
                        AddChild(smashTop);
                    }


                    if (enemy.GetType() == typeof(Wheel))
                    {
                        var rnd = new Random(Guid.NewGuid().GetHashCode());
                        enemy.Rotate(0, 0, 0);
                        enemy.Scale(1f, -1f, 1.0f);
                        enemy.Translate(rnd.Next(0, 1700), t.Position.Y + (t.Height * 0.9f));

                    }
                    if (enemy.GetType() == typeof(FallDown))
                    {
                        var rnd = new Random(Guid.NewGuid().GetHashCode());
                        enemy.Rotate(0, 180, 0);
                        enemy.Scale(2f,2f, 1.0f);
                        enemy.Translate(rnd.Next(0, 1700), t.Position.Y + (t.Height * 0.7f));
                    }
                    AddChild(enemy);
                }
            }

            //Set Everything to default if model or textures are missing
            foreach (GameModel model in m_Child.ToList<GameModel>())
                model.Initialize(context);

            IsInitialized = true;
        }

       protected void AddVolcano(float height, float pos,RenderContext context)
        {
            //Generate a windVolcano
            var volcano = PickupPrefabList.GetPrefab<WindVulcano>(PickupName.WindVulcano);
            volcano.Create(context);
            var rnd = new Random(Guid.NewGuid().GetHashCode());
            volcano.Rotate(0, 0, 0);
            volcano.Scale(2.5f, 7.0f, 1.0f);
            volcano.Translate(rnd.Next(0, 1700), pos - (height * 1.3f));

            if (volcano != null)
                AddChild(volcano);
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

        public virtual void Draw3D(RenderContext context)
        {
            foreach (GameModel model in m_Child)
                if (model.GetType() != typeof(Pickup)) model.Draw(context);
        }
        public virtual void Draw2D(RenderContext context)
        {
            foreach (GameModel model in m_Child)
                if (model.GetType() == typeof(Pickup)) model.Draw(context);
        }

        public virtual void Update(RenderContext context, bool bThreading)
        {
            //update all models in the level
            foreach (GameModel model in m_Child.ToList<GameModel>())
            {
                if (!bThreading && !model.RigidBody.Enabled)
                    model.RigidBody.Enabled = true;

                //if it is a pick up, typecast it to a Pickup
                if (model.Name == "Pickup")
                {
                    var pickup = (Pickup)model;
                    if (pickup.isPickedUp)
                    {
                        //Do Pick up behaviour
                        pickup.OnPickup();

                        if (pickup.GetType() != typeof(WindVulcano) && pickup.GetType() != typeof(WindDownPickup))
                        {
                            context.Game.MainScene.PickupHit = true;
                            RemoveChild(pickup);
                            pickup = null;
                        }

                        if (pickup != null)
                        {
                            if (pickup.GetType() == typeof(WindDownPickup))
                            {
                                context.Game.MainScene.PlayerHit = true;
                                RemoveChild(pickup);
                                pickup = null;
                            }
                        }
                    }
                }

                //likewise for enemies
                if (model.Name == "Enemy")
                {
                    var enemy = (Enemy)model;
                    if (enemy.hasHit)
                    {
                        //enemy hit behavior
                        enemy.OnHit();
                    }
                }

                model.Update(context);
            }

        }

        public void Translate(Vector2 translation)
        {
            //Translates the position o the models
            foreach (GameModel model in m_Child)
                model.Translate(model.Position + translation);

            Position = translation;
        }

        public void Clear()
        {
            NrLevels = 0;
            foreach (GameModel model in m_Child.ToList<GameModel>())
            {
                model.RigidBody.Dispose();
            }
        }
    }
}