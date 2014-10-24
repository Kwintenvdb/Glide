using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace gameProject
{
   static class MathHelp
    {
       public static void Clamp<T>(ref T value, T hi, T lo) where T : IComparable<T>
       {
           if (value.CompareTo(hi) > 0)
               value = hi;

           if (value.CompareTo(lo) < 0)
               value = lo;
       }

       public static float RandomBetween(float min, float max)
       {
           Random random = new Random(Guid.NewGuid().GetHashCode());
           return min + (float)random.NextDouble() * (max - min);
       }

      public static Vector2 WorldToScreen(Vector2 inWorld,RenderContext context)
       {
           return Vector2.Transform(inWorld, context.Camera.View);
       }

      public static Vector2 ScreenToWorld(Vector2 inWorld, RenderContext context)
       {
           return Vector2.Transform(inWorld, Matrix.Invert(context.Camera.View));
       }
       
    }
}
