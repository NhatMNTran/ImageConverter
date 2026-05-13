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

            CurrentFormatComboBox.ItemsSource = FormatRegistry.SupportedFormats;
            TargetFormatComboBox.ItemsSource = FormatRegistry.SupportedFormats;
        }

        private void BrowseFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog
            {
                Filter = "All Supported Files|*.png;*.jpg;*.jpeg;*.gif;*.bmp;*.tiff;*.webp;*.jfif;*.heic;*.mp4"
            };

            if (dialog.ShowDialog() == true)
            {
                InputFileTextBox.Text = dialog.FileName;

                AutoSelectSourceFormat(dialog.FileName);
            }
        }

        private void BrowseDestination_Click(object sender, RoutedEventArgs e)
        {
            using Forms.FolderBrowserDialog folderDialog = new Forms.FolderBrowserDialog();

            if (folderDialog.ShowDialog() == Forms.DialogResult.OK)
            {
                DestinationTextBox.Text = folderDialog.SelectedPath;
            }
        }

        private async void ConvertButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string inputFile = InputFileTextBox.Text;
                string destinationFolder = DestinationTextBox.Text;

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

                ImageFormatInfo targetFormat =
                    (ImageFormatInfo)TargetFormatComboBox.SelectedItem;

                await Task.Run(() =>
                {
                        ImageConverterService.ConvertImage(
                        inputFile,
                        destinationFolder,
                        targetFormat.Extension);
                });

            }
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
            string ext = Path.GetExtension(filePath).ToLower();

            foreach (var item in CurrentFormatComboBox.Items)
            {
                if (item is ImageConverterApp.Models.ImageFormatInfo format)
                {
                    if (format.Extension.ToLower() == ext)
                    {
                        CurrentFormatComboBox.SelectedItem = format;
                        return;
                    }
                }
            }

            // fallback if not found
            CurrentFormatComboBox.SelectedIndex = -1;
        }

        private void AutoSelectDefaultTarget(string inputPath)
        {
            string ext = Path.GetExtension(inputPath).ToLower();

            if (ext == ".mp4")
            {
                TargetFormatComboBox.SelectedItem =
                    FormatRegistry.SupportedFormats.FirstOrDefault(f => f.Extension == ".gif");
            }
        }
    }
}