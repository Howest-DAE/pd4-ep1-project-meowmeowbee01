using Assets.Scripts.Dodgeball.Model;
using Assets.Scripts.Dodgeball.Presenter;
using UnityEngine;

namespace Assets.Scripts.Effects
{
	public class HitParticles : MonoBehaviour
	{
		[SerializeField]
		private Material _redMaterial, _blueMaterial;

		public void Spawn(PlayerColor color, Vector3 ballPosition, PlayerPresenter hitPlayer)
		{
			Vector3 hitDirection = ballPosition - hitPlayer.transform.position + Vector3.up * 1f;
			var particleRenderer = GetComponent<ParticleSystemRenderer>();
			transform.position = ballPosition;
			transform.rotation = Quaternion.LookRotation(hitDirection) * Quaternion.Euler(90f, 0f, 0f);
			particleRenderer.sharedMaterial = color == PlayerColor.Blue ? _blueMaterial : _redMaterial;

			Destroy(this.gameObject, 3f);
		}
	}
}