using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class CraftedItemsClasses {
	
	//Classes for all Craftable Items
	
	public const string CRAFTED_ITEM_LOCATION = "Item Icons/CraftableItems/";
	public static List<Item> AllCraftableItems = new List<Item>(); 
	
	//Wooden Sword
	public static Item WoodenSword(){
		string iconLocation = CRAFTED_ITEM_LOCATION + "Wooden Sword";
		Texture2D icon = Resources.Load (iconLocation) as Texture2D;
		Weapon craftedItem = new Weapon(){
			//Item Info
			Name = "Newbie Sword",
			Value = 1,
			Rarity = RarityTypes.CraftedItem,
			ItemType = ItemEquipType.Weapon,
			RequiredLevel = 1,
			IconPath = iconLocation,
			Icon = icon,
			Description = "A crafted wooden sword!",
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
	
	//Copper Chest
	public static Item CopperChest(){
		string iconLocation = CRAFTED_ITEM_LOCATION + "Copper Chest";
		Texture2D icon = Resources.Load (iconLocation) as Texture2D;
		Armor craftedItem = new Armor(){
			//Item Info
			Name = "Newbie Sword",
			Value = 1,
			Rarity = RarityTypes.CraftedItem,
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
	
	//Linen Gloves
	public static Item LinenGloves(){
		string iconLocation = CRAFTED_ITEM_LOCATION + "Linen Gloves";
		Texture2D icon = Resources.Load (iconLocation) as Texture2D;
		Armor craftedItem = new Armor(){
			//Item Info
			Name = "Newbie Sword",
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
	
	//Linen Boots
	public static Item LinenBoots(){
		string iconLocation = CRAFTED_ITEM_LOCATION + "Linen Boots";
		Texture2D icon = Resources.Load (iconLocation) as Texture2D;
		Armor craftedItem = new Armor(){
			//Item Info
			Name = "Newbie Sword",
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
		AllCraftableItems = new List<Item>();
		AllCraftableItems.Add (WoodenSword());
		AllCraftableItems.Add (CopperChest());
		AllCraftableItems.Add (LinenGloves());
		AllCraftableItems.Add (LinenBoots());
		//AllCraftableItems.Add (CraftableItem);
	}
}
