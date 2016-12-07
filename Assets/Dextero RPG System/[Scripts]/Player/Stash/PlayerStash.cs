using UnityEngine;
using System.Collections;

public class PlayerStash : MonoBehaviour {

	private PlayerNew player;
	
	void Start(){
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerNew>();
	}
	
	// Update is called once per frame
	void OnMouseDown () {
		player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerNew>();
		if(Vector3.Distance(transform.position,player.transform.position) < 3.0f){
			Messenger.Broadcast ("ShowStashWindow");
			StashWindowGUI.openedStash = this;
		}
		else {
			Debug.Log ("Too far");
		}
	}
	
	void Update(){
		if(Vector3.Distance(transform.position,player.transform.position) > 3.0f){
			Messenger.Broadcast("CloseStashWindow");
		}
	}
	
	public void MoveFromBankToPlayer(int arrayNum){
		bool added = player.AddItem (player.Stash[arrayNum]);
		if(added){
			player.Stash.RemoveAt(arrayNum);
		}
	}
}
