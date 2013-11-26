using UnityEngine;
using System.Collections;
using System;

public class NetworkManager : MonoBehaviour {
	private const string typeName = "AntiConsole";
	static public string gameName;
	static public NetworkPlayer[] playerList = new NetworkPlayer[0];
	static public bool[] readyList = new bool[0];
	public GameObject playerObject;
	public static NetworkManager nm;
	private WWW wwwData = null;
	private bool is_thisGameAgain = true;
	
	void Awake(){
		Screen.fullScreen = true;	
	}
	
    private void StartServer(){
		Application.runInBackground = true;
        Network.InitializeServer(32, 25000, !Network.HavePublicAddress());
		Application.ExternalCall("GetGameName");
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
		Application.ExternalCall("SetGameName", gameName);
	}
	
    void OnServerInitialized(){
        Debug.Log("Server Initializied");
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
		networkView.RPC ("SetControls", player, 0,3);
	}
	
	public void StartRound(){
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
				GameObject newPlayerObject = (GameObject) GameObject.Instantiate(playerObject, new Vector3(-10,0,0), Quaternion.identity);
				newPlayerObject.transform.RotateAround(Vector3.zero, Vector3.forward, 360 - 360 / readyPlayers * thisReadyPlayer);
				newPlayerObject.transform.Rotate(new Vector3(0, 0, - 360 + 360 / readyPlayers * thisReadyPlayer));
				tempPlayerObjects[i] = newPlayerObject.transform;
				networkView.RPC ("PlayerObjectCreated", playerList[i]);
				thisReadyPlayer++;
				readyList[i] = false;
			}
			else{
				tempPlayerObjects[i] = null;
			}
		}
		PlayerControls.playerObjects = tempPlayerObjects;
	}
	
	[RPC] void PlayerReady(int player){
		if(!PlayerControls.is_gameOn){
			Debug.Log ("player" +player.ToString () + " ready");
			readyList[player] = true;
			int readyCount = 0;
			for(int i = 0; i < readyList.Length; i++){
				readyCount++;
			}
			//change to other number when ready to play
			if(readyCount>0 && is_thisGameAgain){
				MenuManager.lastTickTime = Time.time + 1;
				MenuManager.is_countdown = true;
			}
		}
	}
	
	[RPC] public void EndRound(){
		PlayerControls.is_gameOn = false;
		is_thisGameAgain = false;
		networkView.RPC ("EndOfRound", RPCMode.Others, PlayerControls.controllingPlayer);
		PlayerControls.controllingPlayer++;
		if(PlayerControls.controllingPlayer >= playerList.Length){
			PlayerControls.controllingPlayer = 0;
		}
	}
	
	[RPC] public void ReplayThisGame(){
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
}