using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(TestScript))]
public class TestScriptEditor : Editor
{
	override public void OnInspectorGUI()
	{	
		TestScript testScript = target as TestScript;
		
		testScript.playerGO = EditorGUILayout.ObjectField("Player",testScript.playerGO, typeof(GameObject)) as GameObject;
		
		if (GUILayout.Button("GiveCraftMats"))
		{
			testScript.GiveCraftMats();
		}
		
		if (GUILayout.Button("GiveRaritys"))
		{
			testScript.GiveAllRaritys();
		}
		
		if (GUILayout.Button("GiveRandomItem"))
		{
			testScript.GiveRandomItem();
		}
		
		if (GUILayout.Button("GiveSockets"))
		{
			testScript.GiveSockets();
		}
		
		if (GUILayout.Button("GiveConsumables"))
		{
			testScript.GiveConsumables();
		}
		
		if (GUILayout.Button("ClearInventory"))
		{
			testScript.player.Inventory = new List<Item>();
		}
		
		GUILayout.BeginHorizontal();
			if (GUILayout.Button("Give Item:"))
			{
				testScript.GiveRandomLevelItem();
			}
		
			testScript.TypeOfItem = GUILayout.TextField(testScript.TypeOfItem);
			testScript.RarityOfItem = GUILayout.TextField(testScript.RarityOfItem);
			testScript.LevelOfItem = int.Parse(GUILayout.TextField(testScript.LevelOfItem.ToString()));
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
			if (GUILayout.Button("Add Exp:"))
			{
				testScript.AddExp();
			}
		
			testScript.ExpToAdd = int.Parse(GUILayout.TextField(testScript.ExpToAdd.ToString()));
		
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
			if (GUILayout.Button("Add Gold:"))
			{
				testScript.AddGold();
			}
		
			testScript.GoldToAdd = int.Parse(GUILayout.TextField(testScript.GoldToAdd.ToString()));
		
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
			if (GUILayout.Button("Increase "))
			{
				testScript.IncreaseAttribute();
			}
		
			testScript.attributeToIncrease = (AttributeName)EditorGUILayout.EnumPopup (testScript.attributeToIncrease);
		
			GUILayout.Label(" by ");
			
			testScript.amountToIncreaseAttribute = int.Parse(GUILayout.TextField(testScript.amountToIncreaseAttribute.ToString()));
		
		GUILayout.EndHorizontal();
		
		if (GUILayout.Button("Respawn"))
			{
				testScript.playerGO.GetComponent<PlayerHealth>().DealDamage(2000000000);
			}
	}
}
