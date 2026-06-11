
using Assets.Scripts.Dodgeball.Model;
using Assets.Scripts.MVP.Presenter;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Dodgeball.Presenter
{
	public class MatchPresenter : PresenterMonobehaviour<MatchModel>
	{
		[SerializeField]
		private ArenaPresenter _arena;

		public void StartMatch(MatchModel model)
		{
			Model = model;
			Model.StartMatch(_arena.Model);

			//Spawn ball
			_arena.SpawnBall(0);

			if (NetworkManager.Singleton.IsServer)
			{
				_arena.SpawnPlayer(model.PlayerRed.PlayerId);
				_arena.SpawnPlayer(model.PlayerBlue.PlayerId);
			}
		}

	}
}
