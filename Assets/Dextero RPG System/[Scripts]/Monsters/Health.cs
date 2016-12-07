using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {
	public float MaxHealth=100;
	public float CurrentHealth;
	public bool Dead;
	
	//Health bar
	public GameObject HealthBar;
	public const float HealthBarXScale = 1;
	
	// Use this for initialization
	void Start () {
		MaxHealth = this.GetComponent<MonsterAI>().MonsterHealth;
		//Set current health to max
		CurrentHealth=MaxHealth;
	}
	
	// Update is called once per frame
	void Update () {
		if(CurrentHealth<=0){
			CurrentHealth=0;
			Dead=true;
		}	
			
		//MAX HEALTH
			if(CurrentHealth>=MaxHealth)CurrentHealth=MaxHealth;
			
			//WHEN DEATH IS UPON HIM
		if(Dead){
				//TELL THE AI SCRIPT HE IS DEAD
			MonsterAI AI=(MonsterAI)GetComponent("FreeAI");
			if(AI){
				if(AI.isDead){}
				else AI.isDead=true;
			}
		}
		
		
		if(this.CompareTag ("Enemy") && HealthBar != null){
			float hpScale = HealthBarXScale * (CurrentHealth / MaxHealth);
			HealthBar.transform.localScale = new Vector3(hpScale,
				HealthBar.transform.localScale.y,
				HealthBar.transform.localScale.z);
		}
		
	}
	
	void OnGUI(){
		Vector3 pos3d = transform.position + Vector3.up * 0.1f;
		Vector3 pos = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().WorldToScreenPoint(pos3d);
		pos.y = Screen.height - pos.y;
		float BarLength = 100 * (CurrentHealth / MaxHealth) + 40;
		Rect r = new Rect(pos.x-BarLength/2, pos.y-20/2 -30, BarLength, 20);
		
		GUI.color = Color.red;
		GUI.Box (r,CurrentHealth.ToString());
	}
}
