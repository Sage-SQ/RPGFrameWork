using UnityEngine;

[System.Serializable]
public class Armor : Clothing {
	private int _armorAmount;		//the armor level of this piece of armor
	
	private float _chanceToBlock; 	//Chance out of 100 to block
	private int _damageBlocked;		//Damage reduction when block occurs
	
	private float _moveSpeedBuff;		//Percentage Movement Speed buff, e.g. 0.06 = 6% move speed buff 
	
	public Armor() {
		_armorAmount = 0;
		_chanceToBlock = 0;
		_damageBlocked = 0;
		_moveSpeedBuff = 0;
	}
	
	public Armor(int armorLvl, float chanceToBlock, int dmgBlock, float move) {
		_armorAmount = armorLvl;
		_chanceToBlock = chanceToBlock;
		_damageBlocked = dmgBlock;
		_moveSpeedBuff = move;
	}
	
	public int ArmorAmt {
		get {return _armorAmount;}
		set {_armorAmount = value;}
	}
	
	public float ChanceToBlock {
		get {return _chanceToBlock;}
		set {_chanceToBlock = value;}
	}
	
	public int DamageBlocked {
		get {return _damageBlocked;}
		set {_damageBlocked = value;}
	}
	
	public float MoveSpeedBuff {
		get {return _moveSpeedBuff;}
		set {_moveSpeedBuff = value;}
	}
	
	public override string ToolTip(){
		
		string numBuffs = "";
		if(this.NumberOfBuffs() > 0){
			numBuffs = "\n" + GetBuffsString() + "\n";
		}
		else{
			numBuffs = "";
		}
		
		string moreStats = "";
		if(this.MoveSpeedBuff > 0 || this.DamageBlocked > 0){
			if(this.MoveSpeedBuff > 0){
				moreStats += "\n" + "+ " + (MoveSpeedBuff*100).ToString("0") + "% Move Speed";
			}
			if(this.DamageBlocked > 0){
				moreStats += "\n" + "Gives " + (ChanceToBlock*100).ToString("0") + "% Chance to block " + DamageBlocked + " damage.";
			}
		}
		
		string socketBuffs = "";
		if(EquippedSockets.Count > 0)
			socketBuffs += "\n \n" + "(" + UsedSockets.ToString() + ") Sockets: \n";
		foreach(SocketItem socket in EquippedSockets){
			socketBuffs += socket.GetBuffsString();
		}
		socketBuffs += "\n";
		
		
		return 	ArmorAmt + " Armor" + "\n" + 
				numBuffs +
				socketBuffs +
				moreStats;

	}
}