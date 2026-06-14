using Assets.Scripts.MVP.Model;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Assets.Scripts.Dodgeball.Model
{
	public enum PlayerColor
	{
		Blue,
		Red
	}
	/// <summary>
	/// Model for the player.
	/// Mainly handles balls.
	/// Should also handle player specific upgrades
	/// </summary>
	public class PlayerModel : ModelBase
	{
		public string PlayFabId;

		public event EventHandler<ThrowBallEventArgs> BallThrown;
		public event EventHandler<PlayerBallTouchEventArgs> TouchedByBall;
		public event EventHandler<PlayerHitEventArgs> HitByPlayerBall;
		public PlayerColor Color
		{
			get => _color;
			set
			{
				if (_color == value)
					return;
				_color = value;
				OnPropertyChanged();
			}
		}

		public BallModel GrabbedBall
		{
			get => _grabbedBall;
			set
			{
				if (_grabbedBall == value)
					return;
				_grabbedBall = value;
				OnPropertyChanged();
			}
		}

		public List<BallModel> BallsInGrabRange = new();
		private BallModel _grabbedBall;
		private PlayerColor _color;

		public ulong PlayerId { get; }

		public PlayerModel(ulong playerId)
		{
			PlayerId = playerId;
		}

		public void SetBallInGrabRange(BallModel ball, bool inRange)
		{
			if (inRange && !BallsInGrabRange.Contains(ball))
			{
				BallsInGrabRange.Add(ball);
			}
			else if (!inRange && BallsInGrabRange.Contains(ball))
			{
				BallsInGrabRange.Remove(ball);
			}
		}

		public virtual void OnThrowBall(BallModel ball, Vector3 velocity)
		{
			BallThrown(this, new ThrowBallEventArgs(ball, velocity));
		}
		public virtual void OnTouchedByBall(BallModel ball)
		{
			TouchedByBall?.Invoke(this, new PlayerBallTouchEventArgs(this, ball));
		}
		public virtual void OnHitByBall(BallModel ball, PlayerColor sourcePlayerColor)
		{
			HitByPlayerBall?.Invoke(this, new PlayerHitEventArgs(this, ball, sourcePlayerColor));
		}
	}
}
