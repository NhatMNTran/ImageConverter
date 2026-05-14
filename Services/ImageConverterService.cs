using ImageMagick;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Webp;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace ImageConverterApp.Services
{
    public static class ImageConverterService
    {
        // Async alows methods to be used while the app is perforning other tasks, such as UI interactions, without freezing the interface.
        public static async void ConvertImage(
            string inputPath,
            string outputFolder,
            string outputExtension)
        {
            string fileName = Path.GetFileNameWithoutExtension(inputPath);
            string outputPath = Path.Combine(outputFolder, fileName + outputExtension);
            string inputExt = Path.GetExtension(inputPath).ToLower();

            //If the input and output matches gif and mp4, use ffmpeg and its seperate functions for conversion instead
            if (inputExt == ".mp4" && outputExtension == ".gif" || inputExt == ".gif" && outputExtension == ".mp4")
            {
                await ImageConverterService.ConvertMp4ToGif(inputPath, outputPath);
                return;
            }

            // HEIC requires Magick.NET
            if (outputExtension == ".heic")
            {
                using var magickImage = new MagickImage(inputPath);
                magickImage.Write(outputPath);
                return;
            }

            // Input HEIC handling
            if (Path.GetExtension(inputPath).ToLower() == ".heic")
            {
                using var magickImage = new MagickImage(inputPath);
                magickImage.Write(outputPath);
                return;
            }

            // Utilize SixLabors's ImageSharp for other formats, as it is more efficient and supports a wide range of formats
            using SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(inputPath);

            //Implement switch case encoders for each file types
            switch (outputExtension.ToLower())
            {
                case ".png":
                    image.Save(outputPath, new PngEncoder());
                    break;

                case ".jpg":
                case ".jpeg":
                    image.Save(outputPath, new JpegEncoder());
                    break;

                case ".gif":
                    image.Save(outputPath, new GifEncoder());
                    break;

                case ".bmp":
                    image.Save(outputPath, new BmpEncoder());
                    break;

                case ".tiff":
                    image.Save(outputPath, new TiffEncoder());
                    break;

                case ".webp":
                    image.Save(outputPath, new WebpEncoder());
                    break;

                default:
                    throw new Exception("Unsupported output format.");
            }
        }

        public static async Task ConvertMp4ToGif(string inputPath, string outputPath)
        {
            string ffmpegPath = "ffmpeg";
            // or full path: C:\\ffmpeg\\bin\\ffmpeg.exe if ffmpeg is not in the system PATH
            string args = string.Empty;

            // Use different FFmpeg arguments based on whether the input is a video or a GIF
            if (Path.GetExtension(inputPath).ToLower() == ".mp4")
            {
                //  IMPORTANT
                // Changes FPS and Scale depending on file size preferences,
                // More FPS and higher scale will result in a larger file size
                args =
                $"-i \"{inputPath}\" " +
                "-vf \"fps=15,scale=480:-1:flags=lanczos\" " +
                "-y " +
                $"\"{outputPath}\"";
            }
            else if (Path.GetExtension(inputPath).ToLower() == ".gif")
            {
                args =
                $"-i \"{inputPath}\" " +
                "-movflags faststart " +
                "-pix_fmt yuv420p " +
                "-vf \"scale=trunc(iw/2)*2:trunc(ih/2)*2\" " +
                "-y " +
                $"\"{outputPath}\"";
            }

            // Configure the process to run FFmpeg with the specified arguments, and redirect output for error handling
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            // Start the FFmpeg process and wait for it to complete, then check the exit code to determine if the conversion was successful
            using Process process = Process.Start(startInfo);
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                string error = process.StandardError.ReadToEnd();
                throw new Exception("FFmpeg conversion failed: " + error);
            }
        }
    }
}
