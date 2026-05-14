namespace ImageConverterApp.Models
{
    //Simple class to represent an image format, including its display name and file extension.
    public class ImageFormatInfo
    {
        public string Name { get; set; }
        public string Extension { get; set; }

        public ImageFormatInfo(string name, string extension)
        {
            Name = name;
            Extension = extension;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
