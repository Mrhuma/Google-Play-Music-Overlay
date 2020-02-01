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

        //Update the border width text whenever the border width slider's value changes
        private void BorderWidthSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            BorderWidthLabel.Content = "Border Width: " + e.NewValue;
        }

        private void BorderActiveCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            //Enable the border controls if the border is enabled
            BorderControlsGrid.IsEnabled = true;
        }

        private void BorderActiveCheckbox_Unchecked(object sender, RoutedEventArgs e)
        {
            //Disable the border controls if the border is disabled
            BorderControlsGrid.IsEnabled = false;
        }

        private void FillColorComboBoxes()
        {
            foreach(HexColor color in HexColor.Colors)
            {
                ComboBoxItem widthItem = new ComboBoxItem
                {
                    Content = color.Name,
                    BorderThickness = new Thickness(1),
                    BorderBrush = new SolidColorBrush(color.ConvertToColor()),
                };

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

                BorderColorComboBox.Items.Add(widthItem);
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
            BorderActiveCheckbox.IsChecked = settings.IsBorder;
            BorderWidthSlider.Value = settings.BorderWidth;

            foreach (ComboBoxItem item in BorderColorComboBox.Items)
            {
                if (item.Content.ToString() == settings.BorderColor)
                {
                    BorderColorComboBox.SelectedItem = item;
                }
            }

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
            mainWindow.UpdateAllColors((bool)BorderActiveCheckbox.IsChecked, (int)BorderWidthSlider.Value, BorderColorComboBox.Text, BackgroundColorComboBox.Text, ForegroundColorComboBox.Text);
        }
    }
}
