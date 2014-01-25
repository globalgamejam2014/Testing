using UnityEngine;
using System.Collections;

//valid powerup strings:
//dblSpeed, halfSpeed, dblJump, halfJump, gravityInvert, controlInvert, dblFireRate, dblSize, halfSize, oneHitKills, activateHazards


public class PU_2xspeed_bare : MonoBehaviour {

	public string powerupType;
	public int numPowerups = 11;

	void Start () {
	
		int randInt = Random.Range (0, numPowerups);
	
		switch (randInt) {

		case 0: powerupType = "dblSpeed";
			break;
		case 1:	powerupType = "halfSpeed";
			break;
		case 2:	powerupType = "dblJump";
			break;
		case 3:	powerupType = "halfJump";
			break;
		case 4:	powerupType = "gravityInvert";
			break;
		case 5:	powerupType = "controlInvert";
			break;
		case 6:	powerupType = "dblFireRate";
			break;
		case 7:	powerupType = "dblSize";
			break;
		case 8:	powerupType = "halfSize";
			break;
		case 9:	powerupType = "oneHitKills";
			break;
		case 10:powerupType = "activateHazards";
			break;
		default:
			break;


		}


	}

	void Update () {
	
	


	}








	void OnCollisionEnter (Collision col) {



		if (col.gameObject.name.Contains ("Player")) {

			Player playerScript = col.gameObject.GetComponent<Player>();

			//double player speed cap
<<<<<<< HEAD
			playerScript.heldPowerup = powerupType;

			Debug.Log (powerupType);
=======
			playerScript.heldPowerup = "dblSpeed";
			JoviosControllerStyle controllerStyle = new JoviosControllerStyle();
			controllerStyle.AddRelativeJoystick("left", "Move Character", "Move");
			controllerStyle.AddButton2("right", new string[] {"Jump"}, new string[] {"Jump"});
			controllerStyle.AddArbitraryButton(new int[] {-2, 4, 4, 4}, "Use Power", "dblSpeed");
			MenuManager.jovios.SetControls(playerScript.jUID, controllerStyle);
>>>>>>> a788a031054d827ab4cfd07dbf65b848235988dc

			KillPowerUp ();


		
		}

	}
	


	void KillPowerUp () {
		Destroy (this.gameObject);
	}
	



}
