using Assets.Scripts.LobbyUI.Model;
using Assets.Scripts.MVP.Presenter;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.LobbyUI.Presenter
{
	public class CreateLobbyUI : PresenterBase<LobbySystemModel>
	{
		private TextField _lobbyNameField;
		private Button _createButton;

		public CreateLobbyUI(VisualElement panelRoot)
		{
			_lobbyNameField = panelRoot.Q<TextField>();
			_createButton = panelRoot.Q<Button>();

			_lobbyNameField.RegisterValueChangedCallback(NameValueChanged);

			_createButton.clicked += async () => await CreateButton_clicked();

		}

		private async Task CreateButton_clicked()
		{
			Debug.Log($"Creating lobby {Model.LobbyName}");
			await Model.CreateLobby();
		}

		private void NameValueChanged(ChangeEvent<string> nameChangeEvent)
		{
			Model.LobbyName = nameChangeEvent.newValue;
		}

		protected override void OnModelPropertyChanged(string propertyName)
		{
			if (propertyName == nameof(Model.LobbyName))
			{
				_lobbyNameField.value = Model.LobbyName;
				_createButton.SetEnabled(!string.IsNullOrWhiteSpace(Model.LobbyName));
			}
		}
	}
}
