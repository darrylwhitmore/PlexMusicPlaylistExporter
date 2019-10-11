using System.IO;
using System.Xml.Linq;

namespace PlexPlaylistExporter {
	public abstract class FilePlaylistWriter : IPlaylistWriter {
		private readonly string destinationFolder;

		protected FilePlaylistWriter( string destinationFolder ) {
			this.destinationFolder = destinationFolder;
		}

		protected StreamWriter CreateStreamWriter( string destinationFileName ) {
			return new StreamWriter( Path.Combine( destinationFolder, destinationFileName ) );
		}

		protected string GetPlaylistTitle( XElement mediaContainerElement ) {
			var title = mediaContainerElement.Attribute( "title" )?.Value;

			if ( title == null ) {
				throw new PlaylistExportException( "Required attribute 'title' not found in media container element." );
			}

			return title;
		}

		public abstract void Write( XElement mediaContainerElement );
	}
}

