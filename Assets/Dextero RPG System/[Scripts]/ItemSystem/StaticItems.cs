using UnityEngine;
using System.Collections;

public static class StaticItems {
	public static Item GoldItem(int amountOfGold){
		return new Item(amountOfGold + " Gold",amountOfGold);
	}
}
