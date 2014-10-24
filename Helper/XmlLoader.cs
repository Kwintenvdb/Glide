using System;
using Microsoft.Xna.Framework.Storage;
using System.Xml.Serialization;
using System.IO;

namespace gameProject
{
    public struct SaveData
    {
        [XmlElement(typeof(int))]
        public int HighestDistance;
    }

    //Screensize and stuff
    public struct WorldSettings
    {
        [XmlElement(typeof(int))]
        public int ScreenWidth;

        [XmlElement(typeof(int))]
        public int ScreenHeight;

        [XmlElement(typeof(bool))]
        public bool Fullscreen;
    }

    public struct PlayerSettings
    {
        [XmlElement(typeof(float))]
        public float multiplier;

        [XmlElement(typeof(float))]
        public float startPosX;

        [XmlElement(typeof(float))]
        public float startPosY;

        [XmlElement(typeof(float))]
        public float turnAmount;

        [XmlElement(typeof(float))]
        public float timeBetweenWindBursts;

        [XmlElement(typeof(float))]
        public float windIncrement;

        [XmlElement(typeof(float))]
        public float finalWindForce; 

        [XmlElement(typeof(float))]
        public float windDirectionX;

        [XmlElement(typeof(float))]
        public float windDirectionY;

        [XmlElement(typeof(float))]
        public float windX;

        [XmlElement(typeof(float))]
        public float windY;

        [XmlElement(typeof(float))]
        public float minWindSpeedX;

        [XmlElement(typeof(bool))]
        public bool toggleTwirl;

    }

    public struct BloomData
    {
        [XmlElement(typeof(float))]
        public float Tresh;

        [XmlElement(typeof(float))]
        public float Blur;

        [XmlElement(typeof(float))]
        public float Bloom;

        [XmlElement(typeof(float))]
        public float Base;

        [XmlElement(typeof(float))]
        public float BloomSat;

        [XmlElement(typeof(float))]
        public float BaseSat;
    }
    
    class XmlLoader
    {
        static public T Load<T>(string fileName)
        {
            T data = Activator.CreateInstance<T>();

            var path = Path.Combine("Content/XML/" + fileName + ".xml");
            
            //error if not found
            if (!File.Exists(path))
                throw new FileLoadException();
             
            //open the file
            FileStream stream = File.Open(path, FileMode.Open, FileAccess.ReadWrite);
                      
            //Read data(This is magic)
            XmlSerializer reader = new XmlSerializer(typeof(T));
            data = (T)reader.Deserialize(stream);

            stream.Close();
            return data;
        }
    }

    class XmlWriter
    {
        static public void SaveScore(SaveData data, string fileName)
        {
            FileStream file = File.Open(fileName, FileMode.OpenOrCreate);

            try
            {
                XmlSerializer serializer = new XmlSerializer(typeof(SaveData));
                serializer.Serialize(file, data);
            }
            finally
            {
                
            }

            file.Close();
        }
    }
}