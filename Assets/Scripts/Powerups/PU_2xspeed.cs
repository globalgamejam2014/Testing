using UnityEngine;
using System.Collections;


//right now, powerup applies to one player as soon as he/she touches it.
//Need to have the powerup apply to every player in the scene, and ONLY after it is activated by a player which has picked it up.



public class PU_2xspeed : MonoBehaviour {


	public float powerupDuration;			//Duration of powerup effect
	
	void Start () {
	
		powerupDuration = 5.0F;

	}

	void Update () {
	
	


	}

	void OnCollisionEnter (Collision col) {



		if (col.gameObject.name.Contains ("Player")) {

			Player playerScript = col.gameObject.GetComponent<Player>();

			//double player speed cap
			playerScript.runSpeed = playerScript.runSpeed * 2;


			//hide powerup
			renderer.enabled = false;
			rigidbody.detectCollisions = false;

			//reset value after a specified time delay and clean up object
			StartCoroutine(ResetValue (powerupDuration, playerScript));

		
		}



	}

	public IEnumerator ResetValue(float time, Player playerScript) {

		Debug.Log ("poop");

		yield return new WaitForSeconds(time);
	

		playerScript.runSpeed = playerScript.runSpeedDefault;

		//remove powerup from scene
		KillPowerUp ();


	}


	void KillPowerUp () {
		Destroy (this.gameObject);
	}
	



}
