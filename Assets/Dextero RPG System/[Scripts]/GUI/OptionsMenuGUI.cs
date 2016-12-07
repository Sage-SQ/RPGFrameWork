using UnityEngine;
using System.Collections;

public class OptionsMenuGUI : MonoBehaviour {
	//public Texture2D cross;
	public static bool ShowMenu = false;
	private bool EscMenu = true;
	private bool DisplayEscMenu = false;
	public GameSettings gameSettings;
	
	// Use this for initialization
	void Start () {
			//MouseSensitivity = MouseLook.sensitivityX = MouseLook.sensitivityY;
		gameSettings = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GameSettings>();
		ShowMenu = false;
	}
	
	void OnGUI(){
		if(ShowMenu){
			DisplayEscMenu = true;
		}
		
		//GUI.DrawTexture(new Rect(Screen.width/2-10, Screen.height/2-10, 20,20), cross);
		
		if(DisplayEscMenu){
			//MyGUI.CanShow = false;
			
			GUILayout.BeginArea (new Rect (Screen.width/2-125,Screen.height/2-125,250,250),"","Box");
			GUILayout.Label ("ESC MENU");
			
			if(EscMenu){
				if(GUILayout.Button("Resume"))
					DisplayEscMenu = false;
				
				if(GUILayout.Button("Load Game")){
					gameSettings.LoadCharacterData();
					DisplayEscMenu = false;
				}
				
				if(GUILayout.Button("Save Game")){
					gameSettings.SaveCharacterData();
					DisplayEscMenu = false;
				}
			}

			Time.timeScale = 0;
			Screen.lockCursor = false;
    		GUILayout.EndArea ();
			
			
			
		}
		else{
			MyGUI.CanShow = true;
			Time.timeScale = 1.0f;
			
		}
		
		ShowMenu = DisplayEscMenu;
	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(KeyCode.Escape)){
			if(DisplayEscMenu){
				DisplayEscMenu = false;
				ShowMenu = false;
			}
			else{
				DisplayEscMenu = true;
				EscMenu = true;
			}
		}
		
		if(ShowMenu){
			DisplayEscMenu = true;
			MyGUI.CanShow = false;
		}		
	}
}
