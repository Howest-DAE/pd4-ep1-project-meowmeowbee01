using Assets.Scripts.MVP.Model;
using System;

namespace Assets.Scripts.Dodgeball.Model
{
	/// <summary>
	/// MatchModel keeps track of the scores and the time.
	/// MatchModel creates the Player Models
	/// </summary>
	public class MatchModel : ModelBase
	{
		public int Id { get; set; }

		public event EventHandler MatchEnded; //Gets invoked when time runs out

		public int SecondsLeft
		{
			get => _timeLeft.Seconds;
		}

		public int MinutesLeft
		{
			get => _timeLeft.Minutes;
		}

		public int ScoreRed
		{
			get => _scoreRed;
			set
			{

				if (_scoreRed == value)
					return;
				_scoreRed = value;
				OnPropertyChanged();
			}
		}

		public int ScoreBlue
		{
			get => _scoreBlue;
			set
			{
				if (_scoreBlue == value)
					return;
				_scoreBlue = value;
				OnPropertyChanged();
			}
		}
		public bool MatchPlaying => _timeLeft > TimeSpan.Zero;

		public PlayerModel PlayerRed { get; }
		public PlayerModel PlayerBlue { get; }


		private int _scoreRed;
		private int _scoreBlue;

		private const int _matchDuration = 20;//seconds
		private TimeSpan _timeLeft = TimeSpan.Zero;

		public MatchModel(ulong redPlayerId, ulong bluePlayerId)
		{
			PlayerBlue = new PlayerModel(bluePlayerId) { Color = PlayerColor.Blue };
			PlayerRed = new PlayerModel(redPlayerId) { Color = PlayerColor.Red };
		}
		public void StartMatch(ArenaModel arena)
		{
			arena.AddPlayer(PlayerBlue);
			arena.AddPlayer(PlayerRed);

			PlayerBlue.TouchedByBall += _player_HitByBall;
			PlayerRed.TouchedByBall += _player_HitByBall;

			_scoreBlue = 0;
			_scoreRed = 0;

			_timeLeft = TimeSpan.FromSeconds(_matchDuration);
		}

		private void _player_HitByBall(object sender, PlayerBallTouchEventArgs e)
		{
			if (e.Player.Color == e.Ball.LastGrabbedPlayerColor) return;
			if (!e.Ball.IsPlayerBall) return;

			e.Player.OnHitByBall(e.Ball, e.Ball.LastGrabbedPlayerColor);

			if (e.Ball.LastGrabbedPlayerColor == PlayerColor.Blue)
			{
				ScoreBlue++;
			}
			else
			{
				ScoreRed++;
			}

		}

		public void UpdateTime(float deltaTime)
		{
			if (!MatchPlaying) { return; }

			int prevSeconds = SecondsLeft;
			int prevMinutes = MinutesLeft;
			_timeLeft -= TimeSpan.FromSeconds(deltaTime);

			if (SecondsLeft != prevSeconds)
			{
				OnPropertyChanged(nameof(SecondsLeft));
			}
			if (MinutesLeft != prevMinutes)
			{
				OnPropertyChanged(nameof(MinutesLeft));
			}

			if (!MatchPlaying)
			{
				OnMatchEnded();
			}
		}

		protected virtual void OnMatchEnded()
		{
			MatchEnded?.Invoke(this, EventArgs.Empty);
		}




	}
}