using UnityEngine;
using System.Collections;

public class SumoCollision : MonoBehaviour {
	
	public int lastPlayerHit = -1;
	private float ringOutTime;
	private float ringOutDuration = 1.5F;
	public bool is_ringOut = false;
	public Vector3 startPosition;
	public int playerNumber;
	
	void Start(){
		playerNumber = transform.parent.GetComponent<Sumo>().playerNumber;
	}
	
	void Update(){
		if(is_ringOut){
			if(ringOutDuration + ringOutTime < Time.time){
				is_ringOut = false;
				transform.position = new Vector3(Random.value * 4, Random.value * 4, 0.5F);
				rigidbody.velocity = Vector3.zero;
				rigidbody.angularVelocity = Vector3.zero;
				transform.parent.GetComponent<Sumo>().boostDuration = 2;
				transform.parent.GetComponent<Sumo>().boostStart = Time.time;
				transform.parent.GetComponent<Sumo>().activeBoost = BonusType.Immunity;
				transform.parent.GetComponent<Sumo>().is_boostActive = true;
			}
		}
	}
	
	void OnCollisionEnter(Collision collision){
		switch(collision.transform.name){
		case "Arena1Platform":
			collision.transform.renderer.enabled = true;
			Jovios.players[playerNumber].statusObject.GetComponent<Status>().chosenArena = 1;
			break;
		case "Arena2Platform":
			collision.transform.renderer.enabled = true;
			Jovios.players[playerNumber].statusObject.GetComponent<Status>().chosenArena = 2;
			break;
		case "Arena3Platform":
			collision.transform.renderer.enabled = true;
			Jovios.players[playerNumber].statusObject.GetComponent<Status>().chosenArena = 3;
			break;
		case "Arena4Platform":
			collision.transform.renderer.enabled = true;
			Jovios.players[playerNumber].statusObject.GetComponent<Status>().chosenArena = 4;
			break;
		case "Platform":
			//collision.transform.renderer.enabled = true;
			Jovios.players[playerNumber].statusObject.GetComponent<Status>().chosenArena = 0;
			break;
		case "Rampage":
			lastPlayerHit = collision.transform.parent.GetComponent<Sumo>().playerNumber;
			rigidbody.velocity = (collision.transform.localScale.x * (transform.position - collision.transform.position) * 100 / transform.parent.GetComponent<Sumo>().defense);
			break;
		case "Hand":
			if(collision.transform.GetComponent<Projectile>().playerNumber != transform.parent.GetComponent<Sumo>().playerNumber){
				lastPlayerHit = collision.transform.GetComponent<Projectile>().playerNumber;
				rigidbody.velocity = (collision.transform.localScale.x * (transform.position - collision.transform.position) * 100 / transform.parent.GetComponent<Sumo>().defense);
				if(!collision.transform.GetComponent<Projectile>().is_fired){
					collision.transform.parent.GetComponent<Sumo>().attackPower = 0;
				}
				else{
					Destroy(collision.gameObject);
				}
			}
			break;
		default:
			break;
		}
	}
	
	void OnCollisionExit(Collision collision){
		switch(collision.transform.name){
		case "Arena1Platform":
			collision.transform.renderer.material.color = new Color(0.8F, 0.8F, 0.8F, 0.2F);
			break;
		case "Arena2Platform":
			collision.transform.renderer.material.color = new Color(0.8F, 0.8F, 0.8F, 0.2F);
			break;
		case "Arena3Platform":
			collision.transform.renderer.material.color = new Color(0.8F, 0.8F, 0.8F, 0.2F);
			break;
		case "Arena4Platform":
			collision.transform.renderer.material.color = new Color(0.8F, 0.8F, 0.8F, 0.2F);
			break;
		default:
			break;
		}
	}
	
	void OnTriggerEnter(Collider other){
		if(transform.parent != other.transform.parent){
			switch(other.transform.name){
			case "Bounds":
				if(MenuManager.gameState == GameState.ChooseArena){
					Jovios.players[playerNumber].statusObject.GetComponent<Status>().xMark.renderer.enabled = true;
					Jovios.players[playerNumber].statusObject.GetComponent<Status>().checkMark.renderer.enabled = false;
					Jovios.SetBasicButtons("Join Game", "Ready to Play?", Jovios.players[playerNumber].networkPlayer);
					Destroy(transform.parent.gameObject);
				}
				is_ringOut = true;
				ringOutTime = Time.time;
				rigidbody.velocity = new Vector3(rigidbody.velocity.x/4, rigidbody.velocity.y/4,0);
				transform.parent.GetComponent<Sumo>().attackPower = 0;
				if(lastPlayerHit > -1){
					GameManager.score[lastPlayerHit]++;
					if(GameManager.winner[0] > -1){
						if(GameManager.score[lastPlayerHit] > GameManager.score[GameManager.winner[0]] || GameManager.winner[0] == lastPlayerHit){
							for(int i = 0; i < Jovios.players.Length; i++){
								Jovios.players[i].statusObject.GetComponent<Status>().crown.renderer.enabled = false;
								Jovios.players[i].playerObject.GetComponent<Sumo>().crown.renderer.enabled = false;
							}
							GameManager.winner = new int[] {-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1,-1};
							GameManager.winner[0] = lastPlayerHit;
							Jovios.players[lastPlayerHit].statusObject.GetComponent<Status>().crown.renderer.enabled = true;
							Jovios.players[lastPlayerHit].playerObject.GetComponent<Sumo>().crown.renderer.enabled = true;
						}
						else if(GameManager.score[lastPlayerHit] == GameManager.score[GameManager.winner[0]]){
							int j = 0;
							Jovios.players[lastPlayerHit].statusObject.GetComponent<Status>().crown.renderer.enabled = true;
							Jovios.players[lastPlayerHit].playerObject.GetComponent<Sumo>().crown.renderer.enabled = true;
							for(int i = 0; i < GameManager.winner.Length; i++){
								if(GameManager.winner[i] == -1){
									j = i;
									break;
								}
							}
							GameManager.winner[j] = lastPlayerHit;
						}
					}
					else{
						GameManager.winner[0] = lastPlayerHit;
						Jovios.players[lastPlayerHit].statusObject.GetComponent<Status>().crown.renderer.enabled = true;
						Jovios.players[lastPlayerHit].playerObject.GetComponent<Sumo>().crown.renderer.enabled = true;
					}
					Jovios.players[lastPlayerHit].statusObject.GetComponent<Status>().score.text = GameManager.score[lastPlayerHit].ToString();
					lastPlayerHit = -1;
				}
				break;
			case "Bumper":
				rigidbody.velocity = (other.transform.localScale.x * (transform.position - other.transform.position) * 15 / transform.parent.GetComponent<Sumo>().defense);
				break;
			case "BonusBox":
				transform.parent.GetComponent<Sumo>().is_boostActive = true;
				transform.parent.GetComponent<Sumo>().activeBoost = other.transform.parent.GetComponent<BonusSpawner>().bonusType;
				transform.parent.GetComponent<Sumo>().boostStart = Time.time;
				transform.parent.GetComponent<Sumo>().boostDuration = 5;
				other.transform.parent.GetComponent<BonusSpawner>().bonusType = BonusType.None;
				break;
			}
		}
	}
	
	void OnTriggerStay(Collider other){
		if(transform.parent != other.transform.parent){
			switch(other.transform.name){
			case "Hand":
				lastPlayerHit = other.transform.parent.GetComponent<Sumo>().playerNumber;
				rigidbody.velocity = (other.transform.localScale.x * (transform.position - other.transform.position) * 100 / transform.parent.GetComponent<Sumo>().defense);
				other.transform.parent.GetComponent<Sumo>().attackPower = 0;
				break;
			case "Bumper":
				rigidbody.velocity = (other.transform.localScale.x * (transform.position - other.transform.position) * 15 / transform.parent.GetComponent<Sumo>().defense);
				break;
			}
		}
	}
}
