using UnityEngine;
using System.Collections.Generic;

public static class ItemGenerator {
	#region "Variables"
	public const int BASE_MELEE_RANGE = 3;
	public const int BASE_RANGED_RANGE = 15;
	public const int BASE_MAGIC_RANGE = 8;
	
	private static string chestRarity = "";
	private static int levelOfChest = 1;
	
	private static RarityTypes itemRarity ;
	
	//Buff Checkers
	
	private	static bool procAdded = false;
	private	static bool dmgAdded = false;
	private	static bool critChanceAdded = false;
	private	static bool critDmgAdded = false;
	private	static bool moveSpeedAdded = false;
	private	static bool socketsAdded = false;
	
	//Resource Paths
	public const string MELEE_WEAPON_PATH = "Item Icons/Weapons";
	public const string SOCKET_PATH = "Item Icons/Sockets";
	public const string ARMOR_PATH = "Item Icons/Armour";
	public const string CONSUMABLE_PATH = "Item Icons/Consumables";
	#endregion
	
	#region "Rarity boundaries"
	
		//These are for the chests found in dungeons:
			//Easy mode: common drops, medium: uncommon, hard: rare,
			//Epic: epic drops, Legendary Difficulty: legendary drops
		// Common		: 95% Common, 5% Uncommon, 0% Rare, 0% Epic, 0% Legendary
			private static int[] _commonDrops = new int[]{95,5,5,0,0};
		// Uncommon		: 35% Common, 60% Uncommon, 5% Rare, 0% Epic, 0% Legendary
			private static int[] _uncommonDrops = new int[]{35,60,5,0,0};
		// Rare			: 15% Common, 35% Uncommon, 45% Rare, 5% Epic, 0% Legendary
			private static int[] _rareDrops = new int[]{15,35,45,6,0};
		// Epic			: 0% Common, 35% Uncommon, 30% Rare, 40% Epic, 5% Legendary
			private static int[] _epicDrops = new int[]{0,35,30,40,5};
		// Legendary	: 0% Common, 0% Uncommon, 20% Rare, 50% Epic, 30% Legendary
			private static int[] _legendDrops = new int[]{0,0,20,50,30};
	
	
		// Any	: 0% Common, 0% Uncommon, 20% Rare, 50% Epic, 30% Legendary
			private static int[] _anyDrops = new int[]{20,20,20,20,20};
		// Random	: 69% Common, 15% Uncommon, 8% Rare, 5% Epic, 3% Legendary
			private static int[] _randomDrops = new int[]{45,28,15,9,3};
	#endregion
		
	#region "Decide What Item To Make"
	//Pass 3 variables
			// 1. The type of item to make, or random.
			// 2. The rarity of the item (higher rarity , better buffs etc)
			//    This should hopefully be a random factor, e.g. 30% of getting rares or w/e
			// 3. Item level, low levels can get rare items but only do low dmg
			//    Higher item levels will be rare and also have higher dps.
	
