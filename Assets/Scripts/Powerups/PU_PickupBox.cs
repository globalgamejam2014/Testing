using UnityEngine;
using System.Collections;

//valid powerup strings:
//dblSpeed, halfSpeed, dblJump, halfJump, gravityInvert, controlInvert, dblFireRate, dblSize, halfSize, oneHitKills, activateHazards


public class PU_PickupBox : MonoBehaviour {

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

			playerScript.heldPowerup = powerupType;

			JoviosControllerStyle controllerStyle = new JoviosControllerStyle();
			controllerStyle.AddAbsoluteJoystick("left", "Move Character", "Move");
			controllerStyle.AddButton2("right", new string[] {"Jump"}, new string[] {"Jump"});
			controllerStyle.AddArbitraryButton(new int[] {-2,4,4,4}, powerupType, "powerup");
			MenuManager.jovios.SetControls(playerScript.jUID, controllerStyle);

			Debug.Log (powerupType);


			KillPowerUp ();


		
		}

	}
	


	void KillPowerUp () {
		Destroy (this.gameObject);
	}
	



}
