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
	}
	
	void GetInput(){
	}
	
	[RPC] void SentJoystick(int player, float vertical, float horizontal, string side){
		if(is_gameOn){
			if(side == "left"){
				playerObjects[player].GetComponent<Sumo>().movement = new Vector2(horizontal, vertical);
			}
			if(side == "right"){
				playerObjects[player].GetComponent<Sumo>().facing = new Vector2(horizontal, vertical);
			}
		}
	}
	
	[RPC] void SentDPad(int player, float vertical, float horizontal, string side){
	}
	
	[RPC] void SentDiagonalDPad(int player, float vertical, float horizontal, string side){
	}
	
	[RPC] void SentButton1(int player, string buttonPress, string side){
	}
	
	[RPC] void SentButton2(int player, string buttonPress, string side){
	}
	
	[RPC] void SentButton3Up(int player, string buttonPress, string side){
	}
	
	[RPC] void SentButton3UpHold(int player, string buttonPress, float holdTime, string side){
	}
	
	[RPC] public void InstantiatePlayerObject(int player, float primaryR, float primaryG, float primaryB, float secondaryR, float secondaryG, float secondaryB, string playerName){
		Color primary = new Color(primaryR, primaryG, primaryB, 1);
		Color secondary = new Color(secondaryR, secondaryG, secondaryB, 1);
		Debug.Log (playerObjects[0]);
		playerObjects[player].GetComponent<Sumo>().SetMyPlayer(player, primary, secondary, playerName);
	}
	
	
	[RPC] public void SetPlayerNumber(int player){}
	[RPC] public void SetControls(int lControls, int rControls){}
	[RPC] void PlayerObjectCreated(){}
	[RPC] void EndOfRound(int player){}
	[RPC] void NewGame(){}
	[RPC] void SentBasicButtonTap(int playerNumber, string button){
		switch(button){
		case "Join Game":
			if(MenuManager.is_countdown){
				SentBasicButtons("Leave Game", "Cancel Countdown", "Prepare to Play!", NetworkManager.playerList[playerNumber]);
			}
			else{
				SentBasicButtons("Leave Game", "Start Game", "Begin the Game?", NetworkManager.playerList[playerNumber]);
			}
			transform.GetComponent<NetworkManager>().PlayerReady(playerNumber);
			break;
			
		case "Leave Game":
			if(MenuManager.is_countdown){
				SentBasicButtons("Join Game", "Cancel Countdown", "Ready to Play?", NetworkManager.playerList[playerNumber]);
			}
			else{
				SentBasicButtons("Join Game", "Start Game", "Ready to Play?", NetworkManager.playerList[playerNumber]);
			}
			break;
			
		case "Cancel Countdown":
			for(int i = 0; i < NetworkManager.readyList.Length; i++){
				if(NetworkManager.readyList[i]){
					SentBasicButtons("Leave Game", "Start Game", "Begin the Game?", NetworkManager.playerList[i]);
				}
				else{
					SentBasicButtons("Join Game", "Start Game", "Ready to Play?", NetworkManager.playerList[i]);
				}
			}
			MenuManager.is_countdown = false;
			MenuManager.timer = MenuManager.countdownTime;
			break;
			
		case "Start Game":
			for(int i = 0; i < NetworkManager.readyList.Length; i++){
				if(NetworkManager.readyList[i]){
					SentBasicButtons("Leave Game", "Cancel Countdown", "Prepare to Play!", NetworkManager.playerList[i]);
				}
				else{
					SentBasicButtons("Join Game", "Cancel Countdown", "Ready to Play?", NetworkManager.playerList[i]);
				}
			}
			MenuManager.lastTickTime = Time.time;
			MenuManager.is_countdown = true;
			transform.GetComponent<NetworkManager>().StartRound();
			break;
			
		case "Play Again!":
			SentBasicButtons("Join Game", "Start Game", "Ready to Play?", NetworkManager.playerList[playerNumber]);
			break;
			
		case "Play a Different Game":
			SentBasicButtons("Sumo", "Dragons", "What game would you like to play?", NetworkManager.playerList[playerNumber]);
			break;
			
		case "Sumo":
			transform.GetComponent<NetworkManager>().NewUrl("http://beta.catduo.com/sumo/game.unity3d");
			break;
			
		case "Dragons":
			transform.GetComponent<NetworkManager>().NewUrl("http://beta.catduo.com/dragon/game.unity3d");
			break;
			
		default:
			break;
		}
	}
	public void SentBasicButtons (string question, NetworkPlayer player){
		networkView.RPC ("SetBasicButtons", player, "", "", "", "", "", "", "", "", question);
	}
	public void SentBasicButtons (string button1, string question, NetworkPlayer player){
		networkView.RPC ("SetBasicButtons", player, button1, "", "", "", "", "", "", "", question);
	}
	public void SentBasicButtons (string button1, string button2, string question, NetworkPlayer player){
		networkView.RPC ("SetBasicButtons", player, button1, button2, "", "", "", "", "", "", question);
	}
	public void SentBasicButtons (string button1, string button2, string button3, string question, NetworkPlayer player){
		networkView.RPC ("SetBasicButtons", player, button1, button2, button3, "", "", "", "", "", question);
	}
	public void SentBasicButtons (string button1, string button2, string button3, string button4, string question, NetworkPlayer player){
		networkView.RPC ("SetBasicButtons", player, button1, button2, button3, button4, "", "", "", "", question);
	}
	public void SentBasicButtons (string button1, string button2, string button3, string button4, string button5, string question, NetworkPlayer player){
		networkView.RPC ("SetBasicButtons", player, button1, button2, button3, button4, button5, "", "", "", question);
	}
	public void SentBasicButtons (string button1, string button2, string button3, string button4, string button5, string button6, string question, NetworkPlayer player){
		networkView.RPC ("SetBasicButtons", player, button1, button2, button3, button4, button5, button6, "", "", question);
	}
	public void SentBasicButtons (string button1, string button2, string button3, string button4, string button5, string button6, string button7, string question, NetworkPlayer player){
		networkView.RPC ("SetBasicButtons", player, button1, button2, button3, button4, button5, button6, button7, "", question);
	}
	public void SentBasicButtons (string button1, string button2, string button3, string button4, string button5, string button6, string button7, string button8, string question, NetworkPlayer player){
		networkView.RPC ("SetBasicButtons", player, button1, button2, button3, button4, button5, button6, button7, button8, question);
	}
	[RPC] void SetBasicButtons (string button1, string button2, string button3, string button4, string button5, string button6, string button7, string button8, string question){}
}