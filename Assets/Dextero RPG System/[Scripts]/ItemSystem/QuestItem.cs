using UnityEngine;

[System.Serializable]
public class QuestItem : Item {
	public QuestItem(){
		Name = "Quest Item";
		ItemType = ItemEquipType.QuestItem;
		Rarity = RarityTypes.QuestItem;
		CanBeSold = false;
		MaxStacks = 100;
		CurStacks = 1;
		CanBeDropped = true;
	}
	
	public QuestItem(string name, string desc, int curStacks, int maxStacks, Texture2D icon){
		Name = name;
		Description = desc;
		
		ItemType = ItemEquipType.QuestItem;
		Rarity = RarityTypes.QuestItem;
		CanBeSold = false;
		CanBeDropped = true;
		MaxStacks = maxStacks;
		CurStacks = curStacks;
		Icon = icon;
	}
	
	public override string ToolTip() {
		return 	"Cannot be sold";
				
	}
}
