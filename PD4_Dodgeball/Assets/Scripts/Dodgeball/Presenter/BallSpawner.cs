using Assets.Scripts.HttpHandlers;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Dodgeball.Presenter
{
	public class BallSpawner : NetworkBehaviour
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
			if (BallHandler.Instance.CurrentBallSpawner == this) SpawnBallRpc();
		}

		[Rpc(SendTo.Server)]
		public void SpawnBallRpc()
		{
			_arena.SpawnBall(SpawnLocationIndex);

		}

		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Player"))
			{
				Debug.Log("player entered spawner");
				ulong id = other.GetComponent<PlayerPresenter>()?.Model?.PlayerId ?? 0;
				BuyBallRpc(id);
			}
		}

		[Rpc(SendTo.Everyone)]
		private void BuyBallRpc(ulong playerId)
		{
			Debug.Log($"buyBallRpc triggered with playerId {playerId}");
			if (playerId != NetworkManager.Singleton.LocalClientId) return;
			BallHandler.Instance.BuyBall(this);
		}
	}
}