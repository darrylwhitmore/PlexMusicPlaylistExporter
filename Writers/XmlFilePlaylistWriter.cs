using System.IO;
using System.Xml;
using System.Xml.Linq;
using PlexPlaylistExporter.WriteSupport;

namespace PlexMusicPlaylistExporter.Writers {
	internal class XmlFilePlaylistWriter : FilePlaylistWriter {
		public XmlFilePlaylistWriter( string destinationFolder ) : base( destinationFolder ) {
		}

		public override void Write( XElement mediaContainerElement ) {
			var destinationFileName = $"{SanitizeFileName( GetPlaylistTitle( mediaContainerElement ) )}.xml";

			var writerSettings = new XmlWriterSettings {
				Indent = true
			};

			using ( var writer = XmlWriter.Create( Path.Combine( DestinationFolder, destinationFileName ), writerSettings ) ) {
				writer.WriteStartDocument();
				writer.WriteStartElement( "Tracks" );

				foreach ( var track in mediaContainerElement.Elements( "Track" ) ) {
					var audioFile = track.Element( "Media" )?.Element( "Part" )?.Attribute( "file" )?.Value;

					if ( audioFile != null ) {
						writer.WriteElementString( "Track", audioFile );
					}
				}

				writer.WriteEndElement();
				writer.WriteEndDocument();
			}
		}
	}
}
