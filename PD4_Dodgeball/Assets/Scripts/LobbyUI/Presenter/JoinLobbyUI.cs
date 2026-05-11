using Assets.Scripts.LobbyUI.Model;
using Assets.Scripts.MVP.Presenter;
using System.Linq;
using System.Threading.Tasks;
using Unity.Services.Lobbies.Models;
using UnityEngine.UIElements;

namespace Assets.Scripts.LobbyUI.Presenter
{
	public class JoinLobbyUI : PresenterBase<LobbySystemModel>
	{
		private ListView _lobbiesListView;
		private Button _joinButton;
		private Button _refreshButton;


		public JoinLobbyUI(VisualElement panelRoot)
		{
			_joinButton = panelRoot.Q<Button>("btn_join");
			_refreshButton = panelRoot.Q<Button>("btn_refresh");

			_joinButton.clicked += async () => await JoinButton_clicked();
			_refreshButton.clicked += async () => await RefreshButton_clicked();

			_lobbiesListView = panelRoot.Q<ListView>();
			_lobbiesListView.bindItem = ItemBinding;
			_lobbiesListView.selectedIndicesChanged += LobbiesListView_selectedIndicesChanged;

			_joinButton.SetEnabled(false);
		}

		//binds the lobby item with the visual element (sets the text)
		private void ItemBinding(VisualElement item, int index)
		{
			Lobby lobby = _lobbiesListView.itemsSource[index] as Lobby;
			item.Q<Label>().text = lobby.Name;
		}

		private async Task RefreshButton_clicked()
		{
			await Model.RefreshList();
		}
		private async Task JoinButton_clicked()
		{
			await Model.JoinLobby();
		}

		private void LobbiesListView_selectedIndicesChanged(System.Collections.Generic.IEnumerable<int> indices)
		{
			if (!indices.Any())
			{
				Model.SelectedLobby = null;
			}
			else
			{
				int selectedIndex = indices.First();
				Model.SelectedLobby = _lobbiesListView.itemsSource[selectedIndex] as Lobby;
			}
		}

		private void UpdateLobbiesList()
		{
			_lobbiesListView.itemsSource = Model.AllLobbies;
			_lobbiesListView.RefreshItems();
		}

		protected override void OnModelPropertyChanged(string propertyName)
		{
			if (propertyName == nameof(Model.AllLobbies))
			{
				UpdateLobbiesList();
			}
			if (propertyName == nameof(Model.SelectedLobby))
			{
				_joinButton.SetEnabled(Model.SelectedLobby != null);
			}
		}
	}
}