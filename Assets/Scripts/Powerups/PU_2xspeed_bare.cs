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
			JoviosControllerStyle controllerStyle = new JoviosControllerStyle();
			controllerStyle.AddRelativeJoystick("left", "Move Character", "Move");
			controllerStyle.AddButton2("right", new string[] {"Jump"}, new string[] {"Jump"});
			controllerStyle.AddArbitraryButton(new int[] {-2, 4, 4, 4}, "Use Power", "dblSpeed");
			MenuManager.jovios.SetControls(playerScript.jUID, controllerStyle);

			KillPowerUp ();


		
		}

	}





	void KillPowerUp () {
		Destroy (this.gameObject);
	}
	



}
