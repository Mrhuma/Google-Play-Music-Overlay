namespace MrhumasMusicOverlay
{
    public class Song
    {
        private string _Title;
        private string _Artist;
        private string _albumArt;
        public string Title { get { return _Title; } set { _Title = value; } }
        public string Artist { get { return _Artist; } set { _Artist = value; } }

        public string albumArt { get { return _albumArt; } set { _albumArt = value; } }

        public Song(string title, string artist, string albumart)
        {
            _Title = title;
            _Artist = artist;
            _albumArt = albumart;
        }
    }
}
