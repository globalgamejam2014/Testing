using UnityEngine;
using System.Collections;

public class DragonCollision : MonoBehaviour {
	
	void OnTriggerEnter(Collider other){
		if(other.transform.parent != transform.parent || other.name == "Segment(Clone)"){
			other.transform.parent.SendMessage("Hit", SendMessageOptions.DontRequireReceiver);
		}
		else if(other.transform.parent.name == "Bounds"){
			transform.parent.SendMessage("Hit", SendMessageOptions.DontRequireReceiver);
		}
	}
	
	void OnCollisionEnter(Collision collision){
		if(collision.transform.parent != transform){
			collision.transform.parent.SendMessage("Hit", SendMessageOptions.DontRequireReceiver);
		}
		else if(collision.transform.parent.name == "Bounds"){
			transform.parent.SendMessage("Hit", SendMessageOptions.DontRequireReceiver);
		}
	}
	
	void OnCollisionStay(Collision collision){
		if(collision.transform.parent.name == "Bounds"){
			transform.parent.SendMessage("Hit", SendMessageOptions.DontRequireReceiver);
		}
	}
	
	void Hit(){
		Debug.Log ("Hit");
		transform.parent.SendMessage("Hit", SendMessageOptions.DontRequireReceiver);
	}
}
