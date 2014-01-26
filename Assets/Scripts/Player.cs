using UnityEngine;
using System.Collections;


//TODO



public class Player : MonoBehaviour, IJoviosControllerListener {
	
	private Jovios jovios;
	public JoviosUserID jUID;
	private bool is_jumping = false;
	private float jumpStart;
	private bool is_shooting = false;
	private float shootStart;
	public Transform playerController;							//set as parent of this gameObject
	public Transform powerupController;							//set to powerup controller in inspector
	public Transform projectile;								//set to projectile
	
	
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
	
	public int controlMultiplier;								//if controls are normal, should be 1. If inverted, -1.
	
	public bool controlsInverted;								//are the player's controls inverted?
	public bool controlsInvertedDefault;
	
	public Vector3 gravityVector;								//vector representing direction of gravity on the player
	public Vector3 gravityVectorDefault;
	
	public float speedProportion;								//ratio of current horizontal speed to maximum horizontal speed
	
	public string heldPowerup;									//powerup currently held by the player 

	public LineRenderer lineRendererComponent;

	public Animator anim;

	public bool isFacingRight;

	public Transform aimTrajectory;

	void Start () {

		isFacingRight = true;

		Player_Controller.UpdateLivesList (jUID.GetIDNumber());
		
		health = 3.0F;
		healthDefault = health;
		jumpSpeed = 10.0F;
		jumpSpeedDefault = jumpSpeed;
		runSpeed = 5.0F;
		runSpeedDefault = runSpeed;
		runAcceleration = 0.5F;
		runAccelerationDefault = runAcceleration;
		projectileFireRate = 1.0F;
		projectileFireRateDefault = projectileFireRate;
		projectileSpeed = 1.0F;
		projectileSpeedDefault = projectileSpeed;
		playerSize = 1.0F;
		playerSizeDefault = playerSize;
		
		controlsInverted = false;
		controlsInvertedDefault = controlsInverted;
		gravityVector = new Vector3(0.0f,-0.5F,0.0f);
		gravityVectorDefault = gravityVector;
		
		heldPowerup = null;

		anim = GetComponentInChildren<Animator> ();
		 

		gameObject.AddComponent<LineRenderer>();
		lineRendererComponent = transform.GetComponent<LineRenderer> ();
		
	}
	
	void FixedUpdate () {
		
		switch (controlsInverted) {
		case true: controlMultiplier = -1;
			break;
		case false: controlMultiplier = 1;
			break;
			
		}
		
		//check speed proportion, for speed capping purposes
		speedProportion = (rigidbody.velocity.x) / runSpeed;
		
		//Gravity tick
		rigidbody.AddForce (gravityVector, ForceMode.VelocityChange);
		
		//Player Movement - Running
		if (Mathf.Abs (speedProportion) < 1) {
			rigidbody.AddForce (controlMultiplier * new Vector3 (jovios.GetPlayer (jUID).GetInput ("left").GetDirection ().x * runAcceleration, 0, 0), ForceMode.VelocityChange);
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
		//player shooting, this creates a bullet
		if(is_shooting && shootStart + projectileFireRate < Time.time){
			GameObject bullet = (GameObject) GameObject.Instantiate(projectile.gameObject, transform.position, Quaternion.identity);
			bullet.GetComponent<Projectile>().Setup(jovios.GetPlayer(jUID).GetInput("left").GetDirection(), projectileSpeed);
			shootStart = Time.time;
		}
		
		
		
	}
	
	
	
	void Update () {

		if (rigidbody.velocity.x < 0 && isFacingRight) {
			isFacingRight = false;
			Vector3 tempVar = anim.transform.localScale;
			tempVar.x *= -1;
			anim.transform.localScale = tempVar;
		}

		else if (rigidbody.velocity.x > 0 && !isFacingRight) {
			isFacingRight = true;
			Vector3 tempVar = anim.transform.localScale;
			tempVar.x *= -1;
			anim.transform.localScale = tempVar;
		}


		//Update line renderer points
		//lineRendererInstance

		anim.SetFloat("speedX", Mathf.Abs (rigidbody.velocity.x));

		lineRendererComponent.SetPosition(0, transform.position);
		lineRendererComponent.SetPosition (1, transform.position + new Vector3(jovios.GetPlayer(jUID).GetInput("left").GetDirection().normalized.x,jovios.GetPlayer(jUID).GetInput("left").GetDirection().normalized.y,0));

		//aimTrajectory.position = transform.position + new Vector3(0,0,-2);
		//aimTrajectory.rotation = Quaternion.FromToRotation(Vector3.right, new Vector3 (jovios.GetPlayer (jUID).GetInput ("left").GetDirection ().normalized.x, jovios.GetPlayer (jUID).GetInput ("left").GetDirection ().normalized.y, 0));

		
		if (jovios.GetPlayer (jUID).GetInput ("left").GetDirection ().y < -0.5f) {
			
			
		}

		
		
	}
	
	
	
	
	public void TakeDamage(float damage) {
		Debug.Log(health);
		health -= damage;
		
		
		if (health < 0) {
			Kill();
		}
		
	}
	
	
	private void Kill() {
		Player_Controller.DecrementLives(jUID.GetIDNumber());
		Player_Controller.Respawn(jUID.GetIDNumber(), transform);
		health = 3;
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
		case "JumpB":
			switch(e.GetAction()){
			case "press":
				is_shooting = true;
				break;
			case "release":
				is_shooting = false;
				break;
			}
			break;
			
		case "powerup":
			powerupController.GetComponent<PU_Controller>().ActivatePowerup (heldPowerup, this, false);
			heldPowerup = null;
			JoviosControllerStyle controllerStyle = new JoviosControllerStyle();
			controllerStyle.AddAbsoluteJoystick("left", "Move Character", "Move");
			controllerStyle.AddButton2("right", new string[] {"Jump"}, new string[] {"Jump"});
			MenuManager.jovios.SetControls(jUID, controllerStyle);
			Debug.Log("powerup used");
			break;
		default:
			Debug.Log(e.GetResponse());
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
	
	void OnTriggerEnter(Collider other){
		if(other.transform.parent.name == "DamagingObjects"){
			TakeDamage(1);
		}
	}
	
	
	
}
