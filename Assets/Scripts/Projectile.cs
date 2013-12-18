using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	
	public Vector3 facing = Vector3.zero;
	public int playerNumber = -1;
	public bool is_fired = false;
	public float fireTime;
	public float fireDuration = 1;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if(facing.magnitude > 0){
			if(!is_fired){
				fireTime = Time.time;
				is_fired = true;
			}
			rigidbody.velocity = facing * 10;
			if(fireTime + fireDuration / 2 < Time.time){
				Destroy(gameObject);
			}
		}
	}
}
