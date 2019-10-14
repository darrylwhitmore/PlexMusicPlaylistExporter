using System.IO;

namespace PlexPlaylistExporter.WriteSupport {
	public abstract class FilePlaylistWriter : PlaylistWriter {
		protected readonly string DestinationFolder;

		protected FilePlaylistWriter( string destinationFolder ) {
			this.DestinationFolder = destinationFolder;
		}

		protected StreamWriter CreateStreamWriter( string destinationFileName ) {
			return new StreamWriter( Path.Combine( DestinationFolder, destinationFileName ) );
		}

		protected string SanitizeFileName( string fileName ) {
			foreach ( var badChar in Path.GetInvalidFileNameChars() ) {
				fileName = fileName.Replace( badChar.ToString(), "" );
			}

			return fileName;
		}
	}
}

