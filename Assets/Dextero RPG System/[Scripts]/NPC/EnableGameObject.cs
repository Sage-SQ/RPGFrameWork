using UnityEngine;
using System.Collections;

public class EnableGameObject : MonoBehaviour {
	
	public int ID;
	public GameObject[] gameObjectToEnable;
	
	// Use this for initialization
	public void EnableGameObjects(){
		foreach(GameObject go in gameObjectToEnable){
			go.SetActive(true);
		}
	}
	
	public void DisableGameObjects(){
		foreach(GameObject go in gameObjectToEnable){
			go.SetActive(false);
		}
	}
}
