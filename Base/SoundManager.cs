using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;

namespace gameProject
{
   
    //Stores the SoundEffect and The Instance in a single class for easy use
   class Sound
    {
        public SoundEffect Effect; //For Loading
        public SoundEffectInstance Instance; //For Playing
        public string Name; //Identifier

        public Sound(string soundName,RenderContext context)
        {
            //Load the File
            Effect = context.Content.Load<SoundEffect>("Sound/" + soundName);
            Name = soundName;
            Instance = Effect.CreateInstance();

        }

    }

    public static class SoundManager
    {
        static Dictionary<string, Sound> m_SoundList = new Dictionary<string, Sound>();
        public static float Volume = 1.0f;

        public static void AddSound(string soundFile,RenderContext context)
        {
            //check if exists and add it it doesn't
            if (!m_SoundList.ContainsKey(soundFile))
            {
                Sound s = new Sound(soundFile, context);
               
                m_SoundList.Add(s.Name, s);
            }
        }

        public static void Play(string key,bool loop = false)
        {
            if (m_SoundList.ContainsKey(key))
            {
                SoundEffectInstance s = m_SoundList[key].Instance;
                s.IsLooped = loop;
                s.Volume = Volume;
                s.Play();


                
            }
            else
                Console.WriteLine("Sound Not Found: " + key);
        }

        public static void Stop(string key)
        {
            if (m_SoundList.ContainsKey(key))
            {
                SoundEffectInstance s = m_SoundList[key].Instance;
                SoundEffect e = m_SoundList[key].Effect;

                s.Stop();
                
            }
            else
                Console.WriteLine("Sound Not Found: " + key);

        }

        public static void Pause(string key)
        {
            if (m_SoundList.ContainsKey(key))
            {
                SoundEffectInstance s = m_SoundList[key].Instance;
                s.Pause();
            }
            else
                Console.WriteLine("Sound Not Found: " + key);
        }

        
    }
}
