using ImageConverterApp.Models;
using ImageConverterApp.Services;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.Win32;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Forms = System.Windows.Forms;

namespace ImageConverterApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Grab supported formats from the Menu Screen and set as source for both combo boxes
            CurrentFormatComboBox.ItemsSource = FormatRegistry.SupportedFormats;
            TargetFormatComboBox.ItemsSource = FormatRegistry.SupportedFormats;
        }

        private void BrowseFile_Click(object sender, RoutedEventArgs e)
        {
            // Filter includes common image formats and MP4 for video-to-GIF conversion
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "All Supported Files|*.png;*.jpg;*.jpeg;*.gif;*.bmp;*.tiff;*.webp;*.jfif;*.heic;*.mp4"
            };

            // If the input file is selected, auto-select the source format and suggest a default target format if applicable
            if (dialog.ShowDialog() == true)
            {
                InputFileTextBox.Text = dialog.FileName;

                AutoSelectSourceFormat(dialog.FileName);
                AutoSelectDefaultTarget(dialog.FileName);
            }
        }

        private void BrowseDestination_Click(object sender, RoutedEventArgs e)
        {
            // Using Windows Forms FolderBrowserDialog to initiate the folder selection process
            using Forms.FolderBrowserDialog folderDialog = new Forms.FolderBrowserDialog();

            // If a folder is selected, update the DestinationTextBox with the chosen path
            if (folderDialog.ShowDialog() == Forms.DialogResult.OK)
            {
                DestinationTextBox.Text = folderDialog.SelectedPath;
            }
        }

        private async void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Put the TextBox values into local variables for easier access and validation
                string inputFile = InputFileTextBox.Text;
                string destinationFolder = DestinationTextBox.Text;

                //If cases for invalid inputs
                if (string.IsNullOrWhiteSpace(inputFile))
                {
                    System.Windows.MessageBox.Show("Please select an image file.");
                    return;
                }

                if (string.IsNullOrWhiteSpace(destinationFolder))
                {
                    System.Windows.MessageBox.Show("Please select a destination folder.");
                    return;
                }

                if (TargetFormatComboBox.SelectedItem == null)
                {
                    System.Windows.MessageBox.Show("Please choose a target format.");
                    return;
                }

                // Grab selected target format from the combo box and cast it to ImageFormatInfo for use in the conversion process
                ImageFormatInfo targetFormat =
                    (ImageFormatInfo)TargetFormatComboBox.SelectedItem;

                // Call the conversion method in the ImageConverterService asynchronously to avoid blocking the UI thread during potentially long-running conversions
                await Task.Run(() =>
                {
                        ImageConverterService.ConvertImage(
                        inputFile,
                        destinationFolder,
                        targetFormat.Extension);
                });

            }

            // If any exceptions occur during the conversion process, catch them and display an error message to the user
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show(
                    $"Conversion failed:\n{ex.Message}",
                    "Error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        private void AutoSelectSourceFormat(string filePath)
        {
            // Extract the file extension from the selected file path and convert it to lowercase for case-insensitive comparison
            string ext = Path.GetExtension(filePath).ToLower();

            // Iterate through the items in the CurrentFormatComboBox to find a matching format based on the file extension
            foreach (var item in CurrentFormatComboBox.Items)
            {
                if (item is ImageConverterApp.Models.ImageFormatInfo format)
                {
                    if (format.Extension.ToLower() == ext)
                    {
                        //Default Filetype for images are .PNG, if the original type is PNg then flip to JPG
                        CurrentFormatComboBox.SelectedItem = format;
                        if (ext == ".png")
                        {
                            TargetFormatComboBox.SelectedItem =
                                FormatRegistry.SupportedFormats.FirstOrDefault(f => f.Extension == ".jpg");
                        }
                        else
                        {
                            TargetFormatComboBox.SelectedItem =
                                FormatRegistry.SupportedFormats.FirstOrDefault(f => f.Extension == ".png");
                        }

                        return;
                    }
                }
            }

            // fallback if not found
            CurrentFormatComboBox.SelectedIndex = -1;
        }

        private void AutoSelectDefaultTarget(string inputPath)
        {
            // Default Destination for GIF are .MP4 and vice versa
            string ext = Path.GetExtension(inputPath).ToLower();

            if (ext == ".mp4")
            {
                TargetFormatComboBox.SelectedItem =
                    FormatRegistry.SupportedFormats.FirstOrDefault(f => f.Extension == ".gif");
            }
            else if (ext == ".gif")
            {
                TargetFormatComboBox.SelectedItem =
                    FormatRegistry.SupportedFormats.FirstOrDefault(f => f.Extension == ".mp4");
            }
        }
    }
}