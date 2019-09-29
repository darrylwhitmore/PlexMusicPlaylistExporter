using System;
using Microsoft.Extensions.Configuration;

namespace PlexMusicPlaylistExporter {
	class Program {
		static void Main( string[] args ) {
			IConfiguration config = new ConfigurationBuilder()
				.AddJsonFile( "appsettings.json", true, true )
				.Build();

			Console.WriteLine( $"IP: {config["plexIp"]}" );
			Console.WriteLine( $"token: {config["plexToken"]}" );
		}
	}
}
