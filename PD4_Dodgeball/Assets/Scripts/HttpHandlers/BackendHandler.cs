using Assets.Scripts.Dodgeball.Model.Dtos;
using Newtonsoft.Json;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts.HttpHandlers
{
	public static class BackendHandler
	{
		private static readonly string _apiBaseUrl = "https://localhost:7043";
		//private static readonly string _apiBaseUrl = "https://pd4backendexercises.azurewebsites.net";

		public static async Task<PlayerDto> GetPlayerAsync(string playFabId)
		{
			var request = UnityWebRequest.Get($"{_apiBaseUrl}/Players/{playFabId}");
			await request.SendWebRequest();

			switch (request.result)
			{
				case UnityWebRequest.Result.InProgress:
					break;
				case UnityWebRequest.Result.Success:
					Debug.Log($"Successfully got the player from the server");
					return JsonConvert.DeserializeObject<PlayerDto>(request.downloadHandler.text);
				case UnityWebRequest.Result.ConnectionError:
					Debug.LogError($"failed to connect to server");
					break;
				case UnityWebRequest.Result.ProtocolError:
					Debug.LogError($"server responded with {request.responseCode}");
					break;
				case UnityWebRequest.Result.DataProcessingError:
					Debug.LogError($"failed process the response from the server");
					break;
				default:
					break;
			}
			return null;
		}

		public static async Task PostPlayerAsync(PlayerPostDto dto)
		{
			var request = UnityWebRequest.Post($"{_apiBaseUrl}/Players", JsonConvert.SerializeObject(dto), "application/json");
			await request.SendWebRequest();
			switch (request.result)
			{
				case UnityWebRequest.Result.InProgress:
					break;
				case UnityWebRequest.Result.Success:
					Debug.Log($"Successfully posted the player to the server");
					break;
				case UnityWebRequest.Result.ConnectionError:
					Debug.LogError($"failed to connect to server");
					break;
				case UnityWebRequest.Result.ProtocolError:
					Debug.LogError($"server responded with {request.responseCode}");
					break;
				case UnityWebRequest.Result.DataProcessingError:
					Debug.LogError($"failed process the response from the server");
					break;
				default:
					break;
			}
		}

		public static async Task<MatchIdDto> PostMatchAsync(MatchPostDto dto)
		{
			var request = UnityWebRequest.Post($"{_apiBaseUrl}/Matches", JsonConvert.SerializeObject(dto), "application/json");
			await request.SendWebRequest();
			switch (request.result)
			{
				case UnityWebRequest.Result.InProgress:
					break;
				case UnityWebRequest.Result.Success:
					Debug.Log($"Successfully posted the match to the server");
					return JsonConvert.DeserializeObject<MatchIdDto>(request.downloadHandler.text);
				case UnityWebRequest.Result.ConnectionError:
					Debug.LogError($"failed to connect to server");
					break;
				case UnityWebRequest.Result.ProtocolError:
					Debug.LogError($"server responded with {request.responseCode}");
					break;
				case UnityWebRequest.Result.DataProcessingError:
					Debug.LogError($"failed process the response from the server");
					break;
				default:
					break;
			}
			return null;
		}

		public static async Task PostHitAsync(int matchId, HitDto dto)
		{
			string uri = $"{_apiBaseUrl}/Matches/{matchId}/PlayerHit";
			var request = UnityWebRequest.Post(uri, JsonConvert.SerializeObject(dto), "application/json");
			await request.SendWebRequest();
			switch (request.result)
			{
				case UnityWebRequest.Result.InProgress:
					break;
				case UnityWebRequest.Result.Success:
					Debug.Log($"Successfully posted the hit to the server");
					break;
				case UnityWebRequest.Result.ConnectionError:
					Debug.LogError($"failed to connect to server");
					break;
				case UnityWebRequest.Result.ProtocolError:
					Debug.LogError($"server responded with {request.responseCode}");
					break;
				case UnityWebRequest.Result.DataProcessingError:
					Debug.LogError($"failed process the response from the server");
					break;
				default:
					break;
			}
		}
	}
}