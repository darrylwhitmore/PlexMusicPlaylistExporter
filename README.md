# PlexMusicPlaylistExporter

Long, long ago I had a lot of old school Windows Media Player playlists that I used to play tunes on my Windows machine.

Then, long ago, I got Sonos, and I started to use it to play the old WMP playlists. This worked great, but maintaining the playlists using WMP was a pain.

Then, less long ago, I got Plex. I ignored its music functionality for a long time because *I had Sonos*.

Then, recently, I replaced by beloved 20 year old Honda CRV with...yes, another CRV. But now I had Android Auto. And what's this? I can play Plex music on Android Auto?! But wait, I have no music on Plex because *I have Sonos*.

So. I pointed Plex to the same music library that feeds Sonos and Presto! It works!

But. Selecting tunes on the go in the Plex app in Android Auto is tedious. I need...PLAYLISTS. 

So. I recreated all my WMP playlists in Plex and boy, is it a much nicer way to do it. BANG! Plex playlists playin' via Android Auto in my car!

But. Now I have 2 sets of playlists to maintain and they're already different, because the Plex playlists have all the stuff I added to the music library but never got around to adding to the old WMP playlists because...tedious.

Maintaining 2 sets of source is a screaming red flag to the developer in me. Obviously, I need a way to sync these playlists.

Does Plex have a REST API that I can use? It does, but they don't call it that. Here's what I found: [Plex Media Server URL Commands](https://support.plex.tv/articles/201638786-plex-media-server-url-commands/). It *doesn't exactly* explain how to do what I want, but after some experimenting, I was able to figure it out.

So now. I need to write a little program to export my Plex playlists to files. That's this project/repository. Later, I'll need to write another program to sync the exported Plex playlists with the old WMP WPL playlist files. 

I decided to put the exporter here on Github, as others might wish to do this. 

Syncing playlist files is a pretty specific task for me, but probably not for others, so that may or may not go on Github when I get to it.

There. That's more than you wanted to know.





