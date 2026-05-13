using System;
using System.Windows;
using Microsoft.Win32;

using ImageConverterApp.Models;
using ImageConverterApp.Services;

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
            Microsoft.Win32.OpenFileDialog dialog = new Microsoft.Win32.OpenFileDialog();

            dialog.Filter = "Image Files|*.png;*.jpg;*.jpeg;*.gif;*.bmp;*.tiff;*.webp;*.jfif;*.heic";

            if (dialog.ShowDialog() == true)
            {
                InputFileTextBox.Text = dialog.FileName;
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

        private void ConvertButton_Click(object sender, RoutedEventArgs e)
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

                ImageConverterService.ConvertImage(
                    inputFile,
                    destinationFolder,
                    targetFormat.Extension);

                System.Windows.MessageBox.Show(
                    "Image converted successfully!",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);
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
    }
}