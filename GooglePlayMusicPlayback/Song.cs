namespace GooglePlayMusicPlayback
{
    class Song
    {
        private bool _Playing;
        private string _Title;
        private string _Artist;
        private string _Album;

        public bool Playing { get { return _Playing; } set { _Playing = value; } }
        public string Title { get { return _Title; } set { _Title = value; } }
        public string Artist { get { return _Artist; } set { _Artist = value; } }
        public string Album { get { return _Album; } set { _Album = value; } }

        public Song(bool playing, string title, string artist, string album)
        {
            _Playing = playing;
            _Title = title;
            _Artist = artist;
            _Album = album;
        }
    }
}
