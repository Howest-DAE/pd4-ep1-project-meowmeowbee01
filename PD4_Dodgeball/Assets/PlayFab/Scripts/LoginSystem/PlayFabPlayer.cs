using Assets.Scripts.Singleton;
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;

namespace Assets.PlayFab.Scripts.LoginSystem
{
	class PlayFabPlayer : Singleton<PlayFabPlayer>
	{
		public string PlayfabId { get; set; }
		public string DisplayName { get; set; }

		public void FetchDisplayName(Action finishedCallback = null)
		{
			GetPlayerProfileRequest request = new()
			{
				PlayFabId = PlayfabId
			};

			PlayFabClientAPI.GetPlayerProfile
			(
				request,
				r =>
				{
					if (r.PlayerProfile != null) DisplayName = r.PlayerProfile.DisplayName;
					finishedCallback?.Invoke();
				},
				e => Debug.LogError("")
			);
		}
	}
}
