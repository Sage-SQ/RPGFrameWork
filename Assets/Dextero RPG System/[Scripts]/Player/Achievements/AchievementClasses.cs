using UnityEngine;
using System.Collections;


public static class AchievementConstants {
	public const string ACHIEVEMENT_LOCATION = "Achievement Icons/";
}


#region "KillMobs"
[System.Serializable]
public class KillOneMob: Achievement {	
	public KillOneMob(){
		Texture2D icon = Resources.Load (AchievementConstants.ACHIEVEMENT_LOCATION + "Kill1Monster") as Texture2D;
		AchievementName = "Monster Hunter";
		AchievementIcon = icon;
		AchievementIconString = AchievementConstants.ACHIEVEMENT_LOCATION + "Kill1Monster";
		AchievementDescription = "Killed a monster.";
		AchievementScore = 10;
	}
	
	public override bool CheckConditions (PlayerNew player)
	{
		if(player.MonstersKilled >= 1){
			return true;
		}
		return false;
	}

}

[System.Serializable]
public class KillTenMobs: Achievement {	
	public KillTenMobs(){
		Texture2D icon = Resources.Load (AchievementConstants.ACHIEVEMENT_LOCATION + "Kill10Monster") as Texture2D;
		AchievementName = "Monster Hunter II";
		AchievementIcon = icon;
		AchievementIconString = AchievementConstants.ACHIEVEMENT_LOCATION + "Kill10Monster";
		AchievementDescription = "Killed 10 monsters";
		AchievementScore = 20;
	}
	
	public override bool CheckConditions (PlayerNew player)
	{
		if(player.MonstersKilled >= 10){
			return true;
		}
		return false;
	}

}

[System.Serializable]
public class KillTwentyMobs: Achievement {	
	public KillTwentyMobs(){
		Texture2D icon = Resources.Load (AchievementConstants.ACHIEVEMENT_LOCATION + "Kill20Monster") as Texture2D;
		AchievementName = "Monster Hunter III";
		AchievementIcon = icon;
		AchievementIconString = AchievementConstants.ACHIEVEMENT_LOCATION + "Kill20Monster";
		AchievementDescription = "Killed 20 monsters";
		AchievementScore = 30;
	}
	
	public override bool CheckConditions (PlayerNew player)
	{
		if(player.MonstersKilled >= 20){
			return true;
		}
		return false;
	}

}
#endregion

#region "EarnGold"

[System.Serializable]
public class EarnTenThousandGold: Achievement {	
	public EarnTenThousandGold(){
		Texture2D icon = Resources.Load (AchievementConstants.ACHIEVEMENT_LOCATION + "Earn10000Gold") as Texture2D;
		AchievementName = "Gold Collector";
		AchievementIcon = icon;
		AchievementIconString = AchievementConstants.ACHIEVEMENT_LOCATION + "Earn10000Gold";
		AchievementDescription = "Earned 10,000 gold.";
		AchievementScore = 10;
	}
	
	public override bool CheckConditions (PlayerNew player)
	{
		if(player.Gold >= 10000){
			return true;
		}
		return false;
	}

}


[System.Serializable]
public class EarnHundredThousandGold: Achievement {	
	public EarnHundredThousandGold(){
		Texture2D icon = Resources.Load (AchievementConstants.ACHIEVEMENT_LOCATION + "Earn100000Gold") as Texture2D;
		AchievementName = "Rich Kid on the Block";
		AchievementIcon = icon;
		AchievementIconString = AchievementConstants.ACHIEVEMENT_LOCATION + "Earn100000Gold";
		AchievementDescription = "Earned 100,000 gold.";
		AchievementScore = 50;
	}
	
	public override bool CheckConditions (PlayerNew player)
	{
		if(player.Gold >= 100000){
			return true;
		}
		return false;
	}

}

#endregion

#region "LevelUp"

[System.Serializable]
public class LevelUpToThree: Achievement {	
	public LevelUpToThree(){
		Texture2D icon = Resources.Load (AchievementConstants.ACHIEVEMENT_LOCATION + "LevelUpTo3") as Texture2D;
		AchievementName = "One Small Step For Man";
		AchievementIcon = icon;
		AchievementIconString = AchievementConstants.ACHIEVEMENT_LOCATION + "LevelUpTo3";
		AchievementDescription = "Leveled up to 3.";
		AchievementScore = 10;
	}
	
	public override bool CheckConditions (PlayerNew player)
	{
		if(player.Level >= 3){
			return true;
		}
		return false;
	}

}


[System.Serializable]
public class LevelUpToFive: Achievement {	
	public LevelUpToFive(){
		Texture2D icon = Resources.Load (AchievementConstants.ACHIEVEMENT_LOCATION + "LevelUpTo5") as Texture2D;
		AchievementName = "One Giant Leap";
		AchievementIcon = icon;
		AchievementIconString = AchievementConstants.ACHIEVEMENT_LOCATION + "LevelUpTo5";
		AchievementDescription = "Leveled up to 5.";
		AchievementScore = 20;
	}
	
	public override bool CheckConditions (PlayerNew player)
	{
		if(player.Level >= 5){
			return true;
		}
		return false;
	}

}


[System.Serializable]
public class LevelUpToOneHundred: Achievement {	
	public LevelUpToOneHundred(){
		Texture2D icon = Resources.Load (AchievementConstants.ACHIEVEMENT_LOCATION + "LevelUpTo100") as Texture2D;
		AchievementName = "Master of Magenetism";
		AchievementIcon = icon;
		AchievementIconString = AchievementConstants.ACHIEVEMENT_LOCATION + "LevelUpTo100";
		AchievementDescription = "Mwahaha, witness...the power of magnetism! Leveled up to 100.";
		AchievementScore = 1000;
	}
	
	public override bool CheckConditions (PlayerNew player)
	{
		if(player.Level >= 100){
			return true;
		}
		return false;
	}

}
#endregion

#region "CompleteQuests"

[System.Serializable]
public class CompleteOneQuest: Achievement {	
	public CompleteOneQuest(){
		Texture2D icon = Resources.Load (AchievementConstants.ACHIEVEMENT_LOCATION + "Complete1Quest") as Texture2D;
		AchievementName = "Adventurer";
		AchievementIcon = icon;
		AchievementIconString = AchievementConstants.ACHIEVEMENT_LOCATION + "Complete1Quest";
		AchievementDescription = "Completed a quest.";
		AchievementScore = 10;
	}
	
	public override bool CheckConditions (PlayerNew player)
	{
		if(player.questsComplete.Count >= 1){
			return true;
		}
		return false;
	}

}


[System.Serializable]
public class CompleteThreeQuest: Achievement {	
	public CompleteThreeQuest(){
		Texture2D icon = Resources.Load (AchievementConstants.ACHIEVEMENT_LOCATION + "Complete3Quest") as Texture2D;
		AchievementName = "Take one quest. Take them all.";
		AchievementIcon = icon;
		AchievementIconString = AchievementConstants.ACHIEVEMENT_LOCATION + "Complete3Quest";
		AchievementDescription = "Completed 3 quests.";
		AchievementScore = 30;
	}
	
	public override bool CheckConditions (PlayerNew player)
	{
		if(player.questsComplete.Count >= 3){
			return true;
		}
		return false;
	}

}


[System.Serializable]
public class CompleteFiveQuest: Achievement {	
	public CompleteFiveQuest(){
		Texture2D icon = Resources.Load (AchievementConstants.ACHIEVEMENT_LOCATION + "Complete5Quest") as Texture2D;
		AchievementName = "Renowned Quester";
		AchievementIcon = icon;
		AchievementIconString = AchievementConstants.ACHIEVEMENT_LOCATION + "Complete5Quest";
		AchievementDescription = "Completed 5 quests.";
		AchievementScore = 50;
	}
	
	public override bool CheckConditions (PlayerNew player)
	{
		if(player.questsComplete.Count >= 5){
			return true;
		}
		return false;
	}

}
#endregion

#region "KillBosses"

[System.Serializable]
public class KillABoss: Achievement {	
	public KillABoss(){
		Texture2D icon = Resources.Load (AchievementConstants.ACHIEVEMENT_LOCATION + "KillBossSkeletonKing") as Texture2D;
		AchievementName = "BOSS Slayer";
		AchievementIcon = icon;
		AchievementIconString = AchievementConstants.ACHIEVEMENT_LOCATION + "KillBossSkeletonKing";
		AchievementDescription = "Killed a boss!";
		AchievementScore = 100;
	}
	
	public override bool CheckConditions (PlayerNew player)
	{
		if(player.BossesKilled.Count >= 1){
			return true;
		}
		return false;
	}

}
#endregion