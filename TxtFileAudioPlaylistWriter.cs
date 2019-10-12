﻿using System.Xml.Linq;
using PlexPlaylistExporter.WriteSupport;

namespace PlexMusicPlaylistExporter {
	internal class TxtFileAudioPlaylistWriter : FilePlaylistWriter {
		public TxtFileAudioPlaylistWriter( string destinationFolder ) : base( destinationFolder ) {
		}

		public override void Write( XElement mediaContainerElement ) {
			var destinationFileName = $"{SanitizeFileName( GetPlaylistTitle( mediaContainerElement ) )}.txt";

			using ( var sw = CreateStreamWriter( destinationFileName ) ) {
				foreach ( var track in mediaContainerElement.Elements( "Track" ) ) {
					var audioFile = track.Element( "Media" )?.Element( "Part" )?.Attribute( "file" )?.Value;

					if ( audioFile != null ) {
						sw.WriteLine( audioFile );
					}
				}
			}
		}
	}
}