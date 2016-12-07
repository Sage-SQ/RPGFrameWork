using UnityEngine;
using System.Collections;

public class TextFaceCamera : MonoBehaviour {
	
	private Transform MainCamera;
	
	// Use this for initialization
	void Start () {
		if(MainCamera == null){
			MainCamera = GameObject.FindGameObjectWithTag("MainCamera").transform;
		}
	}
	
	// Update is called once per frame
	void Update () {
		transform.LookAt (MainCamera);
		transform.rotation = MainCamera.transform.rotation;
	}
}
