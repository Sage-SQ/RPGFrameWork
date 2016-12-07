using UnityEngine;
using System.Collections;

public class HarvestableObject : MonoBehaviour {
	
	private PlayerNew player;
	private GameObject playerCamera;
	public bool harvesting;
	
	public HarvestItem itemDropped;
	private Item HarvestedItem;
	public int materialsRemaining;
	
	void Start () {
		
		FindHarvestItem();
		if(player == null){
			player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerNew>();
		}
		
		if(playerCamera == null){
			playerCamera = GameObject.FindGameObjectWithTag("MainCamera");
		}
		
	}
	
	void Update(){
		if(materialsRemaining == 0) return;
		
		if(playerCamera.GetComponent<ActionRPGCamera>().target != this.transform){
			harvesting = false;
			StopCoroutine("Harvest");
			
		}
	}
	
	void OnMouseUp(){
		
		if(Vector3.Distance (player.transform.position,transform.position) > 2.0f){
			return;
		}

		float angleToTarget = Mathf.Atan2((transform.position.x - player.transform.position.x), (transform.position.z - player.transform.position.z)) * Mathf.Rad2Deg;
		player.transform.eulerAngles = new Vector3(0,angleToTarget, 0);
		
		if(materialsRemaining == 0) return;
	
		if(!harvesting){
			harvesting = true;
			StartCoroutine("Harvest");
		}
		player.playerState = PlayerState.Harvesting;
		playerCamera.GetComponent<ActionRPGCamera>().target = this.transform;
	}
	
	void OnGUI(){
		if(materialsRemaining == 0) return;
		if(harvesting){
			if(GUI.Button (new Rect(Screen.width/2-100,150,200,40),"Stop Harvesting")){
				player.playerState = PlayerState.Normal;
				playerCamera.GetComponent<ActionRPGCamera>().target = player.transform;
				harvesting = false;
			}
		}
	}
	
	void FindHarvestItem(){
		switch (itemDropped) {
		case HarvestItem.Wood:
			HarvestedItem = CraftMaterialsClasses.Wood();
			break;
		case HarvestItem.Linen:
			HarvestedItem = CraftMaterialsClasses.Linen();
			break;
		case HarvestItem.Copper:
			HarvestedItem = CraftMaterialsClasses.Copper();
			break;
		default:
			HarvestedItem = CraftMaterialsClasses.Wood();
			break;
		}
	}
	
	IEnumerator Harvest(){
		if(materialsRemaining == 0){
			harvesting = false;
			player.playerState = PlayerState.Normal;
			playerCamera.GetComponent<ActionRPGCamera>().target = player.transform;
			Destroy (gameObject,3.0f);
			return false;
		}
		yield return new WaitForSeconds(0.5f);
		HarvestedItem = DeepCopy.CopyItem(HarvestedItem);
		ItemSpawner itemspawner = GameObject.FindGameObjectWithTag("ItemSpawner").GetComponent<ItemSpawner>();
		itemspawner.SpawnAnItem(this.transform.position,HarvestedItem);
		materialsRemaining -= 1;
		yield return new WaitForSeconds(0.3f);
		StartCoroutine("Harvest");
	}
}

public enum HarvestItem{
	Wood,
	Copper,
	Linen
}
