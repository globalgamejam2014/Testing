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
	
	private Transform body;
	private Transform hand;
	
	private float flashTime = 0.1F;
	private bool flash = false;

	// Use this for initialization
	void Start () {
		body = transform.FindChild("Body");
		hand = transform.FindChild("Hand");
		speed = 1;
	}
	
	void Update () {
		if(MenuManager.is_choosingArena){
			hand.renderer.enabled = false;
			hand.collider.enabled = false;
		}
		else{
			hand.renderer.enabled = true;
			hand.collider.enabled = true;
		}
		if(is_boostActive){
			switch(activeBoost){
			case BonusType.Speed:
				speed = 1.5F;
				range = 1;
				strength = 1;
				defense = 1;
				hand.renderer.enabled = true;
				hand.collider.enabled = true;
				if(flashTime + 0.2F < Time.time){
					flashTime = Time.time;
					flash = !flash;
				}
				if(flash){
					body.renderer.material.color = primary;
					hand.renderer.material.color = primary;
				}
				else{
					body.renderer.material.color = Color.yellow;
					hand.renderer.material.color = Color.yellow;
				}
				if(boostStart + boostDuration < Time.time){
					speed = 1;
					is_boostActive = false;
					body.renderer.material.color = primary;
					hand.renderer.material.color = primary;
				}
				break;
			
			case BonusType.Immunity:
				defense = 100;
				range = 1;
				strength = 1;
				speed = 1;
				hand.renderer.enabled = true;
				hand.collider.enabled = true;
				if(flashTime + 0.2F < Time.time){
					flashTime = Time.time;
					flash = !flash;
				}
				if(flash){
					body.renderer.material.color = primary;
					hand.renderer.material.color = primary;
				}
				else{
					body.renderer.material.color = Color.white;
					hand.renderer.material.color = Color.white;
				}
				if(boostStart + boostDuration < Time.time){
					defense = 1;
					is_boostActive = false;
					body.renderer.material.color = primary;
					hand.renderer.material.color = primary;
				}
				break;
			
			case BonusType.Rampage:
				defense = 3;
				speed = 1.2F;
				hand.renderer.enabled = false;
				hand.collider.enabled = false;
				body.transform.name = "Hand";
				range = 1;
				if(flashTime + 0.2F < Time.time){
					flashTime = Time.time;
					flash = !flash;
				}
				if(flash){
					body.renderer.material.color = primary;
					hand.renderer.material.color = primary;
				}
				else{
					body.renderer.material.color = Color.red;
					hand.renderer.material.color = Color.red;
				}
				if(boostStart + boostDuration < Time.time){
					defense = 1;
					speed = 1;
					is_boostActive = false;
					hand.renderer.enabled = true;
					hand.collider.enabled = true;
					body.transform.name = "Body";
					body.renderer.material.color = primary;
					hand.renderer.material.color = primary;
				}
				break;
			
			case BonusType.Strength:
				strength = 2;
				range = 1;
				defense = 1;
				speed = 1;
				hand.renderer.enabled = true;
				hand.collider.enabled = true;
				if(flashTime + 0.2F < Time.time){
					flashTime = Time.time;
					flash = !flash;
				}
				if(flash){
					body.renderer.material.color = primary;
					hand.renderer.material.color = primary;
				}
				else{
					body.renderer.material.color = Color.green;
					hand.renderer.material.color = Color.green;
				}
				if(boostStart + boostDuration < Time.time){
					strength = 1;
					is_boostActive = false;
					body.renderer.material.color = primary;
					hand.renderer.material.color = primary;
				}
				break;
			
			case BonusType.Range:
				range = 2;
				strength = 1;
				defense = 1;
				speed = 1;
				hand.renderer.enabled = true;
				hand.collider.enabled = true;
				if(flashTime + 0.2F < Time.time){
					flashTime = Time.time;
					flash = !flash;
				}
				if(flash){
					body.renderer.material.color = primary;
					hand.renderer.material.color = primary;
				}
				else{
					body.renderer.material.color = Color.blue;
					hand.renderer.material.color = Color.blue;
				}
				if(boostStart + boostDuration < Time.time){
					range = 1;
					is_boostActive = false;
					body.renderer.material.color = primary;
					hand.renderer.material.color = primary;
				}
				break;
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
			transform.Translate( new Vector3(speed * movement.y/5, speed * movement.x/5, 0));
			body.rigidbody.angularVelocity = Vector3.zero;
			float handScale = Mathf.Min (0.5F * strength, (0.4F * attackPower / attackMax + 0.2F) * strength);
			hand.localScale = new Vector3(handScale, handScale, handScale);
			if(is_attacking){
				if(attackTime + attackDuration/2 > Time.time){
					hand.localPosition += body.up /2 * range;
				}
				else if (attackTime + attackDuration > Time.time){
					attackPower /= 2;
					hand.localPosition -= body.up /2 * range;
				}
				else{
					is_attacking = false;
					attackPower = 0;
				}
			}
			else{
				hand.position = body.up + body.position;
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
		body.renderer.material.color = primary;
		hand.renderer.material.color = primary;
		body.FindChild("Character").GetComponent<TextMesh>().color = secondary;
		body.FindChild("Character").GetComponent<TextMesh>().text = playerCharacter;
	}
	
	public void Attack(){
		is_attacking = true;
		attackTime = Time.time;
	}
}
