using System;
using System.IO;
using ImageMagick;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Formats.Tiff;
using SixLabors.ImageSharp.Formats.Webp;

namespace ImageConverterApp.Services
{
    public static class ImageConverterService
    {
        public static void ConvertImage(
            string inputPath,
            string outputFolder,
            string outputExtension)
        {
            string fileName = Path.GetFileNameWithoutExtension(inputPath);
            string outputPath = Path.Combine(outputFolder, fileName + outputExtension);

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
    }
}
