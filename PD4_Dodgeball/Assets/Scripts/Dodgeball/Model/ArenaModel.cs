using Assets.Scripts.MVP.Model;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.Dodgeball.Model
{

	/// <summary>
	/// Arena keeps track of the entities on the field.
	/// </summary>
	public class ArenaModel : ModelBase
	{
		public int MatchId { get; set; }

		private List<PlayerModel> _players = new();
		private List<BallModel> _balls = new();

		public void AddPlayer(PlayerModel player)
		{
			_players.Add(player);
		}
		public void AddBall(BallModel ball)
		{
			_balls.Add(ball);
		}

		public PlayerModel GetPlayer(ulong playerId)
		{
			return _players.FirstOrDefault(p => p.PlayerId == playerId);
		}

		public IEnumerable<PlayerModel> GetPlayers()
		{
			return _players;
		}
	}
}