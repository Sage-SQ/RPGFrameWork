using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCQuest : MonoBehaviour {
	private PlayerNew player;
	
	public int QuestID;
	public string QuestName;
	public string QuestText;
	public string QuestNPCName;
	public int QuestExpReward;
	public int QuestGoldReward;
	public List<Item> QuestReward = new List<Item>();
	public bool isComplete;
	public bool isAccepted;
	public bool isFinished;
	public int RewardNum;
	public string rewardType;
	public RequirementsStatus requirementStatus;
	
	//Quest Requirements
	public int[] QuestIDReq;
	public int QuestLevelReq;
	
	//Quest Completion Steps
	public bool trackSteps;
	
	public string nameOfMob;
	public int numberToKill;
	public int numberKilled;
	public bool killDone;
	
	public string nameOfItem;
	public string nameOfMobThatDropsItem;
	public int numberToObtain;
	public int numberObtained;
	public bool itemDone;
	
	public EnableGameObject qego;
	public bool EnableQuestGOPerm;
	public bool talkingCompletesQuest;
	public string nameOfNPCtoTalkTo;
	public string[] npcResponse;
	public bool talkDone;
	
	//TEMP
	public string questTracker;
	
	// Use this for initialization
	void Start () {
		
		if(player == null){
			player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerNew>();
		}
		
		trackSteps = true;
		
		for (int i = 0; i < RewardNum; i++) {
			Item x = ItemGenerator.CreateItem(rewardType,"legendary",player.Level);
			QuestReward.Add(x);
		}
		QuestNPCName = GetComponent<NPCDialog>().NpcName;
		CheckRequirements();
	}
	
	public void OnEnable(){
		Messenger<string>.AddListener("MonsterKilled",AddMobsKilled);
	}
	
	public void OnDisable(){
		Messenger<string>.RemoveListener("MonsterKilled",AddMobsKilled);
	}
		
	
	void Update() {
		if(isAccepted && !isFinished){
				questTracker = "";
				if(numberToKill > 0){
					CheckForMobs();
					questTracker += ToolTipStyle.Break;
				}
				
				if(numberToObtain > 0){
					CheckForItems();
					questTracker += ToolTipStyle.Break;
				}
				
				if(talkingCompletesQuest){
					CheckForTalk();
				}
			
			CheckForCompletion();
		}
		else if(isFinished){
			string addS = numberToKill > 1 ? "s" : "";
			string txtDone = talkDone ? "Done" : "Not Done";
			questTracker = "";
			if(numberToKill > 0){
				questTracker = string.Format("{0}{1} Killed: {2} of {3}",nameOfMob,addS,numberKilled,numberToKill);
			}
			
			if(numberToObtain > 0){
				if(questTracker != "")
					questTracker += ToolTipStyle.Break;
				questTracker += string.Format("{0} Gained: {1} of {2}",nameOfItem,numberObtained,numberToObtain);
			}
			
			if(talkingCompletesQuest){
				if(questTracker != "")
					questTracker += ToolTipStyle.Break;
				questTracker += string.Format("Talked to {0} : {1}",nameOfNPCtoTalkTo,txtDone);
			}
		}
	}
	
	private void AddMobsKilled(string mobName){
		if(isAccepted && !isFinished){
			if(mobName.Contains (nameOfMob)){
				numberKilled += 1;
				if(numberKilled > numberToKill){
					numberKilled = numberToKill;
				}
			}
		}
	}
	
	private void CheckForMobs(){	
		if(numberKilled >= numberToKill)
			killDone = true;
		
		string addS = numberToKill > 1 ? "s" : "";
		questTracker = string.Format("{0}{1} Killed: {2} of {3}",nameOfMob,addS,numberKilled,numberToKill);
	}
	
	private void CheckForItems(){
		numberObtained = 0;
		
		for (int i = 0; i < player.Inventory.Count; i++) {
			if(player.Inventory[i].Name == nameOfItem){
				numberObtained = player.Inventory[i].CurStacks;
				if(player.Inventory[i].CurStacks > numberToObtain){
					player.Inventory[i].CurStacks = numberToObtain;
				}
			}
		}
		
		if(numberObtained >= numberToObtain){
			itemDone = true;
		}
		
		questTracker += string.Format("{0} Gained: {1} of {2}",nameOfItem,numberObtained,numberToObtain);
	}
	
	private void CheckForTalk(){
		string txtDone = talkDone ? "Done" : "Not Done";
		questTracker += string.Format("Talked to {0} : {1}",nameOfNPCtoTalkTo,txtDone);
	}
	
	private void CheckForCompletion(){
		bool completed = true;
		if(numberToKill > 0 && !killDone)
			completed = false;
		if(numberToObtain > 0 && !itemDone)
			completed = false;
		if(talkingCompletesQuest && !talkDone)
			completed = false;
		
		if(completed){
			isComplete = true;
		}
	}
		
	private int numberOfQuestsNeeded = 0;
	private int numberOfQuestsFound = 0;

	public void CheckRequirements() {
		
		numberOfQuestsNeeded = QuestIDReq.Length;
		
		//Check Level
		if(player.Level >= QuestLevelReq){
			requirementStatus = RequirementsStatus.Level;
		}
		
		
		foreach(int nqID in QuestIDReq){
			foreach(NPCQuest nq in player.questsComplete){
				if(nq.QuestID == nqID){
					numberOfQuestsFound = numberOfQuestsFound + 1;
				}
			}
		}
		
		if(numberOfQuestsFound >= numberOfQuestsNeeded){
			if(requirementStatus == RequirementsStatus.Level)
				requirementStatus = RequirementsStatus.All;
			else
				requirementStatus = RequirementsStatus.Quests;
		}
		
		numberOfQuestsFound = 0;
		
	}
	
	//FOR GUI
	
	public string QuestInfo(){
		string questInfo = ToolTipStyle.Blue + QuestName + ToolTipStyle.EndColor
			+ ToolTipStyle.Break + QuestNPCName;
		return questInfo;
	}
	
	public string QuestRewardString(){
		string questRewardString = "Exp: " + QuestExpReward.ToString() + " Gold: " + QuestGoldReward.ToString();
		return questRewardString;
	}
	
	public void CancelQuest(){
		talkDone = killDone = itemDone = isComplete = isAccepted = false;
		numberKilled = numberObtained = 0;
		Debug.Log("Quest Canceled");
		player.QuestsInProgress.Remove(this);
	}
	
	public void LoadFinishedQuest(){
		if(player == null){
			player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerNew>();
		}
		
		foreach(NPCQuest nq in player.questsComplete){
			if (nq.QuestID == this.QuestID) return;
		}
		
		player.questsComplete.Add(this);
	}
	
	public void LoadInProgressQuest(){
		if(player == null){
			player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerNew>();
		}
		
		if(player.QuestsInProgress.Count > 0){
			foreach(NPCQuest nq in player.QuestsInProgress){
				if (nq.QuestID == this.QuestID) return;
			}
		}
		
		player.QuestsInProgress.Add(this);
	}
}
	


public enum RequirementsStatus {
	None,
	Quests,
	Level,
	All
}