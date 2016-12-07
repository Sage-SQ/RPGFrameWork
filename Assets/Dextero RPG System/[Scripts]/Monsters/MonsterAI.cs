using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MonsterAI : MonoBehaviour {
	
	private PlayerNew player;
	public string NameOfMob = "Cube";
	public int LevelOfMob = 1;
	public MonsterType monsterType = MonsterType.Normal;
	public int Damage = 10;
	public int expValue = 30;
	
	public float RunSpeed = 2;
	public float AttacksPerSecond;
	private float OriginalAttacksPerSecond;
	public float timeSinceLastAttack;
	
	public int MonsterHealth;
	
	public string NumberOfItemsToDrop = "random";
	public string ItemTypesToDrop = "random";
	public string ItemsDroppedRarity = "random";
	
	
	
	
	
	//AI
	public bool inCombat;
	public bool isDead;
	public bool EnableCombat;
	
	//sgo
	public GlobalPrefabs globalPrefabs;
	
	//From Player Procs
	public bool isUseless; //Won't do anything
	public bool isStunned;	//can't move
	public bool isSlowed;	//Attack Speed is slowed
	public float amountSlowedBy;	//Attack speed reduction
	public bool isKnockedBack;	//If knocked back( so can't chain knockback)
	public List<string> currentDots = new List<string>();
	
	void Start(){
		if(player == null){
			player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerNew>();
		}
		
		if(globalPrefabs == null){
			globalPrefabs = GameObject.FindGameObjectWithTag("GameMaster").GetComponent<GlobalPrefabs>();
		}
		
		OriginalAttacksPerSecond = AttacksPerSecond;
	}
	
	void Update () {
		
		timeSinceLastAttack += Time.deltaTime;
		
		float curHealth = GetComponent<Health>().CurrentHealth;
		
		if(curHealth <= 0 ){
			MonsterHealth = 0;
			isDead = true;
		}
		
		if(isDead){
				player.AddExp(expValue);
				string nameOfMobToSend = monsterType == MonsterType.Normal ? NameOfMob : "BOSS" + NameOfMob;
				Messenger<string>.Broadcast("MonsterKilled",nameOfMobToSend);
				//Destroy all floating text attached to this
				FloatingTextGUI[] floatingTexts = GetComponentsInChildren<FloatingTextGUI>();
				for (int i = 0; i < floatingTexts.Length; i++) {
					Destroy(floatingTexts[i].gameObject,0.5f);
				}
			
				DropLoot();
				Destroy(gameObject);
	
		}
		else{		
			if(inCombat){
				EnableCombat =true;
			}
			else {
				EnableCombat = false;	
			}
			
			if(isStunned){
				EnableCombat = false;
			}
			
			if(isUseless){
				return;
			}
			
			if(EnableCombat){
				float angleToTarget = Mathf.Atan2((player.transform.position.x - transform.position.x), (player.transform.position.z - transform.position.z)) * Mathf.Rad2Deg;
				transform.eulerAngles = new Vector3(0,angleToTarget, 0);
				
				if(Vector3.Distance (transform.position,player.transform.position) > 2){
					transform.position = Vector3.MoveTowards(transform.position, player.transform.position, RunSpeed * Time.deltaTime);
				}
				
				bool canDealDamage = timeSinceLastAttack > 1 / AttacksPerSecond ? true : false;
						
				if(canDealDamage){
					timeSinceLastAttack = 0;
					PlayerHealth php = player.GetComponent<PlayerHealth>();
					if(php == null) Debug.Log ("No player health");
					DealDamage(php);
					canDealDamage = false;
				}
			}
		}
	}
	
	void DropLoot(){
		GameObject loot = GameObject.FindGameObjectWithTag("ItemSpawner");
		loot.GetComponent<ItemSpawner>().positionToSpawn = transform.position;
		
		if(ItemsDroppedRarity != null)
			loot.GetComponent<ItemSpawner>().chestRarity = ItemsDroppedRarity;
		
		if(ItemTypesToDrop != null)
			loot.GetComponent<ItemSpawner>().chestSpawnType = ItemTypesToDrop;
		
		if(NumberOfItemsToDrop != null)
			loot.GetComponent<ItemSpawner>().itemsInChest = NumberOfItemsToDrop;
		
		loot.GetComponent<ItemSpawner>().chestItemLevel = LevelOfMob;
		
		//Clear loot and populate with random items
		loot.GetComponent<ItemSpawner>().PopulateChest();
		
		//40% Chance to drop gold
		int amountOfGold = Random.Range(100*LevelOfMob,1000*LevelOfMob+1);
		int randomlyAddGold = Random.Range (0,101);
		if(randomlyAddGold > 60) {
			loot.GetComponent<ItemSpawner>().loot.Add (StaticItems.GoldItem(amountOfGold));
		}

		ItemSpawner lootOfMob = loot.GetComponent<ItemSpawner>();
		HandleQuestLoot(lootOfMob);
		
		//Finally, spawn all the items
		loot.GetComponent<ItemSpawner>().SpawnItems();
		
	}
	
	void HandleQuestLoot(ItemSpawner lootOfMob){
		bool dropLoot = false;
		int questIndex = 1000;
		for (int i = 0; i < player.QuestsInProgress.Count; i++) {
			if(player.QuestsInProgress[i].numberToKill > 0){
				if(player.QuestsInProgress[i].nameOfMobThatDropsItem.Contains(NameOfMob)){
					if(!player.QuestsInProgress[i].itemDone){
						dropLoot = true;
						questIndex = i;
					}
				}
			}
		}
		
		if(dropLoot){
			Item QuestItemToAdd = MobQuestItem(questIndex);
			
			//create a new instance
			QuestItemToAdd = new QuestItem(QuestItemToAdd.Name,QuestItemToAdd.Description,QuestItemToAdd.CurStacks,QuestItemToAdd.MaxStacks,QuestItemToAdd.Icon);
			
			if(QuestItemToAdd != null)
				lootOfMob.loot.Add (QuestItemToAdd);
		}
	}
	
	Item MobQuestItem(int questIndex) {
		Debug.Log (QuestItemsClasses.AllQuestItems.Count);
		for (int i = 0; i < QuestItemsClasses.AllQuestItems.Count; i++) {
			//If the name of the quest item is the name of the quest item
			if(player.QuestsInProgress[questIndex].nameOfItem == QuestItemsClasses.AllQuestItems[i].Name){
				return QuestItemsClasses.AllQuestItems[i];
			}
		}
		
		return null;
	}
	
	
	//Deal Damage
	void DealDamage(PlayerHealth hp){
		if(hp){
			
			hp.DealDamage((int)Damage);
	
			player.playerState = PlayerState.Combat;
			player.CheckForExitCombat();
		}
	}
	
	
	//Procs
	//KnockBack
	public void KnockBackSelf(){
		StartCoroutine("KnockBack");
	}
	private IEnumerator KnockBack(){
		StopCoroutine("KnockBack");
		isUseless = true;
		isKnockedBack = true; 
		
		GameObject dmgTxt = (GameObject)Instantiate(globalPrefabs.floatingDamageText);
		dmgTxt.transform.parent = this.transform;
		FloatingTextGUI dmgText = dmgTxt.GetComponent<FloatingTextGUI>();
		dmgText.SetDmgInfo(ToolTipStyle.Purple + "Knockback!" + ToolTipStyle.EndColor,transform.position);
		
		yield return new WaitForSeconds(0.5f);
		isKnockedBack = false;
		isUseless = false;
	}
	
	//Stun
	public void StunSelf(float timeToStunSelf){
		StartCoroutine("Stun",timeToStunSelf);
	}
	private IEnumerator Stun(float timeToStun){
		StopCoroutine("Stun");
		
		GameObject dmgTxt = (GameObject)Instantiate(globalPrefabs.floatingDamageText);
		dmgTxt.transform.parent = this.transform;
		FloatingTextGUI dmgText = dmgTxt.GetComponent<FloatingTextGUI>();
		dmgText.SetDmgInfo(ToolTipStyle.Purple + "Stunned!" + ToolTipStyle.EndColor,transform.position);
		
		isStunned = true;
		yield return new WaitForSeconds(timeToStun);
		isStunned = false;
	}
	
	//Slow
	public void SlowAttackSpeed(float amountToSlow){
		AttacksPerSecond = OriginalAttacksPerSecond;
		StartCoroutine("Slow",amountToSlow);
	}
	private IEnumerator Slow(float amountToReduceBy){//e.g. 0.3f = 30% attack speed lost
		StopCoroutine("Slow");
		
		GameObject dmgTxt = (GameObject)Instantiate(globalPrefabs.floatingDamageText);
		dmgTxt.transform.parent = this.transform;
		FloatingTextGUI dmgText = dmgTxt.GetComponent<FloatingTextGUI>();
		dmgText.SetDmgInfo(ToolTipStyle.Purple + "Attack slowed!" + ToolTipStyle.EndColor,transform.position);
		
		AttacksPerSecond += amountToReduceBy * AttacksPerSecond;
		yield return new WaitForSeconds(3.0f);
		AttacksPerSecond = OriginalAttacksPerSecond;
	}
	
	//DOT
	public void UseDot(string dotName, int damage,int ticks, float timeBetweenTicks){
		for (int i = 0; i < currentDots.Count; i++) {
			if(currentDots[i].Contains(dotName)){
				Debug.Log ("Same DOT, will not cast");
				return;
			}
		}
		
		StartCoroutine(DoDot(dotName,ticks,damage, timeBetweenTicks));
	}
			
	private IEnumerator DoDot(string dotName,int damage, int ticks, float timeBetweenTicks){
		currentDots.Add(dotName);
		for (int i = 0; i < ticks; i++) {
			this.GetComponent<Health>().CurrentHealth -= damage;
			
			GameObject dmgTxt = Instantiate(globalPrefabs.floatingDamageText) as GameObject;
			dmgTxt.transform.parent = this.transform;
			FloatingTextGUI dmgText = dmgTxt.GetComponent<FloatingTextGUI>();
			dmgText.SetDmgInfo(ToolTipStyle.Purple + damage.ToString() + ToolTipStyle.EndColor,transform.position);
			
			
			yield return new WaitForSeconds(timeBetweenTicks);
		}
		currentDots.Remove(dotName);
	}
	
}

public enum MonsterType {
	Normal,
	MiniBoss,
	Boss
}