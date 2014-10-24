using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace gameProject
{
   public class HealthBar
    {
        int m_Amount = 5, m_MaxHealth = 5;
        List<GameSprite> m_HealthBar = new List<GameSprite>();
        GameSprite m_Health;

        public HealthBar(RenderContext context)
        {

            for (int i = 0; i < m_Amount; i++)
            {
                m_Health = new GameSprite("Textures/GUI/Health_GUI",context);
                m_Health.Scale(0.5f);
                m_HealthBar.Add(m_Health);
            }
        }

        public bool RemoveHealth()
        {
            //don't lose health when below 1 but return true to the scne so that player dies/resets/...
            if (m_Amount <= 1)
                return true;

            //Set the position of the removed health sprite out of screen
            // Remove Child works, but no way to add another child because it won't get initialized
            m_Amount--;
            m_HealthBar[m_Amount].Position -= new Microsoft.Xna.Framework.Vector2(0, 200);

            return false;
        }

        public void AddHealth()
        {

            //Don't add health when at max health;
            if (m_Amount >= m_MaxHealth)
                return;

            m_HealthBar[m_Amount].Position += new Microsoft.Xna.Framework.Vector2(0, 200);
            m_Amount++;
            Console.Write("Add Health " + m_Amount + "\n");

        }

        public void Draw(RenderContext context)
        {
            var width = m_Health.Width;
            int i = 0;
            foreach (GameSprite obj in m_HealthBar)
            {
                obj.Position = new Microsoft.Xna.Framework.Vector2(width * i, obj.Position.Y);
                obj.Draw(context);
                i++;
            }

        }

    }
}

