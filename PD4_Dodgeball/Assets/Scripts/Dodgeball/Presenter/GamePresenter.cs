using Assets.Scripts.Dodgeball.Model;
using Assets.Scripts.MVP.Presenter;
using Assets.Scripts.Player;
using UnityEngine;

namespace Assets.Scripts.Dodgeball.Presenter
{
	public class GamePresenter : PresenterMonobehaviour<GameModel>
	{
		[SerializeField]
		private MatchPresenter _matchPresenter;

		[SerializeField]
		private PlayerInputActionConfig _inputActionsConfig;
		public PlayerInputActionConfig InputActionConfig => _inputActionsConfig;

		protected override void Awake()
		{
			Model = new GameModel();
			Model.MatchStarted += Model_MatchStarted;
		}

		private async void Model_MatchStarted(object sender, EventArgs<MatchModel> e)
		{
			await _matchPresenter.StartMatch(e.Value);
		}

		protected override void Start()
		{
			base.Start();
		}

		protected override void Update()
		{
			Model.Update(Time.deltaTime);
			base.Update();
		}

		protected override void OnModelPropertyChanged(string propertyName)
		{
			switch (propertyName)
			{
				case nameof(Model.CurrentGameState):
					GameStateChanged(Model.CurrentGameState);
					break;
			}

			base.OnModelPropertyChanged(propertyName);
		}

		private void GameStateChanged(GameModel.GameState state)
		{
			switch (state)
			{
				case GameModel.GameState.SelectTeam:

					break;

			}
		}
	}
}
