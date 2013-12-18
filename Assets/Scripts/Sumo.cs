using UnityEngine;
using System.Collections;



public class Sumo : MonoBehaviour {

	public float speed = 1;
	public float strength = 1;
	public float range = 1;
	public float defense = 1;
	public Vector2 movement;
	public Vector2 facing;
	public float attackPower = 0;
	public float attackMax = 120;
	public float attackTime;
	private float attackDuration = 0.2F;
	public bool is_attacking = false;
	
	private NetworkPlayer myPlayer;
	public int playerNumber;
	public string playerName;
	private string playerCharacter;
	private TextMesh playerCharacterMesh;
	private Color primary;
	private Color secondary;
	
	public string selectedArena = "";
	public BonusType activeBoost = BonusType.None;
	public bool is_boostActive = false;
	public float boostStart;
	public float boostDuration = 10;
	public Color flashColor = Color.clear;
	
	private Transform body;
	private Transform robot;
	private Transform hand;
	public GameObject projectile;
	private Transform modifiers;
	
	private float flashTime = 0.1F;
	private bool flash = false;

	// Use this for initialization
	void Start () {
		body = transform.FindChild("Body");
		robot = body.FindChild("Robot1").FindChild("Sphere");
		modifiers = transform.FindChild("Modifiers");
		hand = transform.FindChild("Hand");
		hand.GetComponent<Projectile>().playerNumber = playerNumber;
		speed = 1;
	}
	
	void Update () {
		if(MenuManager.is_choosingArena){
			hand.particleSystem.enableEmission = false;
			hand.collider.enabled = false;
			hand.renderer.enabled = false;
		}
		else{
			hand.particleSystem.enableEmission = true;
			hand.collider.enabled = true;
			hand.renderer.enabled = true;
		}
		if(is_boostActive){
			switch(activeBoost){
			case BonusType.Speed:
				speed = 1.5F;
				range = 1;
				strength = 1;
				defense = 1;
				flashColor = Color.yellow;
				body.transform.name = "Body";
				hand.particleSystem.enableEmission = true;
				hand.collider.enabled = true;
				hand.renderer.enabled = true;
				break;
			
			case BonusType.Immunity:
				defense = 100;
				range = 1;
				strength = 1;
				speed = 1;
				flashColor = Color.white;
				body.transform.name = "Body";
				hand.particleSystem.enableEmission = true;
				hand.collider.enabled = true;
				hand.renderer.enabled = true;
				break;
			
			case BonusType.Rampage:
				defense = 3;
				speed = 1.2F;
				hand.particleSystem.enableEmission = false;
				hand.collider.enabled = false;
				hand.renderer.enabled = false;
				body.transform.name = "Rampage";
				range = 1;
				flashColor = Color.red;
				break;
			
			case BonusType.Strength:
				strength = 2;
				range = 1;
				defense = 1;
				speed = 1;
				hand.particleSystem.enableEmission = true;
				hand.renderer.enabled = true;
				hand.collider.enabled = true;
				flashColor = Color.green;
				body.transform.name = "Body";
				break;
			
			case BonusType.Range:
				range = 2;
				strength = 1;
				defense = 1;
				speed = 1;
				hand.particleSystem.enableEmission = true;
				hand.collider.enabled = true;
				hand.renderer.enabled = true;
				flashColor = Color.blue;
				body.transform.name = "Body";
				break;
			}
			if(flashTime + 0.2F < Time.time){
				flashTime = Time.time;
				flash = !flash;
			}
			if(flash){
				robot.renderer.material.color = primary;
			}
			else{
				robot.renderer.material.color = flashColor;
			}
			if(boostStart + boostDuration < Time.time){
				defense = 1;
				speed = 1;
				range = 1;
				strength = 1;
				is_boostActive = false;
				hand.particleSystem.renderer.enabled = true;
				hand.renderer.enabled = true;
				hand.collider.enabled = true;
				body.transform.name = "Body";
				robot.renderer.material.color = primary;
			}
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(!body.GetComponent<SumoCollision>().is_ringOut){	
			if(body.rigidbody.velocity.magnitude < 1){
				body.rigidbody.velocity = new Vector3(0, 0, body.rigidbody.velocity.z);
			}
			else{
				body.rigidbody.velocity *= 0.95F;
			}
			if(facing != Vector2.zero){
				if(!is_attacking){
					attackPower++;
				}
				if((facing.y > 0)){
					body.eulerAngles = new Vector3(body.eulerAngles.x, body.eulerAngles.y, - Vector2.Angle(new Vector2(1,0), facing));
				}
				else{
					body.eulerAngles = new Vector3(body.eulerAngles.x, body.eulerAngles.y, Vector2.Angle(new Vector2(1,0), facing));
				}
			}
			else if(attackPower > 0 && !is_attacking){
				Attack();
			}
			transform.Translate( new Vector3(speed * movement.y/10, speed * movement.x/10, 0));
			body.rigidbody.angularVelocity = Vector3.zero;
			float handScale = Mathf.Min (0.5F * strength, (0.4F * attackPower / attackMax + 0.2F) * strength);
			hand.localScale = new Vector3(handScale, handScale, handScale);
			hand.particleSystem.startSize = handScale/3;
			hand.particleSystem.startLifetime = handScale/3;
			if(is_attacking){
				hand.GetComponent<Projectile>().facing = body.up;
				hand.GetComponent<Projectile>().fireDuration = range;
				is_attacking = false;
				attackPower = 0;
				hand.rigidbody.useGravity = true;
				hand.parent = modifiers;
				attackPower = 0;
				handScale = Mathf.Min (0.5F * strength, (0.4F * attackPower / attackMax + 0.2F) * strength);
				GameObject newHand = (GameObject) GameObject.Instantiate(projectile, body.up + body.position, Quaternion.identity);
				hand = newHand.transform;
				hand.GetComponent<Projectile>().playerNumber = playerNumber;
				hand.name = "Hand";
				hand.parent = transform;
				hand.localScale = new Vector3(handScale, handScale, handScale);
				hand.particleSystem.startSize = handScale/3;
				hand.particleSystem.startLifetime = handScale/3;
			}
			else{
				hand.position = body.up  + body.position;
			}
		}
		else{
			hand.position = body.up + body.position;
		}
	}
	
	public void SetMyPlayer (int player, Color primaryColor, Color secondaryColor, string newPlayerName){
		body.GetComponent<SumoCollision>().startPosition = body.position;
		myPlayer = NetworkManager.playerList[player];
		playerNumber = player;
		primary = primaryColor;
		secondary = secondaryColor;
		playerName = newPlayerName;
		if(newPlayerName.Length>0){
			playerCharacter = newPlayerName[0].ToString();
		}
		else{
			playerCharacter = "";
		}
		robot.renderer.material.color = primary;
		body.FindChild("Character").GetComponent<TextMesh>().color = secondary;
		body.FindChild("Character").GetComponent<TextMesh>().text = playerCharacter;
	}
	
	public void Attack(){
		is_attacking = true;
		attackTime = Time.time;
	}
}