	public static Item CreateItem(string itemType,string chestRarityType, int chestLevel) {
		
		//reset for buffs
		procAdded = false;
		dmgAdded = false;
		critChanceAdded = false;
		critDmgAdded = false;
		moveSpeedAdded = false;
		socketsAdded = false;
		
		//init
		chestRarity = chestRarityType;
		levelOfChest = chestLevel;
		
		//set rarity
		itemRarity = RandomizeRarity();

		Item item = new Item();
		//call the method to create that base item type
			//fill in all of the values for the item type
			
		//RANDOM EQUIPS or RANDOM ALL
		if(itemType == "random"){
			int randomNum = Random.Range(1,100+1);
			if(randomNum < 20){
				itemType = "weapon";
			}
			else if (randomNum < 70){
				itemType = "armor";
			}
			else if(randomNum < 90 ){
				itemType = "consum";
			}
			else {
				itemType = "socket";
			}
			
		}
		else {
			itemType = "armor";
		}
		
		//itemType = "weapon";
		//chestRarity = "legendary";
		
		switch(itemType){
			case "weapon":
				item = CreateWeapon ();
				break;
			case "armor":
				item = CreateArmor ();
				break;
			case "socket":
				item = CreateSocketItem ();
				break;
			case "consum":
				item = CreateConsumable();
				break;
			default:
				item = ItemGenerator.CreateItem("random",chestRarity,levelOfChest);
				break;
		}
	

		//Only for things which can be equipped
		if( item.ItemType != ItemEquipType.Consumable &&
			item.ItemType != ItemEquipType.Socket){
			
			item.Rarity = itemRarity;
			item.RequiredLevel = chestLevel + Random.Range (-4,3+1);
		
			if(item.RequiredLevel == 0 || item.RequiredLevel < 0)
				item.RequiredLevel = 1;
			
			if(item.RequiredLevel > 100)
				item.RequiredLevel = 100;
			

			item = AddBuffs(item);
			item.Name = GenerateName(item);
			item.Value = CalculateValue(item);
		}
		
		
		
		//Finally, if it's a consumable (add other stuff later) we will set item to common
			//later we want to change this for buffs/perm buffs, as they are rarer
			//also want to add similar thing at end for sockets and quest items
			//so quest items aren't showing up as legendary items etc
			//quest items shud have unique color
		if(item.ItemType == ItemEquipType.Consumable){
			item.Rarity = RarityTypes.Common;
		}
		
		//Example
//		if(item.ItemType == ItemEquipType.Weapon){
//			AddProc (item,1.55f);
//			(item as Weapon).ProcModifier = 0.5f;
//			(item as Weapon).Sockets = 5;
//		}
		
		if(item.Description == "")
			item.Description = "An item I've found during my adventures.";
	
		return item;
	}
	#endregion
	
	//*********************************************
	
	#region "Weapon Generation"
	public static Weapon CreateWeapon(){
		//Initialize weapon
		Weapon weapon = new Weapon();

		weapon = CreateMeleeWeapon();
		
		
		//Rarity scaling
		float rarityMod = 0;
		switch(itemRarity){
		case RarityTypes.Common:
			rarityMod = 1;
			break;
		case RarityTypes.Uncommon:
			rarityMod = 1.10f;
			break;
		case RarityTypes.Rare:
			rarityMod = 1.20f;
			break;
		case RarityTypes.Epic:
			rarityMod = 1.35f;
			break;
		case RarityTypes.Legendary:
			rarityMod = 1.55f;
			break;
		}
		
		//Remaining weapon values, scaled off chest level
		
		weapon.MaxDamage= Random.Range (5,10+1) + (int)((Random.Range (20.0f,35.0f+0.1f) * levelOfChest *rarityMod));
		float dmgVarMod = rarityMod - 1.0f;
		weapon.DmgVariance = Random.Range (.1f + dmgVarMod, .13f +(dmgVarMod/1.6f));

		weapon.AttackSpeed = 1.1f + (Random.Range (0.0012f,0.0045f) * levelOfChest * rarityMod);
	
		//Defaults
		weapon.ItemType = ItemEquipType.Weapon;
		weapon.DmgType = DamageType.Normal;
		weapon.Proc = ProcType.None;
		
		//Set up basic name and icon
		string whatPath = MELEE_WEAPON_PATH;
		Object[] textures = Resources.LoadAll(whatPath,typeof(Texture2D));
        weapon.Icon  = textures[Random.Range(0, textures.Length)] as Texture2D;
		weapon.IconPath = MELEE_WEAPON_PATH + "/" + weapon.Icon.name;
		weapon.Name = weapon.Icon.name;
		
		//return weapon
		return weapon;
	}
	
	public static Weapon CreateMeleeWeapon(){
		//create new weapon	
		Weapon meleeWeapon = new Weapon();
		
		//return the new item
		return meleeWeapon;
	}
	#endregion
	
