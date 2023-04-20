# Plex Music Playlist Exporter

The **Plex Music Playlist Exporter** is a little .NET console application that will allow you to export your [Plex](https://www.plex.tv/) music playlists to a file.

## Environment Setup
The simplest, but heaviest, approach is to use [Visual Studio](https://visualstudio.microsoft.com/), which will also get you .NET. You can then build the code in Visual Studio, and execute the application via the command line using .NET.

The lightest approach is to use  [.NET](https://dotnet.microsoft.com/en-us/download) on the command line to both build and execute the application. To build this way:

1. Get the code, either by cloning the repo or downloading. 
2. Open the command line and navigate to the directory where the solution file, PlexMusicPlaylistExporter.sln, is located.
3. Use the [dotnet publish](https://learn.microsoft.com/en-us/dotnet/core/tools/dotnet-publish) command to build the application.


```
> dotnet  publish -c release
```
## Plex Server Info
You'll need to gather the following information about your Plex server:
* The IP address of the server.
* The port of the server (usually 32400 but you may have changed it)
* The authentication token. You can get this by following the instructions in: [Finding an authentication token / X-Plex-Token](https://support.plex.tv/articles/204059436-finding-an-authentication-token-x-plex-token/).

## Application Command Line options
The command line options are:
```
Usage: PlexMusicPlaylistExporter [options]

Options:
  -?|-h|--help                         Show help information
  -t|--token <plexToken>               Your Plex authentication token.
  -pt|--port <plexPort>                Your Plex port.
  -i|--ip <plexIP>                     Your Plex IP address.
  -p|--playlist <playlistName>         The music playlist to export. Use '*' to export ALL music playlists.
  -xs|--excludeSmart                   If specified, smart playlists will be excluded.
  -f|--format <formatType>             The export format: 'json', 'txt' (default if omitted), 'wpl', 'xml'.
  -d|--destinationFolder <folderPath>  The destination folder where the music playlist file will be written  
```
## Examples
This command does not specify an export format, so the default text format is used to produce: ***D:\destination\Sinatra.txt***:
```
> dotnet  PlexMusicPlaylistExporter.dll -t XXXXXX -i 192.168.0.999 -pt 32400 -p Sinatra -d D:\destination
```
This command produces ***D:\destination\Sinatra.json***:
```
> dotnet  PlexMusicPlaylistExporter.dll -t XXXXXX -i 192.168.0.999 -pt 32400 -p Sinatra -f json -d D:\destination
```
This command exports all Plex music playlists to XML files in ***D:\destination***, one file for each playlist:
```
> dotnet  PlexMusicPlaylistExporter.dll -t XXXXXX -i 192.168.0.999 -pt 32400 -p * -f xml -d D:\destination
```

This command exports all Plex music playlists to XML files in ***D:\destination***, one file for each playlist, excluding *smart playlists*:
```
> dotnet  PlexMusicPlaylistExporter.dll -t XXXXXX -i 192.168.0.999 -pt 32400 -p * -xs -f xml -d D:\destination
```

## The Backstory
Long, long, long ago I had a lot of old school **Windows Media Player** (WMP) playlists that I used to play tunes on my Windows machine.

Then, long, long ago, I got **[Sonos](https://www.sonos.com/)**, and I started to use it to play the old WMP playlists. This worked great, but maintaining the playlists using WMP was a pain.

Then, long ago, I got **Plex** for my video library. I ignored its music functionality completely because *I had **Sonos***.

Then, recently, I replaced by beloved 20 year old **Honda CR-V** with...yes, another **CR-V**. But now I had **[Android Auto](https://www.android.com/auto/)**. And what's this? I can play **Plex** music on **Android Auto**?! But wait, I have no music on **Plex** because *I have **Sonos***.

So. I pointed **Plex** to the same music library that feeds **Sonos** and *Presto*! It works!

But. Selecting tunes on the go in the **Plex** app in **Android Auto** was tedious. I needed...*PLAYLISTS*. 

So. I recreated all my WMP playlists in **Plex** and boy, is it a much nicer way to do that work. *BANG*! **Plex** playlists playin' via **Android Auto** in my car!

But. Now I have 2 sets of playlists to maintain and they're already different, because the **Plex** playlists have all the stuff I added to the music library but never got around to adding to the old WMP playlists because...that way is tedious!

Maintaining 2 sets of source was a screaming red flag to the developer in me. Obviously, I needed a way to sync these playlists.

Does **Plex** have a REST API that I can use? It kind of does, but they don't call it that. Here's what I found: [Plex Media Server URL Commands](https://support.plex.tv/articles/201638786-plex-media-server-url-commands/). It *doesn't exactly* explain how to do what I want, but after some experimenting, I was able to figure it out.

So now. I needed to write a little program to export my **Plex** playlists to files. That's this project/repository. Later, I'll need to write another program to sync the exported **Plex** playlists with the old WMP WPL playlist files. 

I decided to put the export app here on Github, as others might wish to do this. 

Syncing playlist files is a pretty specific task for me, but probably not for others, so that may or may not go on Github when I get to it.

There. That's more than you wanted to know.





