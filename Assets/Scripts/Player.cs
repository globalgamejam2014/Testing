using UnityEngine;
using System.Collections;


//TODO



public class Player : MonoBehaviour, IJoviosControllerListener {
	
	private Jovios jovios;
	private JoviosUserID jUID;
	private bool is_jumping = false;
	private float jumpStart;
	public Transform playerController;							//set as parent of this gameObject
	public Transform powerupController;							//set to powerup controller in inspector



	//Moddable player attributes - can be changed by powerups
	public float health;										//Initial (and maximum) player health
	public float healthDefault;
	public float jumpSpeed;										//Jump "impulse" strength
	public float jumpSpeedDefault;
	public float runSpeed;										//maximum speed player can move
	public float runSpeedDefault;
	public float runAcceleration;								//acceleration rate of player when moving left/right
	public float runAccelerationDefault;
	public float projectileFireRate;							//rate of fire
	public float projectileFireRateDefault;
	public float projectileSpeed;								//projectile speed
	public float projectileSpeedDefault;
	public float playerSize;									//player size; 1 by default
	public float playerSizeDefault;

	public int lives;											//number of lives remaining

	public bool controlsInverted;								//are the player's controls inverted?
	public bool controlsInvertedDefault;

	public Vector3 gravityVector;								//vector representing direction of gravity on the player
	public Vector3 gravityVectorDefault;

	public float speedProportion;								//ratio of current horizontal speed to maximum horizontal speed

	public string heldPowerup;									//powerup currently held by the player 


	void Start () {

	

		health = 100.0F;
		healthDefault = health;
		jumpSpeed = 15.0F;
		jumpSpeedDefault = jumpSpeed;
		runSpeed = 5.0F;
		runSpeedDefault = runSpeed;
		runAcceleration = 3.0F;
		runAccelerationDefault = runAcceleration;
		projectileFireRate = 1.0F;
		projectileFireRateDefault = projectileFireRate;
		projectileSpeed = 1.0F;
		projectileSpeedDefault = projectileSpeed;
		playerSize = 1.0F;
		playerSizeDefault = playerSize;

		lives = 3;
		controlsInverted = false;
		controlsInvertedDefault = controlsInverted;
		gravityVector = new Vector3(0.0f,-0.5F,0.0f);
		gravityVectorDefault = gravityVector;

		heldPowerup = null;

	}
	
	void FixedUpdate () {
		//check speed proportion, for speed capping purposes
		speedProportion = (rigidbody.velocity.x) / runSpeed;

		//Gravity tick
		rigidbody.AddForce (gravityVector, ForceMode.VelocityChange);

		//Player Movement - Running
		if (Mathf.Abs (speedProportion) < 1) {
			rigidbody.AddForce (new Vector3 (jovios.GetPlayer (jUID).GetInput ("left").GetDirection ().x * runAcceleration, 0, 0), ForceMode.VelocityChange);
		}

		//Cap horizontal movement speed
		if (Mathf.Abs (speedProportion) > 1) {
			 
			rigidbody.AddForce(new Vector3(-speedProportion,0,0), ForceMode.VelocityChange);

		}


		//Player Movement - Jumping
		if(is_jumping){
			rigidbody.AddForce(new Vector3(0,jumpSpeed,0), ForceMode.VelocityChange);
			//Debug.Log (jumpSpeed);

			is_jumping = false;
		}

	

	}



	void Update () {

		//Activate Powerup - PLACEHOLDER. Need a GetInput from controller.
		if (jovios.GetPlayer (jUID).GetInput ("left").GetDirection ().y > 0.5f && heldPowerup != null) {
			powerupController.GetComponent<PU_Controller>().ActivatePowerup (heldPowerup, this, false);
			heldPowerup = null;

		}

		if (jovios.GetPlayer (jUID).GetInput ("left").GetDirection ().y < -0.5f) {
			Debug.Log (runSpeed + " " + runSpeedDefault);
			
		}


	}


	bool IJoviosControllerListener.ButtonEventReceived(JoviosButtonEvent e){
		switch(e.GetResponse()){
		case "JumpA":
			switch(e.GetAction()){
			case "press":
				is_jumping = true;
				jumpStart = Time.time;
				break;
			case "release":
				is_jumping = false;
				break;
			}
			break;
		default:
			break;
		}
		return false;
	}
	
	public void PlayerSetup(JoviosPlayer p){
		jUID = p.GetUserID();
		jovios = MenuManager.jovios;
		jovios.AddControllerListener(this, jUID);
		transform.parent = playerController;
	}


	


}
