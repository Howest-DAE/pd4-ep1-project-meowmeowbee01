using Assets.Scripts.Dodgeball.Model;
using Unity.Netcode;

namespace Assets.Scripts.Dodgeball.Network
{
	public class BallSync : NetworkBehaviour
	{
		public BallModel Model;

		private NetworkVariable<bool> IsGrabbed = new();
		private NetworkVariable<bool> IsPlayerBall = new();
		private NetworkVariable<PlayerColor> LastGrabbedPlayerColor = new();

		private void Awake()
		{
			IsGrabbed.OnValueChanged += (old, value) => Model.IsGrabbed = value;
			IsPlayerBall.OnValueChanged += (old, value) => Model.IsGrabbed = value;
			LastGrabbedPlayerColor.OnValueChanged += (old, value) => Model.LastGrabbedPlayerColor = value;
		}

		public override void OnNetworkSpawn()
		{
			base.OnNetworkSpawn();

			Model.Id = NetworkObjectId;

			Model.PropertyChanged += Model_PropertyChanged;

			Model.IsGrabbed = IsGrabbed.Value;
			Model.IsPlayerBall = IsPlayerBall.Value;
			Model.LastGrabbedPlayerColor = LastGrabbedPlayerColor.Value;
		}

		private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (!IsServer) return;
			switch (e.PropertyName)
			{
				case nameof(Model.IsGrabbed):
					IsGrabbed.Value = Model.IsGrabbed;
					break;
				case nameof(Model.IsPlayerBall):
					IsPlayerBall.Value = Model.IsGrabbed;
					break;
				case nameof(Model.LastGrabbedPlayerColor):
					LastGrabbedPlayerColor.Value = Model.LastGrabbedPlayerColor;
					break;
				default:
					break;
			}
		}

		[Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
		public void SetGrabbedBallRpc(bool isGrabbed)
		{
			Model.IsGrabbed = isGrabbed;
		}

		[Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
		public void SetPlayerBallRpc(bool isPlayerBall)
		{
			Model.IsPlayerBall = isPlayerBall;
		}

		[Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
		public void SetLastGrabbedPlayerColorRpc(PlayerColor color)
		{
			Model.LastGrabbedPlayerColor = color;
		}
	}
}