using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GooglePlayMusicOverlay
{
    public class Settings
    {
        private string _backgroundColor;
        private string _foregroundColor;
        private double _xCoord;
        private double _yCoord;

        public string BackgroundColor { get { return _backgroundColor; } set { _backgroundColor = value; } }
        public string ForegroundColor { get { return _foregroundColor; } set { _foregroundColor = value; } }
        public double XCoord { get { return _xCoord; } set { _xCoord = value; } }
        public double YCoord { get { return _yCoord; } set { _yCoord = value; } }

        static readonly string settingsFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Google Play Music Overlay\\";
        static readonly string settingsFile = "Settings.json";

        //Returns if the file exists or not
        public static bool CheckForSettingsFile()
        {
            return File.Exists(settingsFolder + settingsFile);
        }

        //Create an empty settings file
        public static void CreateSettingsFile()
        {
            Directory.CreateDirectory(settingsFolder);
            using (FileStream fs = File.Create(settingsFolder + settingsFile))
            {
                fs.Dispose();
            }
        }

        //Write to the settings file
        public static void WriteToFile(Settings settings)
        {
            string json = JsonConvert.SerializeObject(settings, Formatting.Indented);
            File.WriteAllText(settingsFolder + settingsFile, json);
        }

        //Read from the settings file
        public static Settings ReadFromFile()
        {
            string json = File.ReadAllText(settingsFolder + settingsFile);
            return JsonConvert.DeserializeObject<Settings>(json);
        }
    }
}
