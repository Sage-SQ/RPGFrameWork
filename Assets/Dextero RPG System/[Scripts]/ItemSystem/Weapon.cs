using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class Weapon : BuffItem {
	private int _maxDamage;
	private float _dmgVariance;	//Percent of max damage dealt, e.g. 0.2f means weapon can deal 20%-100% of max damage
	

	private DamageType _dmgType;	//Elemental Damage Type
	private int _dmgValue;
	private ProcType _procType;		//Type of proc, e.g. stun
	
	private float _procModifier;	//Proc Modifier: e.g. 2 for 2 life on hit
	private float _attackSpeed;		//Attacks per second 1.0f being 1 attack a second, 2.0 being 2 attacks per second
	private float _critChance; //Chance to critical hit. 0.25 = 25%
	private float _critDamage; //Percent damage dealt on critical hit. 0.25 being 25% extra damage (to the 200%_)
	private int _enchants;			//To use later for enchating items at merchant, max enchant is always 10

	
	public Weapon(){
		_maxDamage = 5;
		_dmgVariance = 0.2f;
		_dmgType = DamageType.Normal;
		_dmgValue = 0;
		_procType = ProcType.None;
		_procModifier= 0;
		_enchants = 0;
		_attackSpeed = 1.2f;
		_critChance = 0.0f;
		_critDamage = 0.0f;
	}
	
	private Weapon(int mDmg, float dmgV,DamageType dmgType, float attackSpeed,
		float critChance, ProcType proc, float procmod,float critDmg, int sockets, int usedSockets,
		int dmgVal, int hands, int enchants){
		_maxDamage = mDmg;
		_dmgVariance = dmgV;
		_dmgType = dmgType;
		_dmgValue = dmgVal;
		_procType = proc;
		_procModifier= procmod;
		_attackSpeed = attackSpeed;
		_critChance = critChance;
		_critDamage = critDmg;
		Sockets = sockets;
		UsedSockets = usedSockets;
		_enchants = enchants;
	}
	
	public int MaxDamage{
		get{return _maxDamage;}
		set{_maxDamage = value;}
	}
	
	public float DmgVariance{
		get{return _dmgVariance;}
		set{_dmgVariance = value;}
	}
	public DamageType DmgType{
		get{return _dmgType;}
		set{_dmgType = value;}
	}
	
	public int DmgValue{
		get{return _dmgValue;}
		set{_dmgValue = value;}
	}
	
	public ProcType Proc{
		get{return _procType;}
		set{_procType = value;}
	}
	
	public float ProcModifier{
		get{return _procModifier;}
		set{_procModifier = value;}
	}
	
	public float AttackSpeed{
		get{return _attackSpeed;}
		set{_attackSpeed = value;}
	}
	
	public float CritChance{
		get{return _critChance;}
		set{_critChance = value;}
	}
	
	public float CritDamage{
		get{return _critDamage;}
		set{_critDamage = value;}
	}
		
	public int Enchants {
		get{return _enchants;}
		set{_enchants = value;}
	}
	
	public int MaxHit{
		get{return ((int)((2.0f + CritDamage) * (MaxDamage + DmgValue)));}
	}
	
	public int MaxDPS{
		get{return (
				(int)
				(AttackSpeed * (MaxDamage+DmgValue))
				);}
	}
	
	public int MinDPS{
		get{return (
				(int)
				(AttackSpeed * ((MaxDamage * DmgVariance) +(DmgValue * DmgVariance)))
				);}
	}

	public string GetDPSString()
	{
		string damageType = DmgType != DamageType.Normal ? DmgType.ToString() : "";
		return MinDPS + " - " + MaxDPS + " (+" + DmgValue.ToString() + " " + damageType + ") ";
	}
	
	public string GetDamageString()
	{
		return (int)(MaxDamage * DmgVariance) + " - " + MaxDamage;
	}
	
	public override string ToolTip(){
		//string minDPS = ((int)(AttackSpeed * (MaxDamage*DmgVariance))).ToString ();
		//string maxDPS = ((int)(AttackSpeed * MaxDamage)).ToString ();
		
		string numBuffs = "";
		if(this.NumberOfBuffs() > 0){
			numBuffs ="\n" + GetBuffsString();
		}
		else{
			numBuffs = "";
		}
		
		
		//Format wording of proc correctly
		string procDesc = "";
		
		switch(Proc){
			case ProcType.GainLifeOnHit:
				procDesc = "\n" + string.Format ("Gain {0} health per physical attack",ProcModifier);
				break;
			case ProcType.GainManaOnHit:
				procDesc = "\n" + string.Format ("Gain {0} mana per physical attack",ProcModifier);
				break;
			case ProcType.Knockback:
				procDesc = "\n" + string.Format ("Has a {0}% chance to cause knockback",(ProcModifier*100).ToString("F2"));
				break;
			case ProcType.None:
				break;
			case ProcType.Poison:
				procDesc = "\n" + string.Format ("Has a {0}% chance to cause poison",(ProcModifier*100).ToString("F2"));
				break;
			case ProcType.Slow:
				procDesc = "\n" + string.Format ("Has a {0}% chance to slow targets attack speed",(ProcModifier*100).ToString("F2"));
				break;
			case ProcType.ConvertToLife:
				procDesc = "\n" + string.Format ("Converts {0}% of damage dealth to health",(ProcModifier*100).ToString("F2"));
				break;
			case ProcType.ConvertToMana:
				procDesc = "\n" + string.Format ("Converts {0}% of damage dealth to mana.",(ProcModifier*100).ToString("F2"));
				break;
			case ProcType.Stun:
				procDesc = "\n" + string.Format ("Has a {0}% chance to stun your target",(ProcModifier*100).ToString("F2"));
				break;
			case ProcType.GainEnergyOnHit:
				procDesc = "\n" + string.Format ("Gain {0} energy per physical attack",ProcModifier.ToString("0.00"));
				break;
			case ProcType.ConvertToEnergy:
				procDesc = "\n" + string.Format ("Converts {0}% of damage dealth to energy",(ProcModifier*100).ToString("F2"));
				break;
				
			
		}
		
		string moreStats = "";
		if(this.Proc != ProcType.None || this.CritChance != 0 || this.CritDamage > 0.0f){
			if(this.CritChance != 0){
				moreStats += "\n" + "+ " + (CritChance*100).ToString ("0") + "% Critical Hit Chance";
			}
			if(this.CritDamage > 0.0f){
				moreStats += "\n" + "Critical hits deal " + ((2.0+CritDamage)*100).ToString("0") + "% damage";
			}
			if(this.Proc != ProcType.None){
				moreStats += procDesc;
			}
		}
		string elementalDmg = "";
		if(this.DmgValue != 0){
				elementalDmg += "+ " + DmgValue + " " + DmgType.ToString() + " damage" + "\n";
		}
		
		string socketBuffs = "";
		if(EquippedSockets.Count > 0)
			socketBuffs += "\n \n" + "(" + UsedSockets.ToString() + ") Sockets: \n";
		foreach(SocketItem socket in EquippedSockets){
			socketBuffs += socket.GetBuffsString();
		}
		socketBuffs += "\n";
		
		//Set correct tooltype to corresponding type
		return "Damage: " +  (int)(MaxDamage * DmgVariance) + " - " + MaxDamage + "\n" +
				elementalDmg +
				"(DPS: " + MinDPS.ToString() + "-" + MaxDPS.ToString() + ")" + "\n" +
				"Attacks Per Second: " + AttackSpeed.ToString ("0.00") +
				numBuffs + 
				socketBuffs +
				moreStats;

		}
		
				
				
	}



public enum DamageType {
	Normal,
	Fire,
	Water,
	Earth,
	Wind,
	Thunder
}

public enum ProcType {
	None,
	Stun,				//ProcModifier is: PercentageChance
	Knockback,			//ProcModifier is: PercentageChance
	GainLifeOnHit,		//ProcModifier is: LifeGained
	ConvertToLife,			//ProcModifier is: Percentage Of Damage dealt that is stolen in health
	GainManaOnHit,		//ProcModifier is: ManaGained
	ConvertToMana,			//ProcModifier is: Percentage Of Damage dealt that is stolen in health
	Slow,				//ProcModifier is: PercentageChance
	Poison,				//ProcModifier is: PercentageChance
	GainEnergyOnHit,
	ConvertToEnergy
}