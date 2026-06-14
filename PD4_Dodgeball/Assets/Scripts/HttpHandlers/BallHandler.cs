using Assets.Scripts.Dodgeball.Presenter;
using Assets.Scripts.Singleton;
using PlayFab;
using PlayFab.ClientModels;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.HttpHandlers
{
	public class BallHandler : MonobehaviourSingleton<BallHandler>
	{
		public event EventHandler BallPurchaseSuccess;

		public BallSpawner CurrentBallSpawner { get; set; }

		public void BuyBall(BallSpawner spawner)
		{
			CurrentBallSpawner = spawner;
			if (!PlayFabClientAPI.IsClientLoggedIn()) return;
			var request = new PurchaseItemRequest
			{
				ItemId = "Ball",
				VirtualCurrency = "GD",
				Price = 5
			};
			PlayFabClientAPI.PurchaseItem(request, OnPurchaseSuccess, OnPurchaseError);
		}

		private void OnPurchaseSuccess(PurchaseItemResult result)
		{
			Debug.Log("Ball purchase successfull");
			//give go for spawning ball
			OnBallSuccessfullyPurchased();
			GetInventory();
		}

		private void OnPurchaseError(PlayFabError error)
		{
			Debug.Log("Ball Purchase error: " + error.GenerateErrorReport());
		}

		private void GetInventory()
		{
			PlayFabClientAPI.GetUserInventory(new GetUserInventoryRequest(), OnGetInventorySucess, InventoryError);
		}

		private void ConsumeBall(string instanceID)
		{
			var request = new ConsumeItemRequest()
			{
				ItemInstanceId = instanceID,
				ConsumeCount = 1
			};

			PlayFabClientAPI.ConsumeItem(request,
				OnConsumeSuccess,
				OnConsumeError);
		}

		private void InventoryError(PlayFabError error)
		{
			Debug.Log(error.GenerateErrorReport());
		}

		private void OnGetInventorySucess(GetUserInventoryResult result)
		{
			List<ItemInstance> InventoryList = result.Inventory;
			if (InventoryList.Count == 0) return;

			foreach (ItemInstance item in InventoryList)
			{
				if (item.ItemId == "Ball")
				{
					ConsumeBall(item.ItemInstanceId);
					break;
				}
			}
		}

		private void OnConsumeSuccess(ConsumeItemResult result)
		{
			Debug.Log("Item consumed successfully! Remaining uses: " + result.RemainingUses);
		}

		private void OnConsumeError(PlayFabError error)
		{
			Debug.LogError("Failed to consume item: " + error.GenerateErrorReport());
		}

		public void OnBallConsumeSuccess()
		{
			Debug.Log("Ball consumption was successful");
		}

		private void OnBallSuccessfullyPurchased()
		{
			BallPurchaseSuccess.Invoke(this, EventArgs.Empty);
		}
	}
}