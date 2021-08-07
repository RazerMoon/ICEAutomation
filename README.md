# ICE Automation

A command line application to bach process image stitching using the marvellous [Image Compose Editor (ICE)](https://www.microsoft.com/en-us/research/product/computational-photography-applications/image-composite-editor).

# Build
Now the project has moved to netcoreapp3.1. I recommend you to use VS Code to work with it, but only .net core 3.1 SDK is required. Follow this:
1) install [dot.net core 3.1 SDK](https://dotnet.microsoft.com/download/dotnet-core/3.1)
2) open a cmd, move to your <projects> folder and execute:
```
> git clone https://github.com/danice/ICEAutomation.git
> cd ICEAutomation
> dotnet build
```
You will found the compiled files in <projects>\ICEAutomation\src\bin\Debug\netcoreapp3.1
Next adjust the ICEAutomation.bat to point to this folder. Then copy the batch file to c:\Windows or some folder in system Path so you can execute the application from any folder.


# Instructions

1. Run this:
```
ICEAutomation process --motion rotatingMotion [No. of images in each folder] *.JPG [Directory with images]
```

Should look like this:

```
ICEAutomation process --motion rotatingMotion 55 *.JPG "G:\SteamLibrary\steamapps\common\Grand Theft Auto V\360Pictures"
```

# Warning

The application uses button labels to automate ICE. Depending of your environment this names can change (for example "Save" button).
You can configure the button labels in your ICE in app.config. 

**The processed files will be copied to the last folder used by ICE. So I recommend manually executing a stich first, to select the destination folder.**

