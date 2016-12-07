using UnityEngine;
using System.Collections;

public class DexteroRPGPackageHelp : MonoBehaviour {
	public bool showHelp;
	string about = "";
	string help = "";
	string nl = "\n";
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown (KeyCode.H)){
			Toggle();
		}
	}
	
	void Start(){

		about += "Welcome to the Dextero RPG system." + nl +

			"There are many features in this RPG system including:" + nl +
			"Save and Load functionality, In-Depth RNG Item Generator, Item System, Inventory System, Character System," + nl +
			"NPC System (Dialog and more), Harvesting System, Quest System, Vendor System," + nl +
			"Crafting system, Achievements system, Scrolling combat text and a ToolTip system." + nl +
			"The entire system is designed in C# and is thus fully customisable to your needs." + nl +
			"" +
			"Most of the code is self explantory. And it is neatly split into the revelant sections which " + nl +
			"you can find in the project window. Any questions feel free to message me at the following email " + nl +
			"LogicSpawnGames@gmail.com";
		
		help += "At the top of the screen you will find tabs to open the inventory," + nl +
			"character window and quest tracker. This can also be done by pressing I, C and K respectively." + nl +
			"Buttons on the bottom right will help you see some of the features in the game. Such as stat scaling " + nl +
			"and being able to spawn items anywhere in the game world." + nl +
			"To use items in the inventory simply right click. To split stacked items, such as consumables," + nl +
			"press SHIFT+ left click on the item. When the STASH is open, press right click to store items." + nl +
			"When the VENDOR is open, press CTRL+Right click to sell an item." + nl +
			"Movement is done by point to click (basic) so you may run into things once in a while" + nl +
			"To use the Give item feature in the test tools, you need to understand the basics of the " + nl +
			"item generator." + nl +
			"Field 1 refers to item type, which is either: random, weapon, armor, consum, socket" + nl +
			"Field 2 refers to item rarity: common, uncommon, rare, epic, legendary, random" + nl +
			"Finally, Field 3 refers to itemLevel: 1-100" + nl +
			"Any other questions please contact: LogicSpawnGames@gmail.com";
	}
	
	void OnGUI(){
		GUILayout.BeginArea(new Rect(200,200,750,600));
		
		if(showHelp){
			GUILayout.Box (about);
			GUILayout.Box (help);	
			GUILayout.Box ("Press H to hide");
		}
		
		GUILayout.EndArea();
	}
	
	void Toggle(){
		showHelp = !showHelp;
	}
}
