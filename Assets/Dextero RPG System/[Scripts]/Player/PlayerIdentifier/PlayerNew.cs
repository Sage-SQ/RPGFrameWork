using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
//You must include these namespaces
//to use BinaryFormatter
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class PlayerNew : PlayerBase {
	
#region "Inventorys, Banks, Stashes"
	public int MaxInventorySpace = 25;
	private List<Item> _inventory = new List<Item>();
	public List<Item> Inventory {
		get{return _inventory;}
		set{_inventory = value;}
	}
	
	private int MaxStashSpace = 125;
	private List<Item> _stash = new List<Item>();
	public List<Item> Stash {
		get{return _stash;}
		set{_stash = value;}
	}

#endregion

#region "Tracking"
	
	private List<NPCQuest> _QuestsComplete = new List<NPCQuest>();
	public List<NPCQuest> questsComplete {
		get{return _QuestsComplete;}
		set{_QuestsComplete = value;}
	}
	
	private List<NPCQuest> _questsInProgress = new List<NPCQuest>();
	public List<NPCQuest> QuestsInProgress {
		get{return _questsInProgress;}
		set{_questsInProgress = value;}
	}
#endregion
	
#region "Achievement Variables"
	private int monstersKilled = 0;
	public int MonstersKilled {
		get{return monstersKilled;}
		set{monstersKilled = value;}
	}
	
	private List<string> _bossesKilled = new List<string>();
	public List<string> BossesKilled {
		get{return _bossesKilled;}
		set{_bossesKilled = value;}
	}
	
#endregion

#region "Variables"

private bool canConsume = true;

#endregion
	// Use this for initialization
	void Start ()
	{	
		//For testing /////////////////////////////////////
		ModifyAttribute(AttributeName.Strength,10);
		ModifyAttribute(AttributeName.Dexterity,10);
		ModifyAttribute(AttributeName.Vitality,10);
		ModifyAttribute(AttributeName.Intelligence,10);
		Name = "Player";
		RefreshVitals();
		///////////////////////////////////////////////////
		
		Item item = ItemGenerator.CreateItem ("consum","random",100);
		item.CurStacks = 10;
		Item itemcopy = DeepCopy.CopyItem (item);
		Inventory.Add (item);
		Inventory.Add (itemcopy);
	}
	
	
	public void OnEnable(){
		Messenger<string>.AddListener("MonsterKilled",AddMobsKilled);
	}
	
	public void OnDisable(){
		Messenger<string>.RemoveListener("MonsterKilled",AddMobsKilled);
	}
	
	// Update is called once per frame
	void Update ()
	{	
		if(Input.GetKeyDown(KeyCode.P)){
			for (int i = 0; i < Vitals.Length; i++) {
				_vitals[i].CurValue = (int)(0.5f * _vitals[i].CurValue);
			}
		}
		
		if(Input.GetKeyDown(KeyCode.O)){
			GiveStuff();
		}
		
	}
	
	//Methods
	
	public void ModifyAttribute(AttributeName att, int amount)
	{
		GetAttribute((int)att).BaseValue += amount;
		UpdateStats();
	}
	
	public bool ModifyGold(int amount){
		Gold += amount;
		return true;
	}
	
	
	//Mobs killed
	private void AddMobsKilled(string mobName){
		MonstersKilled += 1;
		if(mobName.StartsWith("BOSS")){
			BossesKilled.Add (mobName.Substring(4));
		}
	}
	
	//Bank Adding
	public bool AddToBank(int arrayNum){
		
		if(_inventory[arrayNum].MaxStacks > 1){	//maybe check this first
			bool addedToStack = CheckBankStack(_inventory[arrayNum]);
			if(addedToStack){
				_inventory.RemoveAt (arrayNum);
				return true;
			}
		}
		
		if(Stash.Count < MaxStashSpace){
			Stash.Add (_inventory[arrayNum]);
			_inventory.RemoveAt (arrayNum);
			return true;
		}
		Debug.Log ("Bank is full!");
		return false;
	}
	
	private bool CheckBankStack(Item item){
		bool itemsAdded = false;
		foreach(Item i in Stash){
			//Is the item already there
			if(i.Name == item.Name){
				if(i.ItemType == ItemEquipType.QuestItem){
					Debug.Log ("Cannot bank quest items");
					return false;
				}
				//Is there space in that stack to add the current item
				if(i.CurStacks + item.CurStacks <= i.MaxStacks){
					i.CurStacks += item.CurStacks;
					itemsAdded = true;
					return true;
				}
			}
		}
		
		if(!itemsAdded){
			//Manually Check Inventory
			if(Stash.Count < MaxStashSpace){
				Stash.Add (item);
				return true;
			}
			else {
				Debug.Log("Bank too full to add more of this stack");
			}
		}
		
		return false;
		
	}
	
	//Inventory Adding
	public bool AddItem(Item item){
		if(item.Name.Contains("Gold")){
			bool isNotGold = true;
			
			//Just in case we want to put goldin item name, we check if name has numbers in it
			foreach (char c in item.Name)
    			if (char.IsNumber(c))
       				 isNotGold = false;
			
			if(!isNotGold)
				ModifyGold(item.Value);
			return true;
		}
			
		
		if(item.MaxStacks > 1){	//maybe check this first
			bool addedToStack = CheckStack(item);
			if(addedToStack)
				return true;
		}
		
		if(Inventory.Count < MaxInventorySpace){
			Inventory.Add (item);
			return true;
		}
		
		return false;
	}
	
	private bool CheckStack(Item item){
		bool itemsAdded = false;
		foreach(Item i in Inventory){
			//Is the item already there
			if(i.Name == item.Name){
				if(i.ItemType == ItemEquipType.QuestItem){
					i.CurStacks += item.CurStacks;
					if(i.CurStacks > i.MaxStacks)
						i.CurStacks = i.MaxStacks;
					
					return true;
				}
				//Is there space in that stack to add the current item
				if(i.CurStacks + item.CurStacks <= i.MaxStacks){
					i.CurStacks += item.CurStacks;
					itemsAdded = true;
					return true;
				}
			}
		}
		
		if(!itemsAdded){
			//Manually Check Inventory
			if(Inventory.Count < MaxInventorySpace){
				Inventory.Add (item);
				return true;
			}
			else {
				Debug.Log("Inventory too full to add more of this stack");
			}
		}
		
		return false;
		
	}
	
	//Stat Handling
	public void ResetStats()
	{
		HealthFromAttr = ManaFromAttr = EnergyFromAttr = 0;
		PlayerArmor = PlayerDamageBlocked = StrEquipValue = DexEquipValue = IntEquipValue = VitEquipValue = 0;
		PlayerChanceToBlock = PlayerMoveSpeedBuff = 0.0f;
		
		CritChance = BaseCritChance;
		AttackSpeed = MaxDamage = 1;
		DmgVariance = 0.1f;
		CritDamage = 2;
	}
	
	public List<Item> AllEquippedItems(){
		return new List<Item>(){
			EquipedArmorBack, EquipedArmorChest, EquipedArmorFeet, EquipedArmorGloves, EquipedArmorHead, 
			EquipedArmorLegs, EquipedWeapon
		};
	}
	
	public void UpdateStats()
	{
		ResetStats();
		
		List<Item> EquippedItems = AllEquippedItems();
		
		foreach(Item item in EquippedItems){
			if(item != null){
				BuffItem equippedItem = item as BuffItem;
				
				if(equippedItem.EquippedSockets.Count > 0){
					foreach(AttributeBuff ab in equippedItem.EquippedSockets[0].AttribBuffs){
						if (ab.attribute == AttributeName.Vitality){
							VitEquipValue += ab.amount;
						}
						else if(ab.attribute == AttributeName.Strength){
							StrEquipValue += ab.amount;
						}
						else if(ab.attribute == AttributeName.Dexterity){
							DexEquipValue += ab.amount;
						}
						else if(ab.attribute == AttributeName.Intelligence){
							IntEquipValue += ab.amount;
						}
					}
					
					foreach(VitalBuff vb in equippedItem.EquippedSockets[0].VitalBuffs){
						if (vb.vital == VitalName.Health){
							HealthFromAttr += vb.amount;
							
						}
						else if(vb.vital == VitalName.Mana){
							ManaFromAttr += vb.amount;
						}
						else if(vb.vital == VitalName.Energy){
							EnergyFromAttr += vb.amount;
						}
					}
				}
				
				if (item.ItemType == ItemEquipType.Clothing){
					Armor equippedArmor = item as Armor;
					PlayerArmor += equippedArmor.ArmorAmt;
					PlayerChanceToBlock += equippedArmor.ChanceToBlock;
					PlayerDamageBlocked += equippedArmor.DamageBlocked;
					PlayerMoveSpeedBuff += equippedArmor.MoveSpeedBuff;
				}
				
				if(equippedItem.ItemType == ItemEquipType.Weapon){
					Weapon equippedWep= equippedItem as Weapon;
					
					MaxDamage += equippedWep.MaxDamage;
					DmgVariance = equippedWep.DmgVariance; //replace
					AttackSpeed = equippedWep.AttackSpeed; //replace
					CritDamage += equippedWep.CritDamage; //additive, always 2.0
				}
				
				foreach(AttributeBuff ab in equippedItem.AttribBuffs){
					if (ab.attribute == AttributeName.Vitality){
						VitEquipValue += ab.amount;
					}
					else if(ab.attribute == AttributeName.Strength){
						StrEquipValue += ab.amount;
					}
					else if(ab.attribute == AttributeName.Dexterity){
						DexEquipValue += ab.amount;
					}
					else if(ab.attribute == AttributeName.Intelligence){
						IntEquipValue += ab.amount;
					}
				}
			
				foreach(VitalBuff vb in equippedItem.VitalBuffs){
					if (vb.vital == VitalName.Health){
						HealthFromAttr += vb.amount;
						
					}
					else if(vb.vital == VitalName.Mana){
						ManaFromAttr += vb.amount;
					}
					else if(vb.vital == VitalName.Energy){
						EnergyFromAttr += vb.amount;
					}
				}
			}
			
		}
		
		foreach(Attribute att in _attributes){
			if(att.Name == "Strength")
			{
				att.EquipValue = StrEquipValue;
			}
			else if(att.Name == "Vitality")
			{
				att.EquipValue = VitEquipValue;
			}
			else if(att.Name == "Dexterity")
			{
				att.EquipValue = DexEquipValue;
			}
			else if(att.Name == "Intelligence")
			{
				att.EquipValue = IntEquipValue;
			}
			att.RecalculateValue();
		}

		ScaleStats();
	}
	
	private void ScaleStats(){
		//Scale Vitals based on Attributes
		foreach(Vital vit in _vitals){
			if(vit.Name == "Health")
			{
				HealthFromAttr += GetAttribute((int)AttributeName.Strength).AttributeValue * 10 +
								 GetAttribute((int)AttributeName.Vitality).AttributeValue * 50;
				
				vit.MaxValue = HealthFromAttr ;
			}
			else if(vit.Name == "Mana")
			{
				ManaFromAttr += GetAttribute((int)AttributeName.Intelligence).AttributeValue * 10;
				vit.MaxValue = ManaFromAttr ;
			}
			else if(vit.Name == "Energy")
			{
				EnergyFromAttr += GetAttribute((int)AttributeName.Dexterity).AttributeValue * 10;
				vit.MaxValue = EnergyFromAttr ;
			}
		}
		
		//Scale damage based off stat
		MaxDamage += GetAttribute((int)AttributeName.Strength).AttributeValue * 2;
		
		//Scale armour based off stat
		PlayerArmor += GetAttribute((int)AttributeName.Vitality).AttributeValue * 2;
		
		//Scale Crit chance based on level
		BaseCritChance = Level * 0.002f;
		
		//Recalc Movement Speed
		MoveSpeed = 1.0f + PlayerMoveSpeedBuff;
		
		//Recalc Crit Chance
		float wepCritChance = EquipedWeapon != null ? (EquipedWeapon as Weapon).CritChance : 0;
		CritChance = BaseCritChance + wepCritChance; 
	}
	
	private void RefreshVitals(){
		foreach(Vital v in Vitals){
			v.CurValue = v.MaxValue;
		}
	}
	
	public void Respawn(){
		//Load level if needed
		GameObject RespawnLocation = GameObject.FindGameObjectWithTag("Respawn");
		if(RespawnLocation != null) {
			gameObject.transform.position = RespawnLocation.transform.position;
		}
		else {
			Debug.Log ("No respawn gameObject found to use as respawn location.");
		}
		
		CharacterControl.useNewPosition = true;
		CharacterControl.newPosition = transform.position;
		GetVital((int)VitalName.Health).CurValue = (int)((GetVital((int)VitalName.Health).MaxValue)*.75);	//respawn with 10% health
		GetVital((int)VitalName.Mana).CurValue = (int)((GetVital((int)VitalName.Mana).MaxValue)*.75f);
		GetVital((int)VitalName.Energy).CurValue = (int)((GetVital((int)VitalName.Energy).MaxValue)*.75f);
		Alive = true;
	}
	
	
	public bool UseConsumable(int getVitalInt, int amount){
		if(canConsume){
			StartCoroutine(UsePot(getVitalInt, amount));
			return true;
		}
		
		return false;
	}
	
	IEnumerator UsePot(int getVitalInt, int amount){
		canConsume = false;
		GetVital(getVitalInt).CurValue += amount;
		yield return new WaitForSeconds(1.0f);
		canConsume = true;
	}
	
	//Exit Combat Vars
	public int secondsBeforeExitingCombat = 5;
	public int secondsWaited = 0;
	
	public void CheckForExitCombat(){
		if(!_waitingToExitCombat)
			InvokeRepeating("TryToExitCombat",1.0f,1.0f);
		else{
			CancelInvoke("TryToExitCombat");
			secondsWaited = 0;
			InvokeRepeating("TryToExitCombat",1.0f,1.0f);
		}
	}
	
	public void TryToExitCombat(){
		WaitingToExitCombat = true;
		secondsWaited += 1;
		if(secondsWaited > secondsBeforeExitingCombat){
			secondsWaited = 0;
			CancelInvoke("TryToExitCombat");
			playerState = PlayerState.Normal;
			WaitingToExitCombat = false;
		}
	}
	
	void GiveStuff(){
		AddItem(ItemGenerator.CreateItem("weapon","legendary",2));
		
		Item fireItem = ItemGenerator.CreateItem("weapon","legendary",2);
		Weapon fireWeapon = fireItem as Weapon;
		fireWeapon.DmgType = DamageType.Fire;
		fireWeapon.DmgValue = 100;
		fireWeapon.CritChance = 0.7f;
		fireWeapon.CritDamage = 4.0f;
		AddItem(fireWeapon);
		
		Item waterItem = ItemGenerator.CreateItem("weapon","legendary",2);
		Weapon waterWeapon = waterItem as Weapon;
		waterWeapon.DmgType = DamageType.Water;
		waterWeapon.DmgValue = 100;
		AddItem(waterWeapon);
		
		Item knockBackItem = ItemGenerator.CreateItem("weapon","legendary",2);
		Weapon knockBackWep = knockBackItem as Weapon;
		knockBackWep.Proc = ProcType.Knockback;
		knockBackWep.ProcModifier = 0.8f;
		AddItem(knockBackWep);
		
		Item stunItem = ItemGenerator.CreateItem("weapon","legendary",2);
		Weapon stunWep = stunItem as Weapon;
		stunWep.Proc = ProcType.Stun;
		stunWep.ProcModifier = 0.8f;
		AddItem(stunWep);
		
		Item slowItem = ItemGenerator.CreateItem("weapon","legendary",2);
		Weapon slowWep = slowItem as Weapon;
		slowWep.Proc = ProcType.Slow;
		slowWep.ProcModifier = 0.8f;
		AddItem(slowWep);
		
		Item poisonItem = ItemGenerator.CreateItem("weapon","legendary",2);
		Weapon poisonWep = poisonItem as Weapon;
		poisonWep.Proc = ProcType.Poison;
		poisonWep.ProcModifier = 0.8f;
		AddItem(poisonWep);
		
		AddItem(ItemGenerator.CreateItem("armor","legendary",2));
		AddItem(ItemGenerator.CreateItem("armor","legendary",2));
		AddItem(ItemGenerator.CreateItem("armor","legendary",2));
		
		AddItem(ItemGenerator.CreateItem("consum","legendary",25));
		AddItem(ItemGenerator.CreateItem("consum","legendary",25));
		AddItem(ItemGenerator.CreateItem("consum","legendary",25));
		AddItem(ItemGenerator.CreateItem("consum","legendary",25));
		AddItem(ItemGenerator.CreateItem("consum","legendary",25));
		AddItem(ItemGenerator.CreateItem("consum","legendary",25));
		AddItem(ItemGenerator.CreateItem("consum","legendary",25));
		AddItem(ItemGenerator.CreateItem("consum","legendary",25));
		AddItem(ItemGenerator.CreateItem("consum","legendary",25));
		AddItem(ItemGenerator.CreateItem("consum","legendary",25));
		AddItem(ItemGenerator.CreateItem("consum","legendary",25));
		
		AddItem(ItemGenerator.CreateItem("socket","legendary",10));
		AddItem(ItemGenerator.CreateItem("socket","legendary",10));
		AddItem(ItemGenerator.CreateItem("socket","legendary",10));
		AddItem(ItemGenerator.CreateItem("socket","legendary",10));

		ModifyGold(1000);
	}
}

