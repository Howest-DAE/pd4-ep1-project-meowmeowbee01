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
			if (PlayFabClientAPI.IsClientLoggedIn()) PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), r => gold = r.VirtualCurrency["GD"], Debug.LogError);
			await Task.Delay(2000);
			return gold;
		}

		public static void IncreaseGold(int amount) => PlayFabClientAPI.AddUserVirtualCurrency(new() { VirtualCurrency = "GD", Amount = amount }, r => Debug.Log($"Gold: {r.Balance}"), Debug.LogError);
	}
}