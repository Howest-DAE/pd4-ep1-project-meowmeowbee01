using Assets.Scripts.HttpHandlers;
using UnityEngine;

namespace Assets.Scripts.Dodgeball.Presenter
{
	public class BallSpawner : MonoBehaviour //No model to present
	{
		//Properties
		public Transform SpawnLocation => _spawnLocation;
		public int SpawnLocationIndex { get; set; }


		//Inspector fields
		[SerializeField] private Transform _spawnLocation;

		//Private fields
		private ArenaPresenter _arena;

		private void Awake()
		{
			_arena = FindAnyObjectByType<ArenaPresenter>();
			BallHandler.Instance.BallPurchaseSuccess += BallPurchaseSuccess;
		}

		private void BallPurchaseSuccess(object sender, System.EventArgs e)
		{
			if (BallHandler.Instance.CurrentBallSpawner == this) SpawnBall();
		}

		public void SpawnBall()
		{
			_arena.SpawnBall(SpawnLocationIndex);

		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				BallHandler.Instance.BuyBall(this);
			}
		}
	}
}