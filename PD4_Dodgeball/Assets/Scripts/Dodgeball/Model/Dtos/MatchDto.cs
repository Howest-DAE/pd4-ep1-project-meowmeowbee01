using System.Collections.Generic;

namespace Assets.Scripts.Dodgeball.Model.Dtos
{
	public class MatchDto
	{
		public int MatchId { get; set; }

		public List<MatchPlayerDto> MatchPlayerInfos { get; set; }
	}
}