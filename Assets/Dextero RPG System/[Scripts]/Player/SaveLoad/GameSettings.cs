using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections;
//You must include these namespaces
//to use BinaryFormatter
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class GameSettings : MonoBehaviour {
	
	private GameObject pc;
	private PlayerNew playerNew;
	public AchievementHandler achievementHandler;
	public string SaveLoadSlot;
	
	void Awake(){
		DontDestroyOnLoad (this);
	}
	
	// Use this for initialization
	void Start () {
		pc = GameObject.FindGameObjectWithTag ("Player");
		
		if(pc ==null){
			pc = GameObject.FindGameObjectWithTag ("Player");
		}
		
		if(playerNew == null){
			playerNew = pc.GetComponent<PlayerNew>();
		}
		
		if(achievementHandler == null){
			achievementHandler = GameObject.FindGameObjectWithTag ("AchievementHandler").GetComponent<AchievementHandler>();
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void SaveCharacterData(){
		
		if(PlayerPrefs.GetInt("Player_SaveLoad_Slot",1000) == 1000){
			Debug.LogError ("No saveload slot");
			PlayerPrefs.SetInt("Player_SaveLoad_Slot",0);
			return;
		}	
		
		SaveLoadSlot = PlayerPrefs.GetInt("Player_SaveLoad_Slot",1000).ToString() + "_";
		Debug.Log ("Creating new game at save: " + SaveLoadSlot);
		
		
		if(!playerNew.Alive){
			Debug.Log("Can't save when dead");
			return;
		}
		
		//Set the slot to inUse
		PlayerPrefsX.SetBool (SaveLoadSlot+"inUse",true);
		//PlayerPrefs.DeleteAll (); //Needs to be deleted when finalised save and load
	
		PlayerPrefs.SetString (SaveLoadSlot+"Player_Name",playerNew.Name);
		PlayerPrefs.SetInt(SaveLoadSlot+"Player_Class",(int)playerNew.playerClass);
		
		for(int cnt=0;cnt<Enum.GetValues(typeof(AttributeName)).Length;cnt++){
			PlayerPrefs.SetInt (SaveLoadSlot+"Player_Atr_"+((AttributeName)cnt).ToString ()+"_Value",playerNew.GetAttribute(cnt).BaseValue);		
		}
		
		for(int cnt=0;cnt<Enum.GetValues(typeof(VitalName)).Length;cnt++){
			PlayerPrefs.SetInt (SaveLoadSlot+"Player_Vital_"+((VitalName)cnt).ToString ()+"_Value",playerNew.GetVital(cnt).MaxValue);
			PlayerPrefs.SetInt (SaveLoadSlot+"Player_Vital_"+((VitalName)cnt).ToString ()+"_CurValue",playerNew.GetVital(cnt).CurValue);
		}
		
		PlayerPrefs.SetInt (SaveLoadSlot+"Player_Exp",playerNew.Exp);
		PlayerPrefs.SetInt (SaveLoadSlot+"Player_ExpToLevel",playerNew.ExpToLevel);
		PlayerPrefs.SetInt (SaveLoadSlot+"Player_Level",playerNew.Level);
		
		PlayerPrefs.SetInt (SaveLoadSlot+"Player_Gold",playerNew.Gold);
		
		SaveQuestsAndAchievements();
		SaveAllItems();
		
		//When done do a reload for debugging purposes
		//LoadCharacterData();
	}
	
	public void LoadCharacterData(){
		if(playerNew == null){
			playerNew = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerNew>();
		}
		
		if(PlayerPrefs.GetInt("Player_SaveLoad_Slot",1000) == 1000){
			Debug.LogError ("No saveload slot");
			PlayerPrefs.SetInt("Player_SaveLoad_Slot",0);
			return;
		}	
		
		SaveLoadSlot = PlayerPrefs.GetInt("Player_SaveLoad_Slot",1000).ToString() + "_";
		Debug.Log ("Loading save: " + PlayerPrefs.GetInt("Player_SaveLoad_Slot",1000).ToString());
		//Load Name
		playerNew.Name = PlayerPrefs.GetString(SaveLoadSlot+"Player_Name","");
		playerNew.playerClass = (PlayerClass)PlayerPrefs.GetInt(SaveLoadSlot+"Player_Class",0);
		
		//Load Attribute
		for(int cnt=0;cnt<Enum.GetValues(typeof(AttributeName)).Length;cnt++){
			playerNew.GetAttribute(cnt).BaseValue = PlayerPrefs.GetInt (SaveLoadSlot+"Player_Atr_"+((AttributeName)cnt)+"_Value",0);
		}
		//Load Vitals
		
		for(int cnt=0;cnt<Enum.GetValues(typeof(VitalName)).Length;cnt++){
			playerNew.GetVital(cnt).MaxValue = PlayerPrefs.GetInt (SaveLoadSlot+"Player_Vital_"+((VitalName)cnt).ToString ()+"_Value",100);
			playerNew.GetVital(cnt).CurValue = PlayerPrefs.GetInt (SaveLoadSlot+"Player_Vital_"+((VitalName)cnt).ToString ()+"_CurValue",100);
		}
		
		playerNew.Exp = PlayerPrefs.GetInt (SaveLoadSlot+"Player_Exp",0);
		playerNew.ExpToLevel  = PlayerPrefs.GetInt (SaveLoadSlot+"Player_ExpToLevel",100);
		playerNew.Level = PlayerPrefs.GetInt (SaveLoadSlot+"Player_Level",1);
		
		playerNew.Gold = PlayerPrefs.GetInt (SaveLoadSlot+"Player_Gold",0);
		
		LoadQuestsAndAchievements();
		LoadAllItems();
		playerNew.UpdateStats();
	}
	
	//Save and Load Quests and Achievements
	void SaveQuestsAndAchievements(){
		
		SaveQuests();
		
		if(achievementHandler != null){
			var b = new BinaryFormatter();
		   	var m = new MemoryStream();
		    b.Serialize(m, achievementHandler.achievements);
		    PlayerPrefs.SetString(SaveLoadSlot+"Player_AchievementHandler", 
		        Convert.ToBase64String(
		            m.GetBuffer()
		        )
		    );
		}
		
	}
	
	void SaveQuests(){
		string finishedQuestsString = "";
		string seperator = "&";
		string mainSeperator = "|";
		//Save Finished Quests
		bool isFirst = true;
		foreach(NPCQuest nq in playerNew.questsComplete){
			
			string temp = "";
			if(!isFirst){
				temp += mainSeperator;
			}
			else {
				isFirst = false;
			}
			temp += nq.QuestID + seperator;
			temp += nq.isAccepted.ToString() + seperator;
			temp += nq.isComplete.ToString() + seperator;
			temp += nq.isFinished.ToString() + seperator;
			temp += FindQuestInfoOnType(nq);
			
			
			finishedQuestsString += temp;
		}
		
		string inProgressQuestsString = "";
		
		//Save QuestsInProgress
		isFirst = true;
		foreach(NPCQuest nq in playerNew.QuestsInProgress){
			string temp = "";
			if(!isFirst){
				temp += mainSeperator;
			}
			else {
				isFirst = false;
			}
			
			temp += nq.QuestID + seperator;
			temp += nq.isAccepted.ToString() + seperator;
			temp += nq.isComplete.ToString() + seperator;
			temp += nq.isFinished.ToString() + seperator;
			temp += FindQuestInfoOnType(nq);
			
			
			inProgressQuestsString += temp;
		}
		
		PlayerPrefs.SetString(SaveLoadSlot+"Player_QuestsInProgress",inProgressQuestsString);
		PlayerPrefs.SetString(SaveLoadSlot+"Player_QuestsCompleted",finishedQuestsString);
	}
	
	string FindQuestType(NPCQuest nq){
		string temp = "";
		if(nq.talkingCompletesQuest)
			temp += "Talk";
		if(nq.numberToKill > 0)
			temp += "Kill";	
		if(nq.numberToObtain > 0)
			temp += "Loot";
		
		return temp;
	}
	
	string FindQuestInfoOnType(NPCQuest nq){
		string seperator = "&";
		string temp = "";
		
		string type = FindQuestType(nq);
		
		if(type == "Talk"){
			temp += nq.talkDone.ToString();
		}
		else if(type == "Kill"){
			temp += nq.killDone.ToString() + seperator;
			temp += nq.numberKilled.ToString();
		}
		else if(type == "Loot"){
			temp += nq.itemDone.ToString() + seperator;
			temp += nq.numberObtained.ToString();
		}
		else if(type == "TalkKill"){
			temp += nq.talkDone.ToString() + seperator;
			temp += nq.killDone.ToString() + seperator;
			temp += nq.numberKilled.ToString();
		}
		else if(type == "TalkLoot"){
			temp += nq.talkDone.ToString() + seperator;
			temp += nq.itemDone.ToString() + seperator;
			temp += nq.numberObtained.ToString();
		}
		else if(type == "KillLoot"){
			temp += nq.killDone.ToString() + seperator;
			temp += nq.itemDone.ToString() + seperator;
			temp += nq.numberKilled.ToString() + seperator;
			temp += nq.numberObtained.ToString();
		}
		else if(type == "TalkKillLoot"){
			temp += nq.talkDone + seperator;
			temp += nq.killDone.ToString() + seperator;
			temp += nq.itemDone.ToString() + seperator;
			temp += nq.numberKilled.ToString() + seperator;
			temp += nq.numberObtained.ToString();
		}
		
		return temp;
	}
	
	void LoadQuestsAndAchievements(){

		LoadQuests();
		
		var data = PlayerPrefs.GetString(SaveLoadSlot+"Player_AchievementHandler");
	    //If not blank then load it
	    if(!String.IsNullOrEmpty(data))
	    {
	        //Binary formatter for loading back
	        var b = new BinaryFormatter();
	        //Create a memory stream with the data
		    var m = new MemoryStream(Convert.FromBase64String(data));
		    //Load back the scores
			achievementHandler.achievements = new List<Achievement>();
		    achievementHandler.achievements = b.Deserialize(m) as List<Achievement>;
		}
		
		if(achievementHandler != null) achievementHandler.RefreshAchievements();
	}
	
	void LoadQuests(){
		
		playerNew.QuestsInProgress = new List<NPCQuest>();
		playerNew.questsComplete = new List<NPCQuest>();
		
		string finishedQuestsString = PlayerPrefs.GetString(SaveLoadSlot+"Player_QuestsCompleted","");
		string inProgressQuestsString = PlayerPrefs.GetString(SaveLoadSlot+"Player_QuestsInProgress","");

		GameObject[] npcsWithQuests = GameObject.FindGameObjectsWithTag("QuestNPC");
		if(!String.IsNullOrEmpty(finishedQuestsString)){
			string[] finishedQuests = finishedQuestsString.Split ('|');
			foreach (string s in finishedQuests) {
				string[] questDetails = s.Split('&');
		
				int questDetailsID = Int32.Parse (questDetails[0]);
				foreach(GameObject go in npcsWithQuests){
					NPCQuest[] quests = go.GetComponents<NPCQuest>();
					foreach(NPCQuest nq in quests){
						if(nq.QuestID == questDetailsID){
							LoadFinishedQuest(nq,questDetails);
						}
					}
				}
			}
		}
		
		if(!String.IsNullOrEmpty(inProgressQuestsString)){
			string[] inProgressQuests = inProgressQuestsString.Split ('|');
			foreach (string s in inProgressQuests) {
				string[] questDetails = s.Split('&');
				int questDetailsID = Int32.Parse (questDetails[0]);
				
				foreach(GameObject go in npcsWithQuests){
					NPCQuest[] quests = go.GetComponents<NPCQuest>();
					foreach(NPCQuest nq in quests){
						if(nq.QuestID == questDetailsID){
							ApplyQuestInfoByType(nq,questDetails);
						}
					}
				}
			}
		}
	}
	void LoadFinishedQuest(NPCQuest nq, string[] questDetails){
		nq.isAccepted = nq.isComplete = nq.isFinished = true;
		nq.talkDone = nq.itemDone = nq.killDone = true;
		nq.numberKilled = nq.numberToKill;
		nq.numberObtained = nq.numberToObtain;
		
		nq.LoadFinishedQuest();
	}
	/*
	 * 0 - ID
	 * 1 - Accepted
	 * 2 - Completed
	 * 3 - Finished
	 * 4 - TalkDone
	 * 5 - KillDone
	 * 6 - ItemDone
	 * 7 - KillNum
	 * 8 - LootNum
	 * */
	
	void ApplyQuestInfoByType(NPCQuest nq, string[] questDetails){
		string type = FindQuestType(nq);
		if(type == "Talk"){
			nq.isAccepted = true;
			nq.isComplete = TrueOrFalse(questDetails[2]);
			nq.isFinished = false;
			nq.talkDone = TrueOrFalse(questDetails[4]);
			nq.LoadInProgressQuest();
		}
		else if(type == "Kill"){
			nq.isAccepted = true;
			nq.isComplete = TrueOrFalse(questDetails[2]);
			nq.isFinished = false;
			nq.killDone = TrueOrFalse(questDetails[4]);
			nq.numberKilled = Int32.Parse(questDetails[5]);
			nq.LoadInProgressQuest();
		}
		else if(type == "Loot"){
			nq.isAccepted = true;
			nq.isComplete = TrueOrFalse(questDetails[2]);
			nq.isFinished = false;
			nq.itemDone = TrueOrFalse(questDetails[4]);
			nq.LoadInProgressQuest();
		}
		else if(type == "TalkKill"){
			nq.isAccepted = true;
			nq.isComplete = TrueOrFalse(questDetails[2]);
			nq.isFinished = false;
			nq.talkDone = TrueOrFalse(questDetails[4]);
			nq.killDone = TrueOrFalse(questDetails[5]);
			nq.numberKilled = Int32.Parse(questDetails[6]);
			nq.LoadInProgressQuest();
		}
		else if(type == "TalkLoot"){
			nq.isAccepted = true;
			nq.isComplete = TrueOrFalse(questDetails[2]);
			nq.isFinished = false;
			nq.talkDone = TrueOrFalse(questDetails[4]);
			nq.itemDone = TrueOrFalse(questDetails[5]);
			nq.numberObtained = Int32.Parse (questDetails[6]);
			nq.LoadInProgressQuest();
		}
		else if(type == "KillLoot"){
			nq.isAccepted = true;
			nq.isComplete = TrueOrFalse(questDetails[2]);
			nq.isFinished = false;
			nq.killDone = TrueOrFalse(questDetails[4]);
			nq.itemDone = TrueOrFalse(questDetails[5]);
			nq.numberKilled = Int32.Parse (questDetails[6]);
			nq.numberObtained = Int32.Parse (questDetails[7]);
			nq.LoadInProgressQuest();
		}
		else if(type == "TalkKillLoot"){
			nq.isAccepted = true;
			nq.isComplete = TrueOrFalse(questDetails[2]);
			nq.isFinished = false;
			nq.talkDone = TrueOrFalse(questDetails[4]);
			nq.killDone = TrueOrFalse(questDetails[5]);
			nq.itemDone = TrueOrFalse(questDetails[6]);
			nq.numberKilled = Int32.Parse(questDetails[7]);
			nq.numberObtained = Int32.Parse(questDetails[8]);
			nq.LoadInProgressQuest();
		}
		else {
			Debug.Log ("wtf");
		}
	}
	
	bool TrueOrFalse(string s){
		return bool.Parse (s);
	}
	
	//Save and Load Items  - Inventory (Done) - EquippedItems (NotDone) - Bank (NotDoneOrImplemented)	
	void SaveAllItems(){
		//Get a binary formatter
	    var b = new BinaryFormatter();
	    //Create an in memory stream
	    var m = new MemoryStream();
	    //Save the scores
	    b.Serialize(m, playerNew.Inventory);
	    //Add it to player prefs
	    PlayerPrefs.SetString(SaveLoadSlot+"Player_InventoryItems", 
	        Convert.ToBase64String(
	            m.GetBuffer()
	        )
	    );
		
		b = new BinaryFormatter();
		m = new MemoryStream();
		b.Serialize(m, playerNew.Stash);
		PlayerPrefs.SetString(SaveLoadSlot+"Player_StashItems", 
	        Convert.ToBase64String(
	            m.GetBuffer()
	        )
	    );
		
		
		if(playerNew.EquipedWeapon != null){
			b = new BinaryFormatter();
			m = new MemoryStream();
			b.Serialize(m, playerNew.EquipedWeapon);
			PlayerPrefs.SetString(SaveLoadSlot+"Player_EquipedWeapon", 
		        Convert.ToBase64String(
		            m.GetBuffer()
		        )
		    );
		}
		else {
			PlayerPrefs.SetString(SaveLoadSlot+"Player_EquipedWeapon","");
		}
		
		if(playerNew.EquipedArmorBack != null){
			b = new BinaryFormatter();
			m = new MemoryStream();
			b.Serialize(m, playerNew.EquipedArmorBack);
			PlayerPrefs.SetString(SaveLoadSlot+"Player_EquipedArmorBack", 
		        Convert.ToBase64String(
		            m.GetBuffer()
		        )
		    );
		}
		else {
			PlayerPrefs.SetString(SaveLoadSlot+"Player_EquipedArmorBack","");
		}
		
		if(playerNew.EquipedArmorChest != null){
			b = new BinaryFormatter();
			m = new MemoryStream();
			b.Serialize(m, playerNew.EquipedArmorChest);
			PlayerPrefs.SetString(SaveLoadSlot+"Player_EquipedArmorChest", 
		        Convert.ToBase64String(
		            m.GetBuffer()
		        )
		    );
		}
		else {
			PlayerPrefs.SetString(SaveLoadSlot+"Player_EquipedArmorChest","");
		}	
		
		if(playerNew.EquipedArmorFeet != null){
			b = new BinaryFormatter();
			m = new MemoryStream();
			b.Serialize(m, playerNew.EquipedArmorFeet);
			PlayerPrefs.SetString(SaveLoadSlot+"Player_EquipedArmorFeet", 
		        Convert.ToBase64String(
		            m.GetBuffer()
		        )
		    );
		}
		else {
			PlayerPrefs.SetString(SaveLoadSlot+"Player_EquipedArmorFeet","");
		}	
		
						
		if(playerNew.EquipedArmorGloves != null){
			b = new BinaryFormatter();
			m = new MemoryStream();
			b.Serialize(m, playerNew.EquipedArmorGloves);
			PlayerPrefs.SetString(SaveLoadSlot+"Player_EquipedArmorGloves", 
		        Convert.ToBase64String(
		            m.GetBuffer()
		        )
		    );
		}
		else {
			PlayerPrefs.SetString(SaveLoadSlot+"Player_EquipedArmorGloves","");
		}	
		
		if(playerNew.EquipedArmorHead != null){
			b = new BinaryFormatter();
			m = new MemoryStream();
			b.Serialize(m, playerNew.EquipedArmorHead);
			PlayerPrefs.SetString(SaveLoadSlot+"Player_EquipedArmorHead", 
		        Convert.ToBase64String(
		            m.GetBuffer()
		        )
		    );
		}		
		else {
			PlayerPrefs.SetString(SaveLoadSlot+"Player_EquipedArmorHead","");
		}	
		
		if(playerNew.EquipedArmorLegs != null){
			b = new BinaryFormatter();
			m = new MemoryStream();
			b.Serialize(m, playerNew.EquipedArmorLegs);
			PlayerPrefs.SetString(SaveLoadSlot+"Player_EquipedArmorLegs", 
		        Convert.ToBase64String(
		            m.GetBuffer()
		        )
		    );
		}
		else {
			PlayerPrefs.SetString(SaveLoadSlot+"Player_EquipedArmorLegs","");
		}	
		
	}
	
	void LoadAllItems(){
		
		playerNew.Inventory = new List<Item>();
		playerNew.Stash = new List<Item>();
		
		//Get the data
	    var data = PlayerPrefs.GetString(SaveLoadSlot+"Player_InventoryItems");
	    //If not blank then load it
	    if(!String.IsNullOrEmpty(data))
	    {
	        //Binary formatter for loading back
	        var b = new BinaryFormatter();
	        //Create a memory stream with the data
		    var m = new MemoryStream(Convert.FromBase64String(data));
		    //Load back the scores
		    playerNew.Inventory = b.Deserialize(m) as List<Item>;
		}
		
		//Player_StashItems
		
		data = PlayerPrefs.GetString(SaveLoadSlot+"Player_StashItems");
		if(!String.IsNullOrEmpty(data))
	    {
	        var b = new BinaryFormatter();
		    var m = new MemoryStream(Convert.FromBase64String(data));
		    playerNew.Stash = b.Deserialize(m) as List<Item>;
		}
		
		data = PlayerPrefs.GetString(SaveLoadSlot+"Player_EquipedWeapon","");
	    if(!String.IsNullOrEmpty(data))
	    {
	        var b = new BinaryFormatter();
		    var m = new MemoryStream(Convert.FromBase64String(data));
		    playerNew.EquipedWeapon = b.Deserialize(m) as Weapon;
		}
		
		data = PlayerPrefs.GetString(SaveLoadSlot+"Player_EquipedArmorBack");
	    if(!String.IsNullOrEmpty(data))
	    {
	        var b = new BinaryFormatter();
		    var m = new MemoryStream(Convert.FromBase64String(data));
		    playerNew.EquipedArmorBack = b.Deserialize(m) as Armor;
		}
		
		data = PlayerPrefs.GetString(SaveLoadSlot+"Player_EquipedArmorChest");
	    if(!String.IsNullOrEmpty(data))
	    {
	        var b = new BinaryFormatter();
		    var m = new MemoryStream(Convert.FromBase64String(data));
		    playerNew.EquipedArmorChest = b.Deserialize(m) as Armor;
		}
		
		data = PlayerPrefs.GetString(SaveLoadSlot+"Player_EquipedArmorFeet");
	    if(!String.IsNullOrEmpty(data))
	    {
	        var b = new BinaryFormatter();
		    var m = new MemoryStream(Convert.FromBase64String(data));
		    playerNew.EquipedArmorFeet = b.Deserialize(m) as Armor;
		}
		
		data = PlayerPrefs.GetString(SaveLoadSlot+"Player_EquipedArmorGloves");
	    if(!String.IsNullOrEmpty(data))
	    {
	        var b = new BinaryFormatter();
		    var m = new MemoryStream(Convert.FromBase64String(data));
		    playerNew.EquipedArmorGloves = b.Deserialize(m) as Armor;
		}
		
		data = PlayerPrefs.GetString(SaveLoadSlot+"Player_EquipedArmorHead");
	    if(!String.IsNullOrEmpty(data))
	    {
	        var b = new BinaryFormatter();
		    var m = new MemoryStream(Convert.FromBase64String(data));
		    playerNew.EquipedArmorHead = b.Deserialize(m) as Armor;
		}
		
		data = PlayerPrefs.GetString(SaveLoadSlot+"Player_EquipedArmorLegs");
	    if(!String.IsNullOrEmpty(data))
	    {
	        var b = new BinaryFormatter();
		    var m = new MemoryStream(Convert.FromBase64String(data));
		    playerNew.EquipedArmorLegs = b.Deserialize(m) as Armor;
		}
		
		
		
		//Reload icons
		List<Item> AllItems = new List<Item>();
		AllItems.AddRange (playerNew.AllEquippedItems());
		AllItems.AddRange (playerNew.Inventory);
		AllItems.AddRange (playerNew.Stash);
		foreach(Item i in AllItems){
			if(i != null){
				i.Icon = Resources.Load (i.IconPath) as Texture2D;
			}
			BuffItem b = i as BuffItem;
			if(b != null){
				foreach(SocketItem s in b.EquippedSockets){
					s.Icon = Resources.Load (s.IconPath) as Texture2D;
				}
			}
		}
	}
}
