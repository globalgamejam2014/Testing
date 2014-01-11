using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;

public enum JoviosControls{
	Joystick,
	DPad,
	DiagonalDPad,
	Button1,
	Button2,
	CardinalSwipes,
	AllInput
}

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
	public Vector2 direction;
	public string button;
	public PlayerInput(){
		direction = Vector2.zero;
		button = "";
	}
}

public class Jovios : MonoBehaviour {
	public static string version = "0.0.0";
	public static Player[] players;
	private const string typeName = "Jovios";
	static public string gameName;
	public GameObject playerObject;
	public GameObject statusObject;
	public static WWW wwwData = null;
	public static NetworkView thisNetworkView;
	public static GameObject thisGameObject;
	[RPC] public void CheckVersion(string controllerVersion){
		if(controllerVersion == version){
			Debug.Log ("versions Match");
		}
		else if(int.Parse(version.Split('.')[0])<=int.Parse(controllerVersion.Split('.')[0]) && int.Parse(version.Split('.')[1])<=int.Parse(controllerVersion.Split('.')[1]) && int.Parse(version.Split('.')[2])<=int.Parse(controllerVersion.Split('.')[2])){
			Debug.Log ("controller more advanced version");
		}
		else{
			Debug.Log ("controller out of date");
		}
	}
	
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
		thisNetworkView.RPC ("SentPlayerNumber", player, players.Length - 1);
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
				newPlayers[nextPlayerNum] = players[i];
				thisNetworkView.RPC ("SentPlayerNumber", players[i].networkPlayer, nextPlayerNum);
				nextPlayerNum++;
			}
		}
		players = newPlayers;
		for(int i = 0; i < players.Length; i++){
			players[i].statusObject.SendMessage("Reset", i, SendMessageOptions.DontRequireReceiver);
		}
    	Network.RemoveRPCs(player);
    	Network.DestroyPlayerObjects(player);
    }
	
	
	
	
	[RPC] void GetJoystick(int player, float vertical, float horizontal, string side){
		switch(side){
		case "left":
			players[player].left.direction.x = horizontal;
			players[player].left.direction.y = vertical;
			break;
			
		case "right":
			players[player].right.direction.x = horizontal;
			players[player].right.direction.y = vertical;
			break;
		}
	}
	[RPC] void GetDPad(int player, float vertical, float horizontal, string side){
		switch(side){
		case "left":
			players[player].left.direction.x = horizontal;
			players[player].left.direction.y = vertical;
			break;
			
		case "right":
			players[player].right.direction.x = horizontal;
			players[player].right.direction.y = vertical;
			break;
		}
	}
	[RPC] void GetDiagonalDPad(int player, float vertical, float horizontal, string side){
		switch(side){
		case "left":
			players[player].left.direction.x = horizontal;
			players[player].left.direction.y = vertical;
			break;
			
		case "right":
			players[player].right.direction.x = horizontal;
			players[player].right.direction.y = vertical;
			break;
		}
	}
	
	
	
	[RPC] void GetButton1(int playerNumber, string buttonPress, string side){
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
	[RPC] void GetButton2(int playerNumber, string buttonPress, string side){
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
	[RPC] void GetButton3Up(int playerNumber, string buttonPress, string side){
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
	[RPC] void GetButton3UpHold(int playerNumber, string buttonPress, float holdTime, string side){
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
	
	[RPC] void GetBasicButtonTap(int playerNumber, string button){
		if(players[playerNumber].statusObject != null){
			players[playerNumber].statusObject.SendMessage("OnButton", button, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	[RPC] void GetTextResponse(int playerNumber, string button){
		if(players[playerNumber].statusObject != null){
			players[playerNumber].statusObject.SendMessage("OnText", button, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	[RPC] void GetSingleResponse(int playerNumber, string button){
		if(players[playerNumber].statusObject != null){
			players[playerNumber].statusObject.SendMessage("OnButton", button, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	[RPC] void GetMultiResponse(int playerNumber, string button){
		if(players[playerNumber].statusObject != null){
			players[playerNumber].statusObject.SendMessage("OnMulti", button, SendMessageOptions.DontRequireReceiver);
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
	
	public static void SetControls(NetworkPlayer player, int lControls, string lControlsDescription, int rControls, string rControlsDescription){
		thisNetworkView.RPC ("SentControls", player, lControls, lControlsDescription, rControls, rControlsDescription);
	}
	
	[RPC] public void SentPlayerNumber(int player){}
	[RPC] public void SentControls(int lControls, string lControlsDescription, int rControls, string rControlsDescription){}
	[RPC] public void PlayerObjectCreated(){}
	[RPC] public void EndOfRound(int player){}
	[RPC] public void NewGame(){}
		
	public static void SetBasicButtons (string question, NetworkPlayer player){
		thisNetworkView.RPC ("SentButtons", player, "basic", question, "", "", "", "", "", "", "", "", "");
	}
	public static void SetBasicButtons (string button1, string question, NetworkPlayer player){
		thisNetworkView.RPC ("SentButtons", player, "basic", question,  "", button1, "", "", "", "", "", "", "");
	}
	public static void SetBasicButtons (string button1, string button2, string question, NetworkPlayer player){
		thisNetworkView.RPC ("SentButtons", player, "basic", question,  "", button1, button2, "", "", "", "", "", "");
	}
	public static void SetBasicButtons (string button1, string button2, string button3, string question, NetworkPlayer player){
		thisNetworkView.RPC ("SentButtons", player, "basic", question,  "", button1, button2, button3, "", "", "", "", "");
	}
	public static void SetBasicButtons (string button1, string button2, string button3, string button4, string question, NetworkPlayer player){
		thisNetworkView.RPC ("SentButtons", player, "basic", question,  "", button1, button2, button3, button4, "", "", "", "");
	}
	public static void SetBasicButtons (string button1, string button2, string button3, string button4, string button5, string question, NetworkPlayer player){
		thisNetworkView.RPC ("SentButtons", player, "basic", question,  "", button1, button2, button3, button4, button5, "", "", "");
	}
	public static void SetBasicButtons (string button1, string button2, string button3, string button4, string button5, string button6, string question, NetworkPlayer player){
		thisNetworkView.RPC ("SentButtons", player, "basic", question,  "", button1, button2, button3, button4, button5, button6, "", "");
	}
	public static void SetBasicButtons (string button1, string button2, string button3, string button4, string button5, string button6, string button7, string question, NetworkPlayer player){
		thisNetworkView.RPC ("SentButtons", player, "basic", question,  "", button1, button2, button3, button4, button5, button6, button7, "");
	}
	public static void SetBasicButtons (string button1, string button2, string button3, string button4, string button5, string button6, string button7, string button8, string question, NetworkPlayer player){
		thisNetworkView.RPC ("SentButtons", player, "basic", question,  "", button1, button2, button3, button4, button5, button6, button7, button8);
	}
	[RPC] void SentButtons (string type, string question, string actionWord, string button1, string button2, string button3, string button4, string button5, string button6, string button7, string button8){}

	
}