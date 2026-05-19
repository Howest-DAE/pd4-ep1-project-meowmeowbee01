using Assets.Scripts.MVP.Model;
using Unity.Netcode;

namespace Assets.Scripts.Dodgeball.Model.TeamSelection
{
	public class PlayerTeamSelectionModel : ModelBase
	{
		private bool _isReady;
		private PlayerColor _selectedColor;

		public ulong PlayerId { get; }
		public bool IsCurrentPlayer { get => NetworkManager.Singleton.LocalClientId == PlayerId; }


		public bool IsReady
		{
			get { return _isReady; }
			set
			{
				if (_isReady == value) return;
				_isReady = value;
				OnPropertyChanged();
			}
		}

		public PlayerColor SelectedColor
		{
			get => _selectedColor;
			set
			{
				//if(_fieldName.Equals(value))
				if (_selectedColor == value)
					return;
				_selectedColor = value;
				OnPropertyChanged();
			}
		}

		public PlayerTeamSelectionModel(ulong playerId)
		{
			PlayerId = playerId;
		}
	}
}