	#region "Armor Generation"
	public static Armor CreateArmor(){
		//Initialize weapon
		Armor armor = CreateArmorPiece();
		
		//Rarity scaling
		float rarityMod = 0;
		switch(itemRarity){
		case RarityTypes.Common:
			rarityMod = 1;
			break;
		case RarityTypes.Uncommon:
			rarityMod = 1.10f; //max 60
			break;
		case RarityTypes.Rare:
			rarityMod = 1.20f; //max 80
			break;
		case RarityTypes.Epic:
			rarityMod = 1.35f; //max 120
			break;
		case RarityTypes.Legendary:
			rarityMod = 1.50f; //max 200
			break;
		}
		
		//add Armor
		armor.ArmorAmt = (int)(Random.Range (2.0f,10.0f+0.1f) * levelOfChest * rarityMod);
				
		//Chance to block
		if(armor.Slot == EquipmentSlot.Gloves || armor.Slot == EquipmentSlot.Chest){
			//chest and gloves give chance to block
			armor.ChanceToBlock = Random.Range (0.08f,0.22f+0.001f) * rarityMod;
			armor.DamageBlocked = (int)(Random.Range (5.0f,20.0f+0.1f) * levelOfChest * rarityMod);
		}
		
		//Movement Speed
		if(armor.Slot == EquipmentSlot.Boots){
			//boots give chance to block
			armor.MoveSpeedBuff += (10*rarityMod)*0.01f;
		}
		
		//Defaults
		armor.ItemType = ItemEquipType.Clothing;
		
		//Set up basic name and icon
		string whatPath = ARMOR_PATH + "/" + armor.Slot.ToString();
		Object[] textures = Resources.LoadAll(whatPath,typeof(Texture2D));
		if(textures.Length < 1) Debug.Log ("Resources folder missing icons. Please read the README or extract Resources.rar for everything to work!" +
		 	"Any other problems email LogicSpawnGames@gmail.com");
		
        armor.Icon  = textures[Random.Range(0, textures.Length)] as Texture2D;
		armor.IconPath = whatPath + "/" + armor.Icon.name;
		armor.Name = armor.Icon.name;
		
		//return weapon
		return armor;
	}
	public static Armor CreateArmorPiece(){
		Armor armorPiece = new Armor();
		EquipmentSlot armorSlot = EquipmentSlot.Helmet;
		
		EquipmentSlot[] randomArmorSlot = new EquipmentSlot[] {
			EquipmentSlot.Helmet, EquipmentSlot.Chest,EquipmentSlot.Back,
			EquipmentSlot.Gloves, EquipmentSlot.Pants, EquipmentSlot.Boots
		};

		armorSlot = randomArmorSlot[Random.Range (0,randomArmorSlot.Length)];
		armorPiece.Slot = armorSlot;

		//return the new item
		return armorPiece;
	}
	#endregion
	
	#region "Consumable Generation"
	public static Consumable CreateConsumable(){
		
		//decide what type of consumable to make
			//if random, make high chance of vitalpot, medium chance of buffPot,
			//(e.g. +30 might for 3 minutes), rare chance of unique pot
			//(e.g. +jump height, +moveSpeed)
		Consumable consum = CreateVitalPot();
		
		//set tier of consumable based on level
		consum.Tier = 1;
		consum.CurStacks = 1;
		consum.MaxStacks = 50;
		if(levelOfChest < 20)
			consum.Tier = 1;
		else if(levelOfChest < 40)
			consum.Tier = 10;
		else if(levelOfChest < 60)
			consum.Tier = 100;
		else if(levelOfChest < 80)
			consum.Tier = 1000;
		else
			consum.Tier = 2500;
		
		//roman numerals tier
		
		string tierRN = "I";
		
		if(consum.Tier == 1)
			tierRN = "I";
		else if(consum.Tier == 10)
			tierRN = "II";
		else if(consum.Tier == 100)
			tierRN = "III";
		else if(consum.Tier == 1000)
			tierRN = "IV";
		else
			tierRN = "V";

		switch(consum.ConsumType){
		case ConsumableType.Health:
			consum.Value = consum.Tier;
			consum.VitalToRestore = VitalName.Health;
			consum.AmountToHeal = 10 * consum.Tier;
			consum.Name = string.Format("Health Potion [{0}]",tierRN);
			break;
		case ConsumableType.Mana:
			consum.Value = consum.Tier;
			consum.VitalToRestore = VitalName.Mana;
			consum.AmountToHeal = 10 * consum.Tier;
			consum.Name = string.Format("Mana Potion [{0}]",tierRN);
			break;
		case ConsumableType.Energy:
			consum.Value = consum.Tier;
			consum.VitalToRestore = VitalName.Energy;
			consum.AmountToHeal = 10 * consum.Tier;
			consum.Name = string.Format("Energy Potion [{0}]",tierRN);
			break;				
		}
		
		//defaults
		consum.ItemType = ItemEquipType.Consumable;
		
		//Set up basic name and icon
		string whatPath = "";
		whatPath = CONSUMABLE_PATH + "/" + consum.ConsumType.ToString() + consum.Tier.ToString();
	
        consum.Icon  = Resources.Load (whatPath) as Texture2D;
		consum.IconPath = whatPath;
		
		//return consumable
		return consum;
	}
	
