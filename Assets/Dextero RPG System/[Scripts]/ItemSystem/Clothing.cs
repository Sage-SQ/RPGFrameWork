using UnityEngine;

[System.Serializable]
public class Clothing : BuffItem {
	public EquipmentSlot _slot;	//store the slot the jewelry will be in
	
	public Clothing() {
		_slot = EquipmentSlot.Chest;
	}
	
	public Clothing(EquipmentSlot slot) {
		_slot = slot;
	}
	
	public EquipmentSlot Slot{
		get {return _slot;}
		set {_slot = value;}
	}
}

public enum EquipmentSlot {
	Helmet,
	Chest,
	Gloves,
	Pants,
	Boots,
	Back
}