using UnityEngine;


//Maybe needs Consumable type? Health Potion, Mana Potion, Strength Potion etc?


[System.Serializable]
public class Consumable : BuffItem {
	private VitalName _vital;				//a list of vitals to heal
	private int _amountToHeal;		//the amount to heal each vital
	private ConsumableType _type;
	private int _tier;
	
	public Consumable() {
		ItemType = ItemEquipType.Consumable;
		RequiredLevel = 1;
	}
	
	public Consumable(VitalName v, int a,int tier) {
		_vital = v;
		_amountToHeal = a;
		_tier = tier;
		ItemType = ItemEquipType.Consumable;
	}
	
	public int Tier{
		get {return _tier;}
		set {_tier = value;}
	}
	
	public ConsumableType ConsumType{
		get {return _type;}
		set {_type = value;}
	}
	
	public VitalName VitalToRestore{
		get {return _vital;}
		set {_vital = value;}
	}
	
	public int AmountToHeal{
		get {return _amountToHeal;}
		set {_amountToHeal = value;}
	}
	
	public string TierRN(){
		string tierRN = "";
		if(Tier == 1)
			tierRN = "I";
		else if(Tier == 10)
			tierRN = "II";
		else if(Tier == 100)
			tierRN = "III";
		else if(Tier == 1000)
			tierRN = "IV";
		else
			tierRN = "V";
		
		return tierRN;
	}
	
	public override string ToolTip(){
		

		
		return 	"Tier " + TierRN() + " potion";
					
	}
}

public enum ConsumableType {
	Health,
	Mana,
	Energy
}