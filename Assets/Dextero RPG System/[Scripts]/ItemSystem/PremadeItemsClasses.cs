using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class PremadeItemsClasses {
	
	//Classes for all premade items: Starting Gear, Uniques, Unique Quest Rewards etc
	
	public const string PREMADE_ITEM_LOCATION = "Item Icons/PremadeItems/";
	public static List<Item> AllPremadeItems = new List<Item>(); 
	
	//Starting Sword
	public static Item StartingSword(){
		string iconLocation = PREMADE_ITEM_LOCATION + "Newbie Sword";
		Texture2D icon = Resources.Load (iconLocation) as Texture2D;
		Weapon craftedItem = new Weapon(){
			//Item Info
			Name = "Newbie Sword",
			Value = 1,
			Rarity = RarityTypes.Common,
			ItemType = ItemEquipType.Weapon,
			RequiredLevel = 1,
			IconPath = iconLocation,
			Icon = icon,
			Description = "The perfect sword for a beginner!",
			//Weapon Info
			MaxDamage = 35,
			DmgVariance = 0.3f,
			AttackSpeed = 1.0f,
			CritChance = 0,
			CritDamage = 0,
			DmgType = DamageType.Normal,
			DmgValue = 0,
			Proc = ProcType.None,
			ProcModifier = 0,
			Sockets = 0,
			Enchants = 0
		};
		return craftedItem;
	}

	//Armor Example
	public static Item ArmorExample(){
		string iconLocation = PREMADE_ITEM_LOCATION + "Example";
		Texture2D icon = Resources.Load (iconLocation) as Texture2D;
		Armor craftedItem = new Armor(){
			//Item Info
			Name = "Example",
			Value = 1,
			Rarity = RarityTypes.Unique,
			ItemType = ItemEquipType.Clothing,
			RequiredLevel = 1,
			IconPath = iconLocation,
			Icon = icon,
			Description = "A crafted wooden sword!",
			//Armor Info
			ArmorAmt = 25,
			Slot = EquipmentSlot.Chest,
			Sockets = 0
		};
		return craftedItem;
	}
	//Gloves Example
	public static Item GlovesExample(){
		string iconLocation = PREMADE_ITEM_LOCATION + "Example";
		Texture2D icon = Resources.Load (iconLocation) as Texture2D;
		Armor craftedItem = new Armor(){
			//Item Info
			Name = "Example",
			Value = 1,
			Rarity = RarityTypes.CraftedItem,
			ItemType = ItemEquipType.Clothing,
			RequiredLevel = 1,
			IconPath = iconLocation,
			Icon = icon,
			Description = "A crafted wooden sword!",
			//Armor Info
			ArmorAmt = 10,
			Slot = EquipmentSlot.Gloves,
			ChanceToBlock = 0.10f,
			DamageBlocked = 5,
			Sockets = 0
		};
		return craftedItem;
	}
	//Boots Example
	public static Item BootsExample(){
		string iconLocation = PREMADE_ITEM_LOCATION + "Example";
		Texture2D icon = Resources.Load (iconLocation) as Texture2D;
		Armor craftedItem = new Armor(){
			//Item Info
			Name = "Example",
			Value = 1,
			Rarity = RarityTypes.CraftedItem,
			ItemType = ItemEquipType.Clothing,
			RequiredLevel = 1,
			IconPath = iconLocation,
			Icon = icon,
			Description = "A crafted wooden sword!",
			//Armor Info
			ArmorAmt = 10,
			Slot = EquipmentSlot.Boots,
			MoveSpeedBuff = 0.10f,
			Sockets = 0
		};
		return craftedItem;
	}
	
	public static void AddAllItems() {
		AllPremadeItems = new List<Item>();
		//Probably don't need this as I will hand select which class to use
		
		//AllPremadeItems.Add (StartingSword());
		//AllPremadeItems.Add (PremadeItem);
	}
}
