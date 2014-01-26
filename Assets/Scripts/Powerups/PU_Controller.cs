using UnityEngine;
using System.Collections;
using System.Collections.Generic;




public class PU_Controller : MonoBehaviour {


	public Transform playerController;
	public List<Player> playerScripts = new List<Player>();
	public float powerupDuration;
	public float powerupTimer = 4;
	public float powerupPreviousSpawnTime;
	public GameObject powerupBox;



	void Start () {
		powerupPreviousSpawnTime = Time.time;
		powerupDuration = 5.0F;

	}
	

	void Update () {
		if(powerupTimer + powerupPreviousSpawnTime < Time.time){

			//ok we need to spawn a pickup

			GameObject spawnLocs = GameObject.Find("PowerupSpawnLocations");

			PU_Spawner goodSpawner = null;
			int goodIndex = -1;

			for (int i = 0; i < spawnLocs.transform.childCount; i++) {
				PU_Spawner candidate = ((PU_Spawner)spawnLocs.transform.GetChild(i).GetComponent("PU_Spawner"));

				if (!candidate.hasPickup && (goodSpawner == null || goodSpawner.lastSpawnedTime > candidate.lastSpawnedTime)) {
					goodSpawner = candidate;
					goodIndex = i;
				}
			}

			if (goodIndex != -1) {
				Transform spawner =  GameObject.Find ("PowerupSpawnLocations").transform.GetChild (goodIndex);
				PU_PickupBox box = (PU_PickupBox)((GameObject)GameObject.Instantiate(powerupBox, spawner.position, Quaternion.identity)).GetComponent("PU_PickupBox");
				box.spawner = goodSpawner;
				goodSpawner.lastSpawnedTime = Time.time;
				goodSpawner.hasPickup = true;
				//record that we did a spawn
				powerupPreviousSpawnTime = Time.time;
			}

		}

	}



	public void ActivatePowerup (string powerup, Player playerScript, bool isPersonal ) {



		//Apply powerup to all player objects

			switch (powerup) {

				case "dblSpeed":
						//double player speed cap

					if (!isPersonal) {
						for (int i = 0 ; i < playerController.childCount ; i++) {

							playerScripts[i].runSpeed = playerScripts[i].runSpeed * 2.0F;
						}
					}

					if (isPersonal) {
						playerScript.runSpeed = playerScript.runSpeed * 2.0F;
					}
				break;

				case "halfSpeed":
						//halve player speed cap
					if (!isPersonal) {
						for (int i = 0 ; i < playerController.childCount ; i++) {
							
							playerScripts[i].runSpeed = playerScripts[i].runSpeed * 0.5F;
						}
					}

					if (isPersonal) {
						playerScript.runSpeed = playerScript.runSpeed * 0.5F;
					}
				break;

				case "dblJump":
						//double player jump impulse
					if (!isPersonal) {
						for (int i = 0 ; i < playerController.childCount ; i++) {
							
							playerScripts[i].jumpSpeed = playerScripts[i].jumpSpeed * 2.0F;
						}
					}

					if (isPersonal) {
						playerScript.jumpSpeed = playerScript.jumpSpeed * 2.0F;
					}
				break;

				case "halfJump":
						//halve player jump impulse
					if (!isPersonal) {
						for (int i = 0 ; i < playerController.childCount ; i++) {
							
							playerScripts[i].jumpSpeed = playerScripts[i].jumpSpeed * 2.0F;
						}
					}

					if (isPersonal) {
						playerScript.jumpSpeed = playerScript.jumpSpeed * 0.5F;
					}
				break;

				case "gravityInvert":
						//inverse gravity
					if (!isPersonal) {
						for (int i = 0 ; i < playerController.childCount ; i++) {
							
							playerScripts[i].gravityVector = -playerScripts[i].gravityVector;
						}
					}

					if (isPersonal) {
						playerScript.gravityVector = -playerScript.gravityVector;
					}
			
				break;

				case "controlInvert":
						//invert the control scheme
					if (!isPersonal) {
						for (int i = 0 ; i < playerController.childCount ; i++) {
							
							playerScripts[i].controlsInverted = true;
						}
					}

					if (isPersonal) {
						playerScript.controlsInverted = true;
					}
			
				break;

				case "dblFireRate":
						//double player fire rate
					if (!isPersonal) {
						for (int i = 0 ; i < playerController.childCount ; i++) {
							playerScripts[i].projectileFireRate = playerScripts[i].projectileFireRate * 2.0F;
						}
					}

					if (isPersonal) {
						playerScript.projectileFireRate = playerScript.projectileFireRate * 2.0F;
					}
				break;

				case "dblSize":
						//double player size
					if (!isPersonal) {
						for (int i = 0 ; i < playerController.childCount ; i++) {
							playerScripts[i].playerSize = playerScripts[i].playerSize * 2.0F;
						}
					}

					if (isPersonal) {
						playerScript.playerSize = playerScript.playerSize * 2.0F;
					}
				break;

				case "halfSize":
						//halve player size
					if (!isPersonal) {
						for (int i = 0 ; i < playerController.childCount ; i++) {
							playerScripts[i].playerSize = playerScripts[i].playerSize * 0.5F;
						}
					}

					if (isPersonal) {
						playerScript.playerSize = playerScript.playerSize * 0.5F;
					}
				break;

				case "oneHitKills":
						//set each player's health to 1.
					if (!isPersonal) {
						for (int i = 0 ; i < playerController.childCount ; i++) {
							playerScripts[i].health = 1.0F;
						}
					}

					if (isPersonal) {
						playerScript.health = 1.0F;
					}
				break;

				case "activateHazards":

				break;


				default:
				break;

			}



		StartCoroutine (EndPowerupEffect (powerup, playerScript, isPersonal));

	}




	public IEnumerator EndPowerupEffect(string powerup, Player playerScript, bool isPersonal) {

		yield return new WaitForSeconds (powerupDuration);

		switch (powerup) {
			
			case "dblSpeed":
			case "halfSpeed":
				//halve player speed cap
				if (!isPersonal) {
					for (int i = 0 ; i < playerController.childCount ; i++) {
						
						playerScripts[i].runSpeed = playerScripts[i].runSpeedDefault;
					}
				}
				
				if (isPersonal) {
					playerScript.runSpeed = playerScript.runSpeedDefault;
				}
			break;
			
			case "dblJump":
			case "halfJump":
				//halve player jump impulse
				if (!isPersonal) {
					for (int i = 0 ; i < playerController.childCount ; i++) {
						
						playerScripts[i].jumpSpeed = playerScripts[i].jumpSpeedDefault;
					}
				}
				
				if (isPersonal) {
					playerScript.jumpSpeed = playerScript.jumpSpeedDefault;
				}
			break;
			
			case "gravityInvert":
				//inverse gravity
				if (!isPersonal) {
					for (int i = 0 ; i < playerController.childCount ; i++) {
						
						playerScripts[i].gravityVector = playerScripts[i].gravityVectorDefault;
					}
				}
				
				if (isPersonal) {
					playerScript.gravityVector = playerScript.gravityVectorDefault;
				}
			
			break;
			
			case "controlInvert":
				//invert the control scheme
				if (!isPersonal) {
					for (int i = 0 ; i < playerController.childCount ; i++) {
						
						playerScripts[i].controlsInverted = false;
					}
				}
				
				if (isPersonal) {
					playerScript.controlsInverted = false;
				}
			
			break;
			
			case "dblFireRate":
				//double player fire rate
				if (!isPersonal) {
					for (int i = 0 ; i < playerController.childCount ; i++) {
						playerScripts[i].projectileFireRate = playerScripts[i].projectileFireRateDefault;
					}
				}
				
				if (isPersonal) {
					playerScript.projectileFireRate = playerScript.projectileFireRateDefault;
				}
			break;
			
			case "dblSize":
			case "halfSize":
				//halve player size
				if (!isPersonal) {
					for (int i = 0 ; i < playerController.childCount ; i++) {
						playerScripts[i].playerSize = playerScripts[i].playerSizeDefault;
					}
				}
				
				if (isPersonal) {
					playerScript.playerSize = playerScript.playerSizeDefault;
				}
			break;
			
			case "oneHitKills":
				//set each player's health to 1.
				if (!isPersonal) {
					for (int i = 0 ; i < playerController.childCount ; i++) {
						playerScripts[i].health = 100.0F;
					}
				}
				
				if (isPersonal) {
					playerScript.health = 100.0F;
				}
			break;
			
			case "activateHazards":
			
			break;
			
			
			default:
			break;
			
		}
		
	}
	
	
	
	
	
	
	
	//Maintains the list of Player gameObject scripts, so that we don't need to call getComponent every time we want to apply a powerup.
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
