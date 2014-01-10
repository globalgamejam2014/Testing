using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;

public class Player {
	public GameObject statusObject;
	public GameObject playerObject;
	public int playerNumber;
	public string playerName;
	public NetworkPlayer networkPlayer;
	public PlayerInput left;
	public PlayerInput right;
	public string buttonInput;
	public Color primary;
	public Color secondary;
	public Player(){
		playerName = "";
		playerNumber = -1;
		right = new PlayerInput();
		left = new PlayerInput();
		primary = Color.clear;
		secondary = Color.clear;
	}
	public Player(string pName, int pNumber, GameObject pStatus, GameObject pObject, NetworkPlayer pNetwork){
		statusObject = pStatus;
		playerObject = pObject;
		playerName = pName;
		playerNumber = pNumber;
		networkPlayer = pNetwork;
		right = new PlayerInput();
		left = new PlayerInput();
		primary = Color.clear;
		secondary = Color.clear;
	}
}

public class PlayerInput{
	public float vertical;
	public float horizontal;
	public string button;
	public PlayerInput(){
		vertical = 0;
		horizontal = 0;
		button = "";
	}
}

public class Jovios : MonoBehaviour {
	public static Player[] players;
	private const string typeName = "AntiConsole";
	static public string gameName;
	public GameObject playerObject;
	public GameObject statusObject;
	public static WWW wwwData = null;
	public static NetworkView thisNetworkView;
	public static GameObject thisGameObject;
	
	void Start(){
		players = new Player[0];
		thisNetworkView = networkView;
		thisGameObject = gameObject;
		StartServer();
	}
	
    private void StartServer(){
		Application.runInBackground = true;
        Network.InitializeServer(32, 25000, !Network.HavePublicAddress());
		if(Application.isWebPlayer){
			Application.ExternalCall("GetGameName");
		}
		else{
			GetGameName("aaaa");
		}
    }    
	
    void OnServerInitialized(){
        Debug.Log("Server Initializied");
		//TCPGetMessage("serveron", Network.player.externalIP);
		//udp = new UdpClient();
		//udp.EnableBroadcast = true;
		//groupEP = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 15000);
		//IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
	}
	
	void OnApplicationQuit(){
		Network.Disconnect();
	}
	
	void GetGameName(string newGameName){
		if(newGameName.Length == 4){
			gameName = newGameName;
		}
		else{
            gameName = Guid.NewGuid().ToString().Split('-')[1];
		}
        MasterServer.RegisterHost(typeName, gameName);
        WWWForm form = new WWWForm();
        form.AddField("action","create");
        form.AddField ("name",gameName);
        WWW post_req = new WWW("http://localhost/foo.php",form);
		if(Application.isWebPlayer){
			Application.ExternalCall("SetGameName", gameName);
		}
        Debug.Log(gameName);
	}    
	
	public static IEnumerator WaitForDownload(){    
        yield return wwwData;
        wwwData.LoadUnityWeb();  
    }
	[RPC] public static void NewUrl(string url){
		MenuManager.is_loadingNewGame = true;
		thisNetworkView.RPC("NewGame", RPCMode.Others);
        wwwData = new WWW(url);
        thisGameObject.GetComponent<Jovios>().StartCoroutine("WaitForDownload");
	}
	
	void OnPlayerConnected(NetworkPlayer player){
		Debug.Log ("connect");
		Player[] newPlayers = new Player[players.Length + 1];
		for(int i = 0; i < players.Length; i++){ 
			newPlayers[i] = players[i];
		}
		newPlayers[players.Length] = new Player();
		players = newPlayers;
		players[players.Length - 1].playerNumber = players.Length - 1;
		players[players.Length - 1].networkPlayer = player;
		players[players.Length - 1].playerName = "Player";
		players[players.Length - 1].primary = Color.clear;
		players[players.Length - 1].secondary = Color.clear;
		players[players.Length - 1].statusObject = (GameObject) GameObject.Instantiate(statusObject, Vector3.zero, Quaternion.identity);
		players[players.Length - 1].statusObject.SendMessage("SetMyPlayer",players[players.Length - 1], SendMessageOptions.DontRequireReceiver);
		thisNetworkView.RPC ("SetPlayerNumber", player, players.Length - 1);
		thisNetworkView.RPC ("PlayerObjectCreated", player);
	}
	
