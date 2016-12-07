using UnityEngine;

[System.Serializable]
public class SocketItem : BuffItem {
	
	private SocketTypes _socketType;
	private int socketTier;
	
	public SocketTypes SocketType {
		get{return _socketType;}
		set{_socketType = value;}
	}

	public int SocketTier {
		get{return socketTier;}
		set{socketTier = value;}
	}
		
	public SocketItem(){
		Name = "Opal Socket";
		ItemType = ItemEquipType.Socket;
		Rarity = RarityTypes.Rare;
		SocketTier = 1;
		RequiredLevel = 0;
		_socketType = SocketTypes.Ruby;
	}
	
	public SocketItem(SocketTypes socketType){
		_socketType = socketType;
	}
	
	public string SocketString(){
		return Name + "( +" + GetBuffsString() + ")";
	}
	
	public string TierRN(){
		string tierRN = "";
		if(SocketTier == 1)
			tierRN = "I";
		else if(SocketTier == 2)
			tierRN = "II";
		else if(SocketTier == 3)
			tierRN = "III";
		else if(SocketTier == 4)
			tierRN = "IV";
		else
			tierRN = "V";
		
		return tierRN;
	}
	
	public override string ToolTip(){
		return 	"Tier " + TierRN () + " socket";
	}
	
}

public enum SocketTypes {
	//e.g.
	Ruby, //gives health
	Sapphire, //Gives Mana
	Citrine, //gives energy

	Str,		//Strength Rune, etc
	Dex,
	Int,
	Vit
}
