using PlayFab;
using PlayFab.ClientModels;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.HttpHandlers
{
	public static class GoldHandler
	{
		public static async Task<int?> GetGold()
		{
			int? gold = null;
			Task responseTask = Task.Delay(2000);
			if (PlayFabClientAPI.IsClientLoggedIn())
				PlayFabClientAPI.GetUserInventory(
					new GetUserInventoryRequest(),
					r =>
					{
						gold = r.VirtualCurrency["GD"];
						responseTask = Task.CompletedTask;
					},
					e =>
					{
						Debug.LogError(e.GenerateErrorReport());
						responseTask = Task.CompletedTask;
					}
				);
			await responseTask;
			return gold;
		}

		public static async Task IncreaseGold(int amount)
		{
			Task responseTask = Task.Delay(2000);
			PlayFabClientAPI.AddUserVirtualCurrency(
				new() { VirtualCurrency = "GD", Amount = amount },
				r =>
				{
					Debug.Log($"Gold: {r.Balance}");
					responseTask = Task.CompletedTask;
				},
				e =>
				{
					Debug.LogError(e.GenerateErrorReport());
					responseTask = Task.CompletedTask;
				}
			);
			await responseTask;
		}
	}
}