using System;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.IO;
using Newtonsoft.Json.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace GooglePlayMusicOverlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Window Height should be 80
        private int songTextIndex = 0; //The current index of the song text
        private int songTextLength = 0; //The length of the song text

        private int artistTextIndex = 0; //The current index of the song text
        private int artistTextLength = 0; //The length of the song text

        private readonly int maxTextWidth = 16; //The max amount of characters that can be seen on the screen

        private Timer songTimer;
        private Timer artistTimer;
        private readonly int dueTime = 2000; // ms delay till timer starts
        private readonly int Period = 25; // ms inbetween timer callbacks

        private Timer updateSongTimer;
        Song song = null;
        SettingsWindow settingsWindow; //Reference to the settings window

        public MainWindow()
        {
            InitializeComponent();
            //Start Timers
            var autoEvent = new AutoResetEvent(false);
            songTimer = new Timer(ScrollSongText, autoEvent, dueTime, Period);
            artistTimer = new Timer(ScrollArtistText, autoEvent, dueTime, Period);
            updateSongTimer = new Timer(CheckForNewSong, autoEvent, 1000, 1000);
        }

        private async void ScrollSongText(Object stateInfo)
        {
            await Task.Run(() =>
            {
                try
                {
                    if (songTextLength > maxTextWidth) //If the length of the text overflows the screen
                    {
                        if (songTextIndex < songTextLength * 8) //If there is more off-screen text to show
                                                                 //We multiply by 8 because that is the spacing of our font
                        {
                            //Need Dispatcher because the textboxes are on a different thread
                            Dispatcher.Invoke(() => songNameText.ScrollToHorizontalOffset(songTextIndex));
                            songTextIndex += 2;
                        }
                        else //If there is no more off-screen text to show
                        {
                            songTextIndex = 0;
                            Dispatcher.Invoke(() => songNameText.ScrollToHome()); //Go back to the beginning
                            songTimer.Change(dueTime, Period);
                        }
                    }
                }
                catch (Exception) { }
            });
            
        }

        private async void ScrollArtistText(Object stateInfo)
        {   
            await Task.Run(() => 
            {
                try
                {
                    if (artistTextLength > maxTextWidth) //If the length of the text overflows the screen
                    {
                        if (artistTextIndex < artistTextLength * 8) //If there is more off-screen text to show
                                                                     //We multiply by 8 because that is the spacing of our font
                        {
                            //Need Dispatcher because the textboxes are on a different thread
                            Dispatcher.Invoke(() => artistNameText.ScrollToHorizontalOffset(artistTextIndex));
                            artistTextIndex += 2;
                        }
                        else //If there is no more off-screen text to show
                        {
                            artistTextIndex = 0;
                            Dispatcher.Invoke(() => artistNameText.ScrollToHome()); //Go back to the beginning
                            artistTimer.Change(dueTime, Period);
                        }
                    }
                }
                catch (Exception){}
            });
        }

        private async void CheckForNewSong(Object stateInfo)
        {
            await Task.Run(() =>
            {
                string jsonLocation = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Google Play Music Desktop Player\\json_store\\playback.json";
                Song currentSong;

                //We need to be able to read from a file that is being written to, so we set these access settings
                using (var fs = new FileStream(jsonLocation, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var reader = new StreamReader(fs, Encoding.Default))
                {
                    string json = reader.ReadToEnd();
                    JObject JsonObject = JObject.Parse(json);

                    bool playing = (bool)JsonObject.SelectToken("playing");
                    string title = (string)JsonObject.SelectToken("song.title");
                    string artist = (string)JsonObject.SelectToken("song.artist");
                    string albumArt = (string)JsonObject.SelectToken("song.albumArt");

                    currentSong = new Song(playing, title, artist, albumArt);

                    //If no song is playing
                    if (currentSong.Playing == false)
                    {
                        //Set the timers so that they never callback
                        songTimer.Change(int.MaxValue, Period);
                        artistTimer.Change(int.MaxValue, Period);

                        //Reset indexes
                        songTextIndex = 0;
                        artistTextIndex = 0;

                        //Clear the displays
                        Dispatcher.Invoke(() =>
                        {
                            songNameText.Text = "";
                            artistNameText.Text = "";
                            albumArtImage.Source = null;
                        });

                        //Reset the text length
                        songTextLength = 0;
                        artistTextLength = 0;

                        //If a song has been played during this session
                        if (song != null)
                        {
                            song.Playing = false;
                        }
                    }
                    else if (song == null) //If no song has been played yet during this session
                    {
                        //Reset the timers
                        songTimer.Change(dueTime, Period);
                        artistTimer.Change(dueTime, Period);

                        //Reset the indexes
                        songTextIndex = 0;
                        artistTextIndex = 0;

                        //Update the displays
                        Dispatcher.Invoke(() =>
                        {
                            songNameText.Text = currentSong.Title;
                            artistNameText.Text = currentSong.Artist;
                            BitmapImage source = new BitmapImage();
                            source.BeginInit();
                            source.UriSource = new Uri(currentSong.albumArt, UriKind.Absolute);
                            source.EndInit();
                            albumArtImage.Source = source;
                            });

                        //Update the text length
                        songTextLength = currentSong.Title.Length;
                        artistTextLength = currentSong.Artist.Length;

                        //Update the song variable
                        song = currentSong;
                    }
                    //If a new song is playing, or a song gets resumed
                    else if (currentSong.Title != song.Title || currentSong.Artist != song.Artist || currentSong.Playing != song.Playing)
                    {
                        //Reset the timers
                        songTimer.Change(dueTime, Period);
                        artistTimer.Change(dueTime, Period);

                        //Reset the indexes
                        songTextIndex = 0;
                        artistTextIndex = 0;

                        //Update the displays
                        Dispatcher.Invoke(() =>
                        {
                            songNameText.Text = currentSong.Title;
                            artistNameText.Text = currentSong.Artist;
                            BitmapImage source = new BitmapImage();
                            source.BeginInit();
                            source.UriSource = new Uri(currentSong.albumArt, UriKind.Absolute);
                            source.EndInit();
                            albumArtImage.Source = source;
                            songNameText.ScrollToHome();
                            artistNameText.ScrollToHome();
                        });

                        //Update the text length
                        songTextLength = currentSong.Title.Length;
                        artistTextLength = currentSong.Artist.Length;

                        //Update the song variable
                        song = currentSong;
                    }
                }
            });
        }

        private void openSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            settingsWindow = new SettingsWindow();
            settingsWindow.Show();
        }

        //With both of these events combined, the application will always be on top of other windows
        private void Window_PreviewLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = true;
        }

        private void Window_Deactivated(object sender, EventArgs e)
        {
            Window window = (Window)sender;
            window.Topmost = true;
        }

        //Updates the text in the settings window of the main window's current location
        private void Window_LocationChanged(object sender, EventArgs e)
        {
            if (settingsWindow != null)
            {
                settingsWindow.UpdateLocationText(Left, Top);
            }
        }
    }
}