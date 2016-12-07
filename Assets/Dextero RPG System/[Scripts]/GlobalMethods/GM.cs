using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GM  { // GM = Generic Methods
	public static T GetRandomEnum<T>() {
    	System.Array A = System.Enum.GetValues(typeof(T));
    	T V = (T)A.GetValue(UnityEngine.Random.Range(0,A.Length));
    	return V;
	}
}


public class PlayerPrefsX
{
	public static void SetBool(string name, bool booleanValue) 
	{
		PlayerPrefs.SetInt(name, booleanValue ? 1 : 0);
	}
 
	public static bool GetBool(string name)  
	{
	    return PlayerPrefs.GetInt(name) == 1 ? true : false;
	}
 
	public static bool GetBool(string name, bool defaultValue)
	{
	    if(PlayerPrefs.HasKey(name)) 
		{
	        return GetBool(name);
	    }
 
	    return defaultValue;
	}
}