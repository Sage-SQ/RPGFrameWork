using UnityEngine;


[System.Serializable]
public class CraftItem : Item {
	public CraftItem(){
		Name = "Craft Item";
		ItemType = ItemEquipType.CraftItem;
		Rarity = RarityTypes.CraftItem;
		CanBeSold = true;
		MaxStacks = 100;
		CurStacks = 1;
		CanBeDropped = true;
	}
	
	public CraftItem(string name, string desc, int curStacks, int maxStacks, Texture2D icon){
		Name = name;
		Description = desc;
		ItemType = ItemEquipType.CraftItem;
		Rarity = RarityTypes.CraftItem;
		CanBeSold = true;
		CanBeDropped = true;
		MaxStacks = maxStacks;
		CurStacks = curStacks;
		Icon = icon;
	}
	
	public override string ToolTip() {
		return 	"Useful for crafting.";
				
	}
}
