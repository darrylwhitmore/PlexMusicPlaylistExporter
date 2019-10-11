using System;

namespace PlexPlaylistExporter {
	public class PlaylistExportException : Exception {
		public PlaylistExportException( string message ) : base( message ) {
		}
	}
}
