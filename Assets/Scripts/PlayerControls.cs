using UnityEngine;
using System.Collections;

public enum TouchStyle{
	Joystick,
	DPad,
	DiagonalDPad,
	Button1,
	Button2,
	Button3Up,
	Button3UpHold
}

public class PlayerControls : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
		SetControls(2,3);
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	void GetInput(){
	}
	
	[RPC] void SentJoystick(NetworkPlayer player, float vertical, float horizontal){
		Debug.Log (vertical.ToString() + ", " + horizontal.ToString());
	}
	
	[RPC] void SentDPad(NetworkPlayer player, float vertical, float horizontal){
	}
	
	[RPC] void SentDiagonalDPad(NetworkPlayer player, float vertical, float horizontal){
	}
	
	[RPC] void SentButton1(NetworkPlayer player, string buttonPress){
	}
	
	[RPC] void SentButton2(NetworkPlayer player, string buttonPress){
	}
	
	[RPC] void SentButton3Up(NetworkPlayer player, string buttonPress){
	}
	
	[RPC] void SentButton3UpHold(NetworkPlayer player, string buttonPress, float holdTime){
	}
	
	[RPC] public void InstantiatePlayerObject(NetworkPlayer player){
		networkView.RPC ("PlayerObjectCreated", player);
	}
	
	
	[RPC] public void SetControls(int lControls, int rControls){}
	[RPC] void PlayerObjectCreated(){}
}