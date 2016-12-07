using UnityEngine;
using System.Collections;

public class NPCDialogGUI : MonoBehaviour {
	
	public MyGUI GUIHandler;
	public PlayerNew _pc;
	public GUISkin mySkin;
	
	// NPCDialogSpecific
	public bool _displayNPCWindow = false;
	private const int NPC_WINDOW_ID = 6;
	private Rect _npcWindowRect = new Rect(500,25,300,300);
	public static NPCDialog openedNPCDialog;
	public static NPCBlacksmith blackSmith = null;
	public static Item selectedItem = null;
	
	void Start(){
		GUIHandler = GameObject.FindGameObjectWithTag("GUIHandler").GetComponent<MyGUI>();
		_pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerNew>();
		_npcWindowRect = new Rect((Screen.width -5) - 400 ,5,400,400);
	}
	
	void OnGUI(){
		GUI.skin = mySkin;
		
		if(_displayNPCWindow){
			_npcWindowRect = MyGUI.ClampToScreen(GUI.Window(NPC_WINDOW_ID,
				_npcWindowRect,
				npcWindow,
				"","Box"));			//later can change this to variable?
		}
	}
	
	void OnEnable(){
		Messenger.AddListener("DisplayNPCWindow",DisplayNPCWindow);
		Messenger.AddListener("CloseNPCWindow",CloseNPCWindow);
	}
	
	void OnDisable(){
		Messenger.RemoveListener("DisplayNPCWindow",DisplayNPCWindow);
		Messenger.RemoveListener("CloseNPCWindow",CloseNPCWindow);
	}
	
	void npcWindow(int windowid){
				
		if(openedNPCDialog == null)
			return;
		/* WE USE 100 AS THE OPTION NUMBER FOR VENDOR IF NEEDED*/
		/* WE USE 200 AS THE OPTION NUMBER FOR CRAFTING IF NEEDED*/
		//int optionNum = openedNPCDialog.OptionNumber;
		
		if(openedNPCDialog.textNum + 1 != openedNPCDialog.npcText.Length)
				openedNPCDialog.buttonTextNum = 0;
			else
				openedNPCDialog.buttonTextNum = 1;
			//GUI.Box (new Rect((0,0,400,400),"");									//BG
		
			
			if(openedNPCDialog.OptionNumber == 0 || openedNPCDialog.OptionNumber == 1){
				GUI.Box (new Rect(5 ,5,355,30),openedNPCDialog.Title);								//Title
				if(GUI.Button (new Rect(365 ,5,30,30),"X")){											//Close Button
					openedNPCDialog.CloseDialog();
					return;
				}
			}
			
			//Dialogue Options: Conversation, Quests, Vendor
			if(openedNPCDialog.OptionNumber == 0){	//displaying options
				if(GUI.Button (new Rect(5 ,40,390,20),"Conversation"))					//Converse
					openedNPCDialog.OptionNumber = 1;
				for (int i = 0; i < openedNPCDialog.quests.Count; i++) {
					string questInfo = openedNPCDialog.quests[i].QuestName;
				
					if(openedNPCDialog.quests[i].isAccepted) 
						questInfo += " [In Progress]";
				
					if(!openedNPCDialog.quests[i].isAccepted) questInfo = ToolTipStyle.Yellow + questInfo + ToolTipStyle.EndColor;
					else if(openedNPCDialog.quests[i].isAccepted) questInfo = ToolTipStyle.Orange + questInfo + ToolTipStyle.EndColor;
					else if(openedNPCDialog.quests[i].isComplete) questInfo = ToolTipStyle.Green + questInfo + ToolTipStyle.EndColor;
				
					if(GUI.Button (new Rect(5 ,65+ (i*25),390,20),questInfo))
						openedNPCDialog.OptionNumber = 2 + i;
				}
				if(openedNPCDialog.isVendor){
					string vendorInfo = ToolTipStyle.Blue + "*Vendor*" + ToolTipStyle.EndColor;
					if(GUI.Button (new Rect(5 ,65+ (openedNPCDialog.quests.Count*25),390,20),vendorInfo)){
						openedNPCDialog.OptionNumber = 100;
					}
				}
				if(openedNPCDialog.isBlacksmith){
					int isVendorSpace = openedNPCDialog.isVendor ? 25 : 0;
					string blackSmithInfo = ToolTipStyle.Blue + "*Crafting*" + ToolTipStyle.EndColor;
					if(GUI.Button (new Rect(5 ,65+ isVendorSpace+ (openedNPCDialog.quests.Count*25),390,20),blackSmithInfo)){
						openedNPCDialog.OptionNumber = 200;
					}
				}
			}
			//Conversation Option
			else if (openedNPCDialog.OptionNumber == 1) {
				GUI.Box (new Rect(5 ,40,390,330),openedNPCDialog.npcText[openedNPCDialog.textNum],"NPCDialog");					//Dialog
				if(GUI.Button (new Rect(5 ,375,385,20),openedNPCDialog.buttonText[openedNPCDialog.buttonTextNum])){	//Button
					if(openedNPCDialog.textNum + 1 != openedNPCDialog.npcText.Length){ //Go to next conversation text if we're not on the last one
						openedNPCDialog.textNum += 1;
					}
					else {
						if(openedNPCDialog.npcText != openedNPCDialog.npcTextOrig){ //if convo is for quest
							foreach(NPCQuest nq in _pc.QuestsInProgress){
								if(openedNPCDialog.NpcName.Contains(nq.nameOfNPCtoTalkTo)){
									nq.talkDone = true;
								}
							}
						}
						openedNPCDialog.OptionNumber = 0;
						openedNPCDialog.EnterDialog();
					}
				}
			}
			//Vendor Option /*NOTE: arrayNum 0-15 = Vendor, 16-19 = BuyBack for the tooltips*/
			else if(openedNPCDialog.OptionNumber == 100){
				GUI.Box (new Rect(5 ,5,390,30),"Vendor");		//Title
				
				if(GUI.Button (new Rect(365 ,5,30,30),"X"))											//Close Button
						openedNPCDialog.CloseDialog();
				
				int vendorCnt = 0;
				for (int m = 0; m < 4; m++) {
					for (int n = 0; n < 4; n++) {
						if(vendorCnt <= openedNPCDialog.vendor.vendorItems.Count-1){
							if(openedNPCDialog.vendor.vendorItems[vendorCnt] != null){
								if(GUI.Button (new Rect(5 + (n*45),40 + (m*45),40,40),
								new GUIContent(openedNPCDialog.vendor.vendorItems[vendorCnt].Icon,
												string.Format ("Vendor{0}",vendorCnt)
								),
								openedNPCDialog.vendor.vendorItems[vendorCnt].Rarity.ToString())){
									if (Event.current.button == 1) { 
										openedNPCDialog.vendor.PlayerBuyItem(vendorCnt);
										return;
									}
								}
							
								string stackInfo = openedNPCDialog.vendor.vendorItems[vendorCnt].MaxStacks > 1 ? openedNPCDialog.vendor.vendorItems[vendorCnt].CurStacks.ToString() : string.Empty;
								GUI.Box ( new Rect(5 + (n*45),40 + (m*45),40,40),stackInfo,"StackOverlay");
								string moreInfo = openedNPCDialog.vendor.vendorItems[vendorCnt].ItemType == ItemEquipType.Weapon ? "+" + (openedNPCDialog.vendor.vendorItems[vendorCnt] as Weapon).Enchants : string.Empty;
								GUI.Box ( new Rect(5 + (n*45),40 + (m*45),40,40),moreInfo,"ItemInfoOverlay");
							}
						}
						else
						{
							GUI.Box (new Rect(5 + (n*45),40 + (m*45),40,40),"");
						}

						vendorCnt++;
					}
				}
				
				GUI.Box (new Rect(5 ,220,390,30),"BuyBack");		//BuyBack Title
				for (int b = 0; b < 4; b++) {
					if(b <= openedNPCDialog.vendor.buyBackItems.Count-1){
						if(openedNPCDialog.vendor.buyBackItems[b] != null){
							if(GUI.Button (new Rect(5 + (b*45),255,40,40),
							new GUIContent(openedNPCDialog.vendor.buyBackItems[b].Icon,
											string.Format ("Vendor{0}",16+b)
							),
							openedNPCDialog.vendor.buyBackItems[b].Rarity.ToString())){
								if (Event.current.button == 1) {
									openedNPCDialog.vendor.PlayerBuyBack(b);
								}
							}
						
							string stackInfo = openedNPCDialog.vendor.buyBackItems[b].MaxStacks > 1 ? openedNPCDialog.vendor.buyBackItems[b].CurStacks.ToString() : string.Empty;
							GUI.Box ( new Rect(5 + (b*45),255,40,40),stackInfo,"StackOverlay");
							string moreInfo = openedNPCDialog.vendor.buyBackItems[b].ItemType == ItemEquipType.Weapon ? "+" + (openedNPCDialog.vendor.buyBackItems[b] as Weapon).Enchants : string.Empty;
							GUI.Box ( new Rect(5 + (b*45),255,40,40),moreInfo,"ItemInfoOverlay");
						}
					}
					else
					{
						GUI.Box (new Rect(5 + (b*45),255,40,40),"");
					}
				}

			}
			//Blacksmith Option
			else if(openedNPCDialog.OptionNumber == 200){
				blackSmith = openedNPCDialog.GetComponent<NPCBlacksmith>();
				GUI.Box (new Rect(5 ,5,390,30),"Crafting");		//Title
				
				if(GUI.Button (new Rect(365 ,5,30,30),"X"))											//Close Button
					openedNPCDialog.CloseDialog();
			
				for (int i = 0; i < CraftedItemsClasses.AllCraftableItems.Count; i++) {
					GUI.Box (new Rect(5,40 + (i*55),390,50),""); //craftitem bg
					if(GUI.Button (new Rect(10,45 + (i*55),40,40),new GUIContent(CraftedItemsClasses.AllCraftableItems[i].Icon,
												string.Format ("Craft{0}",i)
								))){
						selectedItem = CraftedItemsClasses.AllCraftableItems[i];
					}
					
					string infoString = string.Format("Level {0} {1}",
						CraftedItemsClasses.AllCraftableItems[i].RequiredLevel,
						CraftedItemsClasses.AllCraftableItems[i].Name);
						
					//infoString = check if have all items
					
					if(GUI.Button (new Rect(55,45 + (i*55),335,40),infoString)){
						selectedItem = CraftedItemsClasses.AllCraftableItems[i];
					}
				}
			
				GUI.Box (new Rect(5 ,270,390,15),""); //Seperator
				
				if(selectedItem != null){
					GUI.Box (new Rect(5,_npcWindowRect.height - 105,390,100),"");
					blackSmith.GetMatsInfo (selectedItem);
					for (int m = 0; m < blackSmith.itemsNeeded.Count; m++) {
						GUI.Box (new Rect(10 + (m*45),_npcWindowRect.height - 100,40,40),
							new GUIContent(blackSmith.itemsNeeded[m].Icon,
												string.Format ("CraftMat{0}",m)
								));
						GUI.Box (new Rect(17 + (m*45),_npcWindowRect.height - 62,20,20),blackSmith.itemsNeeded[m].CurStacks.ToString(),"Invisible");
					}
					
					GUI.Box (new Rect(_npcWindowRect.width-130,_npcWindowRect.height - 100,120,30),selectedItem.Name);
				
					if(GUI.Button (new Rect(10,_npcWindowRect.height - 40,380,30),"Craft Item")){
						blackSmith.CraftItem (selectedItem);
					}
				}
				else {
					GUI.Box (new Rect(5,_npcWindowRect.height - 105,390,100),"Choose an Item to Craft");
				}
				
			}
			//Quest Option
			else {
				if(!openedNPCDialog.quests[openedNPCDialog.OptionNumber -2].isFinished){
					//display for quest optionNum
					
					GUI.Box (new Rect(5 ,5,390,30),openedNPCDialog.quests[openedNPCDialog.OptionNumber -2].QuestName);		//Title
					if(GUI.Button (new Rect(365 ,5,30,30),"X"))											//Close Button
						openedNPCDialog.CloseDialog();
					
					GUI.Box (new Rect(5 ,40,390,230),openedNPCDialog.quests[openedNPCDialog.OptionNumber -2].QuestText,"NPCDialog");		//Dialog
					GUI.Box (new Rect(5 ,275,390,20),"Rewards" ,"NPCDialog");		//Rewards
					string rewardText = "Exp: " + openedNPCDialog.quests[openedNPCDialog.OptionNumber -2].QuestExpReward + " " +
						"Gold: " + openedNPCDialog.quests[openedNPCDialog.OptionNumber -2].QuestGoldReward;
					GUI.Box (new Rect(5 ,300,390,20),rewardText,"NPCDialog");		//Exp/Gold Reward
					
					for (int i = 0; i < openedNPCDialog.quests[openedNPCDialog.OptionNumber -2].QuestReward.Count; i++) {
						string stackInfo = openedNPCDialog.quests[openedNPCDialog.OptionNumber -2].QuestReward[i].MaxStacks > 1 
							? openedNPCDialog.quests[openedNPCDialog.OptionNumber -2].QuestReward[i].CurStacks.ToString() 
							: string.Empty;
						
						//Quest Item Reward
						GUI.Box (new Rect(5 + (45*i) ,325,40,40),
						new GUIContent(openedNPCDialog.quests[openedNPCDialog.OptionNumber -2].QuestReward[i].Icon,
							string.Format ("NPCQuest{0}_{1}",openedNPCDialog.OptionNumber -2,i)),
							openedNPCDialog.quests[openedNPCDialog.OptionNumber -2].QuestReward[i].Rarity.ToString());
					
						GUI.Box (new Rect(5 + (45*i) ,325,40,40),stackInfo,"StackOverlay");
					}
					
					if(openedNPCDialog.quests[openedNPCDialog.OptionNumber - 2].requirementStatus == RequirementsStatus.All){
						if(!openedNPCDialog.quests[openedNPCDialog.OptionNumber - 2].isAccepted){
							if(GUI.Button (new Rect(5 ,375,190,20),"Accept Quest")){	//Button
								Debug.Log ("Quest Accepted");
								
								openedNPCDialog.quests[openedNPCDialog.OptionNumber -2].isAccepted = true;
								_pc.QuestsInProgress.Add (openedNPCDialog.quests[openedNPCDialog.OptionNumber-2]);
								
								if(openedNPCDialog.quests[openedNPCDialog.OptionNumber -2].qego != null)
									openedNPCDialog.quests[openedNPCDialog.OptionNumber -2].qego.EnableGameObjects();
							
								openedNPCDialog.OptionNumber = 0;
							}
						}
						else {
							if(openedNPCDialog.quests[openedNPCDialog.OptionNumber - 2].isComplete){
								if(GUI.Button (new Rect(5 ,375,190,20),"Complete Quest")){	//Button
									Debug.Log ("Quest Complete");
									openedNPCDialog.CompleteQuest(openedNPCDialog.quests[openedNPCDialog.OptionNumber -2]);
									openedNPCDialog.OptionNumber = 0;
								}
							}
							else {
								GUI.Button (new Rect(5 ,375,190,20),"In Progress");
							}
						}
					}
					else {
						if(openedNPCDialog.quests[openedNPCDialog.OptionNumber - 2].requirementStatus == RequirementsStatus.Quests){
							string info = string.Format("Requires: Level {0}",openedNPCDialog.quests[openedNPCDialog.OptionNumber - 2].QuestLevelReq);
							GUI.Button (new Rect(5 ,375,190,20),info);
						}
					}
					
					
					if(GUI.Button (new Rect(200,375,190,20),"Close")){	//Button
						openedNPCDialog.CloseDialog();
					}
				}				
			}
		
		
		
		
		
		
		GUI.DragWindow (new Rect (0,0, 10000, 20));
		GUIHandler.SetToolTip();
		GUI.BringWindowToFront (0);
	}
	
	private void DisplayNPCWindow() {
		_displayNPCWindow = true;
	}

	private void CloseNPCWindow(){
			
		openedNPCDialog = null;
		
		selectedItem = null;
		blackSmith = null;
		_displayNPCWindow = false;
		MyGUI.DisplayToolTip = false;
	}
	
}
