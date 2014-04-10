using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player_Controller : MonoBehaviour {


	//Key is the player's jUID.GetIDNumber, Value is the number of lives left.
	public static Dictionary<int, int> livesList = new Dictionary<int, int>();
	public static Dictionary<int, int> scores = new Dictionary<int, int>();

	public static int defaultLives = 3;

	private Jovios jovios;


	void Start () {
		jovios = MenuManager.jovios;
	}
	

	void Update () {
	
	}



	public static void UpdateScore(int userID){
		if (!scores.ContainsKey (userID)) {
			scores.Add (userID, 0);
		}
	}
	
	
	public static void IncrementScore(int userID) {
		
		//check the dictionary for the number of lives the player has...
		
		
		int playerScore = 0;
		scores.TryGetValue (userID, out playerScore);
		scores.Remove (userID);
		scores.Add (userID, playerScore + 1);
		if(MenuManager.jovios.GetPlayer(new JoviosUserID(userID)).GetPlayerObject(2) != null){
			MenuManager.jovios.GetPlayer(new JoviosUserID(userID)).GetPlayerObject(2).GetComponent<TextMesh>().text = (playerScore + 1).ToString();
		}
	}
	
	
	public static void DecrementLives(int userID) {
		
		//check the dictionary for the number of lives the player has...
		
		
		int lastLives = 3;
		livesList.TryGetValue (userID, out lastLives);
		//remove value, replace with one less
		livesList.Remove (userID);
		livesList.Add (userID, lastLives - 1);
		
	}

	public static void UpdateLivesList(int userID) {
	

		//check if this player is already on the list...
		if (!livesList.ContainsKey (userID)) {
			//if the player is not on the list, add it to the list and set its lives.
			livesList.Add (userID, defaultLives);

		}
		//if the player is on the list, do nothing (since lives should be decremented on death, not the birth of the next player object.)



		
	}


	public static void Respawn(int userID, Transform playerObject) {

		//check if player has any lives left...
		int livesLeft = 0;
		livesList.TryGetValue (userID, out livesLeft);

		if (livesLeft > 0) {

			playerObject.transform.position = GameObject.Find ("PlayerSpawnLocations").transform.GetChild (Mathf.FloorToInt(GameObject.Find ("PlayerSpawnLocations").transform.childCount * Random.value)).position;

		}

		if (livesLeft <= 0) {

			Destroy (playerObject.gameObject);
				
		}



	}



	public void PlayerControllerCleanup() {
		livesList.Clear();

	}


}
