using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using PlexPlaylistExporter.WriteSupport;

namespace PlexMusicPlaylistExporter.Writers {
	internal class WplFilePlaylistWriter : FilePlaylistWriter {
		private readonly string generatorValue;

		public WplFilePlaylistWriter( string destinationFolder, string generatorValue ) : base( destinationFolder ) {
			this.generatorValue = generatorValue;
		}

		public override void Write( XElement mediaContainerElement ) {
			var playlistTitle = GetPlaylistTitle( mediaContainerElement );

			var tracks = mediaContainerElement.Elements( "Track" ).ToList();

			var destinationFileName = $"{SanitizeFileName( playlistTitle )}.wpl";

			var writerSettings = new XmlWriterSettings {
				Indent = true,
				OmitXmlDeclaration = true
			};

			using ( var writer = XmlWriter.Create( Path.Combine( DestinationFolder, destinationFileName ), writerSettings ) ) {
				writer.WriteStartDocument();

				writer.WriteRaw( "<?wpl version=\"1.0\"?>" );
				writer.WriteRaw( "\n" );

				writer.WriteStartElement( "smil" );

				writer.WriteStartElement( "head" );

				writer.WriteStartElement( "meta" );
				writer.WriteAttributeString( "name", "Generator" );
				writer.WriteAttributeString( "content", generatorValue );
				writer.WriteEndElement();

				writer.WriteStartElement( "meta" );
				writer.WriteAttributeString( "name", "ItemCount" );
				writer.WriteAttributeString( "content", tracks.Count.ToString() );
				writer.WriteEndElement();

				writer.WriteElementString( "title", playlistTitle );
				writer.WriteEndElement();

				writer.WriteStartElement( "body" );

				writer.WriteStartElement( "seq" );
				foreach ( var track in tracks ) {
					var audioFile = track.Element( "Media" )?.Element( "Part" )?.Attribute( "file" )?.Value;

					if ( audioFile != null ) {
						writer.WriteStartElement( "media" );
						writer.WriteAttributeString( "src", audioFile );
						writer.WriteEndElement();
					}
				}
				writer.WriteEndElement();

				writer.WriteEndElement();

				writer.WriteEndElement();
				writer.WriteEndDocument();
			}
		}
	}
}
