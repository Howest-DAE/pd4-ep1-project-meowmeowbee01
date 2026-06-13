using Assets.Scripts.Dodgeball.Model;
using Unity.Netcode;

namespace Assets.Scripts.Dodgeball.Network
{
	public class MatchUiSync : NetworkBehaviour
	{
		public MatchModel Model;

		public NetworkVariable<int> ScoreRed = new();
		public NetworkVariable<int> ScoreBlue = new();

		private void Awake()
		{
			ScoreRed.OnValueChanged += (old, value) => Model.ScoreRed = value;
			ScoreBlue.OnValueChanged += (old, value) => Model.ScoreBlue = value;
		}

		[Rpc(SendTo.Server)]
		public void RequestUpdateScoreRedRpc(int scoreRed)
		{
			ScoreRed.Value = scoreRed;
		}

		[Rpc(SendTo.Server)]
		public void RequestUpdateScoreBlueRpc(int scoreBlue)
		{
			ScoreBlue.Value = scoreBlue;
		}

		public void InitializeSync()
		{
			Model.ScoreRed = ScoreRed.Value;
			Model.ScoreBlue = ScoreBlue.Value;

			Model.PropertyChanged += Model_PropertyChanged;
		}

		private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (!IsServer) return;

			var model = sender as MatchModel;

			switch (e.PropertyName)
			{
				case nameof(Model.ScoreBlue):
					ScoreBlue.Value = model.ScoreBlue;
					break;
				case nameof(Model.ScoreRed):
					ScoreRed.Value = model.ScoreRed;
					break;
				default:
					break;
			}
		}
	}
}