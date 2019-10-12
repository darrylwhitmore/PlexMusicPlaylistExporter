using System.IO;

namespace PlexPlaylistExporter.WriteSupport {
	public abstract class FilePlaylistWriter : PlaylistWriter {
		private readonly string destinationFolder;

		protected FilePlaylistWriter( string destinationFolder ) {
			this.destinationFolder = destinationFolder;
		}

		protected StreamWriter CreateStreamWriter( string destinationFileName ) {
			return new StreamWriter( Path.Combine( destinationFolder, destinationFileName ) );
		}

		protected string SanitizeFileName( string fileName ) {
			foreach ( var badChar in Path.GetInvalidFileNameChars() ) {
				fileName = fileName.Replace( badChar.ToString(), "" );
			}

			return fileName;
		}
	}
}

