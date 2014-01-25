using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Player_Controller : MonoBehaviour {


	//Key is the player's jUID.GetIDNumber, Value is the number of lives left.
	public static Dictionary<int, int> livesList = new Dictionary<int, int>();

	public static int defaultLives = 3;


	void Start () {
	
	}
	

	void Update () {
	
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






	public void PlayerControllerCleanup() {
		livesList.Clear();

	}


}
