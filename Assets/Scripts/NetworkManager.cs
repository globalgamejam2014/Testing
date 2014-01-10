using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;

public class NetworkManager : MonoBehaviour {
	/*
	
	public UdpClient udp;
	public IPEndPoint endIP;
	float timer;
	IPEndPoint groupEP;
	
	private const string typeName = "AntiConsole";
	static public string gameName;
	static public NetworkPlayer[] playerList = new NetworkPlayer[0];
	static public bool[] readyList = new bool[0];
	static public int readyCount = 0;
	public GameObject playerObject;
	public GameObject statusObject;
	public static NetworkManager nm;
	private WWW wwwData = null;
	private bool is_thisGameAgain = true;
	public static int playerCount = 0;
	
	void Update () {
		if(gameName != null && udp != null){
			SendMessage(gameName);
		}
	}
	
	void SendMessage(string message){
		Debug.Log("sending");
		byte[] messageBytes = System.Text.Encoding.ASCII.GetBytes(message);
		udp.Send(messageBytes, messageBytes.Length, groupEP);
		Debug.Log ("sent");
	}
	
	void TCPGetMessage(string message, string ip){
		Security.PrefetchSocketPolicy(ip, 843);
		Int32 port = 13000;
	    TcpClient client = new TcpClient(ip, port);
	    
	    // Translate the passed message into ASCII and store it as a Byte array.
	    Byte[] data = System.Text.Encoding.ASCII.GetBytes(message);         
	
	    // Get a client stream for reading and writing.
	   //  Stream stream = client.GetStream();
	    
	    NetworkStream stream = client.GetStream();
	
	    // Send the message to the connected TcpServer. 
	    stream.Write(data, 0, data.Length);
	
	    Debug.Log(message);         
	
	    // Receive the TcpServer.response.
	    
	    // Buffer to store the response bytes.
	    data = new Byte[256];
	
	    // String to store the response ASCII representation.
	    String responseData = message + "2";
	
	    // Read the first batch of the TcpServer response bytes.
	    Int32 bytes = stream.Read(data, 0, data.Length);
	    responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
	    gameName += responseData.ToString() + "end";     
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
	
	private IEnumerator WaitForDownload(){
        Debug.Log("Yielding");      
        yield return wwwData;
        Debug.Log("Yielded");
        wwwData.LoadUnityWeb();  
    }    
	public void StartDownload(string sURL){
        wwwData = new WWW(sURL);            
        Debug.Log("Starting download.");
        StartCoroutine("WaitForDownload");
    }

	
	void GetGameName(string newGameName){
		if(newGameName.Length == 4){
			gameName = newGameName;
		}
		else{
            gameName = Guid.NewGuid().ToString().Split('-')[1];
		}
        Debug.Log(gameName);
        MasterServer.RegisterHost(typeName, gameName);
        WWWForm form = new WWWForm();
        form.AddField("action","create");
        form.AddField ("name",gameName);
        WWW post_req = new WWW("http://localhost/foo.php",form);
		if(Application.isWebPlayer){
			Application.ExternalCall("SetGameName", gameName);
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
	
	void OnPlayerDisconnected(NetworkPlayer player){
		playerCount--;
		NetworkPlayer[] tempPlayerList = new NetworkPlayer[playerCount];
		Transform[] tempPlayerObjects = new Transform[playerCount];
		Transform[] tempStatusObjects = new Transform[playerCount];
		bool[] tempReadyList = new bool[playerCount];
		int nextPlayerNum = 0;
		for(int i = 0; i < playerCount + 1; i++){
			if(playerList[i] == player){
				Destroy(PlayerControls.statusObjects[i].gameObject);
				if(PlayerControls.playerObjects[i] != null){
					Destroy(PlayerControls.playerObjects[i].gameObject);
				}
			}
			else{
				Debug.Log (i);
				if(PlayerControls.playerObjects[i] != null){
					PlayerControls.playerObjects[i].GetComponent<Sumo>().playerNumber = nextPlayerNum;
				}
				PlayerControls.statusObjects[i].GetComponent<Status>().playerNumber = nextPlayerNum;
				PlayerControls.statusObjects[i].localPosition = new Vector3(-4.5F + nextPlayerNum, -1.75F, 0);
				tempPlayerList[nextPlayerNum] = playerList[i];
				tempPlayerObjects[nextPlayerNum] = PlayerControls.playerObjects[i];
				tempStatusObjects[nextPlayerNum] = PlayerControls.statusObjects[i];
				tempReadyList[nextPlayerNum] = readyList[i];
				networkView.RPC ("SetPlayerNumber", playerList[i], nextPlayerNum);
				nextPlayerNum++;
			}
		}
		readyList = tempReadyList;
		playerList = tempPlayerList;
		PlayerControls.playerObjects = tempPlayerObjects;
		PlayerControls.statusObjects = tempStatusObjects;
    	Network.RemoveRPCs(player);
    	Network.DestroyPlayerObjects(player);
    }
    
    void OnPlayerConnected(NetworkPlayer player){
		playerCount++;
		NetworkPlayer[] tempPlayerList = new NetworkPlayer[playerCount];
		for(int i = 0; i < playerCount - 1; i++){ 
			tempPlayerList[i] = playerList[i];
		}
		tempPlayerList[playerCount - 1] = player;
		playerList = tempPlayerList;
		GameObject newPlayerObject = (GameObject) GameObject.Instantiate(statusObject, Vector3.zero, Quaternion.identity);
		newPlayerObject.transform.parent = GameObject.Find ("PlayerStatus").transform;
		newPlayerObject.transform.localPosition = new Vector3(-4.5F + (playerCount -1), -1.75F, 0);
		newPlayerObject.transform.localRotation = Quaternion.identity;
		Transform[] tempStatusObjects = new Transform [playerCount];
		for(int i = 0; i < playerCount - 1; i++){
			tempStatusObjects[i] = PlayerControls.statusObjects[i];
		}
		tempStatusObjects[playerCount - 1] = newPlayerObject.transform;
		PlayerControls.statusObjects = tempStatusObjects;
		Transform[] tempPlayerObjects = new Transform [playerCount];
		for(int i = 0; i < playerCount - 1; i++){
			tempPlayerObjects[i] = PlayerControls.playerObjects[i];
		}
		PlayerControls.playerObjects = tempPlayerObjects;
		bool[] tempReadyList = new bool[playerCount];
		for(int i = 0; i < playerCount - 1; i++){
			tempReadyList[i] = readyList[i];
		}
		tempReadyList[playerCount - 1] = false;
		readyList = tempReadyList;
		Debug.Log(playerCount - 1);
		networkView.RPC ("SetPlayerNumber", player, playerCount - 1);
		networkView.RPC ("PlayerObjectCreated", player);
		if(PlayerControls.is_gameOn){
			transform.GetComponent<PlayerControls>().SentBasicButtons("Waiting for round to complete.", player);
		}
		else{
			transform.GetComponent<PlayerControls>().SentBasicButtons("Join Game", "Ready to Play?", player);
		}
	}
	
	public void StartRound(){
		StartRound(0);
	}
	
	public void StartRound(int playerNum){
		Transform po = GameObject.Find ("PlayerObjects").transform;
		int readyPlayers = 0;
		int thisReadyPlayer = 0;
		for(int i = 0; i < playerCount; i++){
			if(readyList[i]){
				readyPlayers++;
			}
		}
		Transform[] tempPlayerObjects = new Transform [playerCount];
		if(MenuManager.is_countdown){
			for(int i = 0; i < po.childCount; i++){
				Destroy(po.GetChild(i).gameObject);
			}
			for(int i = 0; i < playerCount; i++){
				if(readyList[i]){
					GameObject newPlayerObject = (GameObject) GameObject.Instantiate(playerObject, new Vector3(0,-4,0.5F), Quaternion.identity);
					newPlayerObject.transform.RotateAround(Vector3.zero, Vector3.forward, 360 - 360 / readyPlayers * thisReadyPlayer);
					newPlayerObject.transform.Rotate(new Vector3(0, 0, - 360 + 360 / readyPlayers * thisReadyPlayer));
					newPlayerObject.transform.parent = GameObject.Find ("PlayerObjects").transform;
					tempPlayerObjects[i] = newPlayerObject.transform;
					networkView.RPC ("PlayerObjectCreated", playerList[i]);
					thisReadyPlayer++;
				}
				else{
					tempPlayerObjects[i] = null;
				}
			}
			PlayerControls.playerObjects = tempPlayerObjects;
		}
		else{
			GameObject newPlayerObject = (GameObject) GameObject.Instantiate(playerObject, new Vector3(0,-4,0.5F), Quaternion.identity);
			newPlayerObject.transform.RotateAround(Vector3.zero, Vector3.forward, 360 - 360 / readyPlayers * thisReadyPlayer);
			newPlayerObject.transform.Rotate(new Vector3(0, 0, - 360 + 360 / readyPlayers * thisReadyPlayer));
			newPlayerObject.transform.parent = GameObject.Find ("PlayerObjects").transform;
			PlayerControls.playerObjects[playerNum] = newPlayerObject.transform;
			networkView.RPC ("PlayerObjectCreated", playerList[playerNum]);
			thisReadyPlayer++;
		}
	}
	
	public void PlayerReady(int playerNum){
		if(!PlayerControls.is_gameOn){
			readyList[playerNum] = true;
			readyCount = 0;
			for(int i = 0; i < playerCount; i++){
				readyCount++;
			}
			//change to other number when ready to play
			StartRound(playerNum);
		}
	}
	
	public void EndRound(){
		Transform po = GameObject.Find ("PlayerObjects").transform;
		for(int i = 0; i < po.childCount; i++){
			Destroy(po.GetChild(i).gameObject);
		}
		Transform mo = GameObject.Find ("Modifiers").transform;
		for(int i = 0; i < mo.childCount; i++){
			Destroy(mo.GetChild(i).gameObject);
		}
		PlayerControls.is_gameOn = false;
		is_thisGameAgain = false;
		Destroy(PlayerControls.chosenArena);
		Instantiate(transform.GetComponent<PlayerControls>().chooseArena, Vector3.zero, Quaternion.identity);
		for(int i = 0; i < playerCount; i++){
			PlayerControls.statusObjects[i].GetComponent<Status>().status.text = "X";
			PlayerControls.statusObjects[i].GetComponent<Status>().status.color = Color.red;
			transform.GetComponent<PlayerControls>().SentBasicButtons("Play Again!", "Play a Different Game", "Would you like to play this game again?", playerList[i]);
		}
	}
	
	public void ReplayThisGame(){
		is_thisGameAgain = true;
	}
	
	[RPC] public void NewUrl(string url){
		MenuManager.is_loadingNewGame = true;
		networkView.RPC("NewGame", RPCMode.Others);
		StartDownload(url);
	}
	
	void OnApplicationQuit(){
		Network.Disconnect();
	}
	*/
}