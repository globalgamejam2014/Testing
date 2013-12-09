using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;

public class NetworkManager : MonoBehaviour {
	
	
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
	public static NetworkManager nm;
	private WWW wwwData = null;
	private bool is_thisGameAgain = true;
	
	void Update () {
		if(gameName != null && udp != null){
			SendMessage(gameName);
		}
	}
	
	/*void SendMessage(string message){
		Debug.Log("sending");
		byte[] messageBytes = System.Text.Encoding.ASCII.GetBytes(message);
		udp.Send(messageBytes, messageBytes.Length, groupEP);
		Debug.Log ("sent");
	}	*/
	
    private void StartServer(){
		Application.runInBackground = true;
        Network.InitializeServer(32, 25002, !Network.HavePublicAddress());
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
		//udp = new UdpClient();
		//udp.EnableBroadcast = true;
		//groupEP = new IPEndPoint(IPAddress.Parse("255.255.255.255"), 15000);
		//IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
    }
	
	void OnPlayerDisconnected(NetworkPlayer player){
    	Network.RemoveRPCs(player);
    	Network.DestroyPlayerObjects(player);
    }
	
	void OnPlayerConnected(NetworkPlayer player){
		NetworkPlayer[] tempPlayerList = new NetworkPlayer[playerList.Length + 1];
		bool[] tempReadyList = new bool[readyList.Length + 1];
		for(int i = 0; i < playerList.Length; i++){ 
			tempPlayerList[i] = playerList[i];
			tempReadyList[i] = readyList[i];
		}
		tempReadyList[readyList.Length] = false;
		tempPlayerList[playerList.Length] = player;
		playerList = tempPlayerList;
		readyList = tempReadyList;
		networkView.RPC ("SetPlayerNumber", player, tempPlayerList.Length - 1);
		if(PlayerControls.is_gameOn){
			transform.GetComponent<PlayerControls>().SentBasicButtons("Waiting for round to complete.", player);
		}
		else if(MenuManager.is_countdown){
			transform.GetComponent<PlayerControls>().SentBasicButtons("Join Game", "Cancel Countdown", "Ready to Play?", player);
		}
		else{
			transform.GetComponent<PlayerControls>().SentBasicButtons("Join Game", "Start Game", "Ready to Play?", player);
		}
	}
	
	public void StartRound(){
		Transform po = GameObject.Find ("PlayerObjects").transform;
		for(int i = 0; i < po.childCount; i++){
			Destroy(po.GetChild(i).gameObject);
		}
		int readyPlayers = 0;
		int thisReadyPlayer = 0;
		Transform[] tempPlayerObjects = new Transform [playerList.Length];
		for(int i = 0; i < playerList.Length; i++){
			if(readyList[i]){
				readyPlayers++;
			}
		}
		for(int i = 0; i < playerList.Length; i++){
			if(readyList[i]){
				GameObject newPlayerObject = (GameObject) GameObject.Instantiate(playerObject, new Vector3(0,-4,0), Quaternion.identity);
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
	
	public void PlayerReady(int player){
		if(!PlayerControls.is_gameOn){
			readyList[player] = true;
			readyCount = 0;
			for(int i = 0; i < readyList.Length; i++){
				readyCount++;
			}
			//change to other number when ready to play
			StartRound();
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
		for(int i = 0; i < playerList.Length; i++){
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
	
	// Use this for initialization
	void Start () {
		//MasterServer.ipAddress = "192.168.1.57";
		//MasterServer.port = 23466;
		//Network.natFacilitatorIP = "192.168.1.57";
		//Network.natFacilitatorPort = 23466;
		Application.targetFrameRate = 24;
		StartServer();
	}
	
	void OnApplicationQuit(){
		Network.Disconnect();
	}
}