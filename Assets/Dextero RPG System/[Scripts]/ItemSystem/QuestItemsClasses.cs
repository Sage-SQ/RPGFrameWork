using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class QuestItemsClasses {
	
	public const string QUEST_ITEM_LOCATION = "Item Icons/QuestItems/";
	public static List<Item> AllQuestItems = new List<Item>(); 
	
	//For cube shard quest
	public static Item CubeShardItem(){
		string iconPath = QUEST_ITEM_LOCATION + "CubeShard";
		Texture2D icon = Resources.Load (iconPath) as Texture2D;
		QuestItem questItem = new QuestItem(){
			Name = "Cube Shard",
			Description = "A shard of the cube destroyed.",
			CurStacks = 1,
			MaxStacks = 5,
			Icon = icon
			
		};
		return questItem;
	}
	
	public static void AddAllItems() {
		AllQuestItems = new List<Item>();
		AllQuestItems.Add (CubeShardItem());
		//AllQuestItems.Add (QuestItem);
	}
}