	public static Consumable CreateVitalPot(){
		//init
		
		Consumable consum = new Consumable();
		
		//decide what type of pot to make
		ConsumableType consumType = ConsumableType.Health;
		
		consumType = GM.GetRandomEnum<ConsumableType>();
		
		consum.ConsumType = consumType;

		//return the new item
		return consum;
	}
	#endregion
	
	#region "SocketItem Generation"
	public static SocketItem CreateSocketItem(){
		
		SocketItem socket = new SocketItem();
		
		socket.SocketType = GM.GetRandomEnum<SocketTypes>();
		if(levelOfChest < 20)
			socket.SocketTier = 1;
		else if(levelOfChest < 40)
			socket.SocketTier = 2;
		else if(levelOfChest < 60)
			socket.SocketTier = 3;		
		else if(levelOfChest < 80)
			socket.SocketTier = 4;			
		else
			socket.SocketTier = 5;
		
		
		//roman numerals tier
		
		string tierRN = "I";
		
		if(socket.SocketTier == 1)
			tierRN = "I";
		else if(socket.SocketTier == 2)
			tierRN = "II";
		else if(socket.SocketTier == 3)
			tierRN = "III";
		else if(socket.SocketTier == 4)
			tierRN = "IV";
		else
			tierRN = "V";
			
		
		switch (socket.SocketType) {
			case SocketTypes.Citrine:
				socket.Name = "Citrine Gem" + " " + tierRN;
				socket.Description = "";
				socket.AddVitalModifier(new VitalBuff{vital=VitalName.Energy,amount=(int)(1000*socket.SocketTier)});
				break;
			case SocketTypes.Dex:
				socket.Name = "Dexterity Rune" + " " + tierRN;
				socket.Description = "";
				socket.AddAttribModifier(new AttributeBuff{attribute=AttributeName.Dexterity,amount=(int)(100*socket.SocketTier)});
				break;
			case SocketTypes.Int:
				socket.Name = "Intelligence Rune" + " " + tierRN;
				socket.Description = "";
				socket.AddAttribModifier(new AttributeBuff{attribute=AttributeName.Intelligence,amount=(int)(100*socket.SocketTier)});
				break;
			case SocketTypes.Ruby:
				socket.Name = "Ruby Gem" + " " + tierRN;
				socket.Description = "";
				socket.AddVitalModifier(new VitalBuff{vital=VitalName.Health,amount=(int)(1000*socket.SocketTier)});
				break;
			case SocketTypes.Sapphire:
				socket.Name = "Sapphire Gem" + " " + tierRN;
				socket.Description = "";
				socket.AddVitalModifier(new VitalBuff{vital=VitalName.Mana,amount=(int)(1000*socket.SocketTier)});
				break;
			case SocketTypes.Str:
				socket.Name = "Strength Rune" + " " + tierRN;
				socket.Description = "";
				socket.AddAttribModifier(new AttributeBuff{attribute=AttributeName.Strength,amount=(int)(100*socket.SocketTier)});
				break;
			case SocketTypes.Vit:
				socket.Name = "Vitality Rune" + " " + tierRN;
				socket.Description = "";
				socket.AddAttribModifier(new AttributeBuff{attribute=AttributeName.Vitality,amount=(int)(100*socket.SocketTier)});
				break;
			default:
				break;
		}
		
		socket.Value = 1000;
		
		//defaults
		socket.ItemType = ItemEquipType.Socket;
		socket.RequiredLevel = 0;
		
		//Set up basic name and icon
		string whatPath = "";
		whatPath = SOCKET_PATH + "/" + socket.SocketType.ToString() + socket.SocketTier.ToString();
        socket.Icon  = Resources.Load (whatPath) as Texture2D;
		socket.IconPath = whatPath;
		socket.Rarity = RarityTypes.SocketItem;
		//return consumable
		return socket;
	}
	#endregion
	
	#region "Calculate Value Of Item"
	public static int CalculateValue(Item item){
		int valueOfItem = 0;
		

		//first find value of buffs
		valueOfItem = CalculateBuffItemValue(item);
		
		//then specifics
		if(item.ItemType == ItemEquipType.Weapon){
			valueOfItem += CalculateWeaponValue(item);
		}
		else if(item.ItemType == ItemEquipType.Clothing){
			valueOfItem += CalculateArmorValue(item);
		}
		
		//then return value
		
		return valueOfItem;
	}
	
	public static int CalculateBuffItemValue(Item item){
		int valueOfItem = 0;
		
		foreach(AttributeBuff ab in (item as BuffItem).AttribBuffs){
			valueOfItem += ab.amount * 50;
		}
			
		foreach(VitalBuff vb in (item as BuffItem).VitalBuffs){
			valueOfItem += vb.amount / 2;
		}
		
		valueOfItem += (item as BuffItem).Sockets * 2500;
		
		return valueOfItem;
	}
	
	public static int CalculateWeaponValue(Item item){
		int valueOfItem = 0;
		Weapon wep = item as Weapon;
		
		valueOfItem += wep.MaxDPS * 4;
		
		if(wep.DmgType != DamageType.Normal)
			valueOfItem += wep.DmgValue * 25;
		if(wep.Proc != ProcType.None)
			valueOfItem += levelOfChest * 35;
		
		valueOfItem += (int)wep.AttackSpeed * 500;
		valueOfItem += (int)wep.CritChance * 500;
		valueOfItem += (int)((wep.CritDamage - 2.0f) * 80);
		

		
		return valueOfItem;
	}
	
	public static int CalculateArmorValue(Item item){
		int valueOfItem = 0;
		Armor armor = item as Armor;

		valueOfItem += armor.ArmorAmt * 20;
		valueOfItem += (int)armor.ChanceToBlock * 500;
		valueOfItem += armor.DamageBlocked * 4;
		valueOfItem += (int)armor.MoveSpeedBuff * 500;
		
		return valueOfItem;
	}
	#endregion
	
	#region "Generate Name"
	public static string GenerateName(Item item){
		string name = item.Name;
		
		return name;
		
	}
	#endregion
	
	#region "Random Rarity Type"
	public static RarityTypes RandomizeRarity(){
		//Default rarity to common
		RarityTypes itemRarity = RarityTypes.Common;
		//default to the commonDrops randomness
		int[] intToUse = _commonDrops;
		
		//Randomize rarirty depending on chestRarity type
		int randomizer = Random.Range (1,100+1);
		
		
			switch (chestRarity) {
				case "random":
					intToUse = _randomDrops;
					break;
				case "any":
					intToUse = _anyDrops;
					break;
				case "common":
					intToUse = _commonDrops;
					break;
				case "uncommon":
					intToUse = _uncommonDrops;
					break;
				case "rare":
					intToUse = _rareDrops;
					break;
				case "epic":
					intToUse = _epicDrops;
					break;
				case "legendary":
					intToUse = _legendDrops;
					break;
			}
		
		//private static int[] _legendDrops = new int[]{0,0,20,50,30};

		int commonItems 	= intToUse[0];
		int uncommonItems 	= intToUse[0] + intToUse[1];
		int rareItems 		= intToUse[0] + intToUse[1] + intToUse[2];
		int epicItems 		= intToUse[0] + intToUse[1] + intToUse[2] + intToUse[3];
		int legendItems 	= intToUse[0] + intToUse[1] + intToUse[2] +intToUse[3] + intToUse[4];
		
		if(randomizer < (intToUse[0]+1)){
				
			itemRarity = RarityTypes.Common;
	
		}
		else if(commonItems< randomizer && randomizer < uncommonItems + 1){
	
			itemRarity = RarityTypes.Uncommon;
	
		}
		else if(uncommonItems < randomizer && randomizer < rareItems + 1){
			
			itemRarity = RarityTypes.Rare;
	
		}
		else if(rareItems < randomizer && randomizer < epicItems + 1){
	
			itemRarity = RarityTypes.Epic;
			
		}
		else if(epicItems < randomizer && randomizer < legendItems + 1){
	
			itemRarity = RarityTypes.Legendary;
			
		}
		//itemRarity = RarityTypes.Legendary;
		return itemRarity;
	}
	#endregion
	
	#region "Add Random Buffs and Stats"
	public static Item AddBuffs(Item item){

		int buffsWanted = 0; //Number of buffs we want to add
		int StatBuffs = 0; //Number of stat buffs to add
		bool AddStatBuffs = false;
		float rarityMod = 0;
		
		//Check how many buffs will be added
		switch(itemRarity){
			case RarityTypes.Common:
				buffsWanted = Random.Range (0,0+1);
				StatBuffs = 0;
				rarityMod = 1.0f;
				break;
			case RarityTypes.Uncommon:
				buffsWanted = Random.Range (0,0+1);
				StatBuffs = 1;
				rarityMod = 1.2f;
				break;
			case RarityTypes.Rare:
				buffsWanted = Random.Range (0,1+1);
				StatBuffs = 2;
				rarityMod = 1.4f;
				break;
			case RarityTypes.Epic:
				buffsWanted = Random.Range (1,2+1);
				StatBuffs = 2;
				rarityMod = 1.6f;
				break;
			case RarityTypes.Legendary:
				buffsWanted = Random.Range (2,3+1);
				StatBuffs = 3;
				rarityMod = 1.8f;
				break;
		}
		if(StatBuffs > 0)
			AddStatBuffs = true;
		//Add default statbuffs depending on rarity
		
		if(AddStatBuffs){
			if(item.ItemType == ItemEquipType.Weapon){
				Weapon wep = item as Weapon;
				
				//Add 1 stat buff for the CORRECT attrib (str for melee, dex for ranged , int for magic)
				int amountToAdd = (int)(Random.Range (1.0f,2.0f+0.1f) * levelOfChest * rarityMod);
				int randomNum = Random.Range (1,3+1);
				switch(randomNum){
					case 1:
						//add str attrib
						wep.AddAttribModifier (new AttributeBuff{attribute=AttributeName.Strength,amount=amountToAdd});
						break;
					case 2:
						//add dex attrib
						wep.AddAttribModifier (new AttributeBuff{attribute=AttributeName.Dexterity,amount=amountToAdd});
						break;
					case 3:
						//add int attrib
						wep.AddAttribModifier (new AttributeBuff{attribute=AttributeName.Intelligence,amount=amountToAdd});
						break;
				}
				
				for(int cnt = 0; cnt < StatBuffs-1;cnt++){	//Add remaining statbuffs
					//Debug.Log ("Adding random buff " + cnt);
					AddRandomStatBuff(item as BuffItem, rarityMod);
				}

				
				item = wep;
			}
			else{	
				for(int cnt = 0; cnt < StatBuffs;cnt++){
					//Debug.Log ("Adding random buff " + cnt);
					AddRandomStatBuff(item as BuffItem, rarityMod);
				}
			}
			
			//Add random buffs till buffsWanted is reached
			for(int cnt = 0; cnt < buffsWanted;cnt++){
				//Debug.Log ("Adding random buff " + cnt);
				AddRandomBuff(item as BuffItem, rarityMod);
			}
		}
			return item;
	}

