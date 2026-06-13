using Assets.Scripts.Dodgeball.Model;
using Assets.Scripts.Dodgeball.Network;
using Assets.Scripts.MVP.Presenter;
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

		private Label _redScoreLabel, _blueScoreLabel, _timerLabel;

		private void OnEnable()
		{
			//find model
			Model = _gamePresenter.Model.CurrentMatch;
			_sync.Model = Model;
			_sync.InitializeSync();

			UIDocument document = GetComponent<UIDocument>();
			_redScoreLabel = document.rootVisualElement.Q<Label>("RedScore");
			_blueScoreLabel = document.rootVisualElement.Q<Label>("BlueScore");
			_timerLabel = document.rootVisualElement.Q<Label>("Timer");

			UpdateScore();
			UpdateTimerText();
		}

		protected override void OnModelPropertyChanged(string propertyName)
		{
			switch (propertyName)
			{
				case nameof(Model.ScoreRed):
				case nameof(Model.ScoreBlue):
					UpdateScore();
					break;
				case nameof(Model.SecondsLeft):
				case nameof(Model.MinutesLeft):
					UpdateTimerText();
					break;
			}
		}

		void UpdateScore()
		{
			_sync.RequestUpdateScoreRedRpc(Model.ScoreRed);
			_sync.RequestUpdateScoreBlueRpc(Model.ScoreBlue);

			_redScoreLabel.text = _sync.ScoreRed.Value.ToString();
			_blueScoreLabel.text = _sync.ScoreBlue.Value.ToString();
		}

		void UpdateTimerText()
		{
			_timerLabel.text = $"{Model.MinutesLeft:00}:{Model.SecondsLeft:00}";
		}

	}
}
