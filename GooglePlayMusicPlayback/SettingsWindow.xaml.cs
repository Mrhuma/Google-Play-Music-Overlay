using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace GooglePlayMusicOverlay
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            FillColorComboBoxes();
        }

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
            List<Color> colorList = new List<Color>();

            //Loop through every Drawing.Color and add it to a list
            //https://stackoverflow.com/questions/19575872/iterate-through-system-drawing-color-struct-and-use-it-to-create-system-drawing
            foreach (var prop in typeof(Color).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static))
            {
                if (prop.PropertyType == typeof(Color))
                {
                    colorList.Add((Color)prop.GetValue(null));
                }
            }

            foreach(Color color in colorList)
            {
                //Add every color to the foreground comboboxes
                ComboBoxItem foregroundItem = new ComboBoxItem
                {
                    Content = color.Name,
                    Foreground = new System.Windows.Media.SolidColorBrush(ToMediaColor(color)),
                };
                ForegroundColorComboBox.Items.Add(foregroundItem);

                //Add every color to the background comboboxes
                ComboBoxItem backgroundItem = new ComboBoxItem
                {
                    Content = color.Name,
                    Background = new System.Windows.Media.SolidColorBrush(ToMediaColor(color)),
                };
                BackgroundColorComboBox.Items.Add(backgroundItem);
            }
        }

        //Convert a Drawing.Color to a Media.Color
        private System.Windows.Media.Color ToMediaColor(Color color)
        {
            return System.Windows.Media.Color.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}
