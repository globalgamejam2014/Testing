using UnityEngine;
using System.Collections;




public class PU_2xspeed_bare : MonoBehaviour {

	

	void Start () {
	

	}

	void Update () {
	
	


	}



	void OnCollisionEnter (Collision col) {



		if (col.gameObject.name.Contains ("Player")) {

			Player playerScript = col.gameObject.GetComponent<Player>();

			//double player speed cap
			playerScript.heldPowerup = "dblSpeed";

			KillPowerUp ();


		
		}

	}





	void KillPowerUp () {
		Destroy (this.gameObject);
	}
	



}
