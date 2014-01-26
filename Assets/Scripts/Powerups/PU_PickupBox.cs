using UnityEngine;
using System.Collections;

//valid powerup strings:
//dblSpeed, halfSpeed, dblJump, halfJump, gravityInvert, controlInvert, dblFireRate, dblSize, halfSize, oneHitKills, activateHazards


public class PU_PickupBox : MonoBehaviour {
	
	public string powerupType;
	public string powerupImg;
	public int numPowerups = 10;
	public PU_Spawner spawner;

	void Start () {
	
		int randInt = Random.Range (0, numPowerups);
	
		switch (randInt) {
			
		case 0: powerupType = "dblSpeed";
			powerupImg = "http://geisertdotme.files.wordpress.com/2014/01/icon_dblspeed.png?w=48";
			break;
		case 1:	powerupType = "halfSpeed";
			powerupImg = "http://geisertdotme.files.wordpress.com/2014/01/icon_halfspeed.png?w=48";
			break;
		case 2:	powerupType = "dblJump";
			powerupImg = "http://geisertdotme.files.wordpress.com/2014/01/icon_dbljump.png?w=48";
			break;
		case 3:	powerupType = "halfJump";
			powerupImg = "http://geisertdotme.files.wordpress.com/2014/01/icon_halfjump.png?w=48";
			break;
		case 4:	powerupType = "gravityInvert";
			powerupImg = "http://geisertdotme.files.wordpress.com/2014/01/icon_gravityinvert.png?w=48";
			break;
		case 5:	powerupType = "controlInvert";
			powerupImg = "http://geisertdotme.files.wordpress.com/2014/01/icon_gravityinvert2-e1390766021412.png?w=48";
			break;
		case 6:	powerupType = "dblFireRate";
			powerupImg = "http://geisertdotme.files.wordpress.com/2014/01/icon_dblfirerate.png?w=48";
			break;
		case 7:	powerupType = "dblSize";
			powerupImg = "http://geisertdotme.files.wordpress.com/2014/01/icon_dblsize.png?w=48";
			break;
		case 8:	powerupType = "halfSize";
			powerupImg = "http://geisertdotme.files.wordpress.com/2014/01/icon_halfsize.png?w=48";
			break;
		case 9:	powerupType = "oneHitKills";
			powerupImg = "http://geisertdotme.files.wordpress.com/2014/01/icon_onehitkills.png?w=48";
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
			controllerStyle.AddArbitraryButton(new int[] {-2,4,4,4}, powerupImg, "powerup");
			MenuManager.jovios.SetControls(playerScript.jUID, controllerStyle);

			Debug.Log (powerupType);

			spawner.hasPickup = false;

			KillPowerUp ();


		
		}

	}
	


	void KillPowerUp () {
		Destroy (this.gameObject);
	}
	



}
