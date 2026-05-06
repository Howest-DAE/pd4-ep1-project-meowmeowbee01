using Assets.Scripts.Player.Strategies;
using System;
using UnityEngine;

namespace Assets.Scripts.Player
{
	public class PlayerMovement : MonoBehaviour
	{
		private bool _crouching = false;
		public bool IsCrouching => _crouching;

		[SerializeField]
		private CharacterController _characterControl;

		//Movement
		[Header("Movement")]
		[SerializeField]
		private float _moveSpeed = 3f;
		[SerializeField]
		private float _crouchSpeedModifier = 0.3f;
		[SerializeField]
		private float _jumpSpeed = 10f;
		private float _verticalSpeed = 0f;
		private bool _jumpPressed = false;

		[Header("Rotation")]
		//Look rotation
		[SerializeField]
		private Transform _lookRotPivot;

		private Transform _cameraTransform;

		public IPlayerMoveStrategy MoveStrategy
		{
			get => _moveStrategy;
			set
			{
				if (_moveStrategy == value) return;
				if (_moveStrategy != null)
				{
					_moveStrategy.JumpRequested -= _moveStrategy_JumpRequested;
					_moveStrategy.CrouchStarted -= _moveStrategy_CrouchStarted;
					_moveStrategy.CrouchEnded -= _moveStrategy_CrouchEnded;
				}

				_moveStrategy = value;
				if (_moveStrategy == null) return;

				_moveStrategy.JumpRequested += _moveStrategy_JumpRequested;
				_moveStrategy.CrouchStarted += _moveStrategy_CrouchStarted;
				_moveStrategy.CrouchEnded += _moveStrategy_CrouchEnded;
			}
		}

		private void _moveStrategy_CrouchEnded(object sender, EventArgs e)
		{
			_crouching = false;
		}

		private void _moveStrategy_CrouchStarted(object sender, EventArgs e)
		{
			_crouching = true;
		}

		private void _moveStrategy_JumpRequested(object sender, EventArgs e)
		{
			_jumpPressed = true;
		}


		private bool _isCrouched;
		private IPlayerMoveStrategy _moveStrategy;

		void Start()
		{
			Cursor.lockState = CursorLockMode.Locked;
			_cameraTransform = Camera.main.transform;
		}




		// Update is called once per frame
		void Update()
		{
			// Move
			Vector3 movement = MoveStrategy.CalculateMovement();

			// Look
			Vector3 lookDirection = MoveStrategy.CalculateLookDirection();

			//horizontal
			Vector3 horizontalLook = lookDirection;
			horizontalLook.y = 0f;
			transform.rotation = Quaternion.LookRotation(horizontalLook);

			//vertical
			_lookRotPivot.rotation = Quaternion.LookRotation(lookDirection);

			// Crouch
			if (_crouching)
			{
				movement *= _crouchSpeedModifier; //TODO: this needs to happen server side!!
			}

			_characterControl.Move(movement * _moveSpeed * Time.deltaTime);


		}


		private void FixedUpdate()
		{
			// Gravity
			ApplyGravity();

			//Jump
			if (_jumpPressed)
			{
				if (_characterControl.isGrounded)
				{
					_verticalSpeed = _jumpSpeed;
				}
				_jumpPressed = false;
			}

		}



		void ApplyGravity()
		{
			_verticalSpeed += Time.fixedDeltaTime * Physics.gravity.y;
			_characterControl.Move(_verticalSpeed * Time.fixedDeltaTime * Vector3.up);
			if (_characterControl.isGrounded)
			{
				_verticalSpeed = 0f;
			}
		}
	}
}