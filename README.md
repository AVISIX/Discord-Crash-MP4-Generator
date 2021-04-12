# Discord-Crash-MP4-Generator

## How it works

By splitting a Video into multiple pieces (usually 2 is enough) and changing the PixelFormat of one to something different, then merging them back will cause the Chromium Media Player to Shit itself and take Discord down with it, since Discord is built on Electron and Electron is Chromium Based.

## Notice

This was a one time Project, it might not work after some time anymore. At the time of writing this (9th April 2021), it still works.

Also apparently this ONLY works on people who have Hardware Acceleration Enabled in Discord. 

If you find a new or better way to Crash Discord, please make a Pull Request or Fork the Repository. 

## Where is the .exe ?!?!?!

Its in the Bin Folder. If you go to "Discord-Crash-MP4-Generator\Discord Crash MP4 Generator\Discord Crash MP4 Generator\bin\Release\net5.0".

## IT DOESNT WORK 

See what it says in the Console. Most likely you are missing FFmpeg, which you must download at "https://ffmpeg.org/download.html".
As soon as you got it you must place the Binaries into the Directory "C:\FFmpeg\bin". Otherwise the C# Wrapper wont detect it.
