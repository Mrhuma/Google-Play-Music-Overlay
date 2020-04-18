using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace MrhumasMusicOverlay
{
    public class Settings
    {
        private string _backgroundColor;
        private string _foregroundColor;
        private MusicSources _musicSource;
        private string _spotifyClientID;
        private string _spotifyAccessToken;

        public string BackgroundColor { get { return _backgroundColor; } set { _backgroundColor = value; } }
        public string ForegroundColor { get { return _foregroundColor; } set { _foregroundColor = value; } }
        public MusicSources MusicSource { get { return _musicSource; } set { _musicSource = value; } }
        public string SpotifyClientID { get { return _spotifyClientID; } set { _spotifyClientID = value; } }
        public string SpotifyAccessToken { get { return _spotifyAccessToken; } set { _spotifyAccessToken = value; } }

        static readonly string settingsFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Mrhuma\'s Music Overlay\\";
        static readonly string settingsFile = "Settings.json";

        public enum MusicSources { Google, Spotify}

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
