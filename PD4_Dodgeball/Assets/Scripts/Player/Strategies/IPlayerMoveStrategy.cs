using System;
using UnityEngine;

namespace Assets.Scripts.Player.Strategies
{
	public interface IPlayerMoveStrategy
	{
		event EventHandler CrouchStarted;
		event EventHandler CrouchEnded;
		event EventHandler JumpRequested;

		Vector3 CalculateMovement();
		Vector3 CalculateLookDirection();
	}
}