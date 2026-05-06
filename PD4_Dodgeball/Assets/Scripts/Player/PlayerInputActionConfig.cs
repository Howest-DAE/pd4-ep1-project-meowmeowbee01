using UnityEngine;
using UnityEngine.InputSystem;

namespace Assets.Scripts.Player
{
	[CreateAssetMenu(fileName = "PlayerInputConfig", menuName = "Custom/PlayerInputActionSettings")]
	public class PlayerInputActionConfig : ScriptableObject
	{
		//Input actions
		public PlayerInputActions InputActions =>
			new PlayerInputActions()
			{
				MoveAction = _moveAction.action,
				LookAction = _lookAction.action,
				CrouchAction = _crouchAction.action,
				JumpAction = _jumpAction.action,
				GrabBallAction = _grabBallAction.action,
				ThrowBallAction = _throwBallAction.action
			};

		[SerializeField]
		InputActionReference _moveAction, _lookAction, _jumpAction, _crouchAction, _grabBallAction, _throwBallAction;


		//Rotation settings
		public PlayerRotationSettings RotationSettings => _rotationSettings;

		[SerializeField]
		private PlayerRotationSettings _rotationSettings;
	}

	public struct PlayerInputActions
	{
		public InputAction JumpAction { get; set; }
		public InputAction CrouchAction { get; set; }
		public InputAction MoveAction { get; set; }
		public InputAction LookAction { get; set; }
		public InputAction GrabBallAction { get; set; }
		public InputAction ThrowBallAction { get; set; }
	}
}