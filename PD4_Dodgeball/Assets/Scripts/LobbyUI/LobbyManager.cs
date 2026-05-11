using Assets.Scripts.LobbyUI.Model;
using Assets.Scripts.Singleton;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;

namespace Assets.Scripts.LobbyUI
{
	public class LobbyManager : MonobehaviourSingleton<LobbyManager>
	{
		public event EventHandler SessionJoined;
		//public event EventHandler SessionLeft;

		private const int _MAX_PLAYERS = 2;
		private SessionInfo _activeSession;

		private Task _initTask;
		public SessionInfo ActiveSession
		{
			get => _activeSession;
			set
			{
				if (_activeSession == value) return;

				if (_activeSession != null) _activeSession.SessionEnded -= SessionEnded;

				_activeSession = value;

				if (_activeSession != null) _activeSession.SessionEnded += SessionEnded;
			}
		}

		protected override async void Awake()
		{
			await EnsureInitializedAsync();
		}

		private void SessionEnded(object sender, EventArgs e)
		{
			ActiveSession = null;
		}

		private Task EnsureInitializedAsync()
		{
			return _initTask ??= InitializeAsync();
		}

		private static async Task InitializeAsync()
		{
			await UnityServices.InitializeAsync();
			await AuthenticationService.Instance.SignInAnonymouslyAsync();
		}

		private async void Update()
		{
			if (ActiveSession == null) return;
			await ActiveSession.UpdateSessionAsync();
		}

		private void OnSessionJoined()
		{
			if (ActiveSession == null) return;
			var transport = NetworkManager.Singleton.gameObject.GetComponent<UnityTransport>();
			transport.SetRelayServerData(ActiveSession.RelayServerData);
			if (ActiveSession.IsHost)
			{
				NetworkManager.Singleton.StartHost();
				Debug.Log("started as host");
			}
			else
			{
				NetworkManager.Singleton.StartClient();
				Debug.Log("started as client");
			}
			SessionJoined?.Invoke(this, EventArgs.Empty);
		}

		//private void OnSessionLeft()
		//{
		//	SessionLeft?.Invoke(this, EventArgs.Empty);
		//}

		public async Task<List<Lobby>> QueryLobbiesAsync()
		{
			await EnsureInitializedAsync();
			var response = await LobbyService.Instance.QueryLobbiesAsync();
			return response.Results;
		}

		public async Task<Lobby> CreateLobbyAsync(string name)
		{
			await EnsureInitializedAsync();
			Allocation allocation = await RelayService.Instance.CreateAllocationAsync(_MAX_PLAYERS, "europe-west4");
			RelayServerData serverdata = allocation.ToRelayServerData("wss");

			string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

			CreateLobbyOptions options = new()
			{
				Data = new Dictionary<string, DataObject>()
				{
					{ "relayJoinCode", new DataObject(DataObject.VisibilityOptions.Public, joinCode) }
				}
			};
			Lobby createdLobby = await LobbyService.Instance.CreateLobbyAsync(name, _MAX_PLAYERS, options);

			ActiveSession = new(createdLobby, AuthenticationService.Instance.PlayerId, serverdata);
			await ActiveSession.InitializeAsync();

			OnSessionJoined();

			return createdLobby;
		}

		public async Task JoinLobbyAsync(Lobby lobby)
		{
			await EnsureInitializedAsync();
			Lobby joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);
			string joinCode = joinedLobby.Data["relayJoinCode"].Value;
			JoinAllocation alloc = await RelayService.Instance.JoinAllocationAsync(joinCode);
			string relayConnectionType = "wss"; //set to "wss" for web support
			RelayServerData serverdata = alloc.ToRelayServerData(relayConnectionType);

			ActiveSession = new(joinedLobby, AuthenticationService.Instance.PlayerId, serverdata);
			await ActiveSession.InitializeAsync();

			OnSessionJoined();
		}

		//public async Task LeaveLobbyAsync()
		//{
		//	await EnsureInitializedAsync();
		//	string playerId = AuthenticationService.Instance.PlayerId;
		//	//if (ActiveSession.IsHost) await LobbyService.Instance.DeleteLobbyAsync(ActiveSession.Lobby.Id);
		//	//else
		//	await LobbyService.Instance.RemovePlayerAsync(ActiveSession.Lobby.Id, playerId);

		//	ActiveSession = null;

		//	OnSessionLeft();
		//}
	}
}