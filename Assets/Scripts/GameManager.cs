using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	
	private float foodSpawnTime;
	private float foodSpawnLag = 0;
	public GameObject foodObject;
	private Transform modifiers;
	
	// Use this for initialization
	void Start () {
		foodSpawnTime = Time.time;
		modifiers = GameObject.Find ("Modifiers").transform;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(PlayerControls.is_gameOn){
			if(foodSpawnTime + foodSpawnLag < Time.time){
				foodSpawnTime = Time.time;
				foodSpawnLag = 10 / NetworkManager.playerList.Length;
				GameObject foodItem = (GameObject) GameObject.Instantiate(foodObject, new Vector3((Random.value - 0.5F)*12, (Random.value - 0.5F)*9, 0), Quaternion.identity);
				foodItem.transform.parent = modifiers;
			}
		}
	}
}
