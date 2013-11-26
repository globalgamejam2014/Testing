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
	
	static public Transform[] playerObjects = new Transform[0];
	static public bool is_gameOn = false;
	static public int controllingPlayer = 0;
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if(is_gameOn){
			for(int i = 0; i < playerObjects.Length; i++){
				if(playerObjects[i].position.x < -16){
					transform.GetComponent<NetworkManager>().EndRound();
				}
			}
		}
	}
	
	void GetInput(){
	}
	
	[RPC] void SentJoystick(int player, float vertical, float horizontal){
		if(is_gameOn){
			playerObjects[player].GetComponent<Microbe>().movement = new Vector2(horizontal, vertical);
			Debug.Log (vertical.ToString() + ", " + horizontal.ToString());
		}
	}
	
	[RPC] void SentDPad(int player, float vertical, float horizontal){
	}
	
	[RPC] void SentDiagonalDPad(int player, float vertical, float horizontal){
	}
	
	[RPC] void SentButton1(int player, string buttonPress){
		
	}
	
	[RPC] void SentButton2(int player, string buttonPress){
	}
	
	[RPC] void SentButton3Up(int player, string buttonPress){
	}
	
	[RPC] void SentButton3UpHold(int player, string buttonPress, float holdTime){
	}
	
	[RPC] public void InstantiatePlayerObject(int player, float primaryR, float primaryG, float primaryB, float secondaryR, float secondaryG, float secondaryB, string playerName){
		Color primary = new Color(primaryR, primaryG, primaryB, 0.6F);
		Color secondary = new Color(secondaryR, secondaryG, secondaryB, 1);
		playerObjects[player].GetComponent<Microbe>().SetMyPlayer(player, primary, secondary, playerName);
	}
	
	
	[RPC] public void SetPlayerNumber(int player){}
	[RPC] public void SetControls(int lControls, int rControls){}
	[RPC] void PlayerObjectCreated(){}
	[RPC] void EndOfRound(int player){}
	[RPC] void NewGame(){}
}