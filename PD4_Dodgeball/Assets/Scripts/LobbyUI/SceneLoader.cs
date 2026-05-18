using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Scripts.LobbyUI
{
	public class SceneLoader : MonoBehaviour
	{
		private void OnEnable()
		{
			LobbyManager.Instance.SessionJoined += Instance_SessionJoined;
		}

		private void OnDisable()
		{
			LobbyManager.Instance.SessionJoined -= Instance_SessionJoined;
		}

		private void Instance_SessionJoined(object sender, System.EventArgs e)
		{
			if (NetworkManager.Singleton.IsHost) NetworkManager.Singleton.SceneManager.LoadScene("gameplayScene", LoadSceneMode.Single);
		}
	}
}