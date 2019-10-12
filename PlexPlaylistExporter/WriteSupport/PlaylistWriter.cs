using System.Xml.Linq;

namespace PlexPlaylistExporter.WriteSupport {
	public abstract class PlaylistWriter : IPlaylistWriter{
		public abstract void Write( XElement mediaContainerElement );

		protected string GetPlaylistTitle( XElement mediaContainerElement ) {
			var title = mediaContainerElement.Attribute( "title" )?.Value;

			if ( title == null ) {
				throw new PlaylistExportException( "Required attribute 'title' not found in media container element." );
			}

			return title;
		}
	}
}
