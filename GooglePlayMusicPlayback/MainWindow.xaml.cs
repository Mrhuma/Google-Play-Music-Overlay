using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using WebSocketSharp;

namespace GooglePlayMusicOverlay
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int songTextIndex = 0; //The current index of the song text
        private int songTextLength = 0; //The length of the song text

        private int artistTextIndex = 0; //The current index of the song text
        private int artistTextLength = 0; //The length of the song text

        private readonly int maxTextWidth = 16; //The max amount of characters that can be seen on the screen

        private Timer songTimer;
        private Timer artistTimer;
        private readonly int dueTime = 2000; // ms delay till timer starts
        private readonly int Period = 25; // ms inbetween timer callbacks

        Song currentSong = null; //The song currently being played
        Song newSong = new Song(false, "", "", "");
        SettingsWindow settingsWindow; //Reference to the settings window
        Settings settings;

        WebSocket webSocket; //WebSocket to read the song currently being played

        public MainWindow()
        {
            InitializeComponent();

            //Create files if they don't exist
            if (!HexColor.CheckForColorsFile())
            {
                HexColor.CreateColorsFile();

                #region Colors
                HexColor.Colors.Add(new HexColor("White", "#FFFFFF"));
                HexColor.Colors.Add(new HexColor("Black", "#000000"));
                HexColor.Colors.Add(new HexColor("Shark", "#222326"));
                HexColor.Colors.Add(new HexColor("Charcoal", "#444444"));
                HexColor.Colors.Add(new HexColor("Dim Gray", "#666666"));
                HexColor.Colors.Add(new HexColor("Nobel", "#999999"));
                HexColor.Colors.Add(new HexColor("Very Light Gray", "#CCCCCC"));
                HexColor.Colors.Add(new HexColor("Red", "#FF0000"));
                HexColor.Colors.Add(new HexColor("Sweet Pink", "#EA9999"));
                HexColor.Colors.Add(new HexColor("Roman", "#E06666"));
                HexColor.Colors.Add(new HexColor("Free Speech Red", "#CC0000"));
                HexColor.Colors.Add(new HexColor("Dark Red", "#990000"));
                HexColor.Colors.Add(new HexColor("Maroon", "#660000"));
                HexColor.Colors.Add(new HexColor("Orange Peel", "#FF9900"));
                HexColor.Colors.Add(new HexColor("Peach-Orange", "#F9CB9C"));
                HexColor.Colors.Add(new HexColor("Rajah", "#F6B26B"));
                HexColor.Colors.Add(new HexColor("California", "#E69138"));
                HexColor.Colors.Add(new HexColor("Tenne", "#B45F06"));
                HexColor.Colors.Add(new HexColor("Raw Umber", "#783F04"));
                HexColor.Colors.Add(new HexColor("Yellow", "#FFFF00"));
                HexColor.Colors.Add(new HexColor("Cream Brulee", "#FFE599"));
                HexColor.Colors.Add(new HexColor("Dandelion", "#FFD966"));
                HexColor.Colors.Add(new HexColor("Saffron", "#F1C232"));
                HexColor.Colors.Add(new HexColor("Dark Goldenrod", "#BF9000"));
                HexColor.Colors.Add(new HexColor("Lime", "#00FF00"));
                HexColor.Colors.Add(new HexColor("Madang", "#B6D7A8"));
                HexColor.Colors.Add(new HexColor("Gossip", "#93C47D"));
                HexColor.Colors.Add(new HexColor("Apple", "#6AA84F"));
                HexColor.Colors.Add(new HexColor("Bilbao", "#38761D"));
                HexColor.Colors.Add(new HexColor("Myrtle", "#274E13"));
                HexColor.Colors.Add(new HexColor("Aqua", "#00FFFF"));
                HexColor.Colors.Add(new HexColor("Jungle Mist", "#A2C4C9"));
                HexColor.Colors.Add(new HexColor("Neptune", "#76A5AF"));
                HexColor.Colors.Add(new HexColor("Jelly Bean", "#45818E"));
                HexColor.Colors.Add(new HexColor("Teal Blue", "#134F5C"));
                HexColor.Colors.Add(new HexColor("Nordic", "#0C343D"));
                HexColor.Colors.Add(new HexColor("Blue", "#0000FF"));
                HexColor.Colors.Add(new HexColor("Sail", "#9FC5E8"));
                HexColor.Colors.Add(new HexColor("Jordy Blue", "#6FA8DC"));
                HexColor.Colors.Add(new HexColor("Curious Blue", "#3D85C6"));
                HexColor.Colors.Add(new HexColor("Dark Cerulean", "#0B5394"));
                HexColor.Colors.Add(new HexColor("Prussian Blue", "#073763"));
                HexColor.Colors.Add(new HexColor("Electric Purple", "#9900FF"));
                HexColor.Colors.Add(new HexColor("Biloba Flower", "#B4A7D6"));
                HexColor.Colors.Add(new HexColor("True V", "#8E7CC3"));
                HexColor.Colors.Add(new HexColor("Blue Marguerite", "#674EA7"));
                HexColor.Colors.Add(new HexColor("Persian Indigo", "#351C75"));
                HexColor.Colors.Add(new HexColor("Violent Violet", "#20124D"));
                HexColor.Colors.Add(new HexColor("Magenta", "#FF00FF"));
                HexColor.Colors.Add(new HexColor("Maverick", "#D5A6BD"));
                HexColor.Colors.Add(new HexColor("Hopbush", "#C27BA0"));
                HexColor.Colors.Add(new HexColor("Royal Health", "#A64D79"));
                HexColor.Colors.Add(new HexColor("Pompadour", "#741B47"));
                HexColor.Colors.Add(new HexColor("Blackberry", "#4C1130"));
                #endregion

                HexColor.WriteToFile(); //Write the colors to the Colors file
            }
            else
            {
                //Updates the HexColor.Colors List from the file
                HexColor.ReadFromFile();
            }

            //Create files if they don't exist
            if (!Settings.CheckForSettingsFile())
            {
                Settings.CreateSettingsFile();
                //Generates default settings
                settings = new Settings
                {
                    BackgroundColor = "Shark",
                    ForegroundColor = "White",
                    XCoord = 800,
                    YCoord = 300
                };
                Settings.WriteToFile(settings);
            }

            //Setup the websocket events and properties
            webSocket = new WebSocket("ws://localhost:5672");
            webSocket.OnMessage += (sender, e) => WebSocketOnMessage(sender, e);
            //Will try to reconnect if it loses connection
            webSocket.OnClose += (sender, e) => Task.Run(() => ConnectWebSocket(webSocket)).ConfigureAwait(false);

            //Connect the WebSocket
            Task.Run(() => ConnectWebSocket(webSocket)).ConfigureAwait(false);

            //Updates the settings variable from the file
            settings = Settings.ReadFromFile();
            UpdateColorsFromSettings();

            //Set the window location to whatever is saved in the settings
            Top = settings.YCoord;
            Left = settings.XCoord;

            //Start Timers
            var autoEvent = new AutoResetEvent(false);
            songTimer = new Timer(ScrollSongText, autoEvent, dueTime, Period);
            artistTimer = new Timer(ScrollArtistText, autoEvent, dueTime, Period);
        }

        //Keeps trying to connect until it's successful
        private void ConnectWebSocket(WebSocket ws)
        {
            while (ws.ReadyState != WebSocketState.Open)
            {
                //Keep trying to connect until it's successful
                ws.Connect();
            }
        }

        //Scrolls through the text of the song title
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
        
        //Scrolls through the text of the artist name
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

        //WebSocket OnMessage Events
        private void WebSocketOnMessage(object sender, MessageEventArgs e)
        {
            bool updateDisplay = false;
            JObject JsonObject = JObject.Parse(e.Data); //Convert the data to a JSON object
            switch ((string)JsonObject.SelectToken("channel"))
            {
                case "track": //If the track being played changed
                    newSong.Title = (string)JsonObject.SelectToken("payload.title");
                    newSong.Artist = (string)JsonObject.SelectToken("payload.artist");
                    newSong.albumArt = (string)JsonObject.SelectToken("payload.albumArt");
                    updateDisplay = true;
                    break;
                case "playState": //If the song's playstate changed
                    newSong.Playing = (bool)JsonObject.SelectToken("payload");
                    if(newSong.Title != "") //If a song has been selected
                        updateDisplay = true;
                    break;
            }

            if(updateDisplay)
            {
                updateDisplay = false;
                UpdateSongDisplay(newSong);
            }
        }


        //Checks if a new song is playing or if the music was paused
        private void UpdateSongDisplay(Song newSong)
        {
            //If no song is playing
            if (newSong.Playing == false)
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
                if (currentSong != null)
                {
                    currentSong.Playing = false;
                }
            }
            else if (currentSong == null) //If no song has been played yet during this session
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
                    songNameText.Text = newSong.Title;
                    artistNameText.Text = newSong.Artist;
                    BitmapImage source = new BitmapImage();
                    if (newSong.albumArt != "" && newSong.albumArt != null)
                    {
                        source.BeginInit();
                        source.UriSource = new Uri(newSong.albumArt, UriKind.Absolute);
                        source.EndInit();
                    }
                    albumArtImage.Source = source;
                });

                //Update the text length
                songTextLength = newSong.Title.Length;
                artistTextLength = newSong.Artist.Length;

                //Update the song variable
                currentSong = new Song(newSong.Playing, newSong.Title, newSong.Artist, newSong.albumArt);
            }
            //If a new song is playing, or a song gets resumed
            else if (newSong.Title != currentSong.Title || newSong.Artist != currentSong.Artist || newSong.Playing != currentSong.Playing)
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
                    songNameText.Text = newSong.Title;
                    artistNameText.Text = newSong.Artist;
                    BitmapImage source = new BitmapImage();
                    if (newSong.albumArt != "" && newSong.albumArt != null)
                    {
                        source.BeginInit();
                        source.UriSource = new Uri(newSong.albumArt, UriKind.Absolute);
                        source.EndInit();
                    }
                    albumArtImage.Source = source;
                    songNameText.ScrollToHome();
                    artistNameText.ScrollToHome();
                });

                //Update the text length
                songTextLength = newSong.Title.Length;
                artistTextLength = newSong.Artist.Length;

                //Update the song variable
                currentSong = new Song(newSong.Playing, newSong.Title, newSong.Artist, newSong.albumArt);
            }
        }

        //Opens the settings window
        private void OpenSettingsButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                //If the window is already open we show and activate it
                settingsWindow.Show();
                settingsWindow.Activate();
            }
            catch (Exception)
            {
                settingsWindow = new SettingsWindow(this);
                settingsWindow.Show();

                //Makes sure that the settings shown matches the ones from file
                settingsWindow.UpdateSettingsDisplays(settings);
            }
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
            if (settingsWindow != null) //If the settings window is open
            {
                settingsWindow.UpdateLocationText(Left, Top);
            }
        }

        //Update the settings with the saved coords
        public void UpdateSavedCoords(string x, string y)
        {
            settings.XCoord = double.Parse(x);
            settings.YCoord = double.Parse(y);
            Settings.WriteToFile(settings);
        }

        //Moves the window to the saved Coords
        public void MoveToSavedCoords()
        {
            Top = settings.YCoord;
            Left = settings.XCoord;
        }

        //Updates the background color and foreground color with the given parameters
        public void UpdateColors(string backgroundColor, string foregroundColor)
        {
            UpdateAllBackgrounds(backgroundColor);
            UpdateAllForegrounds(foregroundColor);
        }

        //Updates the background color and foreground color from the saved settings
        public void UpdateColorsFromSettings()
        {
            UpdateAllBackgrounds(settings.BackgroundColor);
            UpdateAllForegrounds(settings.ForegroundColor);
        }

        //Updates the background color from the given parameter
        private void UpdateAllBackgrounds(string color)
        {
            //Create a SolidColorBrush with the background color saved in the settings
            SolidColorBrush brush = new SolidColorBrush(HexColor.Colors.First(x => x.Name == color).ConvertToColor());
            this.Background = brush;
            songNameText.Background = brush;
            artistNameText.Background = brush;
            OpenSettingsButton.Background = brush;
        }

        //Updates the foreground color from the given parameter
        private void UpdateAllForegrounds(string color)
        {
            //Create a SolidColorBrush with the foreground color saved in the settings
            SolidColorBrush brush = new SolidColorBrush(HexColor.Colors.First(x => x.Name == color).ConvertToColor());
            this.Foreground = brush;
            songNameText.Foreground = brush;
            artistNameText.Foreground = brush;
            OpenSettingsButton.Foreground = brush;
        }

        //Updates the settings with the colors
        public void UpdateSavedColors(string backgroundColor, string foregroundColor)
        {
            settings.BackgroundColor = backgroundColor;
            settings.ForegroundColor = foregroundColor;
            Settings.WriteToFile(settings);
        }

        //Close the settings window if it's open
        private void Window_Closed(object sender, EventArgs e)
        {
            try
            {
                settingsWindow.Close();
            }
            catch { }
        }
    }
}