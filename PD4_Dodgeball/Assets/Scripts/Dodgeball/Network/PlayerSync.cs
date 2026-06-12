using Assets.Scripts.Dodgeball.Model;
using Assets.Scripts.Dodgeball.Presenter;
using Assets.Scripts.Player;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Dodgeball.Network
{
	[RequireComponent(typeof(PlayerPresenter))]
	public class PlayerSync : NetworkBehaviour
	{
		public PlayerModel Model;
		public NetworkVariable<ulong> BallId { get; set; } = new();

		private PlayerPresenter _presenter;

		private ArenaPresenter _arenaPresenter;

		private void Awake()
		{
			_presenter = GetComponent<PlayerPresenter>();
			_arenaPresenter = FindAnyObjectByType<ArenaPresenter>();

			BallId.OnValueChanged += (old, value) =>
			{
				if (value == 0) return;
				Model.GrabbedBall = _arenaPresenter.GetBallPresenter(value)?.Model;
			};
		}

		protected override void OnNetworkPostSpawn()
		{
			//Find Model from MatchModel
			var model = _arenaPresenter.Model.GetPlayer(OwnerClientId); //sometimes cant find the player
			if (model == null)
				Debug.LogWarning("evil mode");
			Model = model;

			_presenter.Model = Model;
			_presenter.ArenaPresenter = _arenaPresenter;

			_presenter.SetControlledByPlayer(OwnerClientId == NetworkManager.LocalClientId); // DONE: check for the local player id

			_arenaPresenter.AddPlayerPresenter(_presenter);

			Model.PropertyChanged += Model_PropertyChanged;
		}

		private void Model_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
		{
			if (!IsServer) return;
			var model = (PlayerModel)sender;
			switch (e.PropertyName)
			{
				case nameof(Model.GrabbedBall):
					BallId.Value = model.GrabbedBall?.Id ?? 0;
					break;
				default:
					break;
			}
		}

		[Rpc(SendTo.Server)]
		public void RequestGrabRpc()
		{
			if (Model.GrabbedBall != null) return; // only allow to grab 1 ball
			if (!Model.BallsInGrabRange.Any(b => !b.IsPlayerBall)) return;

			var ball = Model.BallsInGrabRange.First(b => !b.IsPlayerBall);
			SetBallRpc(ball.Id);

			Model.BallsInGrabRange.Remove(ball);

			var ballSync = _arenaPresenter.GetBallPresenter(ball.Id).gameObject.GetComponent<BallSync>();

			ballSync.SetGrabbedBallRpc(true);
			ballSync.SetLastGrabbedPlayerColorRpc(Model.Color);
			ballSync.SetPlayerBallRpc(true);
		}

		[Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
		public void SetBallRpc(ulong id)
		{
			Model.GrabbedBall = _arenaPresenter.GetBallPresenter(id)?.Model;
		}

		[Rpc(SendTo.Server)]
		public void RequestThrowRpc(Vector3 velocity)
		{
			if (Model.GrabbedBall == null) return;
			ThrowBallRpc(velocity);
			_arenaPresenter.GetBallPresenter(BallId.Value).gameObject.GetComponent<BallSync>().SetGrabbedBallRpc(false);
		}

		[Rpc(SendTo.Everyone, InvokePermission = RpcInvokePermission.Server)]
		private void ThrowBallRpc(Vector3 velocity)
		{
			var ball = _arenaPresenter.GetBallPresenter(BallId.Value).Model;
			Model.GrabbedBall.IsGrabbed = false;
			Model.GrabbedBall = null;
			Model.OnThrowBall(ball, velocity.ToNumericsVector());
		}
	}
}
