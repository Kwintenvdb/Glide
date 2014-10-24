using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.ComponentModel;

using Microsoft.Xna.Framework;
namespace gameProject
{
    //This class generates the level
    class LevelManager
    {       

        RenderContext m_Context;
        List<Level> m_LevelList = new List<Level>(); // Contains all pieces of a level
        List<Level> m_Level = new List<Level>(); // List hat is generated based on all level Pieces
        List<Level> m_Generate = new List<Level>(); // Stores all levels generated in the 
        public bool m_bUpdate = true;
        float m_LevelOffset = 1.3f;

        Random m_RandGenerator;

        int m_AmountPieces;

        BackgroundWorker m_Worker;
        bool m_bIsThreading = false;

        public LevelManager()
        {
            

        }        

        private void bgw_DoWork(object sender, DoWorkEventArgs e)
        {            
            //Run the Work Event
             GenerateLevel((int)e.Argument, m_Context);
             Console.WriteLine("Running Event");        
                       
        }

        private void m_Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                Console.WriteLine("Error: " + e.Error.ToString());
            }
            else
            {
                //Work Completed Succesfully
                Console.WriteLine("Close Thread");
            }
        }

        public void Generate(int size, RenderContext context)
        {
            m_Context = context;

            m_Worker = new BackgroundWorker();
            
            m_Worker.WorkerReportsProgress = true;
            m_Worker.WorkerSupportsCancellation = true;
            
            m_Worker.DoWork += bgw_DoWork; //Work Event
            m_Worker.RunWorkerCompleted += m_Worker_RunWorkerCompleted;

            //Event that is run in the background;
            Console.WriteLine("Opening new Thread");
            m_Worker.RunWorkerAsync(size);

            m_Worker.CancelAsync();

            m_Worker.Dispose();


        }              

        public void Initialize(RenderContext context)
        {

            m_RandGenerator = new Random(2250);

            if (m_AmountPieces == 0)
            {
                //Add Possible Grounds to the list
                GroundPrefabList.AddPrefab(new GroundPrefab(context, Collision.Col1, "Model/Static/m_Piece1", "Textures/Collision/c_Piece1"));
                GroundPrefabList.AddPrefab(new GroundPrefab(context, Collision.Col2, "Model/Static/m_Piece2", "Textures/Collision/c_Piece2"));
                GroundPrefabList.AddPrefab(new GroundPrefab(context, Collision.Col3, "Model/Static/m_Piece3", "Textures/Collision/c_Piece3"));
                GroundPrefabList.AddPrefab(new GroundPrefab(context, Collision.Col4, "Model/Static/m_Piece4", "Textures/Collision/c_Piece4"));
                //GroundPrefabList.AddPrefab(new GroundPrefab(context, Collision.Col5, "Model/Static/m_Piece5", "Textures/Collision/c_Piece5")); //issue with collision texture
                GroundPrefabList.AddPrefab(new GroundPrefab(context, Collision.Col6, "Model/Static/m_Piece6", "Textures/Collision/c_Piece6"));
                GroundPrefabList.AddPrefab(new GroundPrefab(context, Collision.Col7, "Model/Static/m_Piece7", "Textures/Collision/c_Piece7"));
                GroundPrefabList.AddPrefab(new GroundPrefab(context, Collision.Col8, "Model/Static/m_Piece8", "Textures/Collision/c_Piece8"));


                //Add all pieces to the list and initialize them

                m_LevelList.Add(new Level_0(context));
                m_LevelList.Add(new Level_1(context));
                m_LevelList.Add(new Level_2(context));
                m_LevelList.Add(new Level_3(context));

                m_AmountPieces = m_LevelList.Count();


            }

            //always the same startpieces
            m_Level.Add(m_LevelList[0]);
            m_Level[0].Initialize(context);
            GenerateLevel(50,context);                        
        }     

        public void GenerateLevel(int size,RenderContext context)
        {
            if (size <= 0)
                return;

            m_bIsThreading = true;

            int index = m_Level.Count();
            Level prev;           
            prev = m_Level[index - 1];
           

            for (int i = 0; i < size; ++i)
            {
                //GetType of the random piece and instantiate it
                var newPiece= m_LevelList[m_RandGenerator.Next(1,m_AmountPieces)].GetType();
                var level = (Level)Activator.CreateInstance(newPiece,context);

                //level = ObjectCopier.Clone<Level>(m_LevelList[m_RandGenerator.Next(1, m_AmountPieces)]);
                ////Initialze the new Piece
                level.Initialize(context);                 

                level.Translate(new Vector2((prev.Position.X + (prev.Width * m_LevelOffset)), level.Position.Y));

                //previous level is current level
                prev = level;
                index++;

                ////Add it to the level
                m_Level.Add((level));
            }

            //Done Generating

            m_bIsThreading = false;
           
        }
        
        public void Update(RenderContext context)
        {
            foreach (Level level in m_Level.ToList<Level>())
            {                
                level.Update(context,m_bIsThreading);
            }            
        }

        //Draws all Levels in the list
        public void Draw(RenderContext context)
        {
            foreach (Level level in m_Level.ToList<Level>())              
                level.Draw(context);
        }

        public void ClearLevel()
        {
            m_LevelList.Clear();
        }
       
    }
}
