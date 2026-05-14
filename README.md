# ImageConverter

<img width="600" src="https://github.com/NhatMNTran/ImageConverter/blob/master/AppScreen.png" alt="Image Converter Icon">

Simple Image Converter is a tool that allows you to convert images from one format to another.
It supports a wide range of image formats, including JPEG, PNG, GIF, BMP, and more. 
The tool is easy to use and provides a user-friendly interface for selecting the input and output formats.

I made this tool to help me practice coding in C# and explores packages. (Also when I download images from the net it comes in .JFIF form so I need a way to convert it lol)

- Convert images between formats:
  - PNG
  - JPG / JPEG
  - GIF
  - BMP
  - TIFF
  - WEBP
  - HEIC / JFIF

- Convert media files:
  - MP4 → GIF (Application need to be close after conversion for GIF file to finish loading)
  - GIF → MP4

- Simple graphical user interface (WPF)
- File picker with automatic format detection
- Destination folder selection
- Extendable architecture for adding new formats easily
- Coded in Visual Studio

- Technologies Used
	- C# (.NET 8)
	- WPF (Windows Presentation Foundation)
	- FFmpeg (video processing)
	- ImageSharp (image processing)
	- Magick.NET (HEIC support)

- Requirements

Before running the application, ensure you have:

1. .NET 8 SDK
https://dotnet.microsoft.com/en-us/download

2. FFmpeg installed and added to PATH
https://ffmpeg.org/download.html

To run:

dotnet build  
dotnet run


P.S.: For Windows Users, to create a .exe file so you would not need to initiate prompts every time, you can publish the application as a self-contained executable:

Step 1 — Open Terminal in Project Folder

In your project directory (where .csproj is):

	"cd YourProjectFolder"

Step 2 — Run Publish Command

	"dotnet publish -c Release -r win-x64 --self-contained true"

Step 3 — Find your .exe in the publish folder:

	"bin\Release\net8.0\win-x64\publish\YourAppName.exe"
