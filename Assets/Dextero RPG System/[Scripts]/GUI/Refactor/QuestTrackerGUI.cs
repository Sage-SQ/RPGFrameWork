using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QuestTrackerGUI : MonoBehaviour {
	public MyGUI GUIHandler;
	public PlayerNew _pc;
	public GUISkin mySkin;
	
	// GUISpecific
		
	private const int QUEST_WINDOW_ID = 8;
	private Rect _questWindowRect = new Rect(0,0,0,0);
	public bool _displayQuestTracker = false;
	private int _questWindowPanel = 0;
	private string[] questWindowPanels = new string[] {"Quests","Achievements"};
	private Vector2 questScrollPos = new Vector2();
	private Vector2 questScrollPosBody = new Vector2();
	private bool questTrackerShowActive = false;
	private bool questTrackerShowComplete = true;
	private List<NPCQuest> playerQuests;
	public static NPCQuest selectedNPCQuest;
	private bool ShowMiniQuestTracker;
	//Achievements
	private Vector2 achievementsScroll = new Vector2();
	public static AchievementHandler achievementsHandler;
	
	
	void Start(){
		GUIHandler = GameObject.FindGameObjectWithTag("GUIHandler").GetComponent<MyGUI>();
		_pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerNew>();
		achievementsHandler = GameObject.FindGameObjectWithTag("AchievementHandler").GetComponent<AchievementHandler>();
		_questWindowRect = new Rect(Screen.width/2 - 300 ,Screen.height/2 - 200,600,400);
		questWindowPanels = new string[] {"Quests","Achievements"};
		questTrackerShowActive = true;
		questTrackerShowComplete = true;
		playerQuests = new List<NPCQuest>();
		ShowMiniQuestTracker = true;
	}

	void Update(){
		if(Input.GetKeyUp (KeyCode.K))
		{
			ToggleQuestTrackerWindow();
		}
	}
	
	void OnGUI(){
		GUI.skin = mySkin;
		if(MyGUI.CanShow){
			
			float screenW = Screen.width;
			Rect r = new Rect(screenW - 105, 5, 100, 40); 
			if(GUI.Button (r,"Quests")){
				ToggleQuestTrackerWindow();
			}
			
			GetQuests();
			DrawMiniQuestTracker();
			if(_displayQuestTracker){
				_questWindowRect = MyGUI.ClampToScreen(GUILayout.Window(QUEST_WINDOW_ID,
					_questWindowRect,
					questTrackerWindow,
					"","QuestTrackerWindow"));
			}	
		}
	}
	
	void OnEnable(){

	}
	
	void OnDisable(){

	}
	
	#region "Quest Tracker Window"
	
	private void questTrackerWindow(int windowid) {
		GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			_questWindowPanel = GUILayout.Toolbar (_questWindowPanel,questWindowPanels,GUILayout.Height(20));
		GUILayout.EndHorizontal();
		
		GUILayout.FlexibleSpace();
		
		switch (_questWindowPanel) {
			case 0:
			DisplayQuestTracker();
				break;
			case 1:
			DisplayAchievements();
				break;
		}
		
		GUI.DragWindow (new Rect (0,0, 10000, 20));
		GUIHandler.SetToolTip	();
	}
	
	void DrawMiniQuestTracker(){
		if(_pc.QuestsInProgress.Count > 0){
			string showHide = ShowMiniQuestTracker ? "Hide" : "Show";
			if(GUI.Button (new Rect(Screen.width-55,100,50,25),showHide,"Invisible"))
				ShowMiniQuestTracker = !ShowMiniQuestTracker;
			
			if(ShowMiniQuestTracker){
				string questTrackerMiniString = "";
				questTrackerMiniString += "<size=20>" + "Quests: [      ]" + "</size>"; //space after Quests: is for show button
				for (int i = 0; i < _pc.QuestsInProgress.Count; i++) {
					if(_pc.QuestsInProgress[i].trackSteps){
						questTrackerMiniString += ToolTipStyle.NewLine;
						questTrackerMiniString += ToolTipStyle.Green + _pc.QuestsInProgress[i].QuestName + ToolTipStyle.EndColor +  ToolTipStyle.Break;
						questTrackerMiniString += _pc.QuestsInProgress[i].questTracker;
					}
				}
				GUI.Box (new Rect(Screen.width-205,100,200,400),questTrackerMiniString,"QuestTracker");
			}
		}
	}
	
	void DisplayQuestTracker(){
		//Quest titles
		GUILayout.BeginHorizontal();

			GUILayout.BeginVertical();
				GUILayout.Box (ToolTipStyle.Bold + ToolTipStyle.Red + 
						"Quests" + 
						ToolTipStyle.EndColor + ToolTipStyle.EndBold,
						GUILayout.Height(30), GUILayout.Width(_questWindowRect.width/2 - 10));
		
				GUILayout.BeginHorizontal();
					questTrackerShowActive = GUILayout.Toggle (questTrackerShowActive,"Active");
					questTrackerShowComplete = GUILayout.Toggle (questTrackerShowComplete,"Done");
					GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
		
				questScrollPos = GUILayout.BeginScrollView(questScrollPos,
				GUILayout.MaxWidth(_questWindowRect.width/2 - 10),
				GUILayout.MaxHeight(_questWindowRect.height - 25));
					for (int i = 0; i < playerQuests.Count; i++) {
			
						bool draw = false;
						if(playerQuests[i].isFinished && questTrackerShowComplete)
							draw = true;
						if(!playerQuests[i].isFinished && questTrackerShowActive)
							draw = true;
						if(draw){
							if(GUILayout.Button (playerQuests[i].QuestInfo(),GUILayout.Height(40))){
								selectedNPCQuest = playerQuests[i];
							}
						}
						
					}
				GUILayout.EndScrollView();
		
			GUILayout.EndVertical();
			
			GUILayout.FlexibleSpace();
			
			//Quest Info
			GUILayout.BeginVertical();
				if(selectedNPCQuest != null){
					questScrollPosBody = GUILayout.BeginScrollView(questScrollPosBody,
					GUILayout.MaxWidth(_questWindowRect.width/2 - 10),
					GUILayout.MaxHeight(_questWindowRect.height - 25));
				
						GUILayout.Box (ToolTipStyle.Bold + ToolTipStyle.Blue + 
					selectedNPCQuest.QuestName + 
					ToolTipStyle.EndColor + ToolTipStyle.EndBold,
					GUILayout.Height(30));
				
						GUILayout.Box (selectedNPCQuest.QuestText,"NPCDialog", GUILayout.MaxWidth(_questWindowRect.width/2 - 10),
				GUILayout.MaxHeight(200));
				
						GUILayout.BeginVertical();
							GUILayout.Box (selectedNPCQuest.QuestRewardString());
							GUILayout.BeginHorizontal();
								GUILayout.FlexibleSpace();
									for (int i = 0; i < selectedNPCQuest.QuestReward.Count; i++) {
										GUILayout.Box(
										new GUIContent(selectedNPCQuest.QuestReward[i].Icon,string.Format("QuestReward{0}",i)),
										selectedNPCQuest.QuestReward[i].Rarity.ToString(),
										GUILayout.Height(40), GUILayout.Width (40));
									}
								GUILayout.FlexibleSpace();
							GUILayout.EndHorizontal();
							GUILayout.Box (selectedNPCQuest.questTracker);
						GUILayout.EndVertical();
						GUILayout.BeginHorizontal();							
							if(!selectedNPCQuest.isFinished){
								string trackOrNot = selectedNPCQuest.trackSteps ? "Stop Tracking" : "Track this Quest";
								if(GUILayout.Button (trackOrNot))
									selectedNPCQuest.trackSteps = !selectedNPCQuest.trackSteps;
								GUILayout.Space(5);
								if(GUILayout.Button ("Cancel Quest",GUILayout.Width (90))){
									selectedNPCQuest.CancelQuest();
									selectedNPCQuest = null;
								}
							}
						GUILayout.EndHorizontal();
					GUILayout.EndScrollView();
				}
			GUILayout.EndVertical();
		GUILayout.EndHorizontal();
	}
	
	void GetQuests(){
		if(Event.current.type == EventType.repaint){
			playerQuests = new List<NPCQuest>();
			playerQuests.AddRange(_pc.QuestsInProgress);
			for (int i = _pc.questsComplete.Count; i --> 0;) {
				playerQuests.Add(_pc.questsComplete[i]);
			}
		}
	}
	
	void DisplayAchievements(){
		
		
		//Top
		GUILayout.BeginHorizontal();
			GUILayout.Label("Achievements","TitleTextLeft");
			GUILayout.FlexibleSpace();
			GUILayout.Box (ToolTipStyle.Title + achievementsHandler.totalAchievementScore + ToolTipStyle.EndSize,"AchievementScoreText",GUILayout.Width (200));
			GUILayout.Label("","AchievementScoreIcon");
		GUILayout.EndHorizontal();
		//Main
		achievementsScroll = GUILayout.BeginScrollView(achievementsScroll,GUILayout.MaxWidth(_questWindowRect.width - 10),
				GUILayout.MaxHeight(_questWindowRect.height - 25));
			GUILayout.BeginVertical();
			int cnt = 0;
			int achievementRows = 5;
			int achievementCols = 6;
			for(int y = 0; y < achievementRows; y++){
				GUILayout.BeginHorizontal();
				for(int x = 0; x < achievementCols; x++){
				GUILayout.Space(16);
					if(cnt < achievementsHandler.achievements.Count){
						string guiStyle = achievementsHandler.achievements[cnt].IsAchieved ? "AchievementSlot" : "AchievementLocked";
						GUILayout.Box (new GUIContent(achievementsHandler.achievements[cnt].AchievementIcon,string.Format("Achievements{0}",cnt)),guiStyle,GUILayout.Width(80),GUILayout.Height (80));
					}
			
					cnt++;
				}
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.Space(10);
			}
			GUILayout.EndVertical();

		GUILayout.EndScrollView();
	}
	
	public void ToggleQuestTrackerWindow(){
		_displayQuestTracker = !_displayQuestTracker;
		
		//Clear tooltip if inventory is closed
		if(!_displayQuestTracker){
			MyGUI._toolTip = "";
		}
	}

	#endregion

}
