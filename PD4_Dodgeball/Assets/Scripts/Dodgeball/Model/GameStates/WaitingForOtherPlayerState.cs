namespace Assets.Scripts.Dodgeball.Model.GameStates
{
	public class WaitingForOtherPlayerState : BaseGameState
	{
		private float _timer;//TODO: remove timer
		public WaitingForOtherPlayerState(GameStatesFsm fsm) : base(fsm)
		{
		}

		public override void OnEnter()
		{
			base.OnEnter();

			//TODO: detect when players join -> 
			//When enough players are joined (2), go to the next state
			//FSM.TransitionTo(FSM.SelectTeamState);

			//FAKE waiting: wait 2 seconds
			_timer = 2f;
		}

		public override void Update(float deltaTime)
		{
			base.Update(deltaTime);

			//FAKE: wait 2 seconds before showing the Team selection.
			_timer -= deltaTime;
			if (_timer <= 0f)
			{
				FSM.TransitionTo(FSM.SelectTeamState);
			}

		}
	}
}
