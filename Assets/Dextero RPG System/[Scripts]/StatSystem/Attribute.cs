using UnityEngine;

public class Attribute {
	
	private string _name;
	private int _baseValue;
	private int _attributeValue;
	private int _equipValue;


	public Attribute(){
		_name = "";
		_attributeValue = 0;
	}
	public int BaseValue {
		get{ return _baseValue;}
		set{
			_baseValue = value;
			RecalculateValue();
		}
	}

	public int EquipValue {
		get{ return _equipValue;}
		set{_equipValue = value;}
	}
	
	public int AttributeValue {
		get{ return _attributeValue;}
		set{_attributeValue = value;}
	}
	
	public string Name{
		get{return _name;}
		set{_name = value;}
	}
	
	public string AttributeString() {
		return _name + ": " + _attributeValue.ToString() + " (+" + _equipValue.ToString() + ")";
	}
	
	public void RecalculateValue()
	{
		AttributeValue = BaseValue + EquipValue;
	}
}

public enum AttributeName {

	Strength,
	Dexterity,
	Intelligence,
	Vitality
}