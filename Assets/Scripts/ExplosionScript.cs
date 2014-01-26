using UnityEngine;
using System.Collections;


//explosion script kills the explosion object after the animation has played out.


public class ExplosionScript : MonoBehaviour {


	public Animator anim;
	public float birthTime;
	public float life;


	// Use this for initialization
	void Start () {
	
		life = (10.0f / 24.0f);

		anim.Play ("Explosion");
		birthTime = Time.time;

	}





	// Update is called once per frame
	void Update () {
	
		if (Time.time > birthTime + life) {

			Destroy(this.gameObject);

		}


	}
}
