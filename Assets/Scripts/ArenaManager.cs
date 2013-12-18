using UnityEngine;
using System.Collections;

public class ArenaManager : MonoBehaviour {

	public Transform bonusSpawners;
	public float bonusSpawnTimer = 0;
	
	// Use this for initialization
	void Start () {
		bonusSpawners = transform.FindChild("BonusSpawners");
		bonusSpawnTimer = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if(bonusSpawnTimer + 8 < Time.time){
			bonusSpawnTimer = Time.time;
			bonusSpawners.GetChild(Mathf.FloorToInt(bonusSpawners.childCount * Random.value)).FindChild("BonusSpawner").GetComponent<BonusSpawner>().bonusType = (BonusType)Mathf.FloorToInt(Random.value * 5);
		}
	}
}
