using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LootItem : MonoBehaviour {
	private PlayerNew playerNew;
	public Item myItem;
	public string itemName;
	public void SetItem(Item i){
		myItem = i;
	}
	
	void Start(){
		itemName = GetColor(myItem);
		playerNew = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerNew>();
	}
	
	void OnGUI(){
	 // calculate the world position for the label:
		  var pos3d = transform.position + Vector3.up * 0.1f;
		  // convert it to screen space:
		  var pos = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>().WorldToScreenPoint(pos3d);
		  // flip the Y coordinate vertically to match GUI space:
		  pos.y = Screen.height - pos.y;
		  // create a rect above pos:
		  var r = new Rect(pos.x-1000/2, pos.y-20/2 -10, 1000, 20);
		  // draw it:
		GUILayout.BeginArea(r);
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if(GUILayout.Button(itemName, "Box", GUILayout.ExpandWidth (false))){
			PlayerPickUp();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		
	}
	
	void PlayerPickUp(){
		float viewDist = Vector3.Distance(transform.position,playerNew.transform.position);
			
		if(viewDist < 4){
			bool added = playerNew.AddItem(myItem);
		
			if(added){
				Destroy (gameObject);
			}
		}
		else {
			CharacterControl.useNewPosition = true;
			CharacterControl.newPosition = transform.position;
			CharacterControl.arrivedAtDestination = false;
		}
	}
	
	string GetColor(Item myItem){
		
		if(myItem.Name.Contains("Gold")){
			return ToolTipStyle.Italic + ToolTipStyle.Orange + myItem.Name + ToolTipStyle.EndColor + ToolTipStyle.EndItalic;
		}
		else if(myItem.ItemType == ItemEquipType.Socket){
			return ToolTipStyle.Yellow + myItem.Name + ToolTipStyle.EndColor;
		}
		else if (myItem.ItemType == ItemEquipType.Consumable){
			return ToolTipStyle.Grey + myItem.Name + ToolTipStyle.EndColor;
		}
		else if (myItem.ItemType == ItemEquipType.CraftItem){
			return ToolTipStyle.Brown + myItem.Name + ToolTipStyle.EndColor;
		}
		else if (myItem.ItemType == ItemEquipType.QuestItem){
			return ToolTipStyle.Cyan + myItem.Name + ToolTipStyle.EndColor;
		}
		
		switch(myItem.Rarity){
			case RarityTypes.Common:
				return ToolTipStyle.White + myItem.Name + ToolTipStyle.EndColor;
			case RarityTypes.Uncommon:
				return ToolTipStyle.Green + myItem.Name + ToolTipStyle.EndColor;
			case RarityTypes.Rare:
				return ToolTipStyle.Blue + myItem.Name + ToolTipStyle.EndColor;
			case RarityTypes.Epic:
				return ToolTipStyle.Purple + myItem.Name + ToolTipStyle.EndColor;
			case RarityTypes.Legendary:
				return ToolTipStyle.Orange + myItem.Name + ToolTipStyle.EndColor;
			case RarityTypes.Unique:
				return ToolTipStyle.Red + myItem.Name + ToolTipStyle.EndColor;
			
		}
		
		
		
		return myItem.Name;
	}
	
	void OnMouseDown(){
		PlayerPickUp();
	}
}
