using Assets.Scripts.Dodgeball.Model;
using Assets.Scripts.Dodgeball.Model.TeamSelection;
using Unity.Netcode;

namespace Assets.Scripts.Dodgeball.Network
{
	public class SelectTeamSync : NetworkBehaviour
	{
		public SelectTeamModel Model { get; set; }

		NetworkVariable<PlayerColor> NetworkPlayerColor { get; set; }
		NetworkVariable<bool> IsReady { get; set; }

		private void Awake()
		{
			NetworkPlayerColor = new();
			IsReady = new();

			ulong playerId = NetworkManager.LocalClientId;
			NetworkPlayerColor.OnValueChanged += (old, value) => Model.SetSelection(playerId, value);
			IsReady.OnValueChanged += (old, value) => Model.SetReady(playerId, value);
		}

		public override void OnNetworkSpawn()
		{
			ulong playerId = NetworkManager.LocalClientId;
			Model.SetReady(playerId, IsReady.Value);
			Model.SetSelection(playerId, NetworkPlayerColor.Value);

			Model.PropertyChanged += Model_PropertyChanged;
		}

		private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (!IsServer) return;
			var model = (SelectTeamModel)sender;
			switch (e.PropertyName)
			{
				case nameof(Model.CurrentPlayerColor):
					NetworkPlayerColor.Value = model.CurrentPlayerColor;
					break;
				case nameof(Model.ReadyToPlay):
					IsReady.Value = model.ReadyToPlay;
					break;
				default:
					break;
			}
		}

		[Rpc(SendTo.Everyone)]
		public void SetColorRpc(PlayerColor color)
		{
			ulong playerId = NetworkManager.LocalClientId;
			Model.SetSelection(playerId, color);
		}

		[Rpc(SendTo.Everyone)]
		public void SetReadyRpc(bool ready)
		{
			ulong playerId = NetworkManager.LocalClientId;
			Model.SetReady(playerId, ready);
		}
	}
}