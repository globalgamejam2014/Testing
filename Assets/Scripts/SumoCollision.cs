using UnityEngine;
using System.Collections;

public class SumoCollision : MonoBehaviour {
	
	public int lastPlayerHit = -1;
	private float ringOutTime;
	private float ringOutDuration = 1.5F;
	public bool is_ringOut = false;
	public Vector3 startPosition;
	
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
			transform.parent.GetComponent<Sumo>().selectedArena = "Arena1";
			break;
		case "Arena2Platform":
			collision.transform.renderer.enabled = true;
			transform.parent.GetComponent<Sumo>().selectedArena = "Arena2";
			break;
		case "Arena3Platform":
			collision.transform.renderer.enabled = true;
			transform.parent.GetComponent<Sumo>().selectedArena = "Arena3";
			break;
		case "Arena4Platform":
			collision.transform.renderer.enabled = true;
			transform.parent.GetComponent<Sumo>().selectedArena = "Arena4";
			break;
		case "Platform":
			collision.transform.renderer.enabled = true;
			transform.parent.GetComponent<Sumo>().selectedArena = "";
			break;
		case "Rampage":
			lastPlayerHit = collision.transform.parent.GetComponent<Sumo>().playerNumber;
			rigidbody.velocity = (collision.transform.localScale.x * (transform.position - collision.transform.position) * 100 / transform.parent.GetComponent<Sumo>().defense);
			break;
		case "Hand":
			if(collision.transform.GetComponent<Projectile>().playerNumber != transform.parent.GetComponent<Sumo>().playerNumber){
			Debug.Log(collision.transform.GetComponent<Projectile>().playerNumber.ToString() + transform.parent.GetComponent<Sumo>().playerNumber.ToString());
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
	
	void OnCollisionStay(Collision collision){
		switch(collision.transform.name){
		case "Arena1Platform":
			collision.transform.renderer.material.color = Color.green;
			transform.parent.GetComponent<Sumo>().selectedArena = "Arena1";
			break;
		case "Arena2Platform":
			collision.transform.renderer.material.color = Color.green;
			transform.parent.GetComponent<Sumo>().selectedArena = "Arena2";
			break;
		case "Arena3Platform":
			collision.transform.renderer.material.color = Color.green;
			transform.parent.GetComponent<Sumo>().selectedArena = "Arena3";
			break;
		case "Arena4Platform":
			collision.transform.renderer.material.color = Color.green;
			transform.parent.GetComponent<Sumo>().selectedArena = "Arena4";
			break;
		case "Platform":
			collision.transform.renderer.material.color = Color.green;
			transform.parent.GetComponent<Sumo>().selectedArena = "";
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
				if(MenuManager.is_choosingArena){
					PlayerControls.statusObjects[transform.parent.GetComponent<Sumo>().playerNumber].GetComponent<Status>().status.text = "X";
					PlayerControls.statusObjects[transform.parent.GetComponent<Sumo>().playerNumber].GetComponent<Status>().status.color = Color.red;
					Camera.main.GetComponent<PlayerControls>().SentBasicButtons("Join Game", "Ready to Play?", NetworkManager.playerList[transform.parent.GetComponent<Sumo>().playerNumber]);
					Destroy(transform.parent.gameObject);
				}
				is_ringOut = true;
				ringOutTime = Time.time;
				rigidbody.velocity = new Vector3(rigidbody.velocity.x/4, rigidbody.velocity.y/4,0);
				transform.parent.GetComponent<Sumo>().attackPower = 0;
				if(lastPlayerHit > -1){
					MenuManager.score[lastPlayerHit]++;
					PlayerControls.statusObjects[lastPlayerHit].GetComponent<Status>().status.text = MenuManager.score[lastPlayerHit].ToString();
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
				transform.parent.GetComponent<Sumo>().boostDuration = 10;
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
