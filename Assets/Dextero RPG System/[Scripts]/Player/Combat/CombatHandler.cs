using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatHandler : MonoBehaviour {
	
	public static bool isDead;
	public static bool canDealDamage = true;
	
	private float AttackSpeed=1.5f;
	private bool startRespawn;
	private float timeSinceLastAttack;
	private PlayerNew playerNew;
	private GlobalPrefabs globalPrefabs;
	private int Damage=60;
	
	//For Procs
	private int physicalDamageDealt = 0;
	// Use this for initialization
	void Start () {
		
		playerNew = GetComponent<PlayerNew>();
		
		if(globalPrefabs == null){
			globalPrefabs = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GlobalPrefabs>();
		}
		
		startRespawn =true; //Will start respawn if needed
	
	}
	
	// Update is called once per frame
	void Update () {
		
		//Attack speed is the time between attacks
		AttackSpeed = 1 / playerNew.AttackSpeed;

		PlayerHealth playersHealth=(PlayerHealth)GetComponent("PlayerHealth");
		if(playersHealth){
			if(playersHealth.CurrentHealth<=0)
				isDead=true;
		}
		
		timeSinceLastAttack += Time.deltaTime;
		
		//Check player is not dead before attacking
		if(isDead){
			if(startRespawn){
				StartCoroutine("Death");	
			}
		}
		else{
			if(Input.GetKeyDown(KeyCode.Mouse0) || Input.GetKeyDown(KeyCode.Mouse1) && playerNew.CanFight){
				RaycastHit hitdist = new RaycastHit();
				Ray ray = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
				if(Physics.Raycast(ray, out hitdist))
				{
				    string hitTag = hitdist.transform.tag;
				 	float dist = Vector3.Distance(transform.position,hitdist.transform.position);
				    if (hitTag == "Enemy" && dist < playerNew.AttackRange)
				    {
						float angleToTarget = Mathf.Atan2((hitdist.transform.position.x - transform.position.x), (hitdist.transform.position.z - transform.position.z)) * Mathf.Rad2Deg;
						transform.eulerAngles = new Vector3(0,angleToTarget, 0);
				        canDealDamage = timeSinceLastAttack > AttackSpeed ? true : false;
						
						if(canDealDamage){
							timeSinceLastAttack = 0;
							DealDamage(hitdist.transform);
							canDealDamage = false;
						}
				    }
				}
				
			}
		}
	}
	
	void DealDamage(Transform target){
		Health hp=(Health)target.transform.GetComponent("Health");
		if(hp){
			GameObject dmgTxt = (GameObject)Instantiate(globalPrefabs.floatingDamageText);
			dmgTxt.transform.parent = target;
			FloatingTextGUI ftg = dmgTxt.GetComponent<FloatingTextGUI>();
			ftg.SetDmgInfo("",target.position);
			MonsterAI fa = (MonsterAI)target.GetComponent<MonsterAI>();
			physicalDamageDealt = 0;
			int DamageDealt = CalculateWeaponDamage (ftg,fa);
			ProcHandler(fa);
			fa.inCombat = true;
			hp.CurrentHealth=hp.CurrentHealth-Damage;
			playerNew.playerState = PlayerState.Combat;
			playerNew.CheckForExitCombat();
		}
	}
	
	private int CalculateWeaponDamage(FloatingTextGUI ftg, MonsterAI target){
		Weapon wep = playerNew.EquipedWeapon as Weapon;
		//For now all damage scales off Strength, this is set in scaling of PlayerNew
		
		int critNum = Random.Range (0,100+1);
		bool critHit = false;
		float critChance = playerNew.CritChance;
		float critMultiplier = playerNew.CritDamage;
		
		if(critNum < (critChance*100)){
			critHit = true;
		}
		else {
			critHit = false;
		}
		
		
		//Random WepDamage
		physicalDamageDealt = Damage = Random.Range ((int)(playerNew.MaxDamage * playerNew.DmgVariance),playerNew.MaxDamage+1);
		
		if(critHit){
			Damage = (int)(Damage * critMultiplier);
		}

		string damageString = ToolTipStyle.Red + Damage.ToString() + ToolTipStyle.EndColor;
		ftg.AddToText(damageString);
		
		//Random Elemental Damage
		if(playerNew.EquipedWeapon != null){
			if(wep.DmgType != DamageType.Normal){
				int eleDmg = Random.Range ((int)(wep.DmgValue * wep.DmgVariance),wep.DmgValue+1);
				
				if(critHit){
					eleDmg = (int)(eleDmg * critMultiplier);
				}
				
				Damage += eleDmg;
				//find color
				string eleDmgString = FindElementalDamageString(eleDmg,wep);
				ftg.AddToText(eleDmgString);
			}
		}
		
		if(critHit){
			ftg.damageText.text = ToolTipStyle.Large + ToolTipStyle.Italic + ftg.damageText.text + ToolTipStyle.EndItalic +  ToolTipStyle.EndSize;
		}
		
		return Damage;
	}
	
	private void ProcHandler(MonsterAI fa){
		
		Weapon wep = playerNew.EquipedWeapon as Weapon;
		if(wep != null){
			if(wep.Proc == ProcType.Slow || wep.Proc == ProcType.Poison ||
				wep.Proc == ProcType.Knockback || wep.Proc == ProcType.Stun)
			{
				int randomNum = Random.Range (0,100+1);
				if( randomNum < (wep.ProcModifier*100)){
					switch(wep.Proc){
						case ProcType.Slow:
							fa.SlowAttackSpeed(0.3f);
							break;
						case ProcType.Poison:
							int damageToDeal = (int)(fa.MonsterHealth*0.01);
							fa.UseDot("Poison",damageToDeal,5,0.5f);
							break;
						case ProcType.Knockback:
							fa.KnockBackSelf();
							break;
						case ProcType.Stun:
							fa.StunSelf(3.0f);	
							break;
						
					}
				}
			}
			else {
				switch(wep.Proc){
					case ProcType.GainLifeOnHit:
						playerNew.GetVital((int)VitalName.Health).CurValue += (int)wep.ProcModifier;
						GameObject infoTxt = (GameObject)Instantiate(globalPrefabs.floatingBuffText);
						infoTxt.transform.parent = this.transform;
						FloatingTextGUI ftg = infoTxt.GetComponent<FloatingTextGUI>();
						var info = ToolTipStyle.Green + "+" + ((int)wep.ProcModifier).ToString() + ToolTipStyle.EndColor;
						ftg.SetInfo(info,transform.position);
						break;
					case ProcType.GainEnergyOnHit:
						playerNew.GetVital((int)VitalName.Energy).CurValue += (int)wep.ProcModifier;
						infoTxt = (GameObject)Instantiate(globalPrefabs.floatingBuffText);
						infoTxt.transform.parent = this.transform;
						ftg = infoTxt.GetComponent<FloatingTextGUI>();
						info = ToolTipStyle.Yellow + "+" + ((int)wep.ProcModifier).ToString() + ToolTipStyle.EndColor;
						ftg.SetInfo(info,transform.position);
						break;
					case ProcType.GainManaOnHit:
						playerNew.GetVital((int)VitalName.Mana).CurValue += (int)wep.ProcModifier;
						infoTxt = (GameObject)Instantiate(globalPrefabs.floatingBuffText);
						infoTxt.transform.parent = this.transform;
						ftg = infoTxt.GetComponent<FloatingTextGUI>();
						info = ToolTipStyle.Blue + "+" + ((int)wep.ProcModifier).ToString() + ToolTipStyle.EndColor;
						ftg.SetInfo(info,transform.position);
						break;
					case ProcType.ConvertToLife:
						playerNew.GetVital((int)VitalName.Health).CurValue +=  (int)(wep.ProcModifier * physicalDamageDealt);
						infoTxt = (GameObject)Instantiate(globalPrefabs.floatingBuffText);
						infoTxt.transform.parent = this.transform;
						ftg = infoTxt.GetComponent<FloatingTextGUI>();
						info = ToolTipStyle.Green + "+" + ((int)(wep.ProcModifier* physicalDamageDealt)).ToString() + ToolTipStyle.EndColor;
						ftg.SetInfo(info,transform.position);
						break;
					case ProcType.ConvertToEnergy:
						playerNew.GetVital((int)VitalName.Energy).CurValue += (int)(wep.ProcModifier * physicalDamageDealt);
						infoTxt = (GameObject)Instantiate(globalPrefabs.floatingBuffText);
						infoTxt.transform.parent = this.transform;
						ftg = infoTxt.GetComponent<FloatingTextGUI>();
						info = ToolTipStyle.Yellow + "+" +((int)(wep.ProcModifier* physicalDamageDealt)).ToString() + ToolTipStyle.EndColor;
						ftg.SetInfo(info,transform.position);
						break;
					case ProcType.ConvertToMana:
						playerNew.GetVital((int)VitalName.Mana).CurValue += (int)(wep.ProcModifier * physicalDamageDealt);
						infoTxt = (GameObject)Instantiate(globalPrefabs.floatingBuffText);
						infoTxt.transform.parent = this.transform;
						ftg = infoTxt.GetComponent<FloatingTextGUI>();
						info = ToolTipStyle.Blue + "+" + ((int)(wep.ProcModifier* physicalDamageDealt)).ToString() + ToolTipStyle.EndColor;
						ftg.SetInfo(info,transform.position);
						break;
					default:
						break;
				}
			}
			
		}
	}
	
	private string FindElementalDamageString(int eleDmg, Weapon wep){
		string eledmgstring = "";
		string eleDmgS = eleDmg.ToString();
		switch(wep.DmgType){
			case DamageType.Earth:
				eledmgstring = ToolTipStyle.Olive + eleDmgS + ToolTipStyle.EndColor;
				break;
			case DamageType.Fire:
				eledmgstring = ToolTipStyle.Orange + eleDmgS + ToolTipStyle.EndColor;
				break;
			case DamageType.Thunder:
				eledmgstring = ToolTipStyle.White + eleDmgS + ToolTipStyle.EndColor;
				break;
			case DamageType.Water:
				eledmgstring = ToolTipStyle.Cyan + eleDmgS + ToolTipStyle.EndColor;
				break;
			case DamageType.Wind:
				eledmgstring = ToolTipStyle.Grey + eleDmgS + ToolTipStyle.EndColor;
				break;
		}
		return eledmgstring;
	}

	IEnumerator Death(){
		startRespawn = false;
		playerNew.Alive = false;
		yield return new WaitForSeconds(0.5f);
		playerNew.Respawn();
		isDead = false;
		startRespawn = true;
	}
}
