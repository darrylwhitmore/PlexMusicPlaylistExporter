using System.Collections.Generic;
using System.Xml.Linq;
using PlexPlaylistExporter;

namespace PlexMusicPlaylistExporter {
	internal abstract class FilePlaylistWriter : IPlaylistWriter {
		protected FilePlaylistWriter( string destinationFolder ) {

		}

		public void Write( IEnumerable<XElement> items ) {
			throw new System.NotImplementedException();
		}
	}
}
