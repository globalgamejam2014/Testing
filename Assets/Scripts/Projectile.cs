using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

	private Vector2 fireDirection;
	private float fireSpeed;
	private float damage = 1;
	public int playerID;

	public Transform explosion;

	// Use this for initialization
	void Start () {
		damage = transform.lossyScale.x / 0.2F;
	}

	void FixedUpdate () {

	}

	public void Setup (Vector2 direction, float speed) {
		fireDirection = direction.normalized;
		if(fireDirection == Vector2.zero){
			fireDirection = new Vector2(1,0);
			transform.localPosition += Vector3.right;
		}
		fireSpeed = speed;
		rigidbody.AddForce(new Vector3(fireDirection.x * 10, fireDirection.y * 10, 0), ForceMode.VelocityChange);
	}

	void OnCollisionEnter(Collision collision){
		if(collision.gameObject.name.Contains ("Player")){
			if(collision.gameObject.GetComponent<Player>().jUID.GetIDNumber() != playerID){
				collision.gameObject.GetComponent<Player>().TakeDamage(damage, playerID);
			}
		}

		Instantiate (explosion, transform.position, Quaternion.identity);

		Destroy(gameObject);
	}

	void Update() {

		transform.rotation = Quaternion.FromToRotation (Vector3.right, rigidbody.velocity);

	}




}
