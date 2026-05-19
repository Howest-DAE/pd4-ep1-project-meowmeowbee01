using Assets.Scripts.Dodgeball.Model;
using Assets.Scripts.Dodgeball.Model.TeamSelection;
using Assets.Scripts.Dodgeball.Network;
using Assets.Scripts.MVP.Presenter;
using System;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace Assets.Scripts.Dodgeball.Presenter.GameUI.TeamSelection
{
	public class SelectTeamPresenter : PresenterBase<SelectTeamModel>
	{
		private readonly UIDocument _document;

		private List<PlayerTeamSelectionPresenter> _playerSelectPresenters = new();

		private Button _redButton;
		private Button _blueButton;
		private Button _readyButton;
		private Button _unreadyButton;

		private Label _exceededLabel;

		private SelectTeamSync _sync;

		public SelectTeamPresenter(UIDocument document, SelectTeamModel model, SelectTeamSync sync)
		{
			sync.Initialize(model);
			_sync = sync;
			Model = model;
			_document = document;

			VisualElement player1Row = _document.rootVisualElement.Q<VisualElement>("TeamRow_Player1");
			VisualElement player2Row = _document.rootVisualElement.Q<VisualElement>("TeamRow_Player2");

			_playerSelectPresenters.Add(new PlayerTeamSelectionPresenter(player1Row) { Model = Model.Player1Selection });
			_playerSelectPresenters.Add(new PlayerTeamSelectionPresenter(player2Row) { Model = Model.Player2Selection });

			_redButton = _document.rootVisualElement.Q<Button>("Button_Red");
			_blueButton = _document.rootVisualElement.Q<Button>("Button_Blue");
			_readyButton = _document.rootVisualElement.Q<Button>("Button_Ready");
			_unreadyButton = _document.rootVisualElement.Q<Button>("Button_Unready");

			_exceededLabel = _document.rootVisualElement.Q<Label>("ExceededLabel");

			Model.SelectionChanged += Model_SelectionChanged;
			Model.ReadinessChanged += Model_ReadinessChanged;

			UpdateReadyButtonVisibility();
			UpdateExceedLimitVisibility();
		}


		public void RegisterCallbacks()
		{
			_redButton.clicked += redButton_clicked;
			_blueButton.clicked += blueButton_clicked;

			_readyButton.clicked += readyButton_Clicked;
			_unreadyButton.clicked += unreadyButton_Clicked;
		}

		public void UnregisterCallbacks()
		{
			_redButton.clicked -= redButton_clicked;
			_blueButton.clicked -= blueButton_clicked;

			_readyButton.clicked -= readyButton_Clicked;
			_unreadyButton.clicked -= unreadyButton_Clicked;
		}

		#region UI_eventcallbacks
		void readyButton_Clicked()
		{
			_sync.SetReadyRpc(_sync.NetworkManager.LocalClientId, true);
		}
		void unreadyButton_Clicked()
		{
			_sync.SetReadyRpc(_sync.NetworkManager.LocalClientId, false);
		}
		private void blueButton_clicked()
		{
			_sync.SetColorRpc(_sync.NetworkManager.LocalClientId, PlayerColor.Blue);
		}
		private void redButton_clicked()
		{
			_sync.SetColorRpc(_sync.NetworkManager.LocalClientId, PlayerColor.Red);
		}
		#endregion

		private void Model_ReadinessChanged(object sender, EventArgs e)
		{
			UpdateReadyButtonVisibility();
		}

		private void Model_SelectionChanged(object sender, PlayerIdEventArgs e)
		{
			UpdateReadyButtonVisibility();
		}

		protected override void OnModelPropertyChanged(string propertyName)
		{
			switch (propertyName)
			{
				case nameof(Model.ExceedingTeamLimit):
					UpdateExceedLimitVisibility();
					break;
			}
		}

		void UpdateReadyButtonVisibility()
		{
			bool isReady = Model.IsReady(_sync.NetworkManager.LocalClientId);
			if (isReady)
			{
				//enable unreadybutton
				_unreadyButton.style.display = DisplayStyle.Flex;
				_readyButton.style.display = DisplayStyle.None;
			}
			else
			{
				_readyButton.style.display = DisplayStyle.Flex;
				_unreadyButton.style.display = DisplayStyle.None;
			}
		}

		void UpdateExceedLimitVisibility()
		{
			_exceededLabel.style.display = Model.ExceedingTeamLimit ? DisplayStyle.Flex : DisplayStyle.None;
		}
	}
}
