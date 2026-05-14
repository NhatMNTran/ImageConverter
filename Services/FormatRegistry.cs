using System.Collections.Generic;
using ImageConverterApp.Models;

namespace ImageConverterApp.Services
{
    //Simple list of format supported by the app, add new formats here when expanding the app.
    public static class FormatRegistry
    {
        public static List<ImageFormatInfo> SupportedFormats = new()
        {
            new ImageFormatInfo("PNG", ".png"),
            new ImageFormatInfo("JPG", ".jpg"),
            new ImageFormatInfo("JPEG", ".jpeg"),
            new ImageFormatInfo("GIF", ".gif"),
            new ImageFormatInfo("BMP", ".bmp"),
            new ImageFormatInfo("TIFF", ".tiff"),
            new ImageFormatInfo("WEBP", ".webp"),
            new ImageFormatInfo("JFIF", ".jfif"),
            new ImageFormatInfo("HEIC", ".heic"),
            new ImageFormatInfo("MP4 (Video)", ".mp4")
        };
    }
}
