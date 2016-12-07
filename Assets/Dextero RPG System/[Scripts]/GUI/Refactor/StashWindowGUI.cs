using UnityEngine;
using System.Collections;

public class StashWindowGUI : MonoBehaviour {
	public MyGUI GUIHandler;
	public PlayerNew player;
	public GUISkin mySkin;
	
	// GUISpecific
	public bool _displayStashWindow = false;
	private const int STASH_WINDOW_ID = 10;
	private Rect _stashWindowRect = new Rect();
	public static PlayerStash openedStash;
	private int PageNum = 0;
	private string[] selStrings = new string[] {"1", "2", "3", "4"};
	
	void Start(){
		GUIHandler = GameObject.FindGameObjectWithTag("GUIHandler").GetComponent<MyGUI>();
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerNew>();
		player.playerState = PlayerState.Normal;
		_stashWindowRect = new Rect(Screen.width/2 - 150,Screen.height/2 - 125,250,290);
		selStrings = new string[] {"Tab 1", "Tab 2", "Tab 3", "Tab 4", "Tab 5"};
	}
	
	void OnGUI(){
		GUI.skin = mySkin;
		if(MyGUI.CanShow){			
			if(_displayStashWindow){
				_stashWindowRect = MyGUI.ClampToScreen(GUI.Window(STASH_WINDOW_ID,
					_stashWindowRect,
					stashWindow,
					""));
			}
		}
	}
	
	public void OnEnable(){
		Messenger.AddListener("ShowStashWindow",DisplayStashWindow);
		Messenger.AddListener("CloseStashWindow",CloseStashWindow);
	}
	
	public void OnDisable(){
		Messenger.RemoveListener("ShowStashWindow",DisplayStashWindow);
		Messenger.RemoveListener("CloseStashWindow",CloseStashWindow);
	}
	
	#region "Stash Window"
	private void stashWindow(int windowid){
		
		GUI.Box (new Rect(5 ,5,200,25),"Stash");		//Title
				
		if(GUI.Button (new Rect(210 ,5,30,25),"X")){											//Close Button
				CloseStashWindow();
				//return;
		}
		

		PageNum = GUI.SelectionGrid(new Rect(5, 35, 240, 25), PageNum, selStrings, 5);

		int cnt = 0 + (PageNum*25);
		for (int x = 0; x < 5; x++) {
			for (int y = 0; y < 5; y++) {
				if(cnt <= player.Stash.Count-1){
					if(player.Stash[cnt] != null){
						if(GUI.Button (new Rect(5 + (y*45),65 + (x*45),40,40),
						new GUIContent(player.Stash[cnt].Icon,
										string.Format ("Stash{0}",cnt)
						),
						player.Stash[cnt].Rarity.ToString())){
							if (Event.current.button == 1) { 
								openedStash.MoveFromBankToPlayer(cnt);
								MyGUI._toolTip = "";
								return;
							}
						}
						
						string stackInfo = player.Stash[cnt].MaxStacks > 1 ? player.Stash[cnt].CurStacks.ToString() : string.Empty;
						GUI.Box ( new Rect(5 + (y*45),65 + (x*45),40,40),stackInfo,"StackOverlay");
						string moreInfo = player.Stash[cnt].ItemType == ItemEquipType.Weapon ? "+" + (player.Stash[cnt] as Weapon).Enchants : string.Empty;
						GUI.Box ( new Rect(5 + (y*45),65 + (x*45),40,40),moreInfo,"ItemInfoOverlay");
					}
				}
				else
				{
					GUI.Box (new Rect(5 + (y*45),65 + (x*45),40,40),"");
				}
				cnt++;
			}
		}
		
		GUI.DragWindow (new Rect (0,0, 10000, 20));
		GUIHandler.SetToolTip	();
		GUI.BringWindowToFront (0);
	}
	
	private void DisplayStashWindow() {
		player.playerState = PlayerState.Stash;
		_displayStashWindow = true;
	}

	private void CloseStashWindow(){
		player.playerState = PlayerState.Normal;
		openedStash = null;
		_displayStashWindow = false;
	}
	#endregion

}
