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
        private string ClientID;

        //Authenticate the program
        public void Authenticate(string clientID)
        {
            ImplicitGrantAuth auth = new ImplicitGrantAuth(
            clientID,
            "http://localhost:4002",
            "http://localhost:4002",
            Scope.UserReadPlaybackState
            );

            ClientID = clientID;

            auth.AuthReceived += async (sender, payload) =>
            {
                auth.Stop(); // `sender` is also the auth instance

                api = new SpotifyWebAPI()
                {
                    TokenType = "Bearer",
                    AccessToken = payload.AccessToken
                };
            };

            auth.Start(); // Starts an internal HTTP Server
            auth.OpenBrowser();
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
                    //Update the authorization
                    Console.WriteLine("Auth expired, requesing new token");
                    Authenticate(ClientID);
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
