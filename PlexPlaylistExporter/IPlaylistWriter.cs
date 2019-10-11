using System.Xml.Linq;

namespace PlexPlaylistExporter {
	public interface IPlaylistWriter {
		void Write( XElement mediaContainerElement );
	}
}
