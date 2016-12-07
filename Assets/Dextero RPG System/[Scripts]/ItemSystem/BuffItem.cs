using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic; //So that List<> can be used

[System.Serializable]
public class BuffItem : Item {
	private List<AttributeBuff> _attribBuffs; 
	private List<VitalBuff> _vitalBuffs; 
	private int _buffValue;
	
	private int _sockets; 	//Number of sockets
	private int _usedSockets;
			
	private List<SocketItem> _equippedSockets = new List<SocketItem>();
	
	public BuffItem(){
		_attribBuffs = new List<AttributeBuff>();
		_vitalBuffs = new List<VitalBuff>();
		_buffValue = 0;
		_sockets = 0;
		_usedSockets = 0;
		_equippedSockets = new List<SocketItem>();
	}
	
	public void RemoveAllBuffs(){
		
		for (int i = 0; i < _attribBuffs.Count; i++) {
			_attribBuffs.RemoveAt (i);
		}
		for (int i = 0; i < _vitalBuffs.Count; i++) {
			_vitalBuffs.RemoveAt (i);
		}
	}
	//Adds a buff(AttributeBuff) to the list of buffs
	public void AddAttribModifier(AttributeBuff mod) {
		bool addingTemp = false;
		bool searching = true;
		AttributeBuff temp = new AttributeBuff();
		int x = 0;
		
		if(_attribBuffs.Count > 0){
			foreach(AttributeBuff att in _attribBuffs){
				if(searching){
					if(mod.attribute == att.attribute){
						//Debug.Log ("Tryna add duplicate buff");
						temp = att;
						addingTemp = true;
						searching = false;
						//_attribBuffs.Remove (att);
					}
					x++;	
				}
			}
			
			if(!addingTemp)
				_attribBuffs.Add(mod);
		}
		else {
			_attribBuffs.Add (mod);
		}
		
		if(addingTemp){
			temp.amount += mod.amount;
			_attribBuffs.RemoveAt (x-1);
			_attribBuffs.Add(temp);
		}
	}
	
	//Adds a buff(VitalBuff) to the list of buffs
	public void AddVitalModifier(VitalBuff mod) {
		bool addingTemp = false;
		bool searching = true;
		VitalBuff temp = new VitalBuff();
		int x = 0;
		
		if(_vitalBuffs.Count > 0){
			foreach(VitalBuff att in _vitalBuffs){
				if(searching){
					if(mod.vital == att.vital){
						//Debug.Log ("Tryna add duplicate buff");
						temp = att;
						addingTemp = true;
						searching = false;
						//_attribBuffs.Remove (att);
					}
					x++;
				}
			}
			
			if(!addingTemp)
				_vitalBuffs.Add(mod);
		}
		else {
			_vitalBuffs.Add (mod);
		}
		if(addingTemp){
			temp.amount += mod.amount;
			_vitalBuffs.RemoveAt (x-1);
			_vitalBuffs.Add(temp);
		}
	}
	
	public int NumberOfBuffs(){
		int temp = 0;
			temp += _vitalBuffs.Count;
			temp += _attribBuffs.Count;
		return temp;
	}
	
	
	//Calculate Amount to Add
	private int TotalStatFromBuff(){
		_buffValue=0;
		
		
		if(_attribBuffs.Count > 0){
			foreach(AttributeBuff att in _attribBuffs)
				_buffValue += (int)(att.amount);
		}
		
		if(_vitalBuffs.Count > 0){
			foreach(VitalBuff att in _vitalBuffs)
				_buffValue += (int)(att.amount);
		}
		
		return _buffValue;
	}
	
	//Update the buffs added
	public void Update(){
		//CalculateBuffValues ();
	}
	
	//Buffs for tooltip
	public string GetBuffsString(){
		string temp = "";
		//UnityEngine.Debug.Log (_mods.Count);
		for(int cnt=0;cnt<_attribBuffs.Count;cnt++){
			temp += "+ ";
			temp += _attribBuffs[cnt].amount;
			temp += " " + _attribBuffs[cnt].attribute.ToString ();

			if(cnt < _attribBuffs.Count -1){
				temp += "\n";
			}
			
		}
		
		if(_vitalBuffs.Count > 0 && _attribBuffs.Count >0 )
			temp += "\n";
		
		for(int cnt=0;cnt<_vitalBuffs.Count;cnt++){
			temp += "+ ";
			temp += _vitalBuffs[cnt].amount;
			temp += " " + _vitalBuffs[cnt].vital.ToString ();

			if(cnt < _vitalBuffs.Count -1){
				temp += "\n";
			}
			
		}

		return temp;
	}
	
	public List<AttributeBuff> AttribBuffs {
		get{return _attribBuffs;}
		set{_attribBuffs = value;}
	}
	
	public List<VitalBuff> VitalBuffs {
		get{return _vitalBuffs;}
		set{_vitalBuffs = value;}
	}
	
		
	public int Sockets{
		get{return _sockets;}
		set{_sockets = value;}
	}
	
	public int UsedSockets{
		get{return _usedSockets;}
		set{_usedSockets = value;}
	}
	
	public List<SocketItem> EquippedSockets{
		get{return _equippedSockets;}
		set{_equippedSockets = value;}
	}
	
	public override string UniqueIdentifier(){
		return (Name+Value.ToString()+RequiredLevel+Description+TotalStatFromBuff()+Sockets+UsedSockets);
	}
}
[System.Serializable]
public struct AttributeBuff {
	public AttributeName attribute; 	//attribute which we will add to
	public int amount;				//amount to buff attribute by
}
[System.Serializable]
public struct VitalBuff {
	public VitalName vital; 			//Vital which we will add to
	public int amount;				//amount to buff vital by
}