using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class PU_Controller : MonoBehaviour {


	public Transform playerController;
	public List<Player> playerScripts = new List<Player>();




	void Start () {
	

	}
	

	void Update () {
	

	}



	public void ActivatePowerup (string powerup, Player playerScript, bool isPersonal ) {


		for (int i = 0 ; i < playerController.childCount ; i++) {


		}

		//Apply powerup to all player objects
		if (!isPersonal) {

			switch (powerup) {

			case "dblSpeed":
				//double player speed cap
				for (int i = 0 ; i < playerController.childCount ; i++) {

					playerScripts[i].runSpeed = playerScripts[i].runSpeed * 2.0F;
				}
				break;

			case "halfSpeed":
				//halve player speed cap
				for (int i = 0 ; i < playerController.childCount ; i++) {
					
					playerScripts[i].runSpeed = playerScripts[i].runSpeed * 0.5F;
				}
				break;

			case "dblJump":
				//double player jump impulse
				for (int i = 0 ; i < playerController.childCount ; i++) {
					
					playerScripts[i].jumpSpeed = playerScripts[i].jumpSpeed * 2.0F;
				}
				break;

			case "halfJump":
				//halve player jump impulse
				for (int i = 0 ; i < playerController.childCount ; i++) {
					
					playerScripts[i].jumpSpeed = playerScripts[i].jumpSpeed * 2.0F;
				}
				break;

			case "gravityInvert":
				break;

			case "controlInvert":
				break;

			case "dblFireRate":
				//double player fire rate
				for (int i = 0 ; i < playerController.childCount ; i++) {
					playerScripts[i].projectileFireRate = playerScripts[i].projectileFireRate * 2.0F;
				}
				break;

			case "dblSize":
				//double player size
				for (int i = 0 ; i < playerController.childCount ; i++) {
					playerScripts[i].playerSize = playerScripts[i].playerSize * 2.0F;
				}
				break;

			case "halfSize":
				//halve player size
				for (int i = 0 ; i < playerController.childCount ; i++) {
					playerScripts[i].playerSize = playerScripts[i].playerSize * 0.5F;
				}
				break;

			case "oneHitKills":
				break;

			case "activateHazards":
				break;


			default:
				break;

			}

		}


	}


	public void UpdatePlayerScripts() {


		playerScripts.Clear();

		for (int i = 0 ; i < playerController.childCount ; i++) {
			playerScripts.Add (playerController.GetChild (i).GetComponent<Player>());
			
		}


		for (int i = 0; i < playerController.childCount; i++) {
			Debug.Log (i);	
			Debug.Log (playerScripts[i]);
		}

	}



}
