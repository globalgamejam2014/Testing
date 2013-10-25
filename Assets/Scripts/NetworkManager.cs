using UnityEngine;
using System.Collections;
using System;

public class NetworkManager : MonoBehaviour {
	private const string typeName = "AntiConsole";
	static public string gameName = "Enter Game ID";
	
    private void StartServer(){
        Network.InitializeServer(32, 25000, !Network.HavePublicAddress());
            gameName = Guid.NewGuid().ToString().Split('-')[1];
            Debug.Log(gameName);
        MasterServer.RegisterHost(typeName, gameName);
            
            WWWForm form = new WWWForm();
            form.AddField("action","create");
            form.AddField ("name",gameName);
            WWW post_req = new WWW("http://localhost/foo.php",form);
    }   
	
    void OnServerInitialized(){
        Debug.Log("Server Initializied");
    }
	
	void OnPlayerDisconnected(NetworkPlayer player){
       Network.RemoveRPCs(player);
       Network.DestroyPlayerObjects(player);
    }
	
	void OnPlayerConnected(NetworkPlayer player){
		networkView.RPC ("SetControls", player, 0,0);
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