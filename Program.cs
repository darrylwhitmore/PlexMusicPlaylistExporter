﻿using System;
using System.IO;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Configuration;

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
	}
}
