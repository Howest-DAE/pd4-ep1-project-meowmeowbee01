using System;

namespace Assets.Scripts.Player.Strategies
{
	public interface IBallThrowingStrategy
	{
		event EventHandler GrabBallRequested;
		event EventHandler ThrowBallRequested;

		void Update();
	}
}