//Add _description for displaying info about an item:
//e.g. "This will heal your health"
//e.g. A might sword forged in gallifrey etc
//e.g. "Quest Item: Hand to Alabar"

using UnityEngine;
//[System.Serializable]
[System.Serializable]
public class Item {
	private string _name;
	private int _value;				//Gold Value
	private RarityTypes _rarity;
	private bool _canBeSold;
	
	[System.NonSerialized]
	private Texture2D _icon;
	private string _iconPath;
	
	private int _curStacks;
	private int _maxStacks;
	
	private int _requiredLevel;
	
	
	private ItemEquipType _itemType;
	private bool _canBeDropped;
		//Number you can have in one slot, e.g. 1 for equips, 20 for pots
	
	private string _description;
	
	public Item(){
		_name = "Unnamed Item";
		_value = 100;
		_rarity = RarityTypes.Common;
		_itemType = ItemEquipType.Clothing;
		_requiredLevel = 0;
		_canBeSold = true;
		_icon = Resources.Load ("Item Icons/defaultItem") as Texture2D;
		_iconPath = "Item Icons/defaultItem";
		_maxStacks = 1;
		_curStacks = 1;
		_description = "";
		_canBeDropped = true;
		
	}
	
	public Item(string name, int val, RarityTypes rare, ItemEquipType itemType,
		int reqLvl, int enchants, bool sellable, int maxStacks,
		int curStacks, string description, bool canBeDropped) {
		
		_name = name;
		_value = val;
		_rarity = rare;
		_requiredLevel = reqLvl;
		_canBeSold = sellable;
		_itemType = itemType;
		_maxStacks = maxStacks;
		_curStacks = curStacks;
		_description = description;
		_canBeDropped = canBeDropped;
	}
	
	//For Gold only
	public Item(string name, int val) {
		
		_name = name;
		_value = val;
		_icon = Resources.Load("Item Icons/goldItem") as Texture2D;
		_iconPath = "Item Icons/goldItem";
	}
	
	public string Name{
		get{return _name;}
		set{_name = value;}
	}
	
	public int Value{
		get{return _value;}
		set{_value = value;}
	}
	
	public bool CanBeSold {
		get{return _canBeSold;}
		set{_canBeSold = value;}
	}
	
	public bool CanBeDropped {
		get{return _canBeDropped;}
		set{_canBeDropped = value;}
	}
	
	public RarityTypes Rarity {
		get{return _rarity;}
		set{_rarity = value;}
	}
	
	public ItemEquipType ItemType {
		get{return _itemType;}
		set{_itemType = value;}
	}
	
	public int RequiredLevel {
		get{return _requiredLevel;}
		set{_requiredLevel = value;}
	}
	
	
	public Texture2D Icon {
		get{return _icon;}
		set{_icon = value;}
	}
	
	public string IconPath {
		get{return _iconPath;}
		set{_iconPath = value;}
	}
	
	public int MaxStacks {
		get{return _maxStacks;}
		set{_maxStacks = value;}
	}
	
	public int CurStacks {
		get{return _curStacks;}
		set{_curStacks = value;}
	}
	
	public string Description {
		get{return _description;}
		set{_description = value;}
	}
	
	public virtual string UniqueIdentifier(){
		return (Name+Value.ToString()+RequiredLevel+Description);
	}
	
	public virtual string ToolTip() {
		if(!Name.Contains("Gold")){
			return "Name" + "\n" + 
				"Value: " + Value;
		}
		
		return Value + " Gold!";
	}
}

public enum RarityTypes {
	Common,
	Uncommon,
	Rare,
	Epic,
	Legendary,
	Unique,
	CraftedItem,
	
	//not rarity, just used for identifying unique items
	QuestItem,
	CraftItem,
	SocketItem
}

public enum ItemEquipType {
	Clothing,
	Weapon,
	Consumable,
	QuestItem,
	CraftItem,
	Socket
}