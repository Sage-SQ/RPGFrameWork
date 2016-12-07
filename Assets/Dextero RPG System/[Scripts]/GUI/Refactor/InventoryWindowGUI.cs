using UnityEngine;
using System.Collections;

public class InventoryWindowGUI : MonoBehaviour {
	public MyGUI GUIHandler;
	public PlayerNew _pc;
	public GUISkin mySkin;
	
	//All Purpose Variables
	private float _offset = 10;
	private float itemWidth = 40;			//Width of item in inventory/loot screen
	private float itemHeight = 40;			//Height of item in inventory/loot screen

	// GUISpecific
	public bool _displayInventory = false;
	private const int INVENTORY_WINDOW_ID = 1;
	private Rect _inventoryWindowRect = new Rect(390,50,170,210);
	private int _inventoryRows = 5;
	private int _inventoryCols = 5;
	private int _maxInventorySpace = 25;
	private Rect[] buttonRects;
	private bool showSplit;
	
	void Update(){
		if(Input.GetKeyUp (KeyCode.I)){
			ToggleInventoryWindow();
		}
	}
	
	void Start(){
		GUIHandler = GameObject.FindGameObjectWithTag("GUIHandler").GetComponent<MyGUI>();
		_pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerNew>();
		
		_maxInventorySpace = _pc.MaxInventorySpace;
		_inventoryRows = _maxInventorySpace / _inventoryCols;
		_inventoryWindowRect = new Rect((Screen.width -5) - (itemWidth*_inventoryCols)-_offset ,
			50,
			(itemWidth*_inventoryCols)+_offset,
			(itemHeight*_inventoryRows)+_offset+15+25);
		
		_maxInventorySpace = _inventoryRows*_inventoryCols;
		buttonRects = new Rect[_maxInventorySpace];
	}
	
	void OnGUI(){
		GUI.skin = mySkin;
		if(MyGUI.CanShow){
			
			float screenW = Screen.width;
			Rect r = new Rect(screenW - 210, 5, 100, 40); 
			if(GUI.Button (r,"Inventory")){
				ToggleInventoryWindow();
			}
			
			if(_displayInventory){
				_inventoryWindowRect = MyGUI.ClampToScreen(GUI.Window(INVENTORY_WINDOW_ID,
					_inventoryWindowRect,
					inventoryWindow,
					"Inventory","InventoryWindow"));
			}
		}
		
		if(showSplit){
			ShowSplitStack();
		}
	}
	
	void OnEnable(){

	}
	
	void OnDisable(){

	}
	
	#region "Inventory Window"
	
	private void inventoryWindow(int windowid) {
		bool controlIsDown = false;
		bool shiftIsDown = false;
		int cnt = 0;
		for(int y = 0; y < _inventoryRows; y++){
			if(Input.GetKey(KeyCode.LeftControl) ||Input.GetKey(KeyCode.RightControl) ){
				controlIsDown = true;
			}
			if(Input.GetKey(KeyCode.LeftShift) ||Input.GetKey(KeyCode.RightShift) ){
				shiftIsDown = true;
			}
			
			for(int x = 0; x < _inventoryCols; x++){
				if(cnt < _pc.Inventory.Count){
						
						buttonRects[cnt] = new Rect(5 + (itemWidth * x), 20 + y* itemHeight, itemWidth, itemHeight);
						
						
						
						
					
						//Item Button
						if(GUI.Button ( buttonRects[cnt], 
						new GUIContent(_pc.Inventory[cnt].Icon,string.Format ("Inventory{0}",cnt)),
						_pc.Inventory[cnt].Rarity.ToString())){
							//Left Click To Pick Up Item/Split Stack
							if(!showSplit){
								if (Event.current.button == 0) {
									if(shiftIsDown && _pc.Inventory[cnt].CurStacks > 1){
										showSplit = true;
										MyGUI.inventoryArrayToSplit = cnt;
									}
									else if(!MyGUI.itemIsPickedUp){
										MyGUI.itemIsPickedUp = true;
										MyGUI.SetPickedUpItem(_pc.Inventory[cnt].Icon,string.Format ("Inventory{0}",cnt));
									}
									else{
										if(!MyGUI.itemIsForUse)
											GUIHandler.SwapItems(string.Format ("Inventory{0}",cnt));
										else
											GUIHandler.UseSocket(cnt);
									}
								}
								//Right Click To Use Item
								if (Event.current.button == 1 && !MyGUI.itemIsPickedUp) {
									if(_pc.playerState == PlayerState.Stash){
										_pc.AddToBank(cnt);
									}
									else if(_pc.playerState != PlayerState.NPCDialog){
										InventoryDo("UseItem",cnt);
									}
									else if(controlIsDown && NPCDialogGUI.openedNPCDialog.OptionNumber == 100){ //if in vendor and holding control
										NPCDialogGUI.openedNPCDialog.vendor.PlayerSellItem (cnt);
									}
								
									MyGUI._toolTip = "";
									return;
								}
							}
						}
					
						string stackInfo = _pc.Inventory[cnt].MaxStacks > 1 ? _pc.Inventory[cnt].CurStacks.ToString() : string.Empty;
						GUI.Box ( buttonRects[cnt],stackInfo,"StackOverlay");
						string moreInfo = _pc.Inventory[cnt].ItemType == ItemEquipType.Weapon ? "+" + (_pc.Inventory[cnt] as Weapon).Enchants : string.Empty;
						if(_pc.Inventory[cnt].ItemType == ItemEquipType.QuestItem) moreInfo = "Q";
						GUI.Box ( buttonRects[cnt],moreInfo,"ItemInfoOverlay");
				}
				else{
					GUI.Box ( new Rect(5 + (itemWidth * x), 20 + y* itemHeight, itemWidth, itemHeight), "","inventorySlot");
				}
				
				cnt++;
				
				
			}
		}
		
		if(GUI.Button( new Rect(5, _inventoryWindowRect.height - 25, 20, 20),new GUIContent("", "Deletes items from your inventory."),"Invisible")){
			if (Event.current.button == 0) {
				if(MyGUI.itemIsPickedUp){
					GUIHandler.TrashIconDropItem();
				}
			}
		}
		GUI.Label ( new Rect(27, _inventoryWindowRect.height - 27, _inventoryWindowRect.width - 54, 20),
			_pc.Gold.ToString("N0").Replace(".", ","),"Gold");
		
		GUI.DragWindow (new Rect (0,0, 10000, 20));
		GUIHandler.SetToolTip	();
	}
	
	public void ShowSplitStack(){
		
		Rect sssRect = new Rect(
			(_inventoryWindowRect.x+_inventoryWindowRect.width)/2 -50,
			(_inventoryWindowRect.y+_inventoryWindowRect.height)/2 -35,
			100,
			70);
		
		GUI.Box (sssRect,"");
		
		GUI.Label (new Rect(sssRect.x+5,sssRect.y+5,90,30),"Split by:");
		//MyGUI.amountToSplitBy = int.Parse (GUI.TextField(new Rect(sssRect.x+5,sssRect.y+35,60,30),MyGUI.amountToSplitBy.ToString()));
		MyGUI.amountToSplitBy = GUI.HorizontalSlider (new Rect(sssRect.x+5,sssRect.y+35,60,30),MyGUI.amountToSplitBy,1,_pc.Inventory[MyGUI.inventoryArrayToSplit].CurStacks);
		GUI.Label (new Rect(sssRect.x+5,sssRect.y+50,60,30),MyGUI.amountToSplitBy.ToString());
		MyGUI.amountToSplitBy = Mathf.Clamp (MyGUI.amountToSplitBy,1,_pc.Inventory[MyGUI.inventoryArrayToSplit].CurStacks);
		MyGUI.amountToSplitBy = (int)MyGUI.amountToSplitBy;
		
		if(MyGUI.amountToSplitBy == 0) MyGUI.amountToSplitBy = 1;
		if(GUI.Button (new Rect(sssRect.x+70,sssRect.y+35,30,30),"Ok")){
			if(_pc.Inventory[MyGUI.inventoryArrayToSplit].CurStacks - (int)MyGUI.amountToSplitBy == 0){
				showSplit = false;
				MyGUI.inventoryArrayToSplit = 1000;
			}
			else {
				if(_pc.Inventory.Count + 1 <= _pc.MaxInventorySpace){
					_pc.Inventory[MyGUI.inventoryArrayToSplit].CurStacks -= (int)MyGUI.amountToSplitBy;
					Item newStack = DeepCopy.CopyItem (_pc.Inventory[MyGUI.inventoryArrayToSplit]);
					newStack.CurStacks = (int)MyGUI.amountToSplitBy;
					_pc.Inventory.Add(newStack);
				}
				else {
					Debug.Log ("No space to split stack");
				}
				showSplit = false;
				MyGUI.inventoryArrayToSplit = 1000;
			}
		}
	}
	
