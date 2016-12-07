using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCVendor : MonoBehaviour {
	private PlayerNew player;
	public List<Item> vendorItems = new List<Item>();
	public List<Item> buyBackItems = new List<Item>();
	public const int maxBuyBackItemCount = 4;
	
	void Start () {
		if(player == null){
			player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerNew>();
		}
		
		vendorItems = new List<Item>();
		
		int randomNumberOfItems = Random.Range (2,7+1);
		for (int i = 0; i < randomNumberOfItems; i++) {
			vendorItems.Add(ItemGenerator.CreateItem ("weapon","random",player.Level));
		}
		randomNumberOfItems = Random.Range (2,6+1);
		for (int i = 0; i < randomNumberOfItems; i++) {
			vendorItems.Add(ItemGenerator.CreateItem ("armor","random",player.Level));
		}
		randomNumberOfItems = Random.Range (0,2+1);
		for (int i = 0; i < randomNumberOfItems; i++) {
			vendorItems.Add(ItemGenerator.CreateItem ("socket","random",player.Level));
		}
	}
	
	public bool PlayerBuyItem(int arrayNum){
		Item itemToBuy = vendorItems[arrayNum];
		if(player.Gold >= itemToBuy.Value){ //If we have enough gold to buy
			if(player.AddItem(itemToBuy)){
				player.ModifyGold(-1 * itemToBuy.Value);
				vendorItems.RemoveAt (arrayNum);
				return true;
			}
		}
		
		return false;
	}
	
	public void PlayerSellItem(int inventoryArrayNum){
		Item soldItem = player.Inventory[inventoryArrayNum];
		
		if(soldItem.MaxStacks == 1){
			//Debug.Log(buyBackItems.Count + " " + maxBuyBackItemCount);
			if(buyBackItems.Count == maxBuyBackItemCount){
				buyBackItems.RemoveAt (0); //Remove oldest item
			}
			
			buyBackItems.Add (soldItem);
			player.Inventory.RemoveAt (inventoryArrayNum);
			int sellPrice = (int)(soldItem.Value * 0.65f);	//65% of value earned in gold for selling
			player.ModifyGold(sellPrice);
		}
		else if (soldItem.CurStacks == 1){
			bool itemAdded = false;
			foreach(Item i in buyBackItems){
				//Is the item already there
				if(i.Name == soldItem.Name){
					//Is there space in that stack to add the current item
					if(i.CurStacks + 1 <= i.MaxStacks){
						i.CurStacks += 1;
						player.Inventory.RemoveAt (inventoryArrayNum);
						itemAdded =  true;
					}
				}
			}
			
			if(!itemAdded){
				buyBackItems.Add (soldItem);
				player.Inventory.RemoveAt (inventoryArrayNum);
				int sellPrice = (int)(soldItem.Value * 0.65f);	//65% of value earned in gold for selling
				player.ModifyGold(sellPrice);
			}
		}
		else if(soldItem.CurStacks > 0){
			bool itemAdded = false;
			foreach(Item i in buyBackItems){
				//Is the item already there
				if(i.Name == soldItem.Name){
					//Is there space in that stack to add the current item
					if(i.CurStacks + 1 <= i.MaxStacks){
						i.CurStacks += 1;
						soldItem.CurStacks -= 1;
						
						int sellPrice = (int)(soldItem.Value * 0.65f);	//65% of value earned in gold for selling
						player.ModifyGold(sellPrice);
						
						if(soldItem.CurStacks == 0){
							Debug.Log ("No stacks");
							player.Inventory.RemoveAt (inventoryArrayNum);
						}
						
						itemAdded =  true;
					}
				}
			}
			if(!itemAdded){
				if(buyBackItems.Count == maxBuyBackItemCount){
					buyBackItems.RemoveAt (0); //Remove oldest item
				}
				
				Item onestack = DeepCopy.CopyItem (player.Inventory[inventoryArrayNum]);
				onestack.CurStacks = 1;
				buyBackItems.Add (onestack);
				player.Inventory[inventoryArrayNum].CurStacks -= 1;
				int sellPrice = (int)(soldItem.Value * 0.65f);	//65% of value earned in gold for selling
				player.ModifyGold(sellPrice);
			}
		}
	}
	
	public bool PlayerBuyBack(int arrayNum){
		Item itemToBuyBack = buyBackItems[arrayNum];
		if(player.Gold >= (int)(itemToBuyBack.Value * 0.65f)){ //If we have enough gold to buy
			if(player.AddItem(itemToBuyBack)){
				player.ModifyGold(-(int)(itemToBuyBack.Value * 0.65f));
				buyBackItems.RemoveAt (arrayNum);
				return true;
			}
		}
		
		return false;
	}
}
