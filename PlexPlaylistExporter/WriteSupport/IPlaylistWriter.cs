using System.Xml.Linq;

namespace PlexPlaylistExporter.WriteSupport {
	public interface IPlaylistWriter {
		void Write( XElement mediaContainerElement );
	}
}
