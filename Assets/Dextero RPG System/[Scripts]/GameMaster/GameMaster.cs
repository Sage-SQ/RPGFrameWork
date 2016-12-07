using UnityEngine;
using System.Collections;

public class GameMaster : MonoBehaviour {
	
	public Transform respawnPos;
	public GameObject warriorClassPrefab;
	
	void Awake(){
		string SaveLoadSlot = PlayerPrefs.GetInt("Player_SaveLoad_Slot",1000).ToString() + "_";
		PlayerClass classToSpawn =(PlayerClass)PlayerPrefs.GetInt(SaveLoadSlot+"Player_Class",0); //change for diff save files
		GameObject player = new GameObject();
		
		if(classToSpawn == PlayerClass.Warrior)
			player = Instantiate(warriorClassPrefab) as GameObject;
		//else if class is mage spawn mage etc
		
		respawnPos = GameObject.FindGameObjectWithTag("Respawn").transform;//Later implement multiple respawn points
		if(respawnPos != null) player.transform.position = respawnPos.position;
		else{
			player.transform.position = new Vector3(0,0,0);
			Debug.LogError("NO RESPAWN POINT!");
		}
		player.name = "PlayerCharacter";
	}
	
	void Start(){
		
		
		QuestItemsClasses.AddAllItems();
		CraftedItemsClasses.AddAllItems();
		CraftMaterialsClasses.AddAllItems();
		PremadeItemsClasses.AddAllItems();
		
		LoadCharacter();
	}
	
	void LoadCharacter(){
		GameSettings gsScript  = GetComponent<GameSettings>();
		
		//load character data
		gsScript.LoadCharacterData();
	}
	
}
