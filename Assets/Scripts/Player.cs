using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour, IJoviosControllerListener {
	
	private Jovios jovios;
	private JoviosUserID jUID;
	private bool is_jumping = false;
	private float jumpStart;
	public float jumpSpeed = 37.5F;
	
	void Start () {
	}
	
	void FixedUpdate () {
		rigidbody.AddForce( 1 * new Vector3(jovios.GetPlayer(jUID).GetInput("left").GetDirection().x, 0, 0),ForceMode.VelocityChange);
		if(is_jumping){
			rigidbody.AddForce(new Vector3(0,jumpSpeed,0), ForceMode.VelocityChange);
			is_jumping = false;
		}
	}
	
	bool IJoviosControllerListener.ButtonEventReceived(JoviosButtonEvent e){
		switch(e.GetResponse()){
		case "JumpA":
			switch(e.GetAction()){
			case "press":
				is_jumping = true;
				jumpStart = Time.time;
				break;
			case "release":
				is_jumping = false;
				break;
			}
			break;
		default:
			break;
		}
		return false;
	}
	
	public void PlayerSetup(JoviosPlayer p){
		jUID = p.GetUserID();
		jovios = MenuManager.jovios;
		jovios.AddControllerListener(this, jUID);
	}
}
