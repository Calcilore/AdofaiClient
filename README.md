# Alternative Client for  ADOFAI
## A alternative client for A Dance of Fire and Ice written in MonoGame

This is a simple client for A Dance of Fire and Ice, designed to have better performance than the original client, all this client is designed to do is to load custom levels.

### This client is in early development, dont expect all levels to work with this client

![](Screenshot.png)

# How do I install this?

1. Go to the Releases Page ([https://github.com/Calcilore/AdofaiClient/releases/latest](https://github.com/Calcilore/AdofaiClient/releases/latest))
2. Download the latest version for your computer 
3. Extract and run

# How do I use this?

## I want to use command line arguments
for example: <br>
Adofai.exe C:\Users\Username\Downloads\level.adofai

You can also add -a or --auto for autoplay, or get more usages with --help

You could also open the level by right clicking on the .ADOFAI file and opening it with Adofai.exe

## Where are my custom levels?

Your custom levels are located in "&lt;Your Steam Folder&gt;/steamapps/workshop/content/977950/" 

#### Your steam folder is different depending on the OS you use
On Windows its located at: C:\Program Files (x86)\Steam <br>
On Linux its located at: ~/.local/share/Steam <br>
On Linux using flatpak: ~/.var/app/com.valvesoftware.Steam/data/Steam/

# Current Compatibility Status

## Known issues
 - Zoom is incapable of going below 85%
 - On older version of ADOFAI that use a string for the angledata, not all the angles are supported
 - There is no countdown for the level to start

## Things currently implemented:
 - Song Loading with offset
 - BPM Changing
 - Twirls
 - Camera Controls
 - Position Track
