using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MobSpawner : MonoBehaviour {
	
	public GameObject monsterToSpawn;
	public int numberOfMobsAtOnce;
	public int spawnRange;
	public float timeToWaitBeforeRespawn;
	public List<GameObject> monstersSpawned;
	
	// Use this for initialization
	void Start () {
		for (int i = 0; i < numberOfMobsAtOnce; i++) {
			Vector3 randomPosition = new Vector3(Random.Range(transform.position.x - spawnRange, transform.position.x + spawnRange),
				transform.position.y,
				Random.Range(transform.position.z - spawnRange, transform.position.z + spawnRange));
			
			GameObject mobSpawned = Instantiate(monsterToSpawn,randomPosition,Quaternion.identity) as GameObject;
			mobSpawned.transform.parent = this.transform;
			monstersSpawned.Add(mobSpawned);
		}
		StartCoroutine(SpawnMobs());
	}
	
	IEnumerator SpawnMobs(){
		
		for (int i = 0; i < monstersSpawned.Count; i++) {
			if(monstersSpawned[i] == null){
				monstersSpawned.RemoveAt(i);
			}
		}
		
		if(monstersSpawned.Count < numberOfMobsAtOnce){
			
			Vector3 randomPosition = new Vector3(Random.Range(transform.position.x - spawnRange, transform.position.x + spawnRange),
				transform.position.y,
				Random.Range(transform.position.z - spawnRange, transform.position.z + spawnRange));
			
			GameObject mobSpawned = Instantiate(monsterToSpawn,randomPosition,Quaternion.identity) as GameObject;
			mobSpawned.transform.parent = this.transform;
			monstersSpawned.Add(mobSpawned);
		}
		
		
		
		yield return new WaitForSeconds(timeToWaitBeforeRespawn);
		StartCoroutine(SpawnMobs());
		
	}
}
