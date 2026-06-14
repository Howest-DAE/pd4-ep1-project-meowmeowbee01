using Assets.Scripts.Dodgeball.Model;
using Assets.Scripts.Dodgeball.Network;
using Assets.Scripts.Dodgeball.Presenter;
using Assets.Scripts.MVP.Presenter;
using Assets.Scripts.Player.Strategies;
using UnityEngine;

namespace Assets.Scripts.Player
{
	[RequireComponent(typeof(PlayerSync))]
	[RequireComponent(typeof(PlayerPresenter))]
	public class PlayerThrow : PresenterMonobehaviour<PlayerModel>
	{
		[SerializeField]
		private Transform _aimTransform;

		[SerializeField]
		private FollowTransform _grabPivot;

		[SerializeField]
		private float _throwSpeed = 10f;

		private PlayerPresenter _playerPresenter;
		private PlayerSync _sync;
		private InputBallThrowStrategy _throwingStrategy;

		public InputBallThrowStrategy ThrowingStrategy
		{
			get => _throwingStrategy;
			set
			{
				if (_throwingStrategy == value) return;

				if (_throwingStrategy != null)
				{
					_throwingStrategy.ThrowBallRequested -= _throwingStrategy_ThrowBallRequested;
					_throwingStrategy.GrabBallRequested -= _throwingStrategy_GrabBallRequested;
				}

				_throwingStrategy = value;
				if (_throwingStrategy == null) return;

				_throwingStrategy.ThrowBallRequested += _throwingStrategy_ThrowBallRequested;
				_throwingStrategy.GrabBallRequested += _throwingStrategy_GrabBallRequested;
			}
		}

		private void OnDestroy()
		{
			ThrowingStrategy = null;
		}

		private void _throwingStrategy_GrabBallRequested(object sender, System.EventArgs e)
		{
			_sync.RequestGrabRpc();
		}

		private void _throwingStrategy_ThrowBallRequested(object sender, System.EventArgs e)
		{
			_sync.RequestThrowRpc(CalculateAimVelocity());
		}

		public bool HasBall => Model?.GrabbedBall != null;

		protected override void Awake()
		{
			base.Awake();
			_playerPresenter = GetComponent<PlayerPresenter>();
			_sync = GetComponent<PlayerSync>();
		}

		protected override void Update()
		{
			ThrowingStrategy?.Update();
			base.Update();
		}


		//Model changes

		protected override void OnModelUpdated(PlayerModel previousModel)
		{
			Model.BallThrown += Model_BallThrown;
		}

		protected override void OnModelPropertyChanged(string propertyName)
		{
			switch (propertyName)
			{
				case nameof(Model.GrabbedBall):
					SetGrabbedBall();
					break;
			}
		}

		private void SetGrabbedBall()
		{
			if (Model.GrabbedBall == null) return;
			var presenter = _playerPresenter.ArenaPresenter.GetBallPresenter(Model.GrabbedBall);
			_grabPivot.AddChild(presenter.transform);
			presenter.transform.localPosition = Vector3.zero;

		}

		//Gets invoked when the ball needs to be thrown
		private void Model_BallThrown(object sender, ThrowBallEventArgs e)
		{
			var presenter = _playerPresenter.ArenaPresenter.GetBallPresenter(e.Ball);
			_grabPivot.RemoveChild(presenter.transform);
			presenter.Throw(e.Velocity.ToUnityVector());
		}
		public Vector3 CalculateAimVelocity()
		{
			return _aimTransform.forward * _throwSpeed;
		}
		public Vector3 GetGrabPosition()
		{
			return _grabPivot.transform.position;
		}

		// Triggers
		void OnTriggerEnter(Collider other)
		{
			if (!other.CompareTag("Ball")) return;

			BallPresenter ball = other.GetComponent<BallPresenter>();
			Model.SetBallInGrabRange(ball.Model, true);

		}

		private void OnTriggerExit(Collider other)
		{
			if (!other.CompareTag("Ball")) return;

			BallPresenter ball = other.GetComponent<BallPresenter>();
			Model.SetBallInGrabRange(ball.Model, false);
		}
	}
}
