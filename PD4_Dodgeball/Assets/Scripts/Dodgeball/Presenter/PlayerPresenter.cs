using Assets.Scripts.Dodgeball.Model;
using Assets.Scripts.Effects;
using Assets.Scripts.HttpHandlers;
using Assets.Scripts.MVP.Presenter;
using Assets.Scripts.Player;
using Assets.Scripts.Player.Strategies;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Assets.Scripts.Dodgeball.Presenter
{
	[RequireComponent(typeof(PlayerThrow))]
	[RequireComponent(typeof(PlayerMovement))]
	[RequireComponent(typeof(PlayerCameraAttach))]
	public class PlayerPresenter : PresenterMonobehaviour<PlayerModel>
	{
		//Inspector fields
		[SerializeField]
		private MeshRenderer[] _coloredParts;
		[SerializeField]
		private Material _redMaterial, _blueMaterial;
		[SerializeField]
		private LayerMask _jumpObstaclesMask;
		[SerializeField]
		private GameObject _hitParticlesPrefab;

		[SerializeField]
		private GameObject _aimVisual;

		[SerializeField]
		private TextMeshProUGUI _displayText;

		//Properties
		public PlayerColor Color => Model.Color;
		public ArenaPresenter ArenaPresenter { get; set; }
		public InputBallThrowStrategy ThrowStrategy { get; set; }


		//DONE: replace with Networked PlayerId
		public ulong PlayerId => Model.PlayerId;

		protected override void Awake()
		{
			SetControlledByPlayer(false);
			base.Awake();
		}

		public void DisplayName(string name)
		{
			if (_displayText != null) _displayText.text = name;
		}

		protected override void OnModelUpdated(PlayerModel previousModel)
		{
			if (previousModel != null)
			{
				previousModel.HitByPlayerBall -= Model_HitByBall;
			}

			if (Model != null)
			{
				Model.HitByPlayerBall += Model_HitByBall;
				UpdatePlayerColor();
			}
		}

		private async void Model_HitByBall(object sender, PlayerHitEventArgs e)
		{
			BallPresenter ball = ArenaPresenter.GetBallPresenter(e.Ball);
			if (ball == null) return;

			var particles = Instantiate(_hitParticlesPrefab).GetComponent<HitParticles>();
			particles.Spawn(e.SourcePlayerColor, ball.transform.position, this);

			await BackendHandler.PostHitAsync(ArenaPresenter.Model.MatchId, new() { TargetPlayFabId = Model.PlayFabId, ThrowerPlayFabId = ArenaPresenter.Model.GetPlayers().FirstOrDefault(p => p.PlayFabId != Model.PlayFabId).PlayFabId });
		}

		protected override void OnModelPropertyChanged(string propertyName)
		{
			if (propertyName == nameof(Model.GrabbedBall))
			{
				SetAimVisible(Model.GrabbedBall != null);
			}
			base.OnModelPropertyChanged(propertyName);
		}

		public void SetControlledByPlayer(bool controlled)
		{
			if (controlled)
			{
				GamePresenter presenter = FindAnyObjectByType<GamePresenter>();
				PlayerInputActions inputActions = presenter.InputActionConfig.InputActions;
				GetComponent<PlayerMovement>().MoveStrategy = new PlayerInputMoveStrategy(transform, Camera.main.transform, inputActions, presenter.InputActionConfig.RotationSettings);
				GetComponent<PlayerThrow>().ThrowingStrategy = new InputBallThrowStrategy(inputActions);
			}
			GetComponent<PlayerCameraAttach>().enabled = controlled;

		}

		private void UpdatePlayerColor()
		{
			Material colorMat = Color == PlayerColor.Red ? _redMaterial : _blueMaterial;
			foreach (var renderer in _coloredParts)
			{
				renderer.sharedMaterial = colorMat;
			}
		}

		private void SetAimVisible(bool visible)
		{
			_aimVisual.SetActive(visible);
		}
		// Update is called once per frame
		protected override void Update()
		{
			ThrowStrategy?.Update();
			base.Update();
		}
	}
}
