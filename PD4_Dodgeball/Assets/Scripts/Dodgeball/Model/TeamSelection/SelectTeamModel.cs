using Assets.Scripts.MVP.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Assertions;

namespace Assets.Scripts.Dodgeball.Model.TeamSelection
{
	/// <summary>
	/// Model for the SelectTeam UI.
	/// Represents the choice of a single player
	/// </summary>
	public class SelectTeamModel : ModelBase
	{
		// EVENTS
		public event EventHandler<PlayerIdEventArgs> SelectionChanged;
		public event EventHandler ReadinessChanged;

		// Properties
		public bool ExceedingTeamLimit
		{
			get { return _exceedingTeamLimit; }
			set
			{
				if (_exceedingTeamLimit == value) return;
				_exceedingTeamLimit = value;

				OnPropertyChanged();
			}
		}
		public PlayerColor CurrentPlayerColor
		{
			get
			{
				return _teamSelectionModels.FirstOrDefault(t => t.IsCurrentPlayer)?.SelectedColor ?? PlayerColor.Blue;
			}
		}

		public bool ReadyToPlay => _player1Selection.IsReady && _player2Selection.IsReady;
		public PlayerTeamSelectionModel Player1Selection => _player1Selection;
		public PlayerTeamSelectionModel Player2Selection => _player2Selection;

		//Private fields
		private const int _maxPlayersPerTeam = 1;
		private bool _exceedingTeamLimit;
		private PlayerTeamSelectionModel _player1Selection;
		private PlayerTeamSelectionModel _player2Selection;
		List<PlayerTeamSelectionModel> _teamSelectionModels;

		public SelectTeamModel(ulong player1Id, ulong player2Id)
		{
			_player1Selection = new(player1Id);
			_player2Selection = new(player2Id);
			_teamSelectionModels = new List<PlayerTeamSelectionModel>() { _player1Selection, _player2Selection };
		}

		public ulong GetPlayerIdForColor(PlayerColor playerColor)
		{
			return _teamSelectionModels.FirstOrDefault(p => p.SelectedColor == playerColor)?.PlayerId ?? 0;
		}

		public bool SetSelection(ulong playerId, PlayerColor color)
		{
			var playerSelectionModel = _teamSelectionModels.FirstOrDefault(tsm => tsm.PlayerId == playerId);
			Assert.IsNotNull(playerSelectionModel);

			//don't allow swapping when ready
			if (playerSelectionModel.IsReady) return false;

			//update selection
			playerSelectionModel.SelectedColor = color;

			ExceedingTeamLimit = _teamSelectionModels
				.GroupBy(t => t.SelectedColor)
				.Any(g => g.Count() > _maxPlayersPerTeam);

			OnSelectionChanged(playerId);

			return true;
		}

		public bool IsReady(ulong playerId)
		{
			return _teamSelectionModels.FirstOrDefault(tsm => tsm.PlayerId == playerId).IsReady;
		}

		public void SetReady(ulong playerId, bool ready)
		{
			var model = _teamSelectionModels.FirstOrDefault(tsm => tsm.PlayerId == playerId);
			if (model == null || ExceedingTeamLimit) return;

			model.IsReady = ready;

			OnReadinessChanged();
		}

		protected virtual void OnReadinessChanged()
		{
			ReadinessChanged?.Invoke(this, EventArgs.Empty);
		}

		protected virtual void OnSelectionChanged(ulong playerId)
		{
			SelectionChanged?.Invoke(this, new PlayerIdEventArgs(playerId));
		}
	}
}
