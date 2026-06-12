using Assets.Scripts.Dodgeball.Model;
using Assets.Scripts.MVP.Presenter;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace Assets.Scripts.Dodgeball.Presenter
{
	public class ArenaPresenter : PresenterMonobehaviour<ArenaModel>
	{
		[SerializeField]
		private SpawnArea[] _spawns;
		[SerializeField]
		private List<Transform> _ballSpawnLocations = new();

		[SerializeField]
		private GameObject _playerPrefab;
		[SerializeField]
		private GameObject _ballPrefab;

		private List<PlayerPresenter> _spawnedPlayers = new();
		private List<BallPresenter> _spawnedBalls = new();

		public ReadOnlyCollection<PlayerPresenter> SpawnedPlayers => _spawnedPlayers.AsReadOnly();
		public ReadOnlyCollection<BallPresenter> SpawnedBalls => _spawnedBalls.AsReadOnly();

		[SerializeField]
		private BallSpawner[] _ballSpawners;

		protected override void Awake()
		{
			base.Awake();
			Model = new ArenaModel();

			foreach (var spawner in _ballSpawners)
			{
				int index = RegisterSpawner(spawner.SpawnLocation);
				spawner.SpawnLocationIndex = index;
			}
		}

		//Adds a spawnlocation and stores the index
		public int RegisterSpawner(Transform transform)
		{
			_ballSpawnLocations.Add(transform);
			return _ballSpawnLocations.Count - 1;
		}

		public void SpawnBall(int spawnLocationIndex)
		{
			if (!NetworkManager.Singleton.IsHost) return;
			if (spawnLocationIndex == -1)
			{
				spawnLocationIndex = Random.Range(0, _ballSpawnLocations.Count);
			}
			Transform spawnLocation = _ballSpawnLocations[spawnLocationIndex];
			Instantiate(_ballPrefab, spawnLocation.position, Quaternion.identity).GetComponent<NetworkObject>().Spawn();
		}

		public void SpawnPlayer(ulong playerId)
		{
			PlayerModel model = Model.GetPlayer(playerId);
			var (position, rotation) = GetRandomPlayerSpawn(model.Color);

			GameObject playerGo = Instantiate(_playerPrefab, position, rotation);
			playerGo.GetComponent<NetworkObject>().SpawnWithOwnership(playerId);
		}

		public void AddPlayerPresenter(PlayerPresenter presenter)
		{
			//Don't add model to ArenaModel. Model already exists in Arena Model at this point (created by game)
			_spawnedPlayers.Add(presenter);
		}

		public void AddBall(BallPresenter presenter)
		{
			Model.AddBall(presenter.Model);
			_spawnedBalls.Add(presenter);
		}

		public BallPresenter GetBallPresenter(ulong ballId)
		{
			return _spawnedBalls.FirstOrDefault(b => b.Model.Id == ballId);
		}
		public BallPresenter GetBallPresenter(BallModel model)
		{
			return _spawnedBalls.FirstOrDefault(b => b.Model == model);
		}
		public PlayerPresenter GetPlayerPresenter(ulong playerId)
		{
			return _spawnedPlayers.FirstOrDefault(b => b.PlayerId == playerId);
		}
		public PlayerPresenter GetPlayerPresenter(PlayerModel model)
		{
			return _spawnedPlayers.FirstOrDefault(p => p.Model == model);
		}

		//Player spawn randomization
		public (Vector3 position, Quaternion rotation) GetRandomPlayerSpawn(PlayerColor color)
		{
			var spawns = _spawns.Where(s => s.Color == color).ToList();

			if (!spawns.Any()) return (Vector3.zero, Quaternion.identity);

			var spawn = spawns.Count == 1 ? spawns[0] : spawns[Random.Range(0, spawns.Count)];

			var rotation = spawn.transform.rotation;

			return (spawn.GetRandomVector(), rotation);
		}

	}
}