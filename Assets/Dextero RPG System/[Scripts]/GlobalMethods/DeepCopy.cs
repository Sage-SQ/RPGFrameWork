using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;

public static class DeepCopy{
	public static Item CopyItem(Item other)
	{
	    using (MemoryStream ms = new MemoryStream())
	    {
	        BinaryFormatter formatter = new BinaryFormatter();
	        formatter.Serialize(ms, other);
	        ms.Position = 0;
	        Item item = (Item)formatter.Deserialize(ms);
			item.Icon = Resources.Load(item.IconPath) as Texture2D;
			return item;
	    }
	}
}
