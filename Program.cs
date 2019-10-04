using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;
using NScrape;

namespace PlexMusicPlaylistExporter {
	class Program {
		static int Main( string[] args ) {
			const int appReturnValueOk = 0;
			const int appReturnValueFail = 1;

			IConfiguration config = new ConfigurationBuilder()
				.AddJsonFile( "appsettings.json", true, true )
				.Build();

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
				"The playlist to export",
				CommandOptionType.SingleValue );

			var destinationFolder = app.Option( "-d|--destinationFolder <folderPath>",
				"The destination folder where the playlist file will be written",
				CommandOptionType.SingleValue );

			app.OnExecute( () => {
				var returnValue = appReturnValueOk;

				if ( playlistToExport.HasValue() && destinationFolder.HasValue() ) {
					if ( !Directory.Exists( destinationFolder.Value() ) ) {
						Console.WriteLine( $"Destination folder does not exist: {destinationFolder.Value()}" );
						returnValue = appReturnValueFail;
					}

					var webClient = new WebClient();

					var playlistElement = GetPlaylist( config, webClient, playlistToExport.Value() );
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
			catch ( Exception ex ) {
				Console.WriteLine( $"Unexpected error: {ex.Message}" );
			}

			return appReturnValueFail;
		}

		private static XElement GetPlaylist( IConfiguration config, WebClient webClient, string playlist ) {
			var allPlaylistsUri = new Uri( $"http://{config["plexIp"]}:{config["plexPort"]}/playlists?X-Plex-Token={config["plexToken"]}" );

			Console.WriteLine( $"Getting list of all playlists from {allPlaylistsUri}" );

			using ( var allPlaylistsResponse = webClient.SendRequest( allPlaylistsUri ) ) {
				if ( allPlaylistsResponse.Success ) {
					if ( allPlaylistsResponse is XmlWebResponse xmlResponse ) {
						var playlistElement = xmlResponse.XDocument.Element( "MediaContainer" ).Elements( "Playlist" ).SingleOrDefault( e => e.HasAttributes && e.Attribute( "title" ).Value == playlist );

						return playlistElement;  /here; test, allow feedback for success and graceful failure
					}
				}
			}

			return null;
		}
	}
}
