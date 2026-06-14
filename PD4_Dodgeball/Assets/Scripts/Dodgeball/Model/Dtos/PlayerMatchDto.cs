namespace Assets.Scripts.Dodgeball.Model.Dtos
{
	public class PlayerMatchDto
	{
		public int MatchId { get; set; }
		public string TeamColor { get; set; }
		public bool IsHost { get; set; }
		public int Score { get; set; }
		public int HitsTaken { get; set; }
	}
}