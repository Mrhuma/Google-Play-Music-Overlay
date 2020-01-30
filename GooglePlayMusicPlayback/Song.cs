namespace GooglePlayMusicOverlay
{
    class Song
    {
        private bool _Playing;
        private string _Title;
        private string _Artist;
        private string _albumArt;

        public bool Playing { get { return _Playing; } set { _Playing = value; } }
        public string Title { get { return _Title; } set { _Title = value; } }
        public string Artist { get { return _Artist; } set { _Artist = value; } }

        public string albumArt { get { return _albumArt; } set { _albumArt = value; } }

        public Song(bool playing, string title, string artist, string albumart)
        {
            _Playing = playing;
            _Title = title;
            _Artist = artist;
            _albumArt = albumart;
        }
    }
}
