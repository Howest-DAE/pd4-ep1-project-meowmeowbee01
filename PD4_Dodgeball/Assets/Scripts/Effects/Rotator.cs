using UnityEngine;

namespace Assets.Scripts.Effects
{
	public class Rotator : MonoBehaviour
	{
		[SerializeField]
		private Vector3 _eulerSpeeds;

		// Update is called once per frame
		void Update()
		{
			transform.Rotate(_eulerSpeeds * Time.deltaTime);
		}
	}
}