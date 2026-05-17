using Unity.Netcode;

namespace Assets.Scripts.Dodgeball.Model.GameStates
{
	public class WaitingForOtherPlayerState : BaseGameState
	{
		public WaitingForOtherPlayerState(GameStatesFsm fsm) : base(fsm)
		{
		}

		//DONE: detect when players join -> 
		//When enough players are joined (2), go to the next state
		//FSM.TransitionTo(FSM.SelectTeamState);

		//todo: use events instead of checking in the update loop
		//NetworkManager.Singleton.OnClientConnectedCallback

		public override void Update(float deltaTime)
		{
			base.Update(deltaTime);
			if (NetworkManager.Singleton.ConnectedClients.Count >= 2)
			{
				FSM.TransitionTo(FSM.SelectTeamState);
			}
		}
	}
}
