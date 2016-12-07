using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerBase : MonoBehaviour {
	
	public string _name;
	public int _level;
	
	public PlayerClass playerClass;
	public PlayerState playerState;
	
	public int _exp;
	public int _expToLevel;					//Exp to level
	private float _levelModifier;			//Modifier applied to exp for levels
	private bool _alive;
	private bool _canFight;
	public bool _waitingToExitCombat;
	
	public Attribute[] _attributes;
	public Vital[] _vitals;
	
//	private int craftingLevel;
//	private int miningLevel;
//	private int woodcuttingLevel;
//	private int gatheringLevel;
	
	
	private int healthFromAttr = 0 ;
	public int HealthFromAttr {
		get{return healthFromAttr;}
		set{healthFromAttr = value;}
	}
		
	private int manaFromAttr = 0;
	public int ManaFromAttr {
		get{return manaFromAttr;}
		set{manaFromAttr = value;}
	}
		
	private int energyFromAttr = 0;
	public int EnergyFromAttr {
		get{return energyFromAttr;}
		set{energyFromAttr = value;}
	}
	
	private int strEquipValue = 0;
	public int StrEquipValue {
		get{return strEquipValue;}
		set{strEquipValue = value;}
	}
	
	private int dexEquipValue = 0;
	public int DexEquipValue {
		get{return dexEquipValue;}
		set{dexEquipValue = value;}
	}
	
	private int intEquipValue = 0;
	public int IntEquipValue {
		get{return intEquipValue;}
		set{intEquipValue = value;}
	}
	
	private int vitEquipValue = 0;
	public int VitEquipValue {
		get{return vitEquipValue;}
		set{vitEquipValue = value;}
	}
	
	private GlobalPrefabs globalPrefabs;
	
	public void Awake(){
		_name = "PROLogicX";
		_level = 1;
		
		playerClass = PlayerClass.Warrior;
		playerState = PlayerState.Normal;
		
		_exp = 0;
		_levelModifier = 1.05f;
		_expToLevel = 100;
		_alive = true;		
		_waitingToExitCombat = false;
		_canFight = true;
		_attributes = new Attribute[Enum.GetValues (typeof(AttributeName)).Length];
		_vitals = new Vital[Enum.GetValues (typeof(VitalName)).Length];
		
		
		
		if(globalPrefabs == null){
			globalPrefabs = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GlobalPrefabs>();
		}
		
		SetupStats ();
		SetupVitals();
	}

	#region "Info + Getters Setters"
	
	//General
	private int playerGold;
	public int Gold{
		get{return playerGold;}
		set{playerGold = value;}
	}
	//Need: Playtime, Character Class
		
	
	//Offensive Stats
	
	private int _attackRange = 3; //scales off attr
	public int AttackRange {
		get{return _attackRange;}
		set{_attackRange = value;}
	}
	
	private int _maxDamage = 1; //scales off attr
	public int MaxDamage {
		get{return _maxDamage;}
		set{_maxDamage = value;}
	}
	
	private float _dmgVariance = 0.1f;
	public float DmgVariance {
		get{return _dmgVariance;}
		set{_dmgVariance = value;}
	}
	
	private float _attackSpeed = 1;
	public float AttackSpeed {
		get{return _attackSpeed;}
		set{_attackSpeed = value;}
	}
	
	private float baseCritChance = 0.01f; //scales off level
	public float BaseCritChance {
		get{return baseCritChance;}
		set{baseCritChance = value;}
	}
	
	private float _critChance = 0; 
	public float CritChance {
		get{return _critChance;}
		set{_critChance = value;}
	}
	
	private float _critDamage = 2.0f;
	public float CritDamage {
		get{return _critDamage;}
		set{_critDamage = value;}
	}
	
	public string GetDPSString()
	{
		return (((int)(MaxDamage*DmgVariance)).ToString()+ " - " + 
			((int)(MaxDamage)).ToString()
		);
	}
	
	
	//Defensive Stats
	
	private int _playerArmor = 0;
	public int PlayerArmor {
		get{return _playerArmor;}
		set{_playerArmor = value;}
	}
	
	private float _playerChanceToBlock = 0;
	public float PlayerChanceToBlock {
		get{return _playerChanceToBlock;}
		set{_playerChanceToBlock = value;}
	}
	
	private int _playerDamageBlocked = 0;
	public int PlayerDamageBlocked {
		get{return _playerDamageBlocked;}
		set{_playerDamageBlocked = value;}
	}
	
	private float _playerMoveSpeedBuff = 0;
	public float PlayerMoveSpeedBuff {
		get{return _playerMoveSpeedBuff;}
		set{_playerMoveSpeedBuff = value;}
	}
	
	//Misc Stats
	private float _movementSpeed = 1.0f;
	public float MoveSpeed {
		get{return _movementSpeed;}
		set{_movementSpeed = value;}
	}
	
	#endregion
	
	public string Name{
		get{return _name;}
		set{_name=value;}
	}
	
	public int Level{
		get{return _level;}
		set{_level=value;}
	}
	
	public int Exp{
		get{return _exp;}
		set{_exp=value;}
	}
	
	public void AddExp(int exp){
		_exp += exp;
		StartCoroutine("UpdateLevel");
	}
	
	public bool Alive{
		get{return _alive;}
		set{_alive = value;}
	}
		
	public bool CanFight{
		get{return _canFight;}
		set{_canFight = value;}
	}
	
	public bool WaitingToExitCombat{
		get{return _waitingToExitCombat;}
		set{_waitingToExitCombat = value;}
	}
	
	public int ExpToLevel {
		get{ return _expToLevel;}
		set{_expToLevel = value;}
	}

	public float LevelModifier {
		get{ return _levelModifier;}
		set{_levelModifier = value;}
	}
	
	private int CalculateExpToLevel(){
		return (int)(_expToLevel*_levelModifier);
	}
	
	
	IEnumerator UpdateLevel(){
		if(_exp >= _expToLevel){
			LevelUp();
		}
		else {
			StopCoroutine("UpdateLevel");
		}
		yield return new WaitForSeconds(0.0f);
		StartCoroutine(UpdateLevel());
	}
	
	public void LevelUp(){
		_exp -= _expToLevel;
		_expToLevel= CalculateExpToLevel ();
		_level++;
		IncreaseAttributes();
		StartCoroutine("LevelUpAnimation");
	}
	
	void IncreaseAttributes(){
		
		if(playerClass == PlayerClass.Warrior){
			GetAttribute((int)AttributeName.Strength).BaseValue += 12;
			GetAttribute((int)AttributeName.Dexterity).BaseValue += 3;
			GetAttribute((int)AttributeName.Intelligence).BaseValue += 2;
			GetAttribute((int)AttributeName.Vitality).BaseValue += 8;
		}
		
		SendMessage("UpdateStats");
	}
	
	
	
	IEnumerator LevelUpAnimation(){
		GameObject leveluptext = (GameObject)Instantiate(globalPrefabs.floatingText);
		leveluptext.transform.parent = this.transform;
		leveluptext.GetComponent<FloatingTextGUI>().SetInfo("LEVEL UP!!",transform.position);
		yield return new WaitForSeconds(1.5f);
	}
	
	private void SetupStats(){
		for(int cnt =0; cnt < _attributes.Length;cnt++){
			_attributes[cnt] = new Attribute();
			_attributes[cnt].Name=((AttributeName)cnt).ToString ();
		}
		
	}
	
	private void SetupVitals(){
		for(int cnt =0; cnt < _vitals.Length;cnt++){
			_vitals[cnt] = new Vital();
			_vitals[cnt].Name = ((VitalName)cnt).ToString ();
		}
	}
	
	public Attribute[] Attributes {
		get {return _attributes;}
	}
	
	public Vital[] Vitals {
		get{return _vitals;}
	}

	public Attribute GetAttribute(int index){
		return _attributes[index];
	}
	
	
	public Vital GetVital(int index){
		return _vitals[index];
	}
	
	#region "Items Equipped: vars and GetsAndSets"
	private Item _equipedWeapon;
	private Item _equipedArmorHead;
	private Item _equipedArmorChest;
	private Item _equipedArmorGloves;
	private Item _equipedArmorLegs;
	private Item _equipedArmorFeet;
	private Item _equipedArmorBack;

	//Equipped Weapon
	public Item EquipedWeapon {
		get{return _equipedWeapon as Weapon;}
		set{_equipedWeapon = value;}
	}
	
	//Equipped Armor - Head
	public Item EquipedArmorHead {
		get{return _equipedArmorHead;}
		set{_equipedArmorHead = value;}
	}
	//Equipped Armor - Chest
	public Item EquipedArmorChest {
		get{return _equipedArmorChest;}
		set{_equipedArmorChest = value;}
	}
	//Equipped Armor - Gloves
	public Item EquipedArmorGloves {
		get{return _equipedArmorGloves;}
		set{_equipedArmorGloves = value;}
	}
	
	//Equipped Armor - Legs
	public Item EquipedArmorLegs {
		get{return _equipedArmorLegs;}
		set{_equipedArmorLegs = value;}
	}
	//Equipped Armor - Feet
	public Item EquipedArmorFeet {
		get{return _equipedArmorFeet;}
		set{_equipedArmorFeet = value;}
	}
	//Equipped Armor - Back
	public Item EquipedArmorBack {
		get{return _equipedArmorBack;}
		set{_equipedArmorBack = value;}
	}
	#endregion
	
	#region "Skills"
	#endregion
}

public enum PlayerState {
	Normal,
	Combat,
	NPCDialog,
	Stash,
	Harvesting,
	Cutscene
}

public enum PlayerClass {
	Warrior
}