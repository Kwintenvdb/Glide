using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace gameProject
{
    class Level_0 : Level
    {
        public Level_0()
            : base()
        {
            //nothing to do here
        }

        public override void Initialize(RenderContext context)
        {
            Piece side = PiecePrefabList.GetPrefab(Collision.Col2,"Side");
            side.Rotate(0, 0, -90);            
            side.Translate(-200, 750);
            side.Scale(1f, 1,1);
            AddChild(side);

            Piece ground = PiecePrefabList.GetPrefab(Collision.Col5,"Ground");
            ground.Translate(-200, ground.Position.Y);
            AddChild(ground);

            //lots of volcanos at the begining of the level
            for (int i = 0; i < 2; ++i)
            {
                AddVolcano(ground.Height, ground.Position.Y, context);
            }

            Width = (int)ground.Width * 0.85f;
        }


    }
}
