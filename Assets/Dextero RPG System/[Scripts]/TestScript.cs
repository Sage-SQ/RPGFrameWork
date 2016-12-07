using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class TestScript : MonoBehaviour {
	
	public GameObject playerGO;
	public PlayerNew player;
	
	//Type of Item
	public int LevelOfItem = 1;
	public string TypeOfItem = "random";
	public string RarityOfItem = "random";
	
	public int ExpToAdd;
	public int GoldToAdd;
	
	//Attribute
	public AttributeName attributeToIncrease;
	public int amountToIncreaseAttribute;
	
	// Use this for initialization
	public void Start () {
		playerGO = GameObject.FindGameObjectWithTag("Player");
		if(playerGO != null) player = playerGO.GetComponent<PlayerNew>();	
	}
	
	public void GiveCraftMats(){
				
		for (int i = 0; i < 20; i++) {
			player.AddItem(CraftMaterialsClasses.Copper());
			player.AddItem(CraftMaterialsClasses.Linen());
			player.AddItem(CraftMaterialsClasses.Wood());
		}
	}
	
	public void GiveAllRaritys(){
		for (int i = 0; i < Enum.GetNames(typeof(RarityTypes)).Length; i++) {
			Weapon x = PremadeItemsClasses.StartingSword() as Weapon;
			x.Rarity = (RarityTypes)i;
			player.AddItem(x);
		}

	}
	
	public void GiveRandomItem(){
		player.AddItem(ItemGenerator.CreateItem ("random","random",player.Level));
	}
	
	public void GiveRandomLevelItem(){
		player.AddItem(ItemGenerator.CreateItem (TypeOfItem,RarityOfItem,LevelOfItem));
	}
	
	public void GiveSockets(){
		for (int i = 0; i < 4; i++) {
			player.AddItem(ItemGenerator.CreateItem ("socket","legendary",20));
			player.AddItem(ItemGenerator.CreateItem ("socket","legendary",60));
			player.AddItem(ItemGenerator.CreateItem ("socket","legendary",100));
			
		}
	}
	
	public void GiveConsumables(){
		for (int i = 0; i < 100; i++) {
			player.AddItem(ItemGenerator.CreateItem ("consum","legendary",100));	
		}
	}
	
	public void AddExp(){
		player.AddExp(ExpToAdd);
	}
	
	public void AddGold(){
		player.ModifyGold(GoldToAdd);
	}
	
	public void IncreaseAttribute(){
		player.ModifyAttribute(attributeToIncrease,amountToIncreaseAttribute);
	}
	
	void OnGUI(){
		
		GUILayout.BeginArea(new Rect(Screen.width - 205, Screen.height - 400,200, 440)); 
		GUILayout.Box ("Press H to show Help");
		if (GUILayout.Button("ClearInventory"))
		{
			player.Inventory = new List<Item>();
		}
		
		if (GUILayout.Button("ClearLootOnFloor"))
		{
			GameObject Loot = GameObject.FindGameObjectWithTag("ItemSpawner");
			for (int i = 0; i < Loot.transform.childCount; i++) {
				Destroy (Loot.transform.GetChild (i).gameObject);
			}
		}
		
		if(GUILayout.Button("Spawn Item on Player")){
			GameObject loot = GameObject.FindGameObjectWithTag("ItemSpawner");
			loot.GetComponent<ItemSpawner>().positionToSpawn = player.transform.position;
			loot.GetComponent<ItemSpawner>().chestRarity = "random";
			loot.GetComponent<ItemSpawner>().chestSpawnType = "random";
			loot.GetComponent<ItemSpawner>().itemsInChest = "1";
			loot.GetComponent<ItemSpawner>().chestItemLevel = UnityEngine.Random.Range(0,100+1);
			
			//Clear loot and populate with random items
			loot.GetComponent<ItemSpawner>().PopulateChest();
			loot.GetComponent<ItemSpawner>().SpawnItems();
		}
		
		if (GUILayout.Button("GiveCraftMats"))
		{
			GiveCraftMats();
		}
		
		if (GUILayout.Button("GiveRaritys"))
		{
			GiveAllRaritys();
		}
		
		if (GUILayout.Button("GiveRandomItem"))
		{
			GiveRandomItem();
		}
		
		if (GUILayout.Button("GiveSockets"))
		{
			GiveSockets();
		}
		
		if (GUILayout.Button("GiveConsumables"))
		{
			GiveConsumables();
		}
		
		
		GUILayout.BeginHorizontal();
			if (GUILayout.Button("Give Item:"))
			{
				GiveRandomLevelItem();
			}
		
			TypeOfItem = GUILayout.TextField(TypeOfItem);
			RarityOfItem = GUILayout.TextField(RarityOfItem);
			LevelOfItem = int.Parse(GUILayout.TextField(LevelOfItem.ToString()));
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
			if (GUILayout.Button("Add Exp:"))
			{
				AddExp();
			}
		
			ExpToAdd = int.Parse(GUILayout.TextField(ExpToAdd.ToString()));
		
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
			if (GUILayout.Button("Add Gold:"))
			{
				AddGold();
			}
		
			GoldToAdd = int.Parse(GUILayout.TextField(GoldToAdd.ToString()));
		
		GUILayout.EndHorizontal();
		
		if (GUILayout.Button("+10 Strength "))
		{
			player.ModifyAttribute(AttributeName.Strength,10);
		}
		
		if (GUILayout.Button("+10 Dexterity "))
		{
			player.ModifyAttribute(AttributeName.Dexterity,10);
		}
		
		if (GUILayout.Button("+10 Intelligence "))
		{
			player.ModifyAttribute(AttributeName.Intelligence,10);
		}
		
		if (GUILayout.Button("+10 Vitality "))
		{
			player.ModifyAttribute(AttributeName.Vitality,10);
		}
		
		
		
		if (GUILayout.Button("Respawn"))
		{
			playerGO.GetComponent<PlayerHealth>().DealDamage(2000000000);
		}
		
		GUILayout.EndArea();
	}
}
