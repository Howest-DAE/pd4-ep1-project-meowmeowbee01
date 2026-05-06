using Assets.Scripts.LobbyUI.Model;
using Assets.Scripts.MVP.Presenter;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.LobbyUI.Presenter
{
	public class LobbyUI : PresenterMonobehaviour<LobbySystemModel>
	{
		[SerializeField]
		private UIDocument _lobbyUIDoc;

		private TabView _lobbyTabView;
		private CreateLobbyUI _createPanel = null;
		private JoinLobbyUI _joinPanel = null;

		protected override void Start()
		{
			_lobbyUIDoc.enabled = true;

			Model = new();
			_createPanel = new(_lobbyUIDoc.rootVisualElement.Q("CreatePanel")) { Model = Model };
			_joinPanel = new(_lobbyUIDoc.rootVisualElement.Q("JoinPanel")) { Model = Model };

			RefreshTabView();
		}

		void RefreshTabView()
		{
			//avoid unsubscribing multiple times
			if (_lobbyTabView != null)
				_lobbyTabView.activeTabChanged -= LobbyTabView_activeTabChanged;

			_lobbyTabView = _lobbyUIDoc.rootVisualElement.Q<TabView>("TabSelector");

			//avoid subscribing multiple times
			if (_lobbyTabView != null)
				_lobbyTabView.activeTabChanged += LobbyTabView_activeTabChanged;
		}

		private void LobbyTabView_activeTabChanged(Tab previous, Tab current)
		{
			if (current.tabHeader == _lobbyTabView.GetTabHeader(0))
				Model.CurrentMode = LobbySystemModel.Mode.JoinLobby;
			else
				Model.CurrentMode = LobbySystemModel.Mode.CreateLobby;
		}

		protected override void OnModelUpdated(LobbySystemModel previousModel)
		{
			UpdatePanelSelection();
		}

		protected override void OnModelPropertyChanged(string propertyName)
		{
			if (propertyName == nameof(Model.CurrentMode))
			{
				Debug.Log($"switching to mode {Model.CurrentMode}");
				UpdatePanelSelection();
			}
		}

		private void UpdatePanelSelection()
		{
			switch (Model.CurrentMode)
			{
				case LobbySystemModel.Mode.CreateLobby:
					_lobbyUIDoc.rootVisualElement.style.visibility = Visibility.Visible;
					RefreshTabView();
					_lobbyTabView.selectedTabIndex = 1;
					break;
				case LobbySystemModel.Mode.JoinLobby:
					_lobbyUIDoc.rootVisualElement.style.visibility = Visibility.Visible;
					RefreshTabView();
					_lobbyTabView.selectedTabIndex = 0;
					break;
				default:
					_lobbyUIDoc.rootVisualElement.style.visibility = Visibility.Hidden;
					break;
			}
		}
	}
}
