using UnityEngine;
using System.Collections;

public class Sumo : MonoBehaviour {
	
	public int segmentCount = 2;
	private int segmentLag = 10;
	public float speed;
	public Vector2 movement;
	public Vector2 facing;
	public float attackPower = 0;
	public float attackMax = 100;
	public float attackTime;
	private float attackDuration = 0.2F;
	public bool is_attacking = false;
	
	private NetworkPlayer myPlayer;
	private int playerNumber;
	public string playerName;
	private string playerCharacter;
	private TextMesh playerCharacterMesh;
	private Color primary;
	private Color secondary;
	
	private Transform body;
	private Transform hand;

	// Use this for initialization
	void Start () {
		body = transform.FindChild("Body");
		hand = transform.FindChild("Hand");
		speed = 1;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(!body.GetComponent<SumoCollision>().is_ringOut){	
			if(body.rigidbody.velocity.magnitude < 1){
				body.rigidbody.velocity = Vector3.zero;
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
			transform.Translate( new Vector3(movement.y/1000, movement.x/1000, 0));
			body.rigidbody.angularVelocity = Vector3.zero;
			float handScale = Mathf.Min (0.8F, 0.3F + attackPower / attackMax / 3);
			hand.localScale = new Vector3(handScale, handScale, handScale);
			if(is_attacking){
				if(attackTime + attackDuration/2 > Time.time){
					hand.localPosition += body.up /2;
				}
				else if (attackTime + attackDuration > Time.time){
					hand.localPosition -= body.up /2;
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
	
	public void Hit(){
		Debug.Log ("Hit");
	}
	
	public void Attack(){
		is_attacking = true;
		attackTime = Time.time;
	}
}