	public void ToggleInventoryWindow(){
		_displayInventory = !_displayInventory;
		
		//Clear tooltip if inventory is closed
		if(!_displayInventory){
			MyGUI._toolTip = "";
		}
	}
	

	
	#endregion
	
	#region "Inventory Functions"
	
	public void InventoryDo(string method, int cnt) {
		if(method == "DropItem"){

			if(_pc.Inventory[cnt].CanBeDropped)
				_pc.Inventory.RemoveAt (cnt);
			else
				Debug.Log ("Can't destroy this item!");
		}
		else if(method == "UseItem"){
			if(_pc.Inventory[cnt].ItemType == ItemEquipType.Clothing ||
				_pc.Inventory[cnt].ItemType == ItemEquipType.Weapon){
				if(_pc.playerState == PlayerState.Normal){
					GUIHandler.EquipAnItem(cnt);
				}
				else {
					Debug.Log ("Can't equip item right now.");
				}
			}
			else if(_pc.Inventory[cnt].ItemType == ItemEquipType.Consumable) {
				//IEnumerator for cooldown of using potions
				Consumable c = _pc.Inventory[cnt] as Consumable;
				bool usedPot = _pc.UseConsumable((int)c.VitalToRestore, c.AmountToHeal);
				if(usedPot){
					if(c.CurStacks == 1){
						_pc.Inventory.RemoveAt (cnt);
					}
					else {
						c.CurStacks -= 1;
					}
				}
				else {
					Debug.Log ("Using pots on cooldown");
				}
			}
			else if(_pc.Inventory[cnt].ItemType == ItemEquipType.Socket){
				MyGUI.itemIsPickedUp = true;
				MyGUI.itemPickedUpIcon = _pc.Inventory[cnt].Icon;
				MyGUI.itemIsForUse = true;
				MyGUI.pickedUpItemIdentifier = string.Format ("Inventory{0}",cnt);
			}
			else {
				Debug.Log ("Unknown : This is a " + _pc.Inventory[cnt].Name);
			}
			
		}
		else 
		{
			if(_pc.Inventory.Count < _maxInventorySpace) {
				switch(method){
					//Returning Equipped Items
					case "ReturnEquippedWeapon":
						_pc.AddItem (_pc.EquipedWeapon);
						_pc.EquipedWeapon = null;
						break;
					case "ReturnEquippedArmorHead":
						_pc.AddItem (_pc.EquipedArmorHead);
						_pc.EquipedArmorHead = null;
						break;
					case "ReturnEquippedArmorChest":
						_pc.AddItem (_pc.EquipedArmorChest);
						_pc.EquipedArmorChest = null;
						break;
					case "ReturnEquippedArmorGloves":
						_pc.AddItem (_pc.EquipedArmorGloves);
						_pc.EquipedArmorGloves = null;
						break;
					case "ReturnEquippedArmorLegs":
						_pc.AddItem (_pc.EquipedArmorLegs);
						_pc.EquipedArmorLegs = null;
						break;
					case "ReturnEquippedArmorFeet":
						_pc.AddItem (_pc.EquipedArmorFeet);
						_pc.EquipedArmorFeet = null;
						break;
					case "ReturnEquippedArmorBack":
						_pc.AddItem (_pc.EquipedArmorBack);
						_pc.EquipedArmorBack = null;
						break;
				}
			}
			else {
				Debug.Log ("Inventory is full");
				Debug.Log ("Your Inventory is full.");
			}
		}
		
		_pc.UpdateStats();
	}
	#endregion
	
}
