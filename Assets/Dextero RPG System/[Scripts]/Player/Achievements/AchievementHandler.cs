using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class AchievementHandler : MonoBehaviour {
	
	public List<Achievement> Achievements = new List<Achievement>();
	public List<Achievement> achievements {
		get{return Achievements;}
		set{Achievements = value;}
	}
	
	private int TotalAchievementScore;
	public int totalAchievementScore {
		get{return TotalAchievementScore;}
	}
	
	
	private PlayerNew player;
	
	// Use this for initialization
	void Start () {
		if(player == null){
			player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerNew>();
		}
		
		//If we have no achievement data
		if(Achievements.Count < 1){
			TotalAchievementScore = 0;
			AddAllAchievements();
			
		}	
	}
	
	// Update is called once per frame
	void Update () {
		for (int i = 0; i < Achievements.Count; i++) {
			if(!Achievements[i].IsAchieved){
				bool achieved = achievements[i].CheckConditions(player);
				
				if(achieved){
					ActivateAchievement(Achievements[i]);
					StartCoroutine("ShowAchievement",Achievements[i]);
				}
			}
		}
	}
	
	void ActivateAchievement(Achievement achievement){
		
		TotalAchievementScore += achievement.AchievementScore;
		
		achievement.IsAchieved = true;
		achievement.DateAchieved = DateTime.Now;
		
		Debug.Log (achievement.AchievementName + " " + "ACHIEVED!");
	}
	
	void AddAllAchievements(){
		//Order they will show up
		Achievements.Add (new LevelUpToThree());
		Achievements.Add (new LevelUpToFive());
		Achievements.Add (new LevelUpToOneHundred());
		
		Achievements.Add (new KillOneMob());
		Achievements.Add (new KillTenMobs());
		Achievements.Add (new KillTwentyMobs());
		
		Achievements.Add (new EarnTenThousandGold());
		Achievements.Add (new EarnHundredThousandGold());
		
		Achievements.Add (new CompleteOneQuest());
		Achievements.Add (new CompleteThreeQuest());
		Achievements.Add (new CompleteFiveQuest());
		
		Achievements.Add (new KillABoss());
		
	}
	
	public void RefreshAchievements(){
		TotalAchievementScore = 0;
		foreach(Achievement a in Achievements){
			a.AchievementIcon = Resources.Load (a.AchievementIconString) as Texture2D;
			if(a.IsAchieved){
				TotalAchievementScore += a.AchievementScore;
			}
		}
	}
	
	
	Achievement achievementToShow;
	bool showAchievement;
	public GUISkin mySkin;
	IEnumerator ShowAchievement(Achievement a){
		achievementToShow = a;
		showAchievement = true;
		yield return new WaitForSeconds(3.0f);
		showAchievement = false;
	}
	
	void OnGUI(){
		GUI.skin = mySkin;
		if(showAchievement){
			GUIContent achievementInfo = new GUIContent("",achievementToShow.AchievementIcon);
			Rect popupRect = new Rect(Screen.width/2 - 100,5,250,50);
			GUI.Box (popupRect,"","AchievementPopupBG");
			GUILayout.BeginArea(popupRect);
				GUILayout.BeginHorizontal();
			
					GUILayout.Space (5);
					GUILayout.BeginVertical(GUILayout.Width (40));
						GUILayout.Space (5);
						GUILayout.Box (achievementInfo,"AchievementSlot",GUILayout.Width (40),GUILayout.Height(40));
						GUILayout.Space (5);
					GUILayout.EndVertical();
			
					GUILayout.BeginVertical();
						GUILayout.Space (5);
						GUILayout.Box ("Achievement Unlocked!","Invisible",GUILayout.Height(20));
						GUILayout.Box (achievementToShow.AchievementName,"Invisible",GUILayout.Height(20));
						GUILayout.Space (5);
					GUILayout.EndVertical();
					GUILayout.Space (5);
				GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
	}
	
}
