using Assets.Scripts.Dodgeball.Model.TeamSelection;

namespace Assets.Scripts.Dodgeball.Model.GameStates
{
	public class SelectTeamState : BaseGameState
	{
		public SelectTeamState(GameStatesFsm fsm) : base(fsm)
		{
		}

		public override void OnEnter()
		{
			base.OnEnter();
			GameModel.TeamSelection = new SelectTeamModel();
			GameModel.TeamSelection.ReadinessChanged += TeamSelection_ReadinessChanged;
		}

		private void TeamSelection_ReadinessChanged(object sender, System.EventArgs e)
		{
			if (GameModel.TeamSelection.ReadyToPlay)
			{
				FSM.TransitionTo(FSM.CountdownState);
			}
		}
	}
}
