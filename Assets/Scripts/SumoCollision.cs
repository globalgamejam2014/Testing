using UnityEngine;
using System.Collections;

public class SumoCollision : MonoBehaviour {
	
	private int lastPlayerHit;
	private float ringOutTime;
	private float ringOutDuration = 3;
	public bool is_ringOut = false;
	public Vector3 startPosition;
	
	void Update(){
		if(is_ringOut){
			transform.Translate(0,0,(Time.time - ringOutTime)/10);
			if(ringOutDuration + ringOutTime < Time.time){
				is_ringOut = false;
				transform.position = startPosition;
				rigidbody.velocity = Vector3.zero;
				rigidbody.angularVelocity = Vector3.zero;
			}
		}
	}
	
	void OnTriggerExit(Collider other){
		if(other.name == "Ring"){
			is_ringOut = true;
			ringOutTime = Time.time;
			rigidbody.velocity = new Vector3(rigidbody.velocity.x/4, rigidbody.velocity.y/4,0);
			Debug.Log ("RingOut");
		}
	}
	
	void OnTriggerEnter(Collider other){
		if(other.transform.name == "Hand" && transform.parent != other.transform.parent){
			Debug.Log (other.transform.localScale.x);
			rigidbody.velocity = (2 * other.transform.localScale.x * (transform.position - other.transform.position));
		}
	}
	
	void OnTriggerStay(Collider other){
		if(other.transform.name == "Hand" && transform.parent != other.transform.parent){
			Debug.Log (other.transform.localScale.x);
			rigidbody.velocity = (100 * other.transform.localScale.x * (transform.position - other.transform.position));
		}
	}
	
	void Hit(){
		transform.parent.SendMessage("Hit", SendMessageOptions.DontRequireReceiver);
	}
}
