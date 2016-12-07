using UnityEngine;
using System.Collections;

public class Cull : MonoBehaviour {
	public bool CullAtStart = true;
	void Start () {
		if(CullAtStart)	GetComponent<Renderer>().enabled=false;
	}
}
