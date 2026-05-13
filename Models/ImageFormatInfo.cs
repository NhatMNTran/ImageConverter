namespace ImageConverterApp.Models
{
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
