using System.Linq;
using Unity.Netcode;

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
			//simple version, host should have id 0 so the host will be player 1:
			//GameModel.TeamSelection = new SelectTeamModel(NetworkManager.Singleton.ConnectedClientsIds[0], NetworkManager.Singleton.ConnectedClientsIds[1]);

			//complicated version with explicit logic to make sure the host is player 1:
			ulong localId = NetworkManager.Singleton.LocalClientId;
			ulong otherId = NetworkManager.Singleton.ConnectedClientsIds.First(id => id != localId);

			ulong player1Id = NetworkManager.Singleton.IsHost ? localId : otherId;
			ulong player2Id = NetworkManager.Singleton.IsHost ? otherId : localId;

			GameModel.TeamSelection = new(player1Id, player2Id);

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