	public static Item AddRandomBuff(BuffItem item, float rarityMod){
		//first do basic randomizing of whether to even add buff
		
		int randomnum = Random.Range (1,100+1);
		
		//bools
		bool _addingProc = false;
		bool _addingDmgType = false;
		bool _addingSockets = false;
		
		//initalise buffToAdd
		string[] buffToAdd = new string[]{};
		
		if(randomnum < 91 ){
			
			//check item type
			if (item.ItemType == ItemEquipType.Weapon) {					
				int rnd = Random.Range (0,100+1);
				if(rnd < 61){
					buffToAdd = new string[] {
						"critchance","critdmg","dmgtype","proc","sockets"
					};	
				}
				else{
					buffToAdd = new string[] {
						"maxdmg","dmgvar","attackspeed","statbuff"
					};	
				}
			}
			else if (item.ItemType == ItemEquipType.Clothing){
				
					int rnd = Random.Range (0,100+1);
					if(rnd < 31){
						buffToAdd = new string[] {
							"movespeed","sockets","blockdamage"
						};	
					}
					else{
						buffToAdd = new string[] {
							"statbuff"
						};						
					}
			}
		}
		
		if(randomnum < 91 ){
			string buffAdding = buffToAdd[Random.Range(0,buffToAdd.Length)];
			
			switch(buffAdding){
				case "critchance":
					if(!critChanceAdded){
						(item as Weapon).CritChance = Random.Range (0.005f,0.0036f) *rarityMod * levelOfChest;
						critChanceAdded = true;
					}
					else{AddRandomBuff (item,rarityMod);}
					break;
				case "critdmg":
					if(!critDmgAdded){
						(item as Weapon).CritDamage += Random.Range (0.1f,1.3f) *rarityMod;
						critDmgAdded = true;
					}
					else{AddRandomBuff (item,rarityMod);}
					break;
				case "dmgtype":
					if(!dmgAdded){
						_addingDmgType = true;
						dmgAdded = true;
					}
					else{AddRandomBuff (item,rarityMod);}
					break;
				case "proc":
					if(!procAdded){
						_addingProc = true;
						procAdded = true;
					}
					else{AddRandomBuff (item,rarityMod);}
					break;
				case "maxdmg":
					(item as Weapon).MaxDamage += (int)((Random.Range (1.0f,8.0f+0.1f) * levelOfChest *rarityMod));
					break;
				case "dmgvar":
					(item as Weapon).DmgVariance += Random.Range (0.0005f,0.0021f) * levelOfChest *rarityMod;
					break;
				case "attackspeed":
					(item as Weapon).AttackSpeed += Random.Range (0.1f,0.23f) *rarityMod;
					break;
				case "statbuff":
					AddRandomStatBuff(item,rarityMod);
					break;
				case "movespeed":
					if(!moveSpeedAdded){
						if((item as Armor).Slot == EquipmentSlot.Boots){
							(item as Armor).MoveSpeedBuff += Random.Range (0.03f,0.04f+0.001f) * rarityMod;
							moveSpeedAdded = true;
						}
						else {
							moveSpeedAdded = true;
							AddRandomBuff (item,rarityMod);
						}
					}
					else{AddRandomBuff (item,rarityMod);}
					break;
				case "armor":
					(item as Armor).ArmorAmt += (int)(Random.Range (1.0f,3.0f+0.1f) * levelOfChest * rarityMod);
					break;
				case "block damage":
					(item as Armor).ChanceToBlock += Random.Range (0.01f,0.03f) * rarityMod;
					(item as Armor).DamageBlocked += (int)(Random.Range (2.5f,10.0f+0.1f) * levelOfChest * rarityMod);
					break;
				case "vitalregen":
					break;
				case "sockets":
					if(!socketsAdded){
						_addingSockets = true;
						socketsAdded = true;
					}
					else{AddRandomBuff (item,rarityMod);}
					break;
			}
		}
		
		if(_addingProc){
			AddProc (item, rarityMod);
		}
		
		if(_addingDmgType){
			AddDmgType(item, rarityMod);
		}
		
		if(_addingSockets){
			AddSockets(item, rarityMod);
		}
		
		return item;
	}
	
