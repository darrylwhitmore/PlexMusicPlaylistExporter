using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NScrape;
using PlexPlaylistExporter.WriteSupport;

namespace PlexPlaylistExporter {
	public class Exporter {
		private readonly IWebClient webClient;
		private readonly string plexIp;
		private readonly string plexPort;
		private readonly string plexToken;

		public Exporter( IWebClient webClient, string plexIp, string plexPort, string plexToken ) {
			this.webClient = webClient;
			this.plexIp = plexIp;
			this.plexPort = plexPort;
			this.plexToken = plexToken;
		}

		public void Export( string playlistType, IPlaylistWriter playlistWriter ) {
			var allPlaylists = GetAllPlaylistsOfType( playlistType );

			var allPlaylistNames = allPlaylists.Select( xe => xe.Attribute( "title" )?.Value );

			Export( playlistType, allPlaylistNames, playlistWriter );
		}

		public void Export( string playlistType, IEnumerable<string> playlistNames, IPlaylistWriter playlistWriter ) {
			var allPlaylists = GetAllPlaylistsOfType( playlistType );

			var playlistElements = allPlaylists.Where( xe => playlistNames.Contains( xe.Attribute( "title" )?.Value ) ).ToList();

			if ( playlistElements.Count <= 0 ) {
				throw new PlaylistExportException( $"No playlists of type '{playlistType}' with the specified name(s) were found." );
			}

			foreach ( var playlistElement in playlistElements ) {
				var mediaContainerElement = GetMediaContainer( playlistElement );

				playlistWriter.Write( mediaContainerElement );
			}
		}

		public void Export( string playlistType, string playlistName, IPlaylistWriter playlistWriter ) {
			Export( playlistType, new[] { playlistName }, playlistWriter );
		}

		private IEnumerable<XElement> GetAllPlaylistsOfType( string playlistType ) {
			var allPlaylistsUri = new Uri( $"http://{plexIp}:{plexPort}/playlists?X-Plex-Token={plexToken}" );

			try {
				using ( var allPlaylistsResponse = webClient.SendRequest( allPlaylistsUri ) ) {
					if ( allPlaylistsResponse.Success ) {
						if ( allPlaylistsResponse is XmlWebResponse xmlResponse ) {
							return xmlResponse.XDocument.Element( "MediaContainer" )?.Elements( "Playlist" ).Where( xe => xe.HasAttributes && xe.Attribute( "playlistType" )?.Value == playlistType );
						}

						throw new PlaylistExportException( $"Unexpected response type ('{allPlaylistsResponse.ResponseType}') attempting to get all '{playlistType}' playlists at {allPlaylistsUri}" );
					}

					throw new PlaylistExportException( $"Unsuccessful response attempting to get all '{playlistType}' playlists at {allPlaylistsUri}" );
				}
			}
			catch ( Exception ex ) {
				throw new PlaylistExportException( $"Unexpected failure attempting to get all '{playlistType}' playlists at {allPlaylistsUri}: {ex.Message}" );
			}
		}

		private XElement GetMediaContainer( XElement playlistElement ) {
			var key = playlistElement.Attribute( "key" )?.Value;

			if ( key == null ) {
				throw new PlaylistExportException( "Required attribute 'key' not found in playlist element." );
			}

			var playlistUri = new Uri( $"http://{plexIp}:{plexPort}{key}?X-Plex-Token={plexToken}" );

			using ( var playlistResponse = webClient.SendRequest( playlistUri ) ) {
				if ( playlistResponse.Success ) {
					if ( playlistResponse is XmlWebResponse xmlResponse ) {
						return xmlResponse.XDocument.Element( "MediaContainer" );
					}

					throw new PlaylistExportException( $"Unexpected response type ('{playlistResponse.ResponseType}') attempting to get playlist at {playlistUri}" );
				}

				throw new PlaylistExportException( $"Unsuccessful response attempting to get playlist at {playlistUri}" );
			}
		}
	}
}
