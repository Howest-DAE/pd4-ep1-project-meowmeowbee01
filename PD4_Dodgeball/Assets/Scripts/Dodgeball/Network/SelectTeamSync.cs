using Assets.Scripts.Dodgeball.Model;
using Assets.Scripts.Dodgeball.Model.TeamSelection;
using Unity.Netcode;

namespace Assets.Scripts.Dodgeball.Network
{
	public class SelectTeamSync : NetworkBehaviour
	{
		private SelectTeamModel Model { get; set; }

		NetworkVariable<PlayerColor> Player1Color { get; set; }
		NetworkVariable<bool> Player1IsReady { get; set; }
		NetworkVariable<PlayerColor> Player2Color { get; set; }
		NetworkVariable<bool> Player2IsReady { get; set; }

		private ulong player1Id;
		private ulong player2Id;

		private void Awake()
		{
			Player1Color = new();
			Player1IsReady = new();
			Player2Color = new();
			Player2IsReady = new();

			Player1Color.OnValueChanged += (old, value) => Model.SetSelection(player1Id, value);
			Player1IsReady.OnValueChanged += (old, value) => Model.SetReady(player1Id, value);
			Player2Color.OnValueChanged += (old, value) => Model.SetSelection(player2Id, value);
			Player2IsReady.OnValueChanged += (old, value) => Model.SetReady(player2Id, value);
		}

		public void Initialize(SelectTeamModel model)
		{
			Model = model;

			player1Id = Model.Player1Selection.PlayerId;
			player2Id = Model.Player2Selection.PlayerId;

			Model.SetSelection(player1Id, Player1Color.Value);
			Model.SetReady(player1Id, Player1IsReady.Value);
			Model.SetSelection(player2Id, Player2Color.Value);
			Model.SetReady(player2Id, Player2IsReady.Value);

			Model.PropertyChanged += Model_PropertyChanged;
		}

		private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (!IsServer) return;
			var model = (SelectTeamModel)sender;
			switch (e.PropertyName)
			{
				case nameof(Model.Player1Selection):
					Player1Color.Value = model.Player1Selection.SelectedColor;
					Player1IsReady.Value = model.Player1Selection.IsReady;
					break;
				case nameof(Model.Player2Selection):
					Player2Color.Value = model.Player2Selection.SelectedColor;
					Player2IsReady.Value = model.Player2Selection.IsReady;
					break;
				default:
					break;
			}
		}

		[Rpc(SendTo.Everyone)]
		public void SetColorRpc(ulong playerId, PlayerColor color)
		{
			Model.SetSelection(playerId, color);
		}

		[Rpc(SendTo.Everyone)]
		public void SetReadyRpc(ulong playerId, bool ready)
		{
			Model.SetReady(playerId, ready);
		}
	}
}