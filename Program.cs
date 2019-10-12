using System;
using System.IO;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using PlexPlaylistExporter;
using NScrape;
using PlexMusicPlaylistExporter.Writers;

namespace PlexMusicPlaylistExporter {
	class Program {
		static int Main( string[] args ) {
			const int appReturnValueOk = 0;
			const int appReturnValueFail = 1;

			IConfiguration config = new ConfigurationBuilder()
				.AddJsonFile( "appsettings.json", true, true )
				.Build();

			if ( config["plexIp"].StartsWith( "[" ) || config["plexToken"].StartsWith( "[" ) ) {
				Console.WriteLine( "Configure your Plex settings in 'appsettings.json' and retry." );
				return appReturnValueFail;
			}

			// Command line argument parsing in .NET Core with Microsoft.Extensions.CommandLineUtils
			// https://www.areilly.com/2017/04/21/command-line-argument-parsing-in-net-core-with-microsoft-extensions-commandlineutils/
			//
			// natemcmaster/CommandLineUtils
			// https://github.com/natemcmaster/CommandLineUtils
			var app = new CommandLineApplication {
				Name = "PlexMusicPlaylistExporter",
				Description = "Plex Music Playlist Exporter is a little tool that you can use to export your Plex Music playlists."
			};

			app.HelpOption( "-?|-h|--help" );

			var playlistToExport = app.Option( "-p|--playlist <playlistName>",
				"The music playlist to export. Use '*' to export ALL music playlists.",
				CommandOptionType.SingleValue );

			var destinationFolder = app.Option( "-d|--destinationFolder <folderPath>",
				"The destination folder where the music playlist file will be written",
				CommandOptionType.SingleValue );

			app.OnExecute( () => {
				var returnValue = appReturnValueOk;

				if ( playlistToExport.HasValue() && destinationFolder.HasValue() ) {
					if ( !Directory.Exists( destinationFolder.Value() ) ) {
						Console.WriteLine( $"Destination folder does not exist: {destinationFolder.Value()}" );
						returnValue = appReturnValueFail;
					}

					var playlistExporter = new Exporter( new WebClient(), config["plexIp"], config["plexPort"], config["plexToken"] );

					if ( playlistToExport.Value() == "*" ) {
						// All music playlists
						playlistExporter.Export( "audio", new AudioTxtFilePlaylistWriter( destinationFolder.Value() ) );
					}
					else {
						// Specified music playlist
						playlistExporter.Export( "audio", playlistToExport.Value(), new AudioTxtFilePlaylistWriter( destinationFolder.Value() ) );
					}
				}
				else {
					app.ShowHint();
				}

				return returnValue;
			} );

			try {
				return app.Execute( args );
			}
			catch ( CommandParsingException ex ) {
				Console.WriteLine( $"Unable to parse provided command line options: {ex.Message}" );
			}
			catch ( PlaylistExportException ex ) {
				Console.WriteLine( $"Unable to get playlist: {ex.Message}" );
			}
			catch ( Exception ex ) {
				Console.WriteLine( $"Unexpected error: {ex.Message}" );
			}

			return appReturnValueFail;
		}
	}
}
