using UnityEngine;
using System.Collections;

public class CharacterWindowGUI : MonoBehaviour {
	public MyGUI GUIHandler;
	public PlayerNew _pc;
	public GUISkin mySkin;
	
	//Instances
	public InventoryWindowGUI inventoryWindowGUI;
	
	// GUISpecific
	public bool _displayCharacterWindow = true;
	private const int CHARACTER_WINDOW_ID = 3;
	private Rect _characterWindowRect = new Rect(40,40,300,300);
	private int _characterPanel = 0;
	private Vector2 _charWindowSlider = Vector2.zero;
	private string[] _characterPanelNames = new string[] {"Equipment","Attributes","Skills"};
	void Update(){
		if(Input.GetKeyUp (KeyCode.C))
		{
			ToggleCharWindow();
		}
	}
	
	void Start(){
		GUIHandler = GameObject.FindGameObjectWithTag("GUIHandler").GetComponent<MyGUI>();
		_pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerNew>();
		
		_characterWindowRect = new Rect(5,120,300,300);
		_characterPanelNames = new string[] {"Equipment","Attributes","Skills"};
		
		inventoryWindowGUI = GameObject.FindGameObjectWithTag("GUIHandler").GetComponent<InventoryWindowGUI>();
	}
	
	void OnGUI(){
		GUI.skin = mySkin;
		if(MyGUI.CanShow){
			
			float screenW = Screen.width;
			Rect r = new Rect(screenW - 315, 5, 100, 40); 
			if(GUI.Button (r,"Character Info")){
				ToggleCharWindow();
			}
			
			if(_displayCharacterWindow){
				_characterWindowRect = MyGUI.ClampToScreen(GUI.Window(CHARACTER_WINDOW_ID,
					_characterWindowRect,
					characterWindow,
					"Character Window","CharacterWindow"));
			}
		}
	}
	
	void OnEnable(){

	}
	
	void OnDisable(){

	}
	
	#region "Character Window"

	public void characterWindow(int windowid){
		_characterPanel = GUI.Toolbar (new Rect(5,_characterWindowRect.height - 25,_characterWindowRect.width - 10,20),_characterPanel,_characterPanelNames);
		
		switch(_characterPanel){
			case 0 :
				//GUI.DrawTexture(new Rect(89,31,120,170),characterEquipped);
				break;
			case 1 :
				DisplayAttributes();
				break;
			case 2 :
				DisplaySkills();
				break;
		}
		
		DisplayEquipment();
		
		GUIHandler.SetToolTip	();
		GUI.DragWindow (new Rect (0,0, 10000, 20));
	}
	
	
	private void DisplayEquipment(){
		//Debug.Log ("DISPLAYING EQUIPS");
		
		
		
		//Equipped Weapon
		if(_pc.EquipedWeapon == null){
			if(GUI.Button(new Rect(130,221,40,40),"","Box")){
				if (Event.current.button == 0) {
					if(MyGUI.itemIsPickedUp){
						GUIHandler.SwapItems("EquipedWeapon");
					}
				}
			}
		}
		else {
			if(GUI.Button(new Rect(130,221,40,40),new GUIContent(_pc.EquipedWeapon.Icon, "EquipedWeapon"),_pc.EquipedWeapon.Rarity.ToString())) {
				if (Event.current.button == 0) {
					if(MyGUI.itemIsPickedUp){
						GUIHandler.SwapItems("EquipedWeapon");
					}
				}
				//Right Click To Use Item
				if (Event.current.button == 1 && !MyGUI.itemIsPickedUp) {
					inventoryWindowGUI.InventoryDo ("ReturnEquippedWeapon",0);
				}
			}
			string moreInfo = "+" + (_pc.EquipedWeapon as Weapon).Enchants.ToString();
			GUI.Box (new Rect(130,221,40,40),moreInfo,"ItemInfoOverlay");
		}
		
		//Equipped Head
		if(_pc.EquipedArmorHead == null){
			if(GUI.Button(new Rect(24,21,40,40),"","Box")){
				if (Event.current.button == 0) {
					if(MyGUI.itemIsPickedUp){
						GUIHandler.SwapItems("EquipedArmorHead");
					}
				}
			}
		}
		else {
			if(GUI.Button(new Rect(24,21,40,40),new GUIContent(_pc.EquipedArmorHead.Icon, "EquipedArmorHead"),_pc.EquipedArmorHead.Rarity.ToString ())) {
				if (Event.current.button == 0) {
					if(MyGUI.itemIsPickedUp){
						GUIHandler.SwapItems("EquipedArmorHead");
					}
				}
				//Right Click To Use Item
				if (Event.current.button == 1 && !MyGUI.itemIsPickedUp) {
					inventoryWindowGUI.InventoryDo ("ReturnEquippedArmorHead",0);
				}
			}
		}
	
		//Equipped Chest Armour
		if(_pc.EquipedArmorChest == null){
			if(GUI.Button(new Rect(24,68,40,40),"","Box")){
				if (Event.current.button == 0) {
					if(MyGUI.itemIsPickedUp){
						GUIHandler.SwapItems("EquipedArmorChest");
					}
				}
			}
		}
		else {
			if(GUI.Button(new Rect(24,68,40,40),new GUIContent(_pc.EquipedArmorChest.Icon, "EquipedArmorChest"),_pc.EquipedArmorChest.Rarity.ToString ())) {
				if (Event.current.button == 0) {
					if(MyGUI.itemIsPickedUp){
						GUIHandler.SwapItems("EquipedArmorChest");
					}
				}
				//Right Click To Use Item
				if (Event.current.button == 1 && !MyGUI.itemIsPickedUp) {
					inventoryWindowGUI.InventoryDo ("ReturnEquippedArmorChest",0);
				}
			}
		}
		
		//Equipped Gloves
		if(_pc.EquipedArmorGloves == null){
			if(GUI.Button(new Rect(238,68,40,40),"","Box")){
				if (Event.current.button == 0) {
					if(MyGUI.itemIsPickedUp){
						GUIHandler.SwapItems("EquipedArmorGloves");
					}
				}
			}
		}
		else {
			if(GUI.Button(new Rect(238,68,40,40),new GUIContent(_pc.EquipedArmorGloves.Icon, "EquipedArmorGloves"),_pc.EquipedArmorGloves.Rarity.ToString())) {
				if (Event.current.button == 0) {
					if(MyGUI.itemIsPickedUp){
						GUIHandler.SwapItems("EquipedArmorGloves");
					}
				}
				//Right Click To Use Item
				if (Event.current.button == 1 && !MyGUI.itemIsPickedUp) {
					inventoryWindowGUI.InventoryDo ("ReturnEquippedArmorGloves",0);
				}
			}
		}
		
		//Equipped Legs
		if(_pc.EquipedArmorLegs == null){
			if(GUI.Button(new Rect(24,116,40,40),"","Box")){
				if (Event.current.button == 0) {
					if(MyGUI.itemIsPickedUp){
						GUIHandler.SwapItems("EquipedArmorLegs");
					}
				}
			}
		}
		else {
			if(GUI.Button(new Rect(24,116,40,40),new GUIContent(_pc.EquipedArmorLegs.Icon, "EquipedArmorLegs"),_pc.EquipedArmorLegs.Rarity.ToString ())) {
				if (Event.current.button == 0) {
					if(MyGUI.itemIsPickedUp){
						GUIHandler.SwapItems("EquipedArmorLegs");
					}
				}
				//Right Click To Use Item
				if (Event.current.button == 1 && !MyGUI.itemIsPickedUp) {
					inventoryWindowGUI.InventoryDo ("ReturnEquippedArmorLegs",0);
				}
			}
		}
		
		//Equipped Feet
		if(_pc.EquipedArmorFeet == null){
			if(GUI.Button(new Rect(238,115,40,40),"","Box")){
				if (Event.current.button == 0) {
					if(MyGUI.itemIsPickedUp){
						GUIHandler.SwapItems("EquipedArmorFeet");
					}
				}
			}
		}
		else {
			if(GUI.Button(new Rect(238,115,40,40),new GUIContent(_pc.EquipedArmorFeet.Icon, "EquipedArmorFeet"),_pc.EquipedArmorFeet.Rarity.ToString ())) {
				if (Event.current.button == 0) {
					if(MyGUI.itemIsPickedUp){
						GUIHandler.SwapItems("EquipedArmorFeet");
					}
				}
				//Right Click To Use Item
				if (Event.current.button == 1 && !MyGUI.itemIsPickedUp) {
					inventoryWindowGUI.InventoryDo ("ReturnEquippedArmorFeet",0);
				}
			}
		}
		
		//Equipped Back
		if(_pc.EquipedArmorBack == null){
			if(GUI.Button(new Rect(238,21,40,40),"","Box")){
				if (Event.current.button == 0) {
					if(MyGUI.itemIsPickedUp){
						GUIHandler.SwapItems("EquipedArmorBack");
					}
				}
			}
		}
		else {
			if(GUI.Button(new Rect(238,21,40,40),new GUIContent(_pc.EquipedArmorBack.Icon, "EquipedArmorBack"),_pc.EquipedArmorBack.Rarity.ToString ())) {
				if (Event.current.button == 0) {
					if(MyGUI.itemIsPickedUp){
						GUIHandler.SwapItems("EquipedArmorBack");
					}
				}
				//Right Click To Use Item
				if (Event.current.button == 1 && !MyGUI.itemIsPickedUp) {
					inventoryWindowGUI.InventoryDo ("ReturnEquippedArmorBack",0);
				}
			}
		}
	}
	
