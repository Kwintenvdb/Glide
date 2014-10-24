using System.Collections.Generic;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Common;
using FarseerPhysics.Factories;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarseerPhysics.Common.Decomposition;

namespace gameProject
{
    class BodyFromTexture
    {
        public static void CreateBodyFromImage(
            ref Body body,Game game, 
            World world,Texture2D texture,
            float scaleX = 1.0f,float scaleY = 1.0f)
        {
            //Load the passed texture.
            Texture2D polygonTexture 
                = texture;
            
            //Use an array to hold the textures data.
            uint[] data 
                = new uint[polygonTexture.Width
                    * polygonTexture.Height];

            //Transfer the texture data
            polygonTexture.GetData(data);

            //Find the verticals that make up the outline
            Vertices vertices 
                = PolygonTools.CreatePolygon(data,
                polygonTexture.Width);
                
            Vector2 scale = new 
                Vector2(scaleX * 1.6f
                , scaleY * -2.5f);
            vertices.Scale(ref scale);

            //Partition the concave polygon into a convex one.
            var decomposedVertices = Triangulate.ConvexPartition(vertices, 
                TriangulationAlgorithm.Bayazit);

            //Create a single body
            body =  
                BodyFactory.CreateCompoundPolygon(world, 
                decomposedVertices,1.0f);
            

        }
    }
}
