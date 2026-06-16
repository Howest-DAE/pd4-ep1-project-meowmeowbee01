using Assets.Scripts.Dodgeball.Model;
using Assets.Scripts.HttpHandlers;
using Assets.Scripts.MVP.Presenter;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts.Dodgeball.Presenter.GameUI
{

	/// <summary>
	/// GameUIPresenter is a second presenter for the GameModel which handles the panel switching
	/// </summary>
	public class GameUIPresenter : PresenterMonobehaviour<GameModel>
	{
		[SerializeField]
		private GamePresenter _gamePresenter;

		[SerializeField]
		private UIDocument
			_teamSelectUI,
			_countdownUI,
			_waitingForPlayersUI,
			_matchPlayingUI,
			_gameOverUI;

		[SerializeField] private TMP_Text _textP1;
		[SerializeField] private TMP_Text _textP2;
		[SerializeField] private Canvas _matchEndUi;

		protected override void Start()
		{
			Model = _gamePresenter.Model;
			SetCurrentGameStateUI();
			base.Start();
			Model.MatchStarted += Model_MatchStarted;
		}

		private void Model_MatchStarted(object sender, EventArgs<MatchModel> e)
		{
			Model.CurrentMatch.MatchEnded += CurrentMatch_MatchEnded;
		}

		private async void CurrentMatch_MatchEnded(object sender, System.EventArgs e)
		{
			var match = await BackendHandler.GetMatchAsync(Model.CurrentMatch.Id);
			match.MatchPlayerInfos.ForEach(i => Debug.Log($"name: {i.DisplayName}, score: {i.Score}, hits taken: {i.HitsTaken}"));
			foreach (var info in match.MatchPlayerInfos)
			{
				if (info.IsHost) _textP1.text = $"name: {info.DisplayName}, score: {info.Score}, hits taken: {info.HitsTaken}";
				else _textP2.text = $"name: {info.DisplayName}, score: {info.Score}, hits taken: {info.HitsTaken}";
			}
		}

		protected override void OnModelPropertyChanged(string propertyName)
		{
			switch (propertyName)
			{
				case nameof(Model.CurrentGameState):
					SetCurrentGameStateUI();
					break;
			}

		}

		void SetCurrentGameStateUI()
		{
			_waitingForPlayersUI.gameObject.SetActive(Model.CurrentGameState == GameModel.GameState.WaitForOthers);
			_teamSelectUI.gameObject.SetActive(Model.CurrentGameState == GameModel.GameState.SelectTeam);
			_countdownUI.gameObject.SetActive(Model.CurrentGameState == GameModel.GameState.CountDown);
			_matchPlayingUI.gameObject.SetActive(Model.CurrentGameState == GameModel.GameState.Playing);
			//_gameOverUI.gameObject.SetActive(Model.CurrentGameState == GameModel.GameState.GameOver);
			_matchEndUi.gameObject.SetActive(Model.CurrentGameState == GameModel.GameState.GameOver);
		}
	}
}