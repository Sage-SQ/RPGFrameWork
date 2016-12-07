using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NPCDialog : MonoBehaviour {
	public string Title;
	public string NpcName;
	public TextMesh npcNameLevel;
	public GameObject npcBorder;
	public bool isVendor;
	public bool isBlacksmith;
	public Light HighlightLight;
	public NPCVendor vendor;
	public NPCBlacksmith blacksmith;
	public GUISkin mySkin;
	private PlayerNew player;
	public bool _isNear;
	public string[] npcText;
	public string[] npcTextOrig;
	public int textNum;
	public string[] buttonText = new string[2];
	public int buttonTextNum;
	public List<NPCQuest> quests = new List<NPCQuest>();	//Quests
	public int OptionNumber = 0;
	
	
	public TextMesh hasQuestText;
	private float distToBeActive = 30;
	
	
	// Use this for initialization
	void Start () {
		
		if(player == null){
			player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerNew>();
		}
		npcTextOrig = npcText;
		textNum = 0;
		if(HighlightLight != null)
			HighlightLight.enabled = false;
		if(npcNameLevel != null)
			npcNameLevel.text = NpcName;
		
		vendor = GetComponent<NPCVendor>();
		if(vendor != null) isVendor = true; else isVendor = false;
		
		blacksmith = GetComponent<NPCBlacksmith>();
		if(blacksmith != null) isBlacksmith = true; else isBlacksmith = false;
		
		if(string.IsNullOrEmpty(buttonText[0]))
			buttonText[0] = "Next";
		
		if(string.IsNullOrEmpty(buttonText[1]))
			buttonText[0] = "Close";
	}
	
	void Update(){
		float viewDist = Vector3.Distance(player.transform.position, transform.position);
	
		if(NPCDialogGUI.openedNPCDialog != null && NPCDialogGUI.openedNPCDialog == this)
			if(viewDist > 7 || player.playerState != PlayerState.NPCDialog)
   				CloseDialog();
		
		var allQuests = GetComponents<NPCQuest>();
		if(allQuests.Length > 0) //if we have quests
		{
			CheckQuests();
			if(quests.Count > 0){
				//If has quests and one or more in progress, show grey !
				for (int i = 0; i < quests.Count; i++) {
					if(quests[i].isAccepted)
						hasQuestText.text = ToolTipStyle.Grey + ToolTipStyle.Bold +  "!" + ToolTipStyle.EndBold + ToolTipStyle.EndColor;
				}
				
				//If has quests but none accepted or completed, show yellow !
				for (int i = 0; i < quests.Count; i++) {
					if(!quests[i].isAccepted)
						hasQuestText.text = ToolTipStyle.Yellow + ToolTipStyle.Bold +  "!" + ToolTipStyle.EndBold + ToolTipStyle.EndColor;
				}

				//If has quests and one or more complete, show yellow ?
				for (int i = 0; i < quests.Count; i++) {
					if(quests[i].isComplete)
						hasQuestText.text = ToolTipStyle.Yellow + ToolTipStyle.Bold +  "?" + ToolTipStyle.EndBold + ToolTipStyle.EndColor;
				}
			}
			else if(isVendor){
				hasQuestText.text = "$";
			}
			else {
				hasQuestText.text = "";
			}
		}
		
		if(viewDist < distToBeActive){
			npcNameLevel.text = NpcName;
			npcBorder.SetActive(true);
		}else{
			npcNameLevel.text = "";
			npcBorder.SetActive(false);
			
			if(quests.Count > 0)
				hasQuestText.text = "";
		}
	}
	
	public void CompleteQuest(NPCQuest quest){
		if(player.Inventory.Count + quest.QuestReward.Count  <= player.MaxInventorySpace){
			
			for (int i = 0; i < quest.QuestReward.Count; i+=0) {
				bool addedItem = player.AddItem(quest.QuestReward[i]);
				if(addedItem){
					quest.QuestReward.RemoveAt(i);
				}
				else {
					Debug.Log ("Stack is full");
				}
			}
			
			if(quest.QuestReward.Count == 0){
				player.AddExp(quest.QuestExpReward);
				player.ModifyGold(quest.QuestGoldReward);
				quest.isFinished = true;
				player.questsComplete.Add(quest);
				
				if(quest.qego != null && !quest.EnableQuestGOPerm)
						quest.qego.DisableGameObjects();
				if(quest.numberToObtain > 0){
					for (int i = 0; i < player.Inventory.Count; i++) {
						if(player.Inventory[i].Name == quest.nameOfItem){
							player.Inventory[i].CurStacks -= quest.numberToObtain;
							if(player.Inventory[i].CurStacks <= 0)
								player.Inventory.RemoveAt(i);
						}
					}
				}
				
				player.QuestsInProgress.Remove(quest);
				EnterDialog();
			}
		}
		else {
			Debug.Log("Inventory is Full!");
		}
		
	}
	
	void CheckQuests(){
		
		npcText = npcTextOrig;
		var _quests = GetComponents<NPCQuest>();
		quests = new List<NPCQuest>();
		
		//Check this NPCs quests
		foreach(NPCQuest nq in _quests){
			nq.CheckRequirements();
			if(nq.talkingCompletesQuest && nq.isAccepted && NpcName.Contains(nq.nameOfNPCtoTalkTo)){
				nq.talkDone = true;
			}
			if(!nq.isFinished && nq.requirementStatus != RequirementsStatus.None && nq.requirementStatus != RequirementsStatus.Level)
				quests.Add(nq);
		}
		
		//Check player quests
		foreach(NPCQuest nq in player.QuestsInProgress ){
			nq.CheckRequirements();
		
			if(nq.talkingCompletesQuest && !nq.talkDone &&  NpcName.Contains(nq.nameOfNPCtoTalkTo)){
				npcText = nq.npcResponse;
				OptionNumber = 1;	//Force Conversation
			}
		}
	}
	
	public void EnterDialog(){
		if(player.playerState != PlayerState.Normal) return;
		
		player.playerState = PlayerState.NPCDialog;
		OptionNumber = 0;
		textNum = 0;
		CheckQuests();
		NPCDialogGUI.openedNPCDialog = this;
		Messenger.Broadcast ("DisplayNPCWindow");
	}
	
	public void CloseDialog(){
		player.playerState = PlayerState.Normal;
		Messenger.Broadcast ("CloseNPCWindow");
	}
	
	void OnTriggerEnter(Collider other){
//		if(other.CompareTag("Player") && other.GetType() != typeof(SphereCollider) && !_isNear){
//			EnterDialog();
//		}
		CheckQuests();
	}
	
	void OnTriggerExit(Collider other){
//		if(other.CompareTag("Player") && other.GetType() != typeof(SphereCollider)){
//			CloseDialog();
//		}
	}
	
	void OnMouseDown () {
		float viewDist = Vector3.Distance(player.transform.position, transform.position);
		if(viewDist < 7)
   			EnterDialog();
	}
	
	void OnMouseEnter () {
		if(HighlightLight != null)
		HighlightLight.enabled = true;
	}
	void OnMouseExit () {
		if(HighlightLight != null)
		HighlightLight.enabled = false;
	}
}

public enum NpcType {
	NPC,
	Vendor,
	Bank
}