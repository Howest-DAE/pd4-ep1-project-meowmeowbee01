using Assets.Scripts.Dodgeball.Model;
using Assets.Scripts.Dodgeball.Network;
using Assets.Scripts.HttpHandlers;
using Assets.Scripts.MVP.Presenter;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.Dodgeball.Presenter.GameUI
{
	[RequireComponent(typeof(UIDocument))]
	public class MatchUIPresenter : PresenterMonobehaviour<MatchModel>
	{
		[SerializeField]
		private GameUIPresenter _gamePresenter;

		[SerializeField]
		private MatchUiSync _sync;

		private Label _redScoreLabel, _blueScoreLabel, _timerLabel, _goldLabel;

		private async void OnEnable()
		{
			//find model
			Model = _gamePresenter.Model.CurrentMatch;
			_sync.Model = Model;
			_sync.InitializeSync();

			UIDocument document = GetComponent<UIDocument>();
			_redScoreLabel = document.rootVisualElement.Q<Label>("RedScore");
			_blueScoreLabel = document.rootVisualElement.Q<Label>("BlueScore");
			_timerLabel = document.rootVisualElement.Q<Label>("Timer");
			_goldLabel = document.rootVisualElement.Q<Label>("Gold");

			await UpdateGold();
			UpdateScoreRed();
			UpdateScoreBlue();
			UpdateTimerText();

			BallHandler.Instance.BallPurchaseSuccess += async (sender, e) => await UpdateGold();
		}

		protected override async void OnModelPropertyChanged(string propertyName)
		{
			switch (propertyName)
			{
				case nameof(Model.ScoreRed):
					UpdateScoreRed();
					if (Model.PlayerRed.PlayerId == NetworkManager.Singleton.LocalClientId) await IncreaseGold();
					break;
				case nameof(Model.ScoreBlue):
					UpdateScoreBlue();
					if (Model.PlayerBlue.PlayerId == NetworkManager.Singleton.LocalClientId) await IncreaseGold();
					break;
				case nameof(Model.SecondsLeft):
				case nameof(Model.MinutesLeft):
					UpdateTimerText();
					break;
			}
		}

		void UpdateScoreRed()
		{
			_sync.RequestUpdateScoreRedRpc(Model.ScoreRed);

			_redScoreLabel.text = _sync.ScoreRed.Value.ToString();
		}

		private async Task IncreaseGold()
		{
			GoldHandler.IncreaseGold(15);
			await UpdateGold();
		}

		private async Task UpdateGold()
		{
			await Task.Delay(2000);
			_goldLabel.text = $"Gold: {(await GoldHandler.GetGold())?.ToString() ?? string.Empty}";
		}

		void UpdateScoreBlue()
		{
			_sync.RequestUpdateScoreBlueRpc(Model.ScoreBlue);

			_blueScoreLabel.text = _sync.ScoreBlue.Value.ToString();
		}

		void UpdateTimerText()
		{
			_timerLabel.text = $"{Model.MinutesLeft:00}:{Model.SecondsLeft:00}";
		}

	}
}
