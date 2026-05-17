using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace Assets.Scripts.LobbyUI.Model
{
	public class SessionInfo : INotifyPropertyChanged
	{
		private const float _HEARTBEAT_INTERVAL = 15f;
		public event EventHandler SessionEnded;
		public event PropertyChangedEventHandler PropertyChanged;

		//Properties
		public Lobby Lobby { get; private set; }
		public string LocalPlayerId { get; }
		public RelayServerData RelayServerData { get; }
		public bool IsHost { get; }
		private LobbyEventCallbacks _callbacks;
		private float _heartbeatTimer = _HEARTBEAT_INTERVAL;

		//private int _playerCount;
		//public int PlayerCount
		//{
		//	get => _playerCount;
		//	set
		//	{
		//		if (_playerCount == value) return;
		//		_playerCount = value;
		//		OnPropertyChanged();
		//	}
		//}

		public SessionInfo(Lobby lobby, string playerId, RelayServerData relayServerData)
		{
			Lobby = lobby;
			LocalPlayerId = playerId;
			RelayServerData = relayServerData;
			IsHost = Lobby.HostId == playerId;
		}

		public async Task InitializeAsync() //Async Initialize Pattern
		{
			await UpdateLobbyInfoAsync();
			await RegisterCallbacksAsync();
		}

		private async Task RegisterCallbacksAsync()
		{
			_callbacks = new LobbyEventCallbacks();
			await LobbyService.Instance.SubscribeToLobbyEventsAsync(Lobby.Id, _callbacks);
			_callbacks.LobbyDeleted += OnSessionEnded;
			_callbacks.KickedFromLobby += OnSessionEnded;
			_callbacks.PlayerJoined += Callbacks_PlayerJoined;
			_callbacks.PlayerLeft += Callbacks_PlayerLeft;
		}

		private async void Callbacks_PlayerJoined(List<LobbyPlayerJoined> obj)
		{
			await UpdateLobbyInfoAsync();
		}

		private async void Callbacks_PlayerLeft(List<int> obj)
		{
			await UpdateLobbyInfoAsync();
		}

		private async Task UpdateLobbyInfoAsync()
		{
			Lobby = await LobbyService.Instance.GetLobbyAsync(Lobby.Id);
			//PlayerCount = Lobby.Players.Count;
		}

		public async Task UpdateSessionAsync()
		{
			if (!IsHost) return;

			_heartbeatTimer -= Time.deltaTime;

			if (_heartbeatTimer >= 0) return;

			_heartbeatTimer += _HEARTBEAT_INTERVAL;
			await LobbyService.Instance.SendHeartbeatPingAsync(Lobby.Id);
		}

		public async Task Leave()
		{
			string playerId = AuthenticationService.Instance.PlayerId;
			await LobbyService.Instance.RemovePlayerAsync(Lobby.Id, playerId);
			OnSessionEnded();
		}

		protected virtual void OnSessionEnded()
		{
			SessionEnded?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = "")
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}
}