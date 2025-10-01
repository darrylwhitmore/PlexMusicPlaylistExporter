using System;
using System.IO;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using PlexPlaylistExporter;
using NScrape;
using PlexMusicPlaylistExporter.Writers;
using PlexPlaylistExporter.WriteSupport;

namespace PlexMusicPlaylistExporter {
	class Program {
		static int Main( string[] args ) {
			const int appReturnValueOk = 0;
			const int appReturnValueFail = 1;

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

			// Finding an authentication token / X-Plex-Token
			// https://support.plex.tv/articles/204059436-finding-an-authentication-token-x-plex-token/
			var plexToken = app.Option( "-t|--token <plexToken>",
				"Your Plex authentication token.",
				CommandOptionType.SingleValue );

			var plexPort = app.Option( "-pt|--port <plexPort>",
				"Your Plex port.",
				CommandOptionType.SingleValue );

			var plexIp = app.Option( "-i|--ip <plexIP>",
				"Your Plex IP address.",
				CommandOptionType.SingleValue );

			var useHttps = app.Option( "-https|--useHttps",
				"Provide if your Plex server requires HTTPS.",
				CommandOptionType.SingleOrNoValue );

			var playlistToExport = app.Option( "-p|--playlist <playlistName>",
				"The music playlist to export. Use '*' to export ALL music playlists.",
				CommandOptionType.SingleValue );

			var excludeSmart = app.Option( "-xs|--excludeSmart",
				"If specified, smart playlists will be excluded.",
				CommandOptionType.NoValue );

			var exportFormat = app.Option( "-f|--format <formatType>",
				"The export format: 'json', 'txt' (default if omitted), 'wpl', 'xml'.",
				CommandOptionType.SingleValue );

			var destinationFolder = app.Option( "-d|--destinationFolder <folderPath>",
				"The destination folder where the music playlist file will be written",
				CommandOptionType.SingleValue );

			app.OnExecute( () => {
				if ( !plexToken.HasValue() ) {
					Console.WriteLine( "Plex authentication token is required." );
					return appReturnValueFail;
				}

				if ( !plexIp.HasValue() ) {
					Console.WriteLine( "Plex IP address is required." );
					return appReturnValueFail;
				}

				if ( !plexPort.HasValue() ) {
					Console.WriteLine( "Plex port is required." );
					return appReturnValueFail;
				}

				if ( playlistToExport.HasValue() && destinationFolder.HasValue() ) {
					if ( !Directory.Exists( destinationFolder.Value() ) ) {
						Console.WriteLine( $"Destination folder does not exist: '{destinationFolder.Value()}'" );
						return appReturnValueFail;
					}

					IPlaylistWriter writer;
					var formatType = exportFormat.HasValue() ? exportFormat.Value().ToLowerInvariant() : "txt";
					switch ( formatType ) {
						case "json":
							writer = new JsonFilePlaylistWriter( destinationFolder.Value() );
							break;

						case "txt":
							writer = new TxtFilePlaylistWriter( destinationFolder.Value() );
							break;

						case "wpl":
							writer = new WplFilePlaylistWriter( destinationFolder.Value() , $"Plex Music Playlist Exporter -- {Assembly.GetExecutingAssembly().GetName().Version}" );
							break;

						case "xml":
							writer = new XmlFilePlaylistWriter( destinationFolder.Value() );
							break;

						default:
							Console.WriteLine( $"Invalid export format type: '{formatType}'" );
							return appReturnValueFail;
					}

					var playlistExporter = new Exporter( new WebClient(), useHttps.HasValue(), plexIp.Value(), plexPort.Value(), plexToken.Value() );

					if ( playlistToExport.Value() == "*" ) {
						// All music playlists
						playlistExporter.Export( "audio", excludeSmart.HasValue(), writer );
					}
					else {
						// Specified music playlist
						playlistExporter.Export( "audio", playlistToExport.Value(), excludeSmart.HasValue(), writer );
					}

					return appReturnValueOk;
				}

				Console.WriteLine( "One or more required arguments were not provided." );
				app.ShowHint();
				return appReturnValueFail;
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
