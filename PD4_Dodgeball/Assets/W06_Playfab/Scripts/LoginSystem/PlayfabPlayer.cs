using PD4.Singleton;
using PlayFab;
using PlayFab.ClientModels;
using System;
using UnityEngine;

namespace Assets.W06_Playfab.Scripts.LoginSystem
{
	class PlayfabPlayer : Singleton<PlayfabPlayer>
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
