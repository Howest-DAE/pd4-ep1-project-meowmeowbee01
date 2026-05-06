using System;
using Unity.Services.Lobbies.Models;

namespace Assets.Scripts.LobbyUI.Model
{
	public class LobbyEventArgs : EventArgs
	{
		public LobbyEventArgs(Lobby lobby)
		{
			Lobby = lobby;
		}

		public Lobby Lobby { get; }
	}
}