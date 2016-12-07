using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemSpawner : MonoBehaviour {

	public int chestItemLevel = 0;				//Roughly the level (required level) of items the chest will give
	public string itemsInChest = "random";		//Number of items chest will give
	public int numToPopulate = 0;
	public string chestRarity = "random"; 		//Rarity of chest, higher rarity = higher chance of better items
	public string chestSpawnType = "random";	//What item types to spawn?
	public Vector3 positionToSpawn = new Vector3();
	public List<Item> loot = new List<Item>();
	//Prefabs
	public GameObject itemPrefab;
	public GameObject goldPrefab;
	public GameObject CM_Wood;
	public GameObject CM_Ore;
	public GameObject CM_Cloth;
	
	void Awake(){
		itemsInChest = "random";
		chestRarity = "random"; 
		chestSpawnType = "random";
	}

	public void SpawnItems(){
		foreach (Item i in loot) {
			Vector3 offset = Random.Range (-2.0f,2.0f) * Vector3.forward + Random.Range (-2.0f,2.0f) * Vector3.right;
			GameObject prefabToSpawn = GetPrefab(i);
			GameObject temp = Instantiate (prefabToSpawn,positionToSpawn + 0.5f * Vector3.up + offset,Quaternion.identity) as GameObject;
			LootItem li = temp.GetComponent<LootItem>();
			li.myItem = i;
			temp.transform.parent = this.transform;
		}
	}
	
	public void SpawnAnItem(Vector3 pos, Item item){
		Vector3 offset = Random.Range (-2.0f,2.0f) * Vector3.forward + Random.Range (-2.0f,2.0f) * Vector3.right;
		GameObject prefabToSpawn = GetPrefab(item);
		GameObject temp = Instantiate (prefabToSpawn,pos + 0.5f * Vector3.up + offset,Quaternion.identity) as GameObject;
		LootItem li = temp.GetComponent<LootItem>();
		li.myItem = item;
		temp.transform.parent = this.transform;
	}
	
	public void PopulateChest(){
		loot = new List<Item>();
		
		bool isNum = int.TryParse(itemsInChest,out numToPopulate);
		if(!isNum){
			if(itemsInChest == "random"){
				int randomNum = Random.Range (0,100+1);
				if(randomNum < 55){
					numToPopulate = 0;
				}
				else {
					numToPopulate = Random.Range (0,1+1);
				}
			}
			else {
				numToPopulate = 1;
			}
		}

		for(int cnt = 0; cnt < numToPopulate; cnt++){
			loot.Add (ItemGenerator.CreateItem(chestSpawnType,chestRarity,chestItemLevel));
		}
	}
	
	GameObject GetPrefab(Item i){
		if(i.Name.Contains("Gold")){
			return goldPrefab;
		}
		else if(i.ItemType == ItemEquipType.CraftItem){
			if(i.Name.Contains("Wood")){
				return CM_Wood;
			}
			else if(i.Name.Contains("Ore")){
				return CM_Ore;
			}
			else if(i.Name.Contains("Cloth")){
				return CM_Cloth;
			}
		}
		
		return itemPrefab;
	}
	
}
