using UnityEngine;
using System.Collections;
using System;


[System.Serializable]
public class Achievement {
	public string AchievementName{get;set;}
	

	[System.NonSerialized]
	public Texture2D AchievementIcon;
	public string AchievementIconString;
	
	public string AchievementDescription;
	public int AchievementScore;
	public bool IsAchieved;
	public DateTime DateAchieved;
	
	public virtual bool CheckConditions(PlayerNew player){
		//Check conditions for achievement here
		return false;
	}
	public Achievement(){
		
	}
	
	public Achievement(string name, Texture2D icon, string desc, int score){
		
		AchievementName = name;
		AchievementIcon = icon;
		AchievementIconString = "";
		AchievementDescription = desc;
		AchievementScore = score;
		IsAchieved = false;
		DateAchieved = DateTime.Now;
	}
}