	public static Item AddProc(Item item, float rarityMod){
		ProcType procToAdd = GM.GetRandomEnum<ProcType>();
		
		(item as Weapon).Proc = procToAdd;
		switch(procToAdd){
			case ProcType.GainLifeOnHit:
				(item as Weapon).ProcModifier = Random.Range (10,25) * levelOfChest * rarityMod;
				break;
			case ProcType.GainManaOnHit:
				(item as Weapon).ProcModifier = Random.Range (2,10) * levelOfChest * rarityMod;
				break;
			case ProcType.Knockback:
				(item as Weapon).ProcModifier = Random.Range (0.025f,0.055f) * rarityMod;
				break;
			case ProcType.None:
				AddProc (item, rarityMod);
				break;
			case ProcType.Poison:
				(item as Weapon).ProcModifier = Random.Range (0.025f,0.055f) * rarityMod;
				break;
			case ProcType.Slow:
				(item as Weapon).ProcModifier = Random.Range (0.025f,0.055f) * rarityMod;
				break;
			case ProcType.ConvertToLife:
				(item as Weapon).ProcModifier = Random.Range (0.006f,0.027f + 0.001f) * rarityMod;
				break;
			case ProcType.ConvertToMana:
				(item as Weapon).ProcModifier = Random.Range (0.006f,0.027f + 0.001f) * rarityMod;
				break;
			case ProcType.Stun:
				(item as Weapon).ProcModifier = Random.Range (0.025f,0.055f) * rarityMod;
				break;
			case ProcType.GainEnergyOnHit:
				(item as Weapon).ProcModifier = Random.Range (2,10) * levelOfChest * rarityMod;
				break;
			case ProcType.ConvertToEnergy:
				(item as Weapon).ProcModifier = Random.Range (0.006f,0.027f + 0.001f) * rarityMod;
				break;
			
			}
		
		return item;
	}
	
	public static Item AddDmgType(Item item, float rarityMod){
		DamageType dmgToAdd = GM.GetRandomEnum<DamageType>();
		(item as Weapon).DmgType = dmgToAdd;
		(item as Weapon).DmgValue = (int)(Random.Range (20.0f,35.0f+0.1f) * levelOfChest *rarityMod*1.2f);
		if(dmgToAdd == DamageType.Normal)
			AddDmgType (item, rarityMod);
		
		return item;
	}
	
	public static Item AddSockets(Item item, float rarityMod){
		int maxSockets = 0;
		int minSockets = 0;
		
		switch(itemRarity){
			case RarityTypes.Rare:
				minSockets = 0;
				maxSockets = 1;
				break;
			case RarityTypes.Epic:
				minSockets = 1;
				maxSockets = 2;
				break;
			case RarityTypes.Legendary:
				minSockets = 1;
				maxSockets = 3;
				break;
		}
		
		int socketsToAdd = Random.Range(minSockets,maxSockets+1);
		if(socketsToAdd == 0)		//If fail to add socket, then add another random buff
			AddRandomBuff(item as BuffItem, rarityMod);
		 (item as BuffItem).Sockets = socketsToAdd;
		(item as BuffItem).UsedSockets = 0;
		
		return item;
	}
	
	public static Item AddRandomStatBuff(BuffItem item, float rarityMod){
		
		//initialise new empty string
		string[] attVitToAdd = new string[]{};
			
		//check item type
		if (item.ItemType == ItemEquipType.Weapon) {
			attVitToAdd = new string[] {
				"str","vit","dex","int"
			};	
		}
		else {
			attVitToAdd = new string[] {
				"str","vit","dex","int", "hp","mp","energy"
			};	
		}
		//depending on itemType generate what attrib or vital to add to
		string attVitAdding = attVitToAdd[Random.Range (0,attVitToAdd.Length)];
		//generate amount to add to attrib/vital , then scale it
		int amountToAdd = (int)(Random.Range (1.0f,2.0f+0.1f) * levelOfChest * rarityMod);
		//finally add the stat buff
		switch(attVitAdding){
			case "str":
				item.AddAttribModifier (new AttributeBuff{attribute=AttributeName.Strength,amount=amountToAdd});
				break;
			case "vit":
				item.AddAttribModifier (new AttributeBuff{attribute=AttributeName.Vitality,amount=amountToAdd});
				break;
			case "dex":
				item.AddAttribModifier (new AttributeBuff{attribute=AttributeName.Dexterity,amount=amountToAdd});
				break;
			case "int":
				item.AddAttribModifier (new AttributeBuff{attribute=AttributeName.Intelligence,amount=amountToAdd});
				break;
			case "hp":
				item.AddVitalModifier (new VitalBuff{vital=VitalName.Health,amount=amountToAdd * 75});
				break;
			case "mp":
				item.AddVitalModifier (new VitalBuff{vital=VitalName.Mana,amount=amountToAdd * 25});
				break;
			case "energy":
				item.AddVitalModifier (new VitalBuff{vital=VitalName.Energy,amount=amountToAdd * 25});
				break;		
		}
		return item;
	}
	#endregion
	
}