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
        public static async void ConvertImage(
            string inputPath,
            string outputFolder,
            string outputExtension)
        {
            string fileName = Path.GetFileNameWithoutExtension(inputPath);
            string outputPath = Path.Combine(outputFolder, fileName + outputExtension);
            string inputExt = Path.GetExtension(inputPath).ToLower();

            if (inputExt == ".mp4" && outputExtension == ".gif")
            {
                await ImageConverterService.ConvertMp4ToGif(inputPath, outputPath);
                return;
            }

            if (inputExt == ".gif" && outputExtension == ".mp4")
            {
                await ImageConverterService.ConvertGifToMp4(inputPath, outputPath);
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

            using SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(inputPath);

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
            // or full path: C:\\ffmpeg\\bin\\ffmpeg.exe

            string args =
                $"-i \"{inputPath}\" " +
                "-vf \"fps=12,scale=480:-1:flags=lanczos\" " +
                "-y " +
                $"\"{outputPath}\"";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using Process process = Process.Start(startInfo);
            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                string error = process.StandardError.ReadToEnd();
                throw new Exception("FFmpeg conversion failed: " + error);
            }
        }

        public static async Task ConvertGifToMp4(string inputPath, string outputPath)
        {
            string ffmpegPath = "ffmpeg";
            // or full path: C:\\ffmpeg\\bin\\ffmpeg.exe

            string args =
                $"-i \"{inputPath}\" " +
                "-movflags faststart " +
                "-pix_fmt yuv420p " +
                "-vf \"scale=trunc(iw/2)*2:trunc(ih/2)*2\" " +
                "-y " +
                $"\"{outputPath}\"";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = ffmpegPath,
                Arguments = args,
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using Process process = Process.Start(startInfo);
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                string error = process.StandardError.ReadToEnd();
                throw new Exception("GIF to MP4 conversion failed: " + error);
            }
        }
    }
}
