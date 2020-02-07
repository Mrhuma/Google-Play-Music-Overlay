using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace GooglePlayMusicOverlay
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        MainWindow mainWindow;
        public SettingsWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            FillColorComboBoxes();
            this.mainWindow = mainWindow;
        }

        //Fills the foreground and background comboboxes with each color from the colors file
        private void FillColorComboBoxes()
        {
            foreach(HexColor color in HexColor.Colors)
            {

                ComboBoxItem foregroundItem = new ComboBoxItem
                {
                    Content = color.Name,
                    Foreground = new SolidColorBrush(color.ConvertToColor()),
                };

                ComboBoxItem backgroundItem = new ComboBoxItem
                {
                    Content = color.Name,
                    Background = new SolidColorBrush(color.ConvertToColor()),
                };

                ForegroundColorComboBox.Items.Add(foregroundItem);
                BackgroundColorComboBox.Items.Add(backgroundItem);
            };
        }

        //Updates the textboxes with the inputted coordinates
        //This is called from the Main Window whenever the LocationChanged event is called
        public void UpdateLocationText(double x, double y)
        {
            CurrentXCoordTextBox.Text = x.ToString();
            CurrentYCoordTextBox.Text = y.ToString();
        }

        //Update the selected options with the inputted settings
        public void UpdateSettingsDisplays(Settings settings)
        {
            //Set the color comboboxes
            foreach (ComboBoxItem item in BackgroundColorComboBox.Items)
            {
                if(item.Content.ToString() == settings.BackgroundColor)
                {
                    BackgroundColorComboBox.SelectedItem = item;
                }
            }

            foreach (ComboBoxItem item in ForegroundColorComboBox.Items)
            {
                if (item.Content.ToString() == settings.ForegroundColor)
                {
                    ForegroundColorComboBox.SelectedItem = item;
                }
            }

            //Set the saved coords textboxes
            SavedXCoordTextBox.Text = settings.XCoord.ToString();
            SavedYCoordTextBox.Text = settings.YCoord.ToString();

            //Set the current coords textboxes
            CurrentXCoordTextBox.Text = mainWindow.Left.ToString();
            CurrentYCoordTextBox.Text = mainWindow.Top.ToString();
        }

        //Updates the currently saved coords
        private void SaveCurrentCoordsButton_Click(object sender, RoutedEventArgs e)
        {
            SavedXCoordTextBox.Text = CurrentXCoordTextBox.Text;
            SavedYCoordTextBox.Text = CurrentYCoordTextBox.Text;
            mainWindow.UpdateSavedCoords(CurrentXCoordTextBox.Text, CurrentYCoordTextBox.Text);
        }

        //Moves the main window to the saved coords
        private void LoadSavedCoordsButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.MoveToSavedCoords();
        }

        //Updates the background and foreground colors on the main window
        private void UpdateColorsButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.UpdateColors(BackgroundColorComboBox.Text, ForegroundColorComboBox.Text);
        }

        //Update the example text background whenever a new color is chosen
        private void BackgroundColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string color = ((ComboBoxItem)e.AddedItems[0]).Content.ToString(); //Gets the name of the color that was selected
            SolidColorBrush brush = new SolidColorBrush(HexColor.Colors.First(x => x.Name == color).ConvertToColor());
            songNameText.Background = brush;
            artistNameText.Background = brush;
        }

        //Update the example text foreground whenever a new color is chosen
        private void ForegroundColorComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string color = ((ComboBoxItem)e.AddedItems[0]).Content.ToString(); //Gets the name of the color that was selected
            SolidColorBrush brush = new SolidColorBrush(HexColor.Colors.First(x => x.Name == color).ConvertToColor());
            songNameText.Foreground = brush;
            artistNameText.Foreground = brush;
        }
    }
}
