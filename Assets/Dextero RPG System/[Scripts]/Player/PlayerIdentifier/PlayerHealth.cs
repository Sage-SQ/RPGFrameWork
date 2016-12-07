using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour {
	
	private PlayerNew _pc;
	public bool Invincible;
	private GlobalPrefabs globalPrefabs;
	void Start(){
		if(_pc == null){
			_pc = GetComponent<PlayerNew>();
		}
		
		if(globalPrefabs == null){
			globalPrefabs = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GlobalPrefabs>();
		}
	}
	
	public void DealDamage(int damage){
		int damageToDeal = (int)Random.Range(damage*0.3f,damage+1);
		int baseDamage = (int)(damageToDeal * Random.Range (0.1f,0.2f));
		damageToDeal -= baseDamage;
		if(!Invincible){
			//Armor blocks it
			damageToDeal -= _pc.PlayerArmor;
			
			
			GameObject dmgTxt = Instantiate(globalPrefabs.floatingDamageText,transform.position,Quaternion.identity) as GameObject;
			dmgTxt.transform.parent = this.transform;
			FloatingTextGUI dmgText = dmgTxt.GetComponent<FloatingTextGUI>();
			dmgText.PlayerDamage("",_pc.transform.position,0.5f);
			
			//Chance to block (damage blocked always equal to Pc.DamageBlocked)
			int blockRandom = Random.Range (0,100+1);
			
			if(blockRandom < (_pc.PlayerChanceToBlock*100)){
				
				damageToDeal -= _pc.PlayerDamageBlocked;
				
				dmgText.AddToText(ToolTipStyle.Italic + "\t" + ToolTipStyle.Small + "Block!" + ToolTipStyle.EndSize + ToolTipStyle.EndItalic);
			}
			
			//deal the damage
			if(damageToDeal < 0)
				damageToDeal = 0;
			
			damageToDeal += baseDamage; //will always do some damage
			
			dmgText.AddToText(ToolTipStyle.Break + ToolTipStyle.Brown + "-" + damageToDeal.ToString() + ToolTipStyle.EndColor);
			_pc.GetVital((int)VitalName.Health).CurValue -= damageToDeal;
		}
	}
	
	public void DealFallDamage(int damage){
		GameObject dmgTxt = (GameObject)Instantiate(globalPrefabs.floatingDamageText);
		dmgTxt.transform.parent = this.transform;
		FloatingTextGUI dmgText = dmgTxt.GetComponent<FloatingTextGUI>();
		string damageString = ToolTipStyle.Bold + ToolTipStyle.Red + "-" + damage.ToString() + ToolTipStyle.EndColor + ToolTipStyle.EndBold;
		dmgText.PlayerDamage(damageString,this.transform.position,1.5f);
		_pc.GetVital((int)VitalName.Health).CurValue -= damage;
	}
	
	public int CurrentHealth {
		get{return CheckHealth();}
	}
	public int MaxHealth {
		get{return CheckMaxHealth();}
	}
	
	int CheckHealth(){
		return _pc.GetVital((int)VitalName.Health).CurValue;
	}
	
	int CheckMaxHealth(){
		return _pc.GetVital((int)VitalName.Health).MaxValue;
	}
}
