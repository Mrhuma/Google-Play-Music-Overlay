using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using SpotifyAPI.Web;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;

namespace MrhumasMusicOverlay
{
    public class SpotifyAPI
    {
        public bool authorized = false;
        private SpotifyWebAPI api;
        private TokenSwapWebAPIFactory webApiFactory;

        //Authenticate the program
        public async Task Authenticate()
        {
            //The server side php code came from here: https://github.com/rollersteaam/spotify-token-swap-php
            webApiFactory = new TokenSwapWebAPIFactory("http://mrhumagames.com/MrhumasMusicOverlay/index.php")
            {
                Scope = Scope.UserReadCurrentlyPlaying,
                AutoRefresh = true,
                HostServerUri = "http://localhost:4002/auth",
                Timeout = 30
            };

            //If the user denied the request
            webApiFactory.OnAuthFailure += (sender, e) =>
            {
                //Let the user know they must accept in order to use the program with Spotify
                MessageBox.Show("You must accept the authentication request in order to use this program with Spotify.");
                authorized = false;
            };

            //If the user accepted the request
            webApiFactory.OnAuthSuccess += (sender, e) =>
            {
                authorized = true;
            };

            try
            {
                api = await webApiFactory.GetWebApiAsync();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Spotify failed to load: " + ex.Message);
            }
        }
        
        public async Task<Song> GetCurrentSong()
        {
            try
            {
                PlaybackContext playback = await api.GetPlayingTrackAsync().ConfigureAwait(true);

                //If a track was found
                if (playback.Item != null)
                {
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

                //If a 401 error occured, this most likely means that the authorization token expired
                if (playback.HasError() && playback.Error.Status == 401)
                {
                }

                //The only time it should reach here is when an error occurs
                throw new Exception();
            }
            catch(Exception)
            {
                //If a song wasn't successfully found, we return a blank song
                return new Song("", "", "");
            }
        }

        public void Disconnect()
        {
            api.Dispose();
        }
    }
}
