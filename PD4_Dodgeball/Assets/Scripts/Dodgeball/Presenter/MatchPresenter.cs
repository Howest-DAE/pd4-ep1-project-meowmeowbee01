
using Assets.PlayFab.Scripts.LoginSystem;
using Assets.Scripts.Dodgeball.Model;
using Assets.Scripts.Dodgeball.Model.Dtos;
using Assets.Scripts.Dodgeball.Network;
using Assets.Scripts.HttpHandlers;
using Assets.Scripts.MVP.Presenter;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Dodgeball.Presenter
{
	[RequireComponent(typeof(MatchSync))]
	public class MatchPresenter : PresenterMonobehaviour<MatchModel>
	{
		[SerializeField]
		private ArenaPresenter _arena;

		private MatchSync _sync;

		public async Task StartMatch(MatchModel model)
		{
			_sync = GetComponent<MatchSync>();
			model.StartMatch(_arena.Model);
			Model = model;
			_sync.Model = Model;

			//Spawn ball
			_arena.SpawnBall(0);

			if (NetworkManager.Singleton.IsServer)
			{
				_arena.SpawnPlayer(model.PlayerRed.PlayerId);
				_arena.SpawnPlayer(model.PlayerBlue.PlayerId);

				await Task.Delay(2000);

				MatchPostDto dto = new()
				{
					BluePlayerPlayFabId = model.PlayerBlue.PlayFabId,
					RedPlayerPlayFabId = model.PlayerRed.PlayFabId,
					HostPlayFabId = PlayFabPlayer.Instance.PlayfabId
				};

				Debug.Log($"blue: {dto.BluePlayerPlayFabId}, red: {dto.RedPlayerPlayFabId}, host: {dto.HostPlayFabId}");

				var idDto = await BackendHandler.PostMatchAsync(dto);

				Debug.Log($"id: {idDto?.MatchId}");

				if (idDto != null)
				{
					_arena.Model.MatchId = idDto.MatchId;
					_sync.SetMatchIdRpc(idDto.MatchId);
				}
			}
		}
	}
}