using Assets.Scripts.Player.Strategies;
using System;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Player
{
	public class PlayerMovement : NetworkBehaviour
	{
		private NetworkVariable<bool> _crouching = new(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
		public bool IsCrouching => _crouching.Value;

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
		private NetworkVariable<bool> _jumpPressed = new(false);
		private Vector3 _movementInput;
		private Vector3 _lookDirectionInput;

		[Header("Rotation")]
		//Look rotation
		[SerializeField]
		private Transform _lookRotPivot;

		public PlayerInputMoveStrategy MoveStrategy
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
			_crouching.Value = false;
		}

		private void _moveStrategy_CrouchStarted(object sender, EventArgs e)
		{
			_crouching.Value = true;
		}

		private void _moveStrategy_JumpRequested(object sender, EventArgs e)
		{
			RequestJumpRpc();
		}

		[Rpc(SendTo.Server)]
		private void RequestJumpRpc()
		{
			_jumpPressed.Value = true;
		}

		private PlayerInputMoveStrategy _moveStrategy;

		void Start()
		{
			Cursor.lockState = CursorLockMode.Locked;
		}

		void Update()
		{
			if (IsOwner)
			{
				// Move
				_movementInput = MoveStrategy?.CalculateMovement() ?? Vector3.zero;

				// Look
				_lookDirectionInput = MoveStrategy?.CalculateLookDirection() ?? Vector3.zero;

				//vertical
				_lookRotPivot.rotation = Quaternion.LookRotation(_lookDirectionInput);
			}
		}

		[Rpc(SendTo.Server)]
		private void MoveRpc(Vector3 movementInput, Vector3 lookDirectionInput)
		{
			movementInput.y = 0; //sanitize

			//horizontal
			Vector3 horizontalLook = lookDirectionInput;
			horizontalLook.y = 0f;
			transform.rotation = Quaternion.LookRotation(horizontalLook);

			// Crouch
			if (_crouching.Value)
			{
				movementInput *= _crouchSpeedModifier; //DONE: this needs to happen server side!!
			}

			_characterControl.Move(_moveSpeed * Time.fixedDeltaTime * movementInput.normalized); //normalize for sanitization
		}

		private void FixedUpdate()
		{
			if (IsOwner)
			{
				MoveRpc(_movementInput, _lookDirectionInput);
			}

			// Gravity
			ApplyGravity();

			//Jump
			if (_jumpPressed.Value)
			{
				JumpRpc();
			}
		}

		[Rpc(SendTo.Server)]
		private void JumpRpc()
		{
			if (_characterControl.isGrounded)
				_verticalSpeed = _jumpSpeed;
			_jumpPressed.Value = false;
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