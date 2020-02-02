using System.Collections.Generic;
using System.Drawing;
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
            CurrentXCoordTextBox.Text = mainWindow.Left.ToString();
            CurrentYCoordTextBox.Text = mainWindow.Top.ToString();
        }
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

            SavedXCoordTextBox.Text = settings.XCoord.ToString();
            SavedYCoordTextBox.Text = settings.YCoord.ToString();
        }

        private void SaveCurrentCoordsButton_Click(object sender, RoutedEventArgs e)
        {
            SavedXCoordTextBox.Text = CurrentXCoordTextBox.Text;
            SavedYCoordTextBox.Text = CurrentYCoordTextBox.Text;
            mainWindow.UpdateSavedCoords(CurrentXCoordTextBox.Text, CurrentYCoordTextBox.Text);
        }

        private void LoadSavedCoordsButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.MoveToSavedCoords();
        }

        private void UpdateColorsButton_Click(object sender, RoutedEventArgs e)
        {
            mainWindow.UpdateAllColors(BackgroundColorComboBox.Text, ForegroundColorComboBox.Text);
        }

        private void Window_Closed(object sender, System.EventArgs e)
        {
            this.Close();
        }
    }
}
