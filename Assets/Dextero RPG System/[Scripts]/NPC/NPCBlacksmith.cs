using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCBlacksmith : MonoBehaviour {
	
	private PlayerNew player;
	public List<Item> itemsNeeded = new List<Item>();
	// Use this for initialization
	void Start () {
		if(player == null){
			player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerNew>();
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnMouseDown(){

	}
	
	public void CraftItem(Item item){
		//FOREACH LOOP THROUGH EACH ITEM, TEMP INT TO CHECK EACH STACK, BOOL TO CHECK IF TEMP = NUMNEEDED(== ITEM.CURSTACKS)
		//WE SET ITEMSNEED.CURSTACKS AS THE NUMBER NEEDED
		
		//Check level if(player.CraftingLevel >= item.Level) {
		
		GetMatsInfo(item); //sets itemsNeeded
		
		bool haveAllItems = true;
		for (int i = 0; i < itemsNeeded.Count; i++) {
			int tempMatCount = 0;
			int matCountNeed = itemsNeeded[i].CurStacks;
			foreach (Item x  in player.Inventory) {
				if(x.Name == itemsNeeded[i].Name){
					tempMatCount += x.CurStacks;
				}
			}
			if(tempMatCount < matCountNeed){
				haveAllItems = false;
			}
			//Debug.Log (itemsNeeded[i].Name + " " + itemsNeeded[i].CurStacks + );
		}
		
		if(haveAllItems){
			if(player.Inventory.Count < player.MaxInventorySpace){
				for (int i = 0; i < itemsNeeded.Count; i++) {
					int itemsLeftToTake = itemsNeeded[i].CurStacks;
					
					for (int u = 0; u < player.Inventory.Count; u++) {
						if(player.Inventory[u].Name == itemsNeeded[i].Name){
							int numTaken = 0;
							if(player.Inventory[u].CurStacks - itemsLeftToTake <= 0){
								numTaken += player.Inventory[u].CurStacks;
								player.Inventory.RemoveAt (u);
								u--;
							}
							else {
								player.Inventory[u].CurStacks -= itemsLeftToTake;
								if(player.Inventory[u].CurStacks == 0){
									player.Inventory.RemoveAt (u);
									u--;
								}
								numTaken += itemsLeftToTake;
							}
							
							itemsLeftToTake -= numTaken;
						}
					}	
				}
				
				player.Inventory.Add (DeepCopy.CopyItem(item));
			}
			else{
				Debug.Log ("Inventory is full");
			}
		}
		else {
			Debug.Log ("Don't have all items");
		}
		//else { Debug.Log("Not high enough crafting level");
	}
	
	public void GetMatsInfo(Item item){
		itemsNeeded = new List<Item>();

		switch(item.RequiredLevel){
			case 1:
				AddToItemsNeeded(CraftMaterialsClasses.Wood(),1);
				AddToItemsNeeded(CraftMaterialsClasses.Linen(),1);
				AddToItemsNeeded(CraftMaterialsClasses.Copper(),1);
				break;
			case 10:
				AddToItemsNeeded(CraftMaterialsClasses.Wood(),3);
				AddToItemsNeeded(CraftMaterialsClasses.Linen(),3);
				AddToItemsNeeded(CraftMaterialsClasses.Copper(),3);
				break;
			case 20:
				AddToItemsNeeded(CraftMaterialsClasses.Wood(),5);
				AddToItemsNeeded(CraftMaterialsClasses.Linen(),5);
				AddToItemsNeeded(CraftMaterialsClasses.Copper(),5);
				break;
			case 30:
				AddToItemsNeeded(CraftMaterialsClasses.Wood(),5);
				AddToItemsNeeded(CraftMaterialsClasses.Linen(),5);
				AddToItemsNeeded(CraftMaterialsClasses.Copper(),5);
				break;
			case 50:
				AddToItemsNeeded(CraftMaterialsClasses.Wood(),5);
				AddToItemsNeeded(CraftMaterialsClasses.Linen(),5);
				AddToItemsNeeded(CraftMaterialsClasses.Copper(),5);
				break;
			case 75:
				AddToItemsNeeded(CraftMaterialsClasses.Wood(),5);
				AddToItemsNeeded(CraftMaterialsClasses.Linen(),5);
				AddToItemsNeeded(CraftMaterialsClasses.Copper(),5);
				break;
		}
		
		int multiplier = 1;
		if(item.ItemType == ItemEquipType.Clothing){
			Armor armor = item as Armor;
			if(armor.Slot == EquipmentSlot.Helmet || armor.Slot == EquipmentSlot.Back){
				multiplier = 1;
			}
			else if(armor.Slot == EquipmentSlot.Boots || armor.Slot == EquipmentSlot.Gloves) {
				multiplier = 2;
			}
			else if(armor.Slot == EquipmentSlot.Pants || armor.Slot == EquipmentSlot.Chest) {
				multiplier = 4;
			}
		}
		else if (item.ItemType == ItemEquipType.Weapon){
			multiplier = 4;
		}
		
		string tempDebug = "We need \n";
		for (int i = 0; i < itemsNeeded.Count; i++) {
			itemsNeeded[i].CurStacks *= multiplier;
			tempDebug += itemsNeeded[i].Name + " x" + itemsNeeded[i].CurStacks + "\n";
		}
		
		//Debug.Log (tempDebug); if you want to debug items needed
		
		return;
	}
	
	void AddToItemsNeeded(Item item, int quantity){
		Item craftMat = DeepCopy.CopyItem (item);
		craftMat.CurStacks = quantity;
		itemsNeeded.Add(craftMat);
	}
}

