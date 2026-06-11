using Assets.Scripts.Dodgeball.Model;
using Assets.Scripts.Dodgeball.Presenter;
using Unity.Netcode;

namespace Assets.Scripts.Dodgeball.Network
{
	[UnityEngine.RequireComponent(typeof(PlayerPresenter))]
	public class PlayerSync : NetworkBehaviour
	{
		public PlayerModel Model;
		private PlayerPresenter _presenter;

		private void Awake()
		{
			_presenter = GetComponent<PlayerPresenter>();
		}

		public override void OnNetworkSpawn()
		{
			//Find Model from MatchModel
			var arenaPresenter = FindAnyObjectByType<ArenaPresenter>();
			Model = arenaPresenter.Model.GetPlayer(OwnerClientId);

			_presenter.Model = Model;
			_presenter.ArenaPresenter = arenaPresenter;

			_presenter.SetControlledByPlayer(OwnerClientId == NetworkManager.LocalClientId); // DONE: check for the local player id

			arenaPresenter.AddPlayerPresenter(_presenter);
		}
	}
}
