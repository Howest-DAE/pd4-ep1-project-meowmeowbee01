using Assets.Scripts.Dodgeball.Model;
using Unity.Netcode;

namespace Assets.Scripts.Dodgeball.Network
{
	public class MatchSync : NetworkBehaviour
	{
		public MatchModel Model { get; set; }
		[Rpc(SendTo.Everyone)]
		public void SetMatchIdRpc(int matchId)
		{
			Model.Id = matchId;
		}
	}
}