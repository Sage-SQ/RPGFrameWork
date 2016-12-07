

public class Vital  {
	private string _name;
	private int _curValue;
	private int _maxValue;
	
	public Vital(){
		_name = "Unnamed Vital";
		_maxValue = 100;
		_curValue = _maxValue;
	}
	
	public string Name{
		get{return _name;}
		set{_name = value;}
	}
	
	public int CurValue{
		get{
			if( _curValue > _maxValue )
				_curValue = _maxValue;
			return _curValue;
		}
		set{ 
			if( _curValue + value < 0 )
				_curValue = 0;
			else
				_curValue = value;
		}
	}
	
	public int MaxValue {
		get{ return _maxValue;}
		set{ _maxValue = value;}
	}
	
}

public enum VitalName{
	Health,
	Mana,
	Energy
}