using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;

namespace MrhumasMusicOverlay
{
    public class SpotifyAPI
    {
        private SpotifyWebAPI api;

        //Authenticate the program
        public void Authenticate(Settings settings)
        {
            //Hide the ClientID from the repo
#if(DEBUG)
            string clientID = System.IO.File.ReadAllText("..\\..\\SpotifyClientID.txt");
#else
            string clientID = System.IO.File.ReadAllText("SpotifyClientID.txt");
#endif

            ImplicitGrantAuth auth = new ImplicitGrantAuth(
            clientID,
            "http://localhost:4002",
            "http://localhost:4002",
            Scope.UserReadPlaybackState
            );

            auth.AuthReceived += async (sender, payload) =>
            {
                auth.Stop(); // `sender` is also the auth instance

                //Save the Access Token for future use
                settings.SpotifyAccessToken = payload.AccessToken;
                Settings.WriteToFile(settings);
            };

            auth.Start(); // Starts an internal HTTP Server
            auth.OpenBrowser();
        }

        public void Connect(string accessToken)
        {
            api = new SpotifyWebAPI()
            {
                TokenType = "Bearer",
                AccessToken = accessToken
            };
        }
        
        public async Task<Song> GetCurrentSong()
        {
            try
            {
                PlaybackContext playback = await api.GetPlayingTrackAsync().ConfigureAwait(true);
                FullTrack track = playback.Item;
                string artistName = "";

                foreach (SimpleArtist artist in track.Artists)
                {
                    artistName += artist.Name;

                    //If it isn't the last artist
                    if (artist != track.Artists[track.Artists.Count - 1])
                    {
                        artistName += ", ";
                    }
                }
                return new Song(track.Name, artistName, track.Album.Images[0].Url);
            }
            catch(Exception) { return new Song("", "", ""); }
        }
        

        public void Disconnect()
        {
            api.Dispose();
        }
    }
}
