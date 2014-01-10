using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour {
	
	private Transform modifiers;
	public static int[] score;
	public static int winner = 0;
	public static GameObject chosenArena;
	public static GameObject[] arenas;
	public GameObject arena1;
	public GameObject arena2;
	public GameObject arena3;
	public GameObject arena4;
	public GameObject arenaSelection;	
	public static Transform bonusSpawners;
	public static float bonusSpawnTimer = 0;
	
	// Use this for initialization
	void Start () {
		arenas = new GameObject[] {arenaSelection, arena1, arena2, arena3, arena4}; 
		chosenArena = (GameObject) GameObject.Instantiate(arenaSelection, Vector3.zero, Quaternion.identity);
		modifiers = GameObject.Find ("Modifiers").transform;
	}
	
	// Update is called once per frame
	void Update () {
		if(bonusSpawnTimer + 8 < Time.time && MenuManager.gameState == GameState.GameOn){
			bonusSpawnTimer = Time.time;
			bonusSpawners.GetChild(Mathf.FloorToInt(bonusSpawners.childCount * Random.value)).FindChild("BonusSpawner").GetComponent<BonusSpawner>().bonusType = (BonusType)Mathf.FloorToInt(Random.value * 5);
		}
	}
	
	public static void StartRound(){
		for(int i = 0; i < Jovios.players.Length; i++){
			if(Jovios.players[i].statusObject.GetComponent<Status>().is_ready){
				Jovios.players[i].statusObject.GetComponent<Status>().score.text = "0";
				Jovios.players[i].statusObject.GetComponent<Status>().score.color = Color.white;
				Jovios.SentControls(Jovios.players[i].networkPlayer, 0, "Move Character",0, "Move for Direction\nRelease to Fire");
			}
			else{
				Jovios.SentBasicButtons("Waiting for round to complete.", Jovios.players[i].networkPlayer);
			}
		}
		score = new int[Jovios.players.Length];
		Debug.Log(score[0]);
	}
	
	public static void EndRound(){
		Destroy(chosenArena);
		MenuManager.gameState = GameState.ChooseArena;
		chosenArena = (GameObject) GameObject.Instantiate(arenas[0], Vector3.zero, Quaternion.identity);
		for(int i = 0; i < Jovios.players.Length; i++){
			Jovios.players[i].statusObject.GetComponent<Status>().xMark.renderer.enabled = true;
			Jovios.players[i].statusObject.GetComponent<Status>().checkMark.renderer.enabled = false;
		}
	}
	
	public static void ChooseArena(int selectedArena){
		if(selectedArena > 0){
			Destroy (chosenArena);
			chosenArena = (GameObject) GameObject.Instantiate(arenas[selectedArena], Vector3.zero, Quaternion.identity);
			MenuManager.lastTickTime = Time.time;
			MenuManager.gameState = GameState.Countdown;
			GameManager.StartRound();
			bonusSpawners = chosenArena.transform.FindChild("BonusSpawners");
			bonusSpawnTimer = Time.time;
		}
	}
}
