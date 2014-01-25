using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	private Vector2 fireDirection;
	private float fireSpeed;
	private float damage = 1;

	// Use this for initialization
	void Start () {
	
	}

	void FixedUpdate () {

	}

	public void Setup (Vector2 direction, float speed) {
		fireDirection = direction.normalized;
		if(fireDirection == Vector2.zero){
			fireDirection = new Vector2(1,0);
		}
		fireSpeed = speed;
		transform.position += new Vector3(Mathf.Ceil(fireDirection.x) - 0.5F,0,0);  // offset for mouth
		rigidbody.AddForce(new Vector3(fireDirection.x * 10, fireDirection.y * 10, 0), ForceMode.VelocityChange);
	}

	void OnCollisionEnter(Collision collision){
		if(collision.gameObject.name.Contains ("Player")){
			collision.gameObject.GetComponent<Player>().TakeDamage(damage);
		}
		Destroy(gameObject);
	}
}