	private void DisplayAttributes(){
		string attributes = "";
		attributes += "Character Class:" + _pc.playerClass.ToString() + "\n";
		
			foreach(Attribute att in _pc.Attributes){
				attributes += att.AttributeString() + "\n";
			}
		
		
			foreach(Vital vit in _pc.Vitals){
				attributes += vit.Name + ": " + vit.CurValue + "/" + vit.MaxValue + "\n";
			}
		
		Weapon weapon = _pc.EquipedWeapon as Weapon;
		attributes += "\n" + "Offensive: \n";
		if(weapon != null){
			attributes += "Damage: " + _pc.GetDPSString() + " (+" +  weapon.DmgValue  + ")" + "\n";
			attributes += "Attack Speed: " + _pc.AttackSpeed.ToString ("0.00") + "\n";
			attributes += "Critical Chance: " + (_pc.CritChance*100).ToString ("0") + "%";
			attributes += " (" +  (_pc.CritDamage*100).ToString ("0") + "%)" + "\n";
			attributes += "Max Hit: " + ((int)(_pc.MaxDamage*_pc.CritDamage) + (int)(weapon.DmgValue*_pc.CritDamage)).ToString() +  "\n";
		}
		else{
			attributes += "Damage: " + _pc.GetDPSString() + "\n";
			attributes += "Attack Speed: " + _pc.AttackSpeed.ToString ("0.00") + "\n";
			attributes += "Critical Chance: " + (_pc.CritChance*100).ToString ("0") + "%";
			attributes += " (" +  (_pc.CritDamage*100).ToString ("0") + "%)" + "\n";
			attributes += "Max Hit: " + ((int)(_pc.MaxDamage*_pc.CritDamage)).ToString() +  "\n";
		}
			
				
		attributes += "\n" + "Defensive: \n";
			attributes += "Armor: " + _pc.PlayerArmor + "\n";
			attributes += "Chance To Block: " + (_pc.PlayerChanceToBlock*100).ToString("0") + "\n";
			attributes += "Damage Blocked: " + _pc.PlayerDamageBlocked + "\n";
		
		attributes += "\n" + "Misc: \n";
			attributes += "MoveSpeed: " + (_pc.MoveSpeed*100).ToString("0") + "%" + "\n";
		
		//GUI.Box (new Rect(69,21,164,195),attributes,"CharacterWindowAttr");
		GUILayout.BeginArea(new Rect(69,21,164,195));
			_charWindowSlider = GUILayout.BeginScrollView(_charWindowSlider);
				GUILayout.Box (attributes,"CharacterWindowAttr");
			GUILayout.EndScrollView();
		GUILayout.EndArea();
		
		
		

	}
	
	private void DisplaySkills(){
		string skills = "";
		
		skills += "Skills: \n";
		skills += "No Skills available. \n";
		
		GUI.Box (new Rect(69,21,164,195),skills);
	}
	
	public void ToggleCharWindow(){
		_displayCharacterWindow = !_displayCharacterWindow;
		if(!_displayCharacterWindow)
			MyGUI._displayToolTip = false;
	}
	#endregion

}
