using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NScrape;
using PlexPlaylistExporter.WriteSupport;

namespace PlexPlaylistExporter {
	// Plex Media Server URL Commands
	// https://support.plex.tv/articles/201638786-plex-media-server-url-commands/
	public class Exporter {
		private readonly IWebClient webClient;
		private readonly string plexProtocol;
		private readonly string plexIp;
		private readonly string plexPort;
		private readonly string plexToken;

		public Exporter( IWebClient webClient, bool useHttps, string plexIp, string plexPort, string plexToken ) {
			this.webClient = webClient;
			this.plexProtocol = "http" + ( useHttps ? "s" : "" );
			this.plexIp = plexIp;
			this.plexPort = plexPort;
			this.plexToken = plexToken;
		}

		public void Export( string playlistType, bool excludeSmart, IPlaylistWriter playlistWriter ) {
			var allPlaylists = GetAllPlaylistsOfType( playlistType, excludeSmart );

			var allPlaylistNames = allPlaylists.Select( xe => xe.Attribute( "title" )?.Value );

			Export( playlistType, allPlaylistNames, excludeSmart, playlistWriter );
		}

		public void Export( string playlistType, IEnumerable<string> playlistNames, bool excludeSmart, IPlaylistWriter playlistWriter ) {
			var allPlaylists = GetAllPlaylistsOfType( playlistType, excludeSmart );

			var playlistElements = allPlaylists.Where( xe => playlistNames.Contains( xe.Attribute( "title" )?.Value ) ).ToList();

			if ( playlistElements.Count <= 0 ) {
				throw new PlaylistExportException( $"No playlists of type '{playlistType}' with the specified name(s) were found." );
			}

			foreach ( var playlistElement in playlistElements ) {
				var mediaContainerElement = GetMediaContainer( playlistElement );

				playlistWriter.Write( mediaContainerElement );
			}
		}

		public void Export( string playlistType, string playlistName, bool excludeSmart, IPlaylistWriter playlistWriter ) {
			Export( playlistType, new[] { playlistName }, excludeSmart, playlistWriter );
		}

		private IEnumerable<XElement> GetAllPlaylistsOfType( string playlistType, bool excludeSmart ) {
			var allPlaylistsUri = new Uri( $"{plexProtocol}://{plexIp}:{plexPort}/playlists?X-Plex-Token={plexToken}" );

			try {
				using ( var allPlaylistsResponse = webClient.SendRequest( allPlaylistsUri ) ) {
					if ( allPlaylistsResponse.Success ) {
						if ( allPlaylistsResponse is XmlWebResponse xmlResponse ) {
							var validSmart = excludeSmart
								? new[] {
									"0"
								}
								: new[] {
									"0", "1"
								};

							return xmlResponse.XDocument.Element( "MediaContainer" )?.Elements( "Playlist" ).
								Where( xe => xe.HasAttributes && 
								             xe.Attribute( "playlistType" )?.Value == playlistType && 
								             validSmart.Contains( xe.Attribute( "smart" )?.Value ) );
						}

						if ( allPlaylistsResponse is HtmlWebResponse htmlResponse ) {
							// Check whether the token is bad. This is a kludge: HttpWebResponse does not expose the underlying
							// response status code, so we have to examine the HTML.
							if ( htmlResponse.Html.Contains( "401" ) ) {
								throw new PlaylistExportException( $"The provided Plex authentication token '{plexToken}' is invalid." );
							}
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

			var playlistUri = new Uri( $"{plexProtocol}://{plexIp}:{plexPort}{key}?X-Plex-Token={plexToken}" );

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
