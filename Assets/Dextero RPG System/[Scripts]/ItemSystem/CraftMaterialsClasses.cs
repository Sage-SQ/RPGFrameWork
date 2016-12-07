using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public static class CraftMaterialsClasses {
	
	//Classes for all craft materials
	//Naming convention: X Wood, X Ore, X Cloth
	
	public const string CRAFT_MATERIAL_LOCATION = "Item Icons/CraftMaterials/";
	public static List<Item> AllCraftMaterials = new List<Item>(); 
	
	//Copper
	public static Item Copper(){
		string iconLocation = CRAFT_MATERIAL_LOCATION + "Copper";
		Texture2D icon = Resources.Load (iconLocation) as Texture2D;;
		CraftItem questItem = new CraftItem(){
			//Item Info
			Value = 10,
			Rarity = RarityTypes.CraftItem,
			ItemType = ItemEquipType.CraftItem,
			//Craft Material Info
			Name = "Copper Ore",
			Description = "Someone must of mined this copper.",
			CurStacks = 1,
			MaxStacks = 20,
			IconPath = iconLocation,
			Icon = icon
		};
		return questItem;
	}
	
	//Wood
	public static Item Wood(){
		string iconLocation = CRAFT_MATERIAL_LOCATION + "Wood";
		Texture2D icon = Resources.Load (iconLocation) as Texture2D;;
		CraftItem questItem = new CraftItem(){
			//Item Info
			Value = 10,
			Rarity = RarityTypes.CraftItem,
			ItemType = ItemEquipType.CraftItem,
			//Craft Material Info
			Name = "Oak Wood",
			Description = "Wood from an oak tree.",
			CurStacks = 1,
			MaxStacks = 20,
			IconPath = iconLocation,
			Icon = icon,
		};
		
		return questItem;
	}
	
	//Linen
	public static Item Linen(){
		string iconLocation = CRAFT_MATERIAL_LOCATION + "Linen";
		Texture2D icon = Resources.Load (iconLocation) as Texture2D;;
		CraftItem questItem = new CraftItem(){
			//Item Info
			Value = 10,
			Rarity = RarityTypes.CraftItem,
			ItemType = ItemEquipType.CraftItem,
			//Craft Material Info
			Name = "Linen Cloth",
			Description = "Some linen cloth.",
			CurStacks = 1,
			MaxStacks = 20,
			IconPath = iconLocation,
			Icon = icon,
		};

		return questItem;
	}
	
	public static void AddAllItems() {
		AllCraftMaterials = new List<Item>();
		AllCraftMaterials.Add (Copper());
		AllCraftMaterials.Add (Wood());
		AllCraftMaterials.Add (Linen());
		//AllCraftMaterials.Add (CraftMaterial);
	}
}