	void OnPlayerDisconnected(NetworkPlayer player){
		Player[] newPlayers = new Player[players.Length - 1];
		int nextPlayerNum = 0;
		for(int i = 0; i < players.Length; i++){
			if(players[i].networkPlayer == player){
				Destroy (players[i].playerObject);
				Destroy (players[i].statusObject);
			}
			else{
				newPlayers[i] = players[nextPlayerNum];
				nextPlayerNum++;
			}
		}
		players = newPlayers;
    	Network.RemoveRPCs(player);
    	Network.DestroyPlayerObjects(player);
    }
	
	
	
	
	[RPC] void SentJoystick(int player, float vertical, float horizontal, string side){
		switch(side){
		case "left":
			players[player].left.horizontal = horizontal;
			players[player].left.vertical = vertical;
			break;
			
		case "right":
			players[player].right.horizontal = horizontal;
			players[player].right.vertical = vertical;
			break;
		}
	}
	[RPC] void SentDPad(int player, float vertical, float horizontal, string side){
		switch(side){
		case "left":
			players[player].left.horizontal = horizontal;
			players[player].left.vertical = vertical;
			break;
			
		case "right":
			players[player].right.horizontal = horizontal;
			players[player].right.vertical = vertical;
			break;
		}
	}
	[RPC] void SentDiagonalDPad(int player, float vertical, float horizontal, string side){
		switch(side){
		case "left":
			players[player].left.horizontal = horizontal;
			players[player].left.vertical = vertical;
			break;
			
		case "right":
			players[player].right.horizontal = horizontal;
			players[player].right.vertical = vertical;
			break;
		}
	}
	
	
	
	[RPC] void SentButton1(int playerNumber, string buttonPress, string side){
		switch(side){
		case "left":
			if(players[playerNumber].statusObject != null){
				players[playerNumber].statusObject.SendMessage("OnButton", "Jovios--left--press--" + buttonPress, SendMessageOptions.DontRequireReceiver);
			}
			break;
			
		case "right":
			if(players[playerNumber].statusObject != null){
				players[playerNumber].statusObject.SendMessage("OnButton", "Jovios--right--press--" + buttonPress, SendMessageOptions.DontRequireReceiver);
			}
			break;
		}
	}
	[RPC] void SentButton2(int playerNumber, string buttonPress, string side){
		switch(side){
		case "left":
			if(players[playerNumber].statusObject != null){
				players[playerNumber].statusObject.SendMessage("OnButton", "Jovios--left--press--" + buttonPress, SendMessageOptions.DontRequireReceiver);
			}
			break;
			
		case "right":
			if(players[playerNumber].statusObject != null){
				players[playerNumber].statusObject.SendMessage("OnButton", "Jovios--right--press--" + buttonPress, SendMessageOptions.DontRequireReceiver);
			}
			break;
		}
	}
	[RPC] void SentButton3Up(int playerNumber, string buttonPress, string side){
		switch(side){
		case "left":
			if(players[playerNumber].statusObject != null){
				players[playerNumber].statusObject.SendMessage("OnButton", "Jovios--left--press--" + buttonPress, SendMessageOptions.DontRequireReceiver);
			}
			break;
			
		case "right":
			if(players[playerNumber].statusObject != null){
				players[playerNumber].statusObject.SendMessage("OnButton", "Jovios--right--press--" + buttonPress, SendMessageOptions.DontRequireReceiver);
			}
			break;
		}
	}
	[RPC] void SentButton3UpHold(int playerNumber, string buttonPress, float holdTime, string side){
		switch(side){
		case "left":
			if(players[playerNumber].statusObject != null){
				players[playerNumber].statusObject.SendMessage("OnButton", "Jovios--left--press--" + buttonPress, SendMessageOptions.DontRequireReceiver);
			}
			break;
			
		case "right":
			if(players[playerNumber].statusObject != null){
				players[playerNumber].statusObject.SendMessage("OnButton", "Jovios--right--press--" + buttonPress, SendMessageOptions.DontRequireReceiver);
			}
			break;
		}
	}
	
	[RPC] public void InstantiatePlayerObject(int playerNumber, float primaryR, float primaryG, float primaryB, float secondaryR, float secondaryG, float secondaryB, string playerName){
		Debug.Log (playerName);
		players[playerNumber].primary = new Color(primaryR, primaryG, primaryB, 1);
		players[playerNumber].secondary = new Color(secondaryR, secondaryG, secondaryB, 1);
		players[playerNumber].playerName = playerName;
		players[playerNumber].playerNumber = playerNumber;
		players[playerNumber].statusObject.SendMessage("SetMyPlayer",players[playerNumber], SendMessageOptions.DontRequireReceiver);
	}
	
	public static void SentControls(NetworkPlayer player, int lControls, string lControlsDescription, int rControls, string rControlsDescription){
		thisNetworkView.RPC ("SetControls", player, lControls, lControlsDescription, rControls, rControlsDescription);
	}
	
	[RPC] public void SetPlayerNumber(int player){}
	[RPC] public void SetControls(int lControls, string lControlsDescription, int rControls, string rControlsDescription){}
	[RPC] public void PlayerObjectCreated(){}
	[RPC] public void EndOfRound(int player){}
	[RPC] public void NewGame(){}
	[RPC] void SentBasicButtonTap(int playerNumber, string button){
		if(players[playerNumber].statusObject != null){
			players[playerNumber].statusObject.SendMessage("OnButton", button, SendMessageOptions.DontRequireReceiver);
		}
	}
		
	public static void SentBasicButtons (string question, NetworkPlayer player){
		thisNetworkView.RPC ("SetButtons", player, "basic", question, "", "", "", "", "", "", "", "", "");
	}
	public static void SentBasicButtons (string button1, string question, NetworkPlayer player){
		thisNetworkView.RPC ("SetButtons", player, "basic", question,  "", button1, "", "", "", "", "", "", "");
	}
	public static void SentBasicButtons (string button1, string button2, string question, NetworkPlayer player){
		thisNetworkView.RPC ("SetButtons", player, "basic", question,  "", button1, button2, "", "", "", "", "", "");
	}
	public static void SentBasicButtons (string button1, string button2, string button3, string question, NetworkPlayer player){
		thisNetworkView.RPC ("SetButtons", player, "basic", question,  "", button1, button2, button3, "", "", "", "", "");
	}
	public static void SentBasicButtons (string button1, string button2, string button3, string button4, string question, NetworkPlayer player){
		thisNetworkView.RPC ("SetButtons", player, "basic", question,  "", button1, button2, button3, button4, "", "", "", "");
	}
	public static void SentBasicButtons (string button1, string button2, string button3, string button4, string button5, string question, NetworkPlayer player){
		thisNetworkView.RPC ("SetButtons", player, "basic", question,  "", button1, button2, button3, button4, button5, "", "", "");
	}
	public static void SentBasicButtons (string button1, string button2, string button3, string button4, string button5, string button6, string question, NetworkPlayer player){
		thisNetworkView.RPC ("SetButtons", player, "basic", question,  "", button1, button2, button3, button4, button5, button6, "", "");
	}
	public static void SentBasicButtons (string button1, string button2, string button3, string button4, string button5, string button6, string button7, string question, NetworkPlayer player){
		thisNetworkView.RPC ("SetButtons", player, "basic", question,  "", button1, button2, button3, button4, button5, button6, button7, "");
	}
	public static void SentBasicButtons (string button1, string button2, string button3, string button4, string button5, string button6, string button7, string button8, string question, NetworkPlayer player){
		thisNetworkView.RPC ("SetButtons", player, "basic", question,  "", button1, button2, button3, button4, button5, button6, button7, button8);
	}
	[RPC] void SetButtons (string type, string question, string actionWord, string button1, string button2, string button3, string button4, string button5, string button6, string button7, string button8){}

	
}