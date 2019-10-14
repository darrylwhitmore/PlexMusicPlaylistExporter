using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json;
using PlexPlaylistExporter.WriteSupport;

namespace PlexMusicPlaylistExporter.Writers {
	internal class JsonFilePlaylistWriter : FilePlaylistWriter {
		public JsonFilePlaylistWriter( string destinationFolder ) : base( destinationFolder ) {
		}

		public override void Write( XElement mediaContainerElement ) {
			var destinationFileName = $"{SanitizeFileName( GetPlaylistTitle( mediaContainerElement ) )}.json";

			var tracks = new List<string>();

			foreach ( var track in mediaContainerElement.Elements( "Track" ) ) {
				var audioFile = track.Element( "Media" )?.Element( "Part" )?.Attribute( "file" )?.Value;

				if ( audioFile != null ) {
					tracks.Add( audioFile );
				}
			}

			var jsonData = JsonConvert.SerializeObject( tracks, Formatting.Indented );
			File.WriteAllText( Path.Combine( DestinationFolder, destinationFileName ), jsonData );
		}
	}
}
