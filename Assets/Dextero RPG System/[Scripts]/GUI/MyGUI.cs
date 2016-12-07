//Debug.Log ((_pc.EquipedWeapon as Weapon).MaxDamage);
//That's how you get info etc, if you try use weapon as armor for example
//you'll get null reference.

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MyGUI : MonoBehaviour {
	
	#region "Variables"
	
	//Instances
	public InventoryWindowGUI inventoryWindowGUI;

	public GameObject target;
	public PlayerNew player;

	public GUISkin mySkin;
	public static bool CanShow = true;
	

	
	//Tooltips
	//public so other scripts can clear/change it
	public static string _toolTip = "";
	private Texture2D _toolTipTexture;
	private string _toolTipName;
	private string _toolTipDescription;
	private int _toolTipWidth = 310;
	private Item objectForTooltip;
	private bool tooltipIsText = false;

	
	//************************************************************
	//Inventory Window Variables
	//************************************************************

		
	//************************************************************
	//Tooltip Window Variables
	//************************************************************
	
	public static bool _displayToolTip = false;
		public static bool DisplayToolTip{
			get{return _displayToolTip;}
			set{_displayToolTip = value;}
		}
	private const int TOOLTIP_WINDOW_ID = 2;
	private Rect _toolTipRect = new Rect(0,0,0,0);
	private Vector2 mousePos;
	
	//************************************************************
	//Picked Up Icon Window Variables
	//************************************************************
	
	private const int PICKUP_WINDOW_ID = 7;
	private Rect _pickedUpIconRect = new Rect(0,0,40,40);
	//Moving items/Skills around/Using sockets
	//private Item itemPickedUp; //don't think I need this since I will find it from Identifier
	public static Texture2D itemPickedUpIcon;
	public static string pickedUpItemIdentifier;
	public static bool itemIsPickedUp = false;
	public static bool itemIsForUse;
	public static bool displaySplitStack;
	public static int inventoryArrayToSplit;
	public static float amountToSplitBy = 1;
	
	private enum WindowType {
		None,
		Inventory,
		CharacterWindow
		//SkillWindow
		//Bank
		//Vendor
	}
	
	#endregion
	
	#region "Start, Update, Messengers"
	// Use this for initialization
	
	void Start () {
		if(target == null){
			target = GameObject.FindGameObjectWithTag("Player");
		}
		
		player = target.GetComponent<PlayerNew>();
		
		_pickedUpIconRect = new Rect(0,0,40,40);
		
		inventoryWindowGUI = GameObject.FindGameObjectWithTag("GUIHandler").GetComponent<InventoryWindowGUI>();
	}
	
	
	public void OnEnable(){
		
	}
	
	public void OnDisable(){

	}
	
	// Update is called once per frame
	
	void Update () {		
		
	}
	
	#endregion
	
	#region "OnGUI"
	//OnGUI
	
	void OnGUI() {
		
		if(CanShow){
			GUI.skin = mySkin;
			
			DisplayPlayerVitalsAndExp();
					
			PositionToolTipWindow();
			if(_displayToolTip){
				_toolTipRect = GUILayout.Window(TOOLTIP_WINDOW_ID,
					ClampToScreen(_toolTipRect),
					toolTipWindow,
					"Info","ToolTipWindow");
				GUI.BringWindowToFront (TOOLTIP_WINDOW_ID);	//If the tooltip is displayed bring it to the front
			}
			
			
		}
		
		PositionPickUpWindow();
		if(itemIsPickedUp){
			_pickedUpIconRect = GUI.Window(PICKUP_WINDOW_ID,
					ClampToScreen(_pickedUpIconRect),
					DrawPickedUpItem,
					"","Invisible");
		}
		GUI.BringWindowToFront (PICKUP_WINDOW_ID);
	}
	#endregion
	
	#region "Useful Functions/Methods"
	public static Rect ClampToScreen(Rect r)
	{
		if(r.y >= Screen.height / 2){
			r.y = Mathf.Clamp(r.y,0,Screen.height-(r.height+5));
		}
		else {
			r.y = Mathf.Clamp(r.y,5,Screen.height-(r.height));
		}
		
		if(r.x >= Screen.width / 2){
			r.x = Mathf.Clamp(r.x,0,Screen.width-(r.width+5));
		}
		else {
			r.x = Mathf.Clamp(r.x,5,Screen.width-(r.width));
		}
		
	    return r;
	}
	#endregion
	
	#region "Pick Up Items"
	
	public void UseSocket(int inventorySlot){

		int slotOfSocket = FindInventorySpot(pickedUpItemIdentifier);
	
		BuffItem item = player.Inventory[inventorySlot] as BuffItem;
		bool addedSocket = false;
		if(item != null){
			if(item.Sockets - item.UsedSockets > 0){
				if(item.UsedSockets > 0){
					SocketItem socket = player.Inventory[slotOfSocket] as SocketItem;
					//Add all buffs from socket to the first socket in slot for easier tooltipping etc
					foreach(AttributeBuff att in socket.AttribBuffs){
						item.EquippedSockets[0].AddAttribModifier (att);
					}
					foreach(VitalBuff vit in socket.VitalBuffs){
						item.EquippedSockets[0].AddVitalModifier (vit);
					}
					
					socket.RemoveAllBuffs();
					
					item.EquippedSockets.Add (socket);
					item.UsedSockets += 1;
					addedSocket = true;
				}
				else {
					item.EquippedSockets.Add (player.Inventory[slotOfSocket] as SocketItem);
					item.UsedSockets += 1;
					addedSocket = true;
				}
			}
			else {
				Debug.Log ("No socket space!");
			}
		}
		else {
			Debug.Log ("Can't add sockets to this item!");
		}
		
		if(addedSocket){
			player.Inventory.RemoveAt(slotOfSocket);
		}
		
		itemIsPickedUp = false;
		itemIsForUse = false;
		pickedUpItemIdentifier = "";
	}
	
	public static void SetPickedUpItem(Texture2D icon, string identifier){
		itemPickedUpIcon = icon;
		pickedUpItemIdentifier = identifier;
	}
	
	public void DrawPickedUpItem(int windowid){
		GUI.Box(new Rect(0,0,40,40),itemPickedUpIcon);
		if(itemIsForUse)
			GUI.Box(new Rect(0,0,40,40),"Use","UseSocket");
	}
	
	private void PositionPickUpWindow(){
		mousePos = Event.current.mousePosition;
		_pickedUpIconRect = ClampToScreen(new Rect(mousePos.x + 5, mousePos.y + 5,_pickedUpIconRect.width,_pickedUpIconRect.height));
	}
	
	private int FindInventorySpot(string id) {
	
		if(id.StartsWith ("Inventory"))
		{
			int arrayNumber = int.Parse(id.Substring(9));
			
			if(player.Inventory[arrayNumber] != null)
				return arrayNumber;
		}
		
		return 1000;
	}
	
	public void TrashIconDropItem(){
		WindowType sourceType = GetSourceOfItem(pickedUpItemIdentifier);
		if(sourceType == WindowType.Inventory){
			inventoryWindowGUI.InventoryDo("DropItem",FindInventorySpot(pickedUpItemIdentifier));
			itemIsPickedUp = false;
			pickedUpItemIdentifier = "";
		}
	}
	
	public void SwapItems(string targetIdentifier){
		WindowType sourceType = GetSourceOfItem(pickedUpItemIdentifier);
		WindowType targetType = GetSourceOfItem(targetIdentifier);
		
		if(sourceType == WindowType.Inventory){
			if(targetType == WindowType.Inventory){
				InventoryToInventory(targetIdentifier);
			}
			else if (targetType == WindowType.CharacterWindow){
				InventoryToCharacterWindow(targetIdentifier);
			}
			//bank
			//vendor
			//skillbar
		}
		else {
			Debug.Log ("Diff target mate");
		}
	}
	private void InventoryToInventory(string targetIdentifier){
		bool removeItem = false;
		
		//For Inventory
		int origItemLocation = FindInventorySpot(pickedUpItemIdentifier);
		int targetLocation = FindInventorySpot(targetIdentifier);
		
		if(player.Inventory[origItemLocation].Name == player.Inventory[targetLocation].Name){
			
			if(origItemLocation != targetLocation){
				for (int i = 0; i < player.Inventory[origItemLocation].CurStacks; i++) {
					if(player.Inventory[targetLocation].CurStacks + 1 <= player.Inventory[targetLocation].MaxStacks){
						player.Inventory[targetLocation].CurStacks += 1;
						player.Inventory[origItemLocation].CurStacks -= 1;
						i--;
						if(player.Inventory[origItemLocation].CurStacks == 0){
							removeItem = true;
						}
					}
				}
				itemIsPickedUp = false;
				pickedUpItemIdentifier = "";
				if(removeItem) player.Inventory.RemoveAt (origItemLocation);
	
				return;
			}
			else {
				itemIsPickedUp = false;
				pickedUpItemIdentifier = "";
				return;
			}
			
			
		}
		else {
			Item temp = player.Inventory[origItemLocation];
			player.Inventory[origItemLocation] = player.Inventory[targetLocation];
			player.Inventory[targetLocation] = temp;
			itemIsPickedUp = false;
			pickedUpItemIdentifier = "";
		}
	}
	private void InventoryToCharacterWindow(string targetIdentifier){
		string origItemLocation = pickedUpItemIdentifier;
		int targetLocation = FindInventorySpot(origItemLocation);
		
		Item temp = player.EquipedWeapon;
		switch(targetIdentifier){
			//Returning Equipped Items
			case "EquipedWeapon":
				temp = player.EquipedWeapon;
				break;
			case "EquipedArmorHead":
				temp = player.EquipedArmorHead;
				break;
			case "EquipedArmorChest":
				temp = player.EquipedArmorChest;
				break;
			case "EquipedArmorGloves":
				temp = player.EquipedArmorGloves;
				break;
			case "EquipedArmorLegs":
				temp = player.EquipedArmorLegs;
				break;
			case "EquipedArmorFeet":
				temp = player.EquipedArmorFeet;
				break;
			case "EquipedArmorBack":
				temp = player.EquipedArmorBack;
				break;
		}
	
		bool success = EquipAnItem(targetLocation);
		bool removeItem = true;
		
		if(success){
			switch(targetIdentifier){
				//Returning Equipped Items
				case "EquipedWeapon":
					if(player.EquipedWeapon == temp)removeItem = false;
					break;
				case "EquipedArmorHead":
					if(player.EquipedArmorHead == temp)removeItem = false;
					break;
				case "EquipedArmorChest":
					if(player.EquipedArmorChest == temp)removeItem = false;
					break;
				case "EquipedArmorGloves":
					if(player.EquipedArmorGloves == temp)removeItem = false;
					break;
				case "EquipedArmorLegs":
					if(player.EquipedArmorLegs == temp)removeItem = false;
					break;
				case "EquipedArmorFeet":
					if(player.EquipedArmorFeet == temp)removeItem = false;
					break;
				case "EquipedArmorBack":
					if(player.EquipedArmorBack == temp)removeItem = false;
					break;
			}
			
			if(removeItem){
				if(temp != null)
					player.Inventory[targetLocation] = temp;
			}
		}
		else {
			Debug.Log ("Can't Equip this!");
		}
		
		player.UpdateStats();
		itemIsPickedUp = false;
		pickedUpItemIdentifier = "";
	}
	
	private WindowType GetSourceOfItem(string targetIdentifier){
		if(targetIdentifier.StartsWith ("Inventory"))
		{
			return WindowType.Inventory;
		}
		if (targetIdentifier.StartsWith("Equiped"))
		{
			return WindowType.CharacterWindow;
		}		
		return WindowType.None;
	}
	
	#endregion
	
	#region "ToolTip Window"
	
	public void toolTipWindow(int windowid){
		
		if(Event.current.type == EventType.layout){
			objectForTooltip = FindToolTipType(_toolTip);
			if(objectForTooltip == null)
				tooltipIsText = true;
			else
				tooltipIsText = false;
		}
		
		if(!tooltipIsText){
			if(objectForTooltip.ItemType == ItemEquipType.Weapon)
				DrawWepToolTip(objectForTooltip);
			else if(objectForTooltip.ItemType == ItemEquipType.Clothing)
				DrawArmorToolTip(objectForTooltip);
			else if(objectForTooltip.ItemType == ItemEquipType.Consumable)
				DrawConsumToolTip(objectForTooltip);
			else if(objectForTooltip.ItemType == ItemEquipType.QuestItem)
				DrawQuestItemToolTip(objectForTooltip);
			else if(objectForTooltip.ItemType == ItemEquipType.Socket)
				DrawSocketToolTip(objectForTooltip);
			else if(objectForTooltip.ItemType == ItemEquipType.CraftItem)
				DrawCraftMaterialToolTip(objectForTooltip);
		}
		else if(tooltipIsText){
			if(_toolTip.Contains("Achievements")){
				DrawAchievement();
			}
			else
				GUILayout.Box (_toolTip,"ToolTipTextOnly");
		}
		
	}
	
	public void DrawWepToolTip(Item item){
		Weapon wep = item as Weapon;
		//For All
		GUILayout.Box (wep.Name,"ToolTipTitle");
			GUILayout.BeginHorizontal(GUILayout.MaxWidth(_toolTipWidth));
				GUILayout.Label (wep.Icon,wep.Rarity.ToString(),GUILayout.Width(75),GUILayout.Height(75));
				GUI.Box(GUILayoutUtility.GetLastRect(),"+" + wep.Enchants,"ToolTipInfoOverlay");
				GUILayout.BeginVertical(GUILayout.MaxHeight (75));
					GUILayout.Label (wep.MaxHit.ToString(),"ToolTipMainInfo",GUILayout.Height(55),GUILayout.Width (170));
					GUILayout.Label ("Max Hit","ToolTipInfoLabel",GUILayout.Height(20));
				GUILayout.EndVertical();
				GUILayout.Label (wep.ItemType.ToString(),"ToolTipType");
			GUILayout.EndHorizontal();
		
		//Body Tooltip
		GUILayout.Box (wep.ToolTip(),"ToolTipBody",GUILayout.MaxWidth(_toolTipWidth));
		
		
		GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
		
			for (int i = 0; i < wep.Sockets - wep.UsedSockets; i++) {
				GUILayout.Box ("",GUILayout.Width (40),GUILayout.Height(40));
			}
		
			for (int i = wep.EquippedSockets.Count; i --> 0; )
			{
			    GUILayout.Box (wep.EquippedSockets[i].Icon,GUILayout.Width (40),GUILayout.Height(40));
			}
		GUILayout.EndHorizontal ();
		//All
		GUILayout.Box ("_____________________________________","Seperator");
		GUILayout.Box("\"" + wep.Description + "\"","ToolTipDescription");
		
		//Footer
		GUILayout.BeginHorizontal ();
			GUILayout.Box(wep.Rarity.ToString(),"ToolTipFooter", GUILayout.Width (125));
			GUILayout.Box ("Lv." + wep.RequiredLevel.ToString(),"ToolTipFooter", GUILayout.Width (60));
			GUILayout.FlexibleSpace();
			GUILayout.Label(Resources.Load ("Item Icons/goldItem") as Texture2D,"ToolTipFooter",GUILayout.Height(15),GUILayout.Width (15));
			GUILayout.Box (wep.Value.ToString(),"ToolTipFooter");
		GUILayout.EndHorizontal ();
	
		
	}
	
	public void DrawArmorToolTip(Item item){
		Armor arm = item as Armor;
		
		//For All
		GUILayout.Box (arm.Name,"ToolTipTitle");
			GUILayout.BeginHorizontal(GUILayout.MaxWidth(_toolTipWidth));
				GUILayout.Label (arm.Icon,arm.Rarity.ToString(),GUILayout.Width(75),GUILayout.Height(75));
				GUILayout.BeginVertical(GUILayout.MaxHeight (75));
					GUILayout.Label (arm.ArmorAmt.ToString(),"ToolTipMainInfo",GUILayout.Height(55),GUILayout.Width (170));
					GUILayout.FlexibleSpace();
					GUILayout.Label ("Armor","ToolTipInfoLabel",GUILayout.Height(20));
				GUILayout.EndVertical();
				GUILayout.Label (arm.Slot.ToString(),"ToolTipType");
			GUILayout.EndHorizontal();
		
		//Body Tooltip
		GUILayout.Box (arm.ToolTip(),"ToolTipBody",GUILayout.MaxWidth(_toolTipWidth));
		
		
		GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
		
			for (int i = 0; i < arm.Sockets - arm.UsedSockets; i++) {
				GUILayout.Box ("",GUILayout.Width (40),GUILayout.Height(40));
			}
		
			for (int i = arm.EquippedSockets.Count; i --> 0; )
			{
			    GUILayout.Box (arm.EquippedSockets[i].Icon,GUILayout.Width (40),GUILayout.Height(40));
			}
		GUILayout.EndHorizontal ();
		//All
		GUILayout.Box ("_____________________________________","Seperator");
		GUILayout.Box("\"" + arm.Description + "\"","ToolTipDescription");
		
		//Footer
		GUILayout.BeginHorizontal ();
			GUILayout.Box(arm.Rarity.ToString(),"ToolTipFooter", GUILayout.Width (125));
			GUILayout.Box ("Lv." + arm.RequiredLevel.ToString(),"ToolTipFooter", GUILayout.Width (60));
			GUILayout.FlexibleSpace();
			GUILayout.Label(Resources.Load ("Item Icons/goldItem") as Texture2D,"ToolTipFooter",GUILayout.Height(15),GUILayout.Width (15));
			GUILayout.Box (arm.Value.ToString(),"ToolTipFooter");
		GUILayout.EndHorizontal ();
	}
	
	public void DrawConsumToolTip(Item item){
		Consumable consum = item as Consumable;
		
		//For All
		GUILayout.Box (consum.Name,"ToolTipTitle");
			GUILayout.BeginHorizontal(GUILayout.MaxWidth(_toolTipWidth));
				GUILayout.Label (consum.Icon,consum.Rarity.ToString(),GUILayout.Width(75),GUILayout.Height(75));
				GUI.Box(GUILayoutUtility.GetLastRect(),consum.TierRN(),"ToolTipInfoOverlay");
				GUI.Box(GUILayoutUtility.GetLastRect(),consum.CurStacks.ToString(),"ToolTipStacks");
				GUILayout.BeginVertical(GUILayout.MaxHeight (75));
					GUILayout.Label ("+" + consum.AmountToHeal.ToString(),"ToolTipMainInfo",GUILayout.Height(55));
					GUILayout.Label (consum.VitalToRestore.ToString(),"ToolTipInfoLabel",GUILayout.Height(20));
				GUILayout.EndVertical();
				GUILayout.Label (consum.ItemType.ToString(),"ToolTipType");
			GUILayout.EndHorizontal();
		
		//Body Tooltip
		GUILayout.Box (consum.ToolTip(),"ToolTipBody",GUILayout.MaxWidth(_toolTipWidth));
		//All
		GUILayout.Box ("_____________________________________","Seperator");
		GUILayout.Box("\"" + consum.Description + "\"","ToolTipDescription");
		
		//Footer
		GUILayout.BeginHorizontal ();
			GUILayout.Box(consum.Rarity.ToString(),"ToolTipFooter", GUILayout.Width (125));
			GUILayout.Box ("Lv." + consum.RequiredLevel.ToString(),"ToolTipFooter", GUILayout.Width (60));
			GUILayout.FlexibleSpace();
			GUILayout.Label(Resources.Load ("Item Icons/goldItem") as Texture2D,"ToolTipFooter",GUILayout.Height(15),GUILayout.Width (15));
			GUILayout.Box (consum.Value.ToString(),"ToolTipFooter");
		GUILayout.EndHorizontal ();
	}
	
	public void DrawCraftMaterialToolTip(Item item){
		CraftItem craftMat = item as CraftItem;
		
		//For All
		GUILayout.Box (craftMat.Name,"ToolTipTitle");
			GUILayout.BeginHorizontal(GUILayout.MaxWidth(_toolTipWidth));
				GUILayout.Label (craftMat.Icon,craftMat.Rarity.ToString(),GUILayout.Width(75),GUILayout.Height(75));
				GUI.Box(GUILayoutUtility.GetLastRect(),craftMat.CurStacks.ToString(),"ToolTipStacks");
				GUILayout.BeginVertical(GUILayout.MaxHeight (75));
					GUILayout.Label ("Material","ToolTipMainInfo",GUILayout.Height(55));
					GUILayout.Label ("","ToolTipInfoLabel",GUILayout.Height(20));
				GUILayout.EndVertical();
				GUILayout.Label (craftMat.ItemType.ToString(),"ToolTipType");
			GUILayout.EndHorizontal();
		
		//Body Tooltip
		//GUILayout.Box (craftMat.ToolTip(),"ToolTipBody",GUILayout.MaxWidth(_toolTipWidth));
		//All
		GUILayout.Box ("_____________________________________","Seperator");
		GUILayout.Box("\"" + craftMat.Description + "\"","ToolTipDescription");
		
		//Footer
		GUILayout.BeginHorizontal ();
			GUILayout.Box(craftMat.Rarity.ToString(),"ToolTipFooter", GUILayout.Width (125));
			GUILayout.Box ("","ToolTipFooter", GUILayout.Width (60));
			GUILayout.FlexibleSpace();
			GUILayout.Label(Resources.Load ("Item Icons/goldItem") as Texture2D,"ToolTipFooter",GUILayout.Height(15),GUILayout.Width (15));
			GUILayout.Box (craftMat.Value.ToString(),"ToolTipFooter");
		GUILayout.EndHorizontal ();
	}
	
	public void DrawSocketToolTip(Item item){
		SocketItem socket = item as SocketItem;
		
		//For All
		GUILayout.Box (socket.Name,"ToolTipTitle");
			GUILayout.BeginHorizontal(GUILayout.MaxWidth(_toolTipWidth));
				GUILayout.Label (socket.Icon,socket.Rarity.ToString(),GUILayout.Width(75),GUILayout.Height(75));
				GUI.Box(GUILayoutUtility.GetLastRect(),socket.TierRN(),"ToolTipInfoOverlay");
				GUILayout.BeginVertical(GUILayout.MaxHeight (75));
					string buffString = "";	
					int buffAmt = 0;
					if(socket.VitalBuffs.Count > 0){
						buffString = socket.VitalBuffs[0].vital.ToString();
						buffAmt = socket.VitalBuffs[0].amount;
					}
					else {
						buffString = socket.AttribBuffs[0].attribute.ToString();
						buffAmt = socket.AttribBuffs[0].amount;
					}
					GUILayout.Label ("+" + buffAmt.ToString(),"ToolTipMainInfo",GUILayout.Height(55));
					GUILayout.Label (buffString	,"ToolTipInfoLabel",GUILayout.Height(20));
				GUILayout.EndVertical();
				GUILayout.Label (socket.ItemType.ToString(),"ToolTipType");
			GUILayout.EndHorizontal();
		
		//Body Tooltip
		GUILayout.Box (socket.ToolTip(),"ToolTipBody",GUILayout.MaxWidth(_toolTipWidth));
		//All
		GUILayout.Box ("_____________________________________","Seperator");
		GUILayout.Box("\"" + socket.Description + "\"","ToolTipDescription");
		
		//Footer
		GUILayout.BeginHorizontal ();
			GUILayout.Box(socket.Rarity.ToString(),"ToolTipFooter", GUILayout.Width (125));
			GUILayout.Box ("Any Lv","ToolTipFooter", GUILayout.Width (60));
			GUILayout.FlexibleSpace();
			GUILayout.Label(Resources.Load ("Item Icons/goldItem") as Texture2D,"ToolTipFooter",GUILayout.Height(15),GUILayout.Width (15));
			GUILayout.Box (socket.Value.ToString(),"ToolTipFooter");
		GUILayout.EndHorizontal ();
	}
	
	public void DrawQuestItemToolTip(Item item){
		QuestItem questItem = item as QuestItem;
		
		//For All
		GUILayout.Box (questItem.Name,"ToolTipTitle");
			GUILayout.BeginHorizontal(GUILayout.MaxWidth(_toolTipWidth));
				GUILayout.Label (questItem.Icon,questItem.Rarity.ToString(),GUILayout.Width(75),GUILayout.Height(75));
				GUILayout.BeginVertical(GUILayout.MaxHeight (75));
					GUILayout.Label (questItem.CurStacks.ToString(),"ToolTipMainInfo",GUILayout.Height(55));
					GUILayout.Label ("In Stack","ToolTipInfoLabel",GUILayout.Height(20));
				GUILayout.EndVertical();
				GUILayout.Label (questItem.ItemType.ToString(),"ToolTipType");
			GUILayout.EndHorizontal();
		
		//Body Tooltip
		GUILayout.Box (questItem.ToolTip(),"ToolTipBody",GUILayout.MaxWidth(_toolTipWidth));
		//All
		GUILayout.Box ("_____________________________________","Seperator");
		GUILayout.Box("\"" + questItem.Description + "\"","ToolTipDescription");
		
		//Footer
		GUILayout.BeginHorizontal ();
			GUILayout.Box(questItem.Rarity.ToString(),"ToolTipFooter", GUILayout.Width (125));
			GUILayout.Box ("Any Lv","ToolTipFooter", GUILayout.Width (60));
			GUILayout.FlexibleSpace();
			GUILayout.Label(Resources.Load ("Item Icons/goldItem") as Texture2D,"ToolTipFooter",GUILayout.Height(15),GUILayout.Width (15));
			GUILayout.Box (questItem.Value.ToString(),"ToolTipFooter");
		GUILayout.EndHorizontal ();
	}
	
	public void DrawAchievement(){
		
		int arrayNumber = int.Parse(_toolTip.Substring(12));
		Achievement a = QuestTrackerGUI.achievementsHandler.achievements[arrayNumber];
		
		//For All
		GUILayout.Box (a.AchievementName,"ToolTipTitle");
			GUILayout.BeginHorizontal(GUILayout.MaxWidth(_toolTipWidth));
				string guiStyle = a.IsAchieved ? "AchievementSlot" : "AchievementLocked";
		
				
				GUILayout.BeginVertical(GUILayout.Width(80),GUILayout.Height(100));
					GUILayout.Label (a.AchievementIcon,guiStyle,GUILayout.Width(80),GUILayout.Height(80));
					GUILayout.Label (a.AchievementScore.ToString(),"ToolTipAchievementScore",GUILayout.Width(80),GUILayout.Height(10));
				GUILayout.EndVertical();
		
				GUILayout.BeginVertical(GUILayout.Width(200));
					GUILayout.Label (a.AchievementDescription,"ToolTipBody");
					string showDate = a.IsAchieved ? "Date Achieved:" + a.DateAchieved.ToShortDateString() : "";
					GUILayout.Label (showDate,"ToolTipBody");
				GUILayout.EndVertical();
			GUILayout.EndHorizontal();
	}
	
	private void PositionToolTipWindow(){
		mousePos = Event.current.mousePosition;
		_toolTipRect = ClampToScreen(new Rect(mousePos.x + 5, mousePos.y + 5,_toolTipWidth,_toolTipRect.height));
		_toolTipRect.height = 0;
	}
	
	public void SetToolTip() {
		if(Event.current.type == EventType.repaint ){
			if(GUI.tooltip  == ""){
				//Then Set tool tip to empty
				_toolTip = GUI.tooltip;
				_displayToolTip=false;
			}
			
			if(GUI.tooltip != ""){
				_toolTip = GUI.tooltip;
				_displayToolTip=true;
			}
		
		}
	}
	
	private Item FindToolTipType(string _toolTip) {
	
		if(_toolTip.StartsWith ("Inventory"))
		{
			int arrayNumber = int.Parse(_toolTip.Substring(9));
			
			if(player.Inventory[arrayNumber] != null)
				return player.Inventory[arrayNumber];
		}
		if (_toolTip.StartsWith("Equiped"))
		{
			switch(_toolTip){
				case "EquipedWeapon":
					return player.EquipedWeapon;
				case "EquipedArmorHead":
					return player.EquipedArmorHead;
				case "EquipedArmorChest":
					return player.EquipedArmorChest;
				case "EquipedArmorLegs":
					return player.EquipedArmorLegs;
				case "EquipedArmorGloves":
					return player.EquipedArmorGloves;
				case "EquipedArmorFeet":
					return player.EquipedArmorFeet;
				case "EquipedArmorBack":
					return player.EquipedArmorBack;
			}
		}
		if(_toolTip.StartsWith("QuestReward")){
			int arrayNumber = int.Parse(_toolTip.Substring(11));
			if(QuestTrackerGUI.selectedNPCQuest.QuestReward[arrayNumber] != null){
				return QuestTrackerGUI.selectedNPCQuest.QuestReward[arrayNumber];
			}

			return null;
		}
		if(_toolTip.StartsWith("CraftMat"))
		{
			int arrayNumber = int.Parse(_toolTip.Substring(8));
			
			if(NPCDialogGUI.blackSmith.itemsNeeded[arrayNumber] != null)
				return NPCDialogGUI.blackSmith.itemsNeeded[arrayNumber];
		}
		if(_toolTip.StartsWith("Craft"))
		{
			int arrayNumber = int.Parse(_toolTip.Substring(5));
			
			if(CraftedItemsClasses.AllCraftableItems[arrayNumber] != null)
				return CraftedItemsClasses.AllCraftableItems[arrayNumber];
		}
		if(_toolTip.StartsWith("NPCQuest"))
		{
			//NPCQuest0 underscore 1
			
			//Split quest identifier by underscore, so can detect 2 digit numbers
			string[] questIdentifiers = _toolTip.Split ('_');			
			int questArrayNumber = int.Parse(questIdentifiers[0].Substring(8));
			int itemArrayNumber = int.Parse(questIdentifiers[1]);
			
			if(NPCDialogGUI.openedNPCDialog.quests[questArrayNumber].QuestReward[itemArrayNumber] != null)
				return NPCDialogGUI.openedNPCDialog.quests[questArrayNumber].QuestReward[itemArrayNumber];
			
		}
		if(_toolTip.StartsWith("Stash"))
		{
			int arrayNumber = int.Parse(_toolTip.Substring(5));
			
			if(player.Stash[arrayNumber] != null)
				return player.Stash[arrayNumber];
		}
		if(_toolTip.StartsWith("Vendor"))
		{
			int arrayNumber = int.Parse(_toolTip.Substring(6));
			
			if(arrayNumber <= 15){
				if(arrayNumber <= NPCDialogGUI.openedNPCDialog.vendor.vendorItems.Count-1){
					if(NPCDialogGUI.openedNPCDialog.vendor.vendorItems[arrayNumber] != null){
						return NPCDialogGUI.openedNPCDialog.vendor.vendorItems[arrayNumber];
					}
				}
			}
			else {
				arrayNumber -= 16;
				if(arrayNumber <= NPCDialogGUI.openedNPCDialog.vendor.buyBackItems.Count){
					if(NPCDialogGUI.openedNPCDialog.vendor.buyBackItems[arrayNumber] != null){
						return NPCDialogGUI.openedNPCDialog.vendor.buyBackItems[arrayNumber];
					}
				}
			}
		}	
		
		return null;
	}
	#endregion
	
	#region "Equip Item Functions"
	
	public bool EquipAnItem(int cnt){
		
		Item itemToEquip = player.Inventory[cnt];
		
		if(itemToEquip.ItemType == ItemEquipType.Weapon){
				Debug.Log ("EquippingWeaponItem");
				if(player.EquipedWeapon == null) {
				player.EquipedWeapon = player.Inventory[cnt];
				player.Inventory.RemoveAt(cnt);
				}
				else {
					Item temp = player.EquipedWeapon;
					player.EquipedWeapon = player.Inventory[cnt];
					player.Inventory[cnt] = temp;
				}
		}			

		else if (itemToEquip.ItemType == ItemEquipType.Clothing){
			
			if((itemToEquip as Armor).Slot == EquipmentSlot.Chest){
				Debug.Log ("EquippingChestItem");
				if(player.EquipedArmorChest == null) {
				player.EquipedArmorChest = player.Inventory[cnt];
				player.Inventory.RemoveAt(cnt);
				}
				else {
					Item temp = player.EquipedArmorChest;
					player.EquipedArmorChest = player.Inventory[cnt];
					player.Inventory[cnt] = temp;
				}
			}
			
			if((itemToEquip as Armor).Slot == EquipmentSlot.Helmet){
				Debug.Log ("EquippingHelmetItem");
				if(player.EquipedArmorHead == null) {
				player.EquipedArmorHead = player.Inventory[cnt];
				player.Inventory.RemoveAt(cnt);
				}
				else {
					Item temp = player.EquipedArmorHead;
					player.EquipedArmorHead = player.Inventory[cnt];
					player.Inventory[cnt] = temp;
				}
			}
			
			if((itemToEquip as Armor).Slot == EquipmentSlot.Pants){
				Debug.Log ("EquippingLegsItem");
				if(player.EquipedArmorLegs == null) {
				player.EquipedArmorLegs = player.Inventory[cnt];
				player.Inventory.RemoveAt(cnt);
				}
				else {
					Item temp = player.EquipedArmorLegs;
					player.EquipedArmorLegs = player.Inventory[cnt];
					player.Inventory[cnt] = temp;
				}
			}
			
			if((itemToEquip as Armor).Slot == EquipmentSlot.Gloves){
				Debug.Log ("EquippingGlovesItem");
				if(player.EquipedArmorGloves == null) {
				player.EquipedArmorGloves = player.Inventory[cnt];
				player.Inventory.RemoveAt(cnt);
				}
				else {
					Item temp = player.EquipedArmorGloves;
					player.EquipedArmorGloves = player.Inventory[cnt];
					player.Inventory[cnt] = temp;
				}
			}
			
			if((itemToEquip as Armor).Slot == EquipmentSlot.Boots){
				Debug.Log ("EquippingFeetItem");
				if(player.EquipedArmorFeet == null) {
				player.EquipedArmorFeet = player.Inventory[cnt];
				player.Inventory.RemoveAt(cnt);
				}
				else {
					Item temp = player.EquipedArmorFeet;
					player.EquipedArmorFeet = player.Inventory[cnt];
					player.Inventory[cnt] = temp;
				}
			}
			
			if((itemToEquip as Armor).Slot == EquipmentSlot.Back){
				Debug.Log ("EquippingBackItem");
				if(player.EquipedArmorBack == null) {
				player.EquipedArmorBack = player.Inventory[cnt];
				player.Inventory.RemoveAt(cnt);
				}
				else {
					Item temp = player.EquipedArmorBack;
					player.EquipedArmorBack = player.Inventory[cnt];
					player.Inventory[cnt] = temp;
				}
			}
		}
		else{
			Debug.Log ("Cannot equip this.");
			return false;
		}
		
		player.UpdateStats();
		_toolTip = "";
		return true;
		
	}
	#endregion
	
	#region "Char Mini and Exp Bar"
	
	private void DisplayPlayerVitalsAndExp(){
		//vital bars
		float hp = (float)(player.GetVital((int)VitalName.Health).CurValue) / (float)(player.GetVital((int)VitalName.Health).MaxValue) * 125.0f;
		float mp = (float)(player.GetVital((int)VitalName.Mana).CurValue) / (float)(player.GetVital((int)VitalName.Mana).MaxValue) * 125.0f;;
		float ep = (float)(player.GetVital((int)VitalName.Energy).CurValue) / (float)(player.GetVital((int)VitalName.Energy).MaxValue) * 125.0f;;
		
		string hptxt = string.Format ("HP: {0}/{1}",player.GetVital((int)VitalName.Health).CurValue,player.GetVital((int)VitalName.Health).MaxValue);
		string mptxt = string.Format ("MP: {0}/{1}",player.GetVital((int)VitalName.Mana).CurValue,player.GetVital((int)VitalName.Mana).MaxValue);
		string eptxt = string.Format ("EP: {0}/{1}",player.GetVital((int)VitalName.Energy).CurValue,player.GetVital((int)VitalName.Energy).MaxValue);
		
		GUI.Box (new Rect(20,15,125,15),string.Format("{0} [Lv.{1}]",player.Name,player.Level),"Bar");	//player name
		
		GUI.Box (new Rect(20,35,125,10),"","Bar"); //bg
		GUI.Box (new Rect(20,35,hp,10),"","HPBar");
		GUI.Box (new Rect(20,35,125,10),hptxt,"TextValues"); //label
		
		GUI.Box (new Rect(20,50,125,10),"","Bar"); //bg
		GUI.Box (new Rect(20,50,mp,10),"","MPBar");
		GUI.Box (new Rect(20,50,125,10),mptxt,"TextValues"); //label
		
		GUI.Box (new Rect(20,65,125,10),"","Bar"); //bg
		GUI.Box (new Rect(20,65,ep,10),"","EPBar");
		GUI.Box (new Rect(20,65,125,10),eptxt,"TextValues"); //label
		
		float exp = (float)player.Exp / (float)player.ExpToLevel * 455;
		string exptxt = string.Format ("EXP: {0}/{1}",player.Exp,player.ExpToLevel);
		GUI.Box (new Rect(Screen.width/2 - 225,Screen.height - 30,455,10),"","MPBar"); //exp bar bg
		GUI.Box (new Rect(Screen.width/2 - 225,Screen.height - 30,exp,10),"","EPBar"); //exp bar
		GUI.Box (new Rect(Screen.width/2 - 225,Screen.height - 30,455,10),exptxt,"TextValues"); //label
		
	}
	
	#endregion
		
}