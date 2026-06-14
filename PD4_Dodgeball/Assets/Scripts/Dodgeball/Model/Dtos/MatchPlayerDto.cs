namespace Assets.Scripts.Dodgeball.Model.Dtos
{
	public class MatchPlayerDto
	{
		public string PlayFabId { get; set; }
		public string DisplayName { get; set; }
		public bool IsHost { get; set; }
		public int Score { get; set; }
		public int HitsTaken { get; set; }
	}
}