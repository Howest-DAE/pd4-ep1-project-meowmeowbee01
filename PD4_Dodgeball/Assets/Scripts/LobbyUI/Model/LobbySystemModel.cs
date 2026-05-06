using Assets.Scripts.MVP.Model;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Lobbies.Models;

namespace Assets.Scripts.LobbyUI.Model
{
	public class LobbySystemModel : ModelBase
	{
		#region Enums
		public enum Mode
		{
			JoinLobby,
			CreateLobby
		}
		#endregion

		#region Fields
		private Mode _currentMode;
		private string _lobbyName;
		private Lobby _selectedLobby;
		#endregion

		#region Properties
		public Mode CurrentMode
		{
			get
			{
				return _currentMode;
			}
			set
			{
				if (_currentMode == value) return;
				_currentMode = value;
				OnPropertyChanged();
			}
		}

		public string LobbyName
		{
			get => _lobbyName;
			set
			{
				if (_lobbyName == value) return;
				_lobbyName = value;
				OnPropertyChanged();
			}
		}

		public List<Lobby> AllLobbies { get; private set; }

		public Lobby SelectedLobby
		{
			get => _selectedLobby;
			set
			{
				if (_selectedLobby == value) return;
				_selectedLobby = value;
				OnPropertyChanged();
			}
		}
		#endregion

		#region Constructor
		public LobbySystemModel()
		{
			AllLobbies = new();
			CurrentMode = Mode.JoinLobby;
		}
		#endregion

		#region Lobby Management Methods
		public async Task CreateLobby()
		{
			await LobbyManager.Instance.CreateLobbyAsync(LobbyName);
		}

		public async Task RefreshList()
		{
			AllLobbies = await LobbyManager.Instance.QueryLobbiesAsync();
			OnPropertyChanged(nameof(AllLobbies));
		}

		public async Task JoinLobby()
		{
			await LobbyManager.Instance.JoinLobbyAsync(SelectedLobby);
		}

		//public async Task LeaveLobby()
		//{
		//	await LobbyManager.Instance.LeaveLobbyAsync();
		//	CurrentMode = Mode.CreateLobby;
		//	OnPropertyChanged(nameof(CurrentMode));
		//}
		#endregion
	}
}