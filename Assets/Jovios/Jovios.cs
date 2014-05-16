﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using JoviosSimpleJSON;
using SimpleJson;
using System.Text;
using WebSocket4Net;
using SocketIOClient;
using pomeloUnityClient;
using System;

//TODO List
//take in array of userids to change player ordering
//versioning with the controller so that it always works with the games (think about how it would work, not actually doing it)
//look into unit testing on unity
//more comments and grouping
//IJovios for API help (list of all public functions)
//UI for 2 button and directional swiping
//Add non-relative directional inputs

//networking TODO
//move connection items to a different class
//take in array of userids to change player ordering
//versioning with the controller so that it always works with the games (think about how it would work, not actually doing it)
//look into unit testing on unity
//more comments and grouping
//IJovios for API help (list of all public functions)
//Add Button Release
//UI for 2 button and directional swiping
//Add non-relative directional inputs
//Dictionary reset on player disconnect
//list of players instead of array Network and Jovios


public class Jovios : MonoBehaviour {
	
	//this is the connection string for the player to type into the controller
	private string gameName{get; set;}
	
	//this will call the approriate jovios object creation code, such that it will work properly with Unity
	public static Jovios Create(){
		GameObject joviosGameObject = new GameObject();
		joviosGameObject.AddComponent<Jovios>();
		joviosGameObject.AddComponent<NetworkView>();
		joviosGameObject.name = "JoviosObject";
		joviosGameObject.networkView.stateSynchronization = NetworkStateSynchronization.Off;
		return joviosGameObject.GetComponent<Jovios>();
	}
	
	//Players are stored in a list and you can get the player calling GetPlayer(id) with id being either the PlayerNumber or the JoviosUserID
	private List<JoviosPlayer> players = new List<JoviosPlayer>();
	private Dictionary<int, int> deviceIDToPlayerNumber = new Dictionary<int, int>();
	public JoviosPlayer GetPlayer(int playerNumber){
		if(playerNumber < players.Count){
			return players[playerNumber];
		}
		else{
			return null;
		}
	}
	public JoviosPlayer GetPlayer(JoviosUserID jUID){
		if(deviceIDToPlayerNumber.ContainsKey(jUID.GetIDNumber())){
			return players[deviceIDToPlayerNumber[jUID.GetIDNumber()]];
		}
		else{
			return null;
		}
	}
	public int GetPlayerCount(){
		if(players != null){
			return players.Count;
		}
		else{
			return 0;
		}
	}
	
	
	//listening to when players connect, disconnect, and change their information
	private List<IJoviosPlayerListener> playerListeners = new List<IJoviosPlayerListener>();
	public void AddPlayerListener(IJoviosPlayerListener listener){
		playerListeners.Add(listener);
	}
	public void RemovePlayerListener(IJoviosPlayerListener listener){
		playerListeners.Remove(listener);
	}
	public void RemoveAllPlayerListeners(){
		playerListeners = new List<IJoviosPlayerListener>();
	}
	
	
	//listening to each player's controller
	public List<IJoviosControllerListener> GetControllerListeners(JoviosUserID jUID){
		return GetPlayer(jUID).GetControllerListeners();
	}
	public List<IJoviosControllerListener> GetControllerListeners(int playerNumber){
		return GetPlayer(playerNumber).GetControllerListeners();
	}
	public List<IJoviosControllerListener> GetControllerListeners(){
		List<IJoviosControllerListener> allControllerListeners = new List<IJoviosControllerListener>();
		foreach(JoviosPlayer player in players){
			foreach(IJoviosControllerListener listener in player.GetControllerListeners()){
				allControllerListeners.Add(listener);
			}
		}
		return allControllerListeners;
	}
	public void AddControllerListener(IJoviosControllerListener listener){
		foreach(JoviosPlayer player in players){
			player.AddControllerListener(listener);
		}
	}
	public void AddControllerListener(IJoviosControllerListener listener, int playerNumber){
		GetPlayer(playerNumber).AddControllerListener(listener);
	}
	public void AddControllerListener(IJoviosControllerListener listener, JoviosUserID jUID){
		GetPlayer(jUID).AddControllerListener(listener);
	}
	public void RemoveControllerListener(IJoviosControllerListener listener){
		foreach(JoviosPlayer player in players){
			player.RemoveControllerListener(listener);
		}
	}
	public void RemoveControllerListener(IJoviosControllerListener listener, int playerNumber){
		GetPlayer(playerNumber).RemoveControllerListener(listener);
	}
	public void RemoveControllerListener(IJoviosControllerListener listener, JoviosUserID jUID){
		GetPlayer(jUID).RemoveControllerListener(listener);
	}
	public void RemoveAllControllerListeners(){
		foreach(JoviosPlayer player in players){
			player.RemoveAllControllerListeners();
		}
	}
	public void RemoveAllControllerListeners(int playerNumber){
		GetPlayer(playerNumber).RemoveAllControllerListeners();
	}
	public void RemoveAllControllerListeners(JoviosUserID jUID){
		GetPlayer(jUID).RemoveAllControllerListeners();
	}
	
	public void SetControls(JoviosUserID jUID, string presetController){
		if(controllerStyles.ContainsKey(presetController)){
			JoviosControllerStyle jcs = new JoviosControllerStyle();
			GameObject go = controllerStyles[presetController];
			for(int i = 0; i < go.transform.childCount; i++){
				go.transform.GetChild(i).GetComponent<JoviosControllerConstructor>().AddControllerComponent(jcs);
			}
			SetControls(jUID, jcs);
		}
		else{
			Debug.Log ("wrong key: " + presetController);
		}
	}
	
	//this will set the controlls of a given player
	public void SetControls(JoviosUserID jUID, JoviosControllerStyle controllerStyle){
		GetPlayer(jUID).SetControllerStyle(controllerStyle);
		AddToPacket(jUID, controllerStyle.GetJSON());
	}
	
	//When a controller connects it will check the version so that it can know if the controller is out of date.  If the game is out of date the controller should still work with it (only 1.0.0 and greater)
	private string version = "0.0.0";
	public string GetVersion(){
		return version;
	}
	[RPC] public void CheckVersion(string controllerVersion, int deviceID){
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
	
	Dictionary<string, GameObject> controllerStyles = new Dictionary<string, GameObject>();
	Dictionary<string, Texture2D> exportTextures = new Dictionary<string, Texture2D>();
	public void StartServer(List<GameObject> setControllerStyles, List<Texture2D> setExportTextures, string thisGameName = ""){
		foreach(GameObject go in setControllerStyles){
			controllerStyles.Add(go.name, go);
		}
		foreach(Texture2D tex in setExportTextures){
			exportTextures.Add(tex.name, tex);
		}
		StartServer (thisGameName);
	}
	
	//this starts the unity server and udp broadcast to local network
	public string iconURL{get; set;}
	public string gameCode{get; private set;}
	public void StartServer(string thisGameName = ""){
		NodeLinux();
		udpPort = 24000;
		unityPort = 25000;
		StartUnity();
		Application.runInBackground = true;
		SetGameName(thisGameName);
		udpEndpoint = new IPEndPoint(IPAddress.Any, udpPort);
		udpBroadcastEndpoint = new IPEndPoint(IPAddress.Broadcast, udpPort);
		StartCoroutine("BroadcastPresence");
		Debug.Log("begun");
		//Thread tr = new Thread(new ThreadStart(UDPListening));
		//tr.Start();
	}
	
	PomeloClient pclient;
	//Login the chat application and new PomeloClient.
	void NodeLinux() {
		string url = "54.187.155.212:3014";
		pclient = new PomeloClient(url);
		pclient.init();
		JsonObject userMessage = new JsonObject();
		userMessage.Add("uid", "0");
		pclient.request("gate.gateHandler.queryEntry", userMessage, (data)=>{
			System.Object code = null;
			if(data.TryGetValue("code", out code)){
				if(Convert.ToInt32(code) == 500) {
					return;
				} else {
					pclient.disconnect();
					pclient = null;
					System.Object host, port;
					if (data.TryGetValue("host", out host) && data.TryGetValue("port", out port)) {
						pclient = new PomeloClient("http://" + "54.187.155.212" + ":" + port.ToString());
						pclient.init();
						pclient.On("onAdd", (data3)=> {
							var userJoined = JSON.Parse(data3.ToString());
						});
						pclient.On("onResponse", (data2)=> {
							addMessage(data2.ToString());
						});
						pclient.On("onLeave", (data4)=> {
							var disconnectJSON = JSON.Parse(data4.ToString());
							PlayerDisconnectedInterpret.Add(disconnectJSON["body"]["user"].AsInt);
						});
						pclient.On("onError", (data5)=> {
							System.Object nodeLinuxError;
							data5.TryGetValue("body", out nodeLinuxError);
							Debug.Log(nodeLinuxError.ToString());
						});
						JoinNodeLinux();
					}
				} 
			}
		});
		externalIP = Network.player.externalIP;
	}

	void addMessage(string messge) {
		var packet = JSON.Parse(messge);
		mainThreadInterpret.Add(packet["body"]["msg"]);
	}
	
	//Entry chat application.
	void JoinNodeLinux(){
		JsonObject userMessage = new JsonObject();
		userMessage.Add("username", "0");
		userMessage.Add("rid", "0");
		userMessage.Add("hosting", true);
		if (pclient != null) {
			pclient.request("connector.entryHandler.enter", userMessage, (data)=>{
				System.Object gameID;
				data.TryGetValue("gameID", out gameID);
				gameCode = gameID.ToString();
			});
		}
	}
	
	public void SendNodeLinux(string content, string target = "*"){
		JsonObject message = new JsonObject();
		message.Add("rid", gameCode);
		message.Add("content", content);
		message.Add("from", "0");
		message.Add("target", target);	
		pclient.request("chat.chatHandler.send", message, (data) => {
		});
	}


	void StartUnity(){
		UnityNetworkConnect();
	}
	void UnityNetworkConnect(){
		Network.InitializeServer(32, unityPort, !Network.HavePublicAddress());
	}
	
	//this is the external IP gained from either the website or the unity system
	private string externalIP;
	IEnumerator GetIP(){
		WWW www = new WWW("http://checkip.dyndns.org");
		yield return www;
		if(www.error == null){
			externalIP=www.text;
			externalIP=externalIP.Substring(externalIP.IndexOf(":")+1);
			externalIP=externalIP.Substring(0,externalIP.IndexOf("<"));
		}
		else{
			externalIP = Network.player.externalIP;
		}
		string toSend = gameName+";"+Network.player.ipAddress+";"+gameCode+";"+unityPort+";"+iconURL;
		if(Application.isWebPlayer){
			Application.ExternalCall("SetGameName", gameName);
		}
	}
	private WWW wwwData = null;
	private Dictionary<int, NetworkPlayer> networkPlayers = new Dictionary<int, NetworkPlayer>();
	private int networkPlayerCount = 0;
	private const string typeName = "Jovios";

	//this sets the gamename
	void SetGameName(string newGameName){
		if(newGameName != ""){
			gameName = newGameName;
		}
		else{
			gameName = "New Game";
		}
		StartCoroutine("GetIP");
	}    
	
	//if this is in the web player it will download a new game
	public IEnumerator WaitForDownload(){    
		yield return wwwData;
		wwwData.LoadUnityWeb();  
	}
	[RPC] public void NewUrl(string url){
		networkView.RPC("NewGame", RPCMode.Others);
		wwwData = new WWW(url);
		StartCoroutine("WaitForDownload");
	}
	
	//this is the unity newtorkign connection and disconnection information
	void OnPlayerConnected(NetworkPlayer player){
		string playerJSON;
		playerJSON = "{'packet':{'playerNumber':"+networkPlayerCount.ToString()+"}}";
		networkView.RPC ("SendPacket",player,playerJSON);
		networkPlayers.Add(networkPlayerCount, player);
		networkPlayerCount ++;
		foreach(KeyValuePair<string, Texture2D> kvp in exportTextures){
			networkView.RPC ("SendImage",player, kvp.Key, kvp.Value.EncodeToPNG());
		}
	}
	
	//this is the unity newtorkign connection and disconnection information
	void OnPlayerDisconnected(NetworkPlayer player){
		for(int i = 0; i < players.Count; i++){
			if(GetPlayer(i).GetNetworkPlayer() == player){
				PlayerDisconnected(GetPlayer(i));
			}
		}
	}
	public void SetNetworkPlayer(int deviceID, int playerNumber){
		GetPlayer(new JoviosUserID(deviceID)).SetNetworkPlayer(networkPlayers[playerNumber]);
	}
	
	public int udpPort {get; private set;}
	public int unityPort {get; private set;}
	public IEnumerator BroadcastPresence(){
		string toSend = "'packet':{'session':'"+gameName+";"+Network.player.ipAddress+";"+gameCode+";"+unityPort+";"+iconURL+"'}";
		byte[] sendBytes = Encoding.ASCII.GetBytes(toSend);
		udpClient.Send(sendBytes, sendBytes.Length, udpBroadcastEndpoint);
		yield return new WaitForSeconds(5);
		StartCoroutine("BroadcastPresence");
	}

	int webServerPort = 8080;
	NetworkStream sWeb;
	StreamReader srWeb;
	StreamWriter swWeb;		

	UdpClient udpClient = new UdpClient();
	IPEndPoint udpEndpoint;
	IPEndPoint udpBroadcastEndpoint;

	
	
	private Dictionary<int, string> packetJSON = new Dictionary<int, string>();
	private Dictionary<int, JoviosNetworkingState> networkingStates = new Dictionary<int, JoviosNetworkingState>();
	private Dictionary<int, string> connectionJSON = new Dictionary<int, string>();
	List<string> mainThreadInterpret = new List<string>();
	List<string> PlayerConnectedInterpret = new List<string>();
	List<int> PlayerDisconnectedInterpret = new List<int>();
	string udpThreadInterpret = "";
	//this sends out the packets as they are generated
	void FixedUpdate(){
		if(mainThreadInterpret.Count > 0){
			for(int i = 0; i < mainThreadInterpret.Count; i++){
				SendPacket(mainThreadInterpret[i]);
			}
			mainThreadInterpret = new List<string>();
		}
		if(PlayerConnectedInterpret.Count > 0){
			for(int i = 0; i < PlayerConnectedInterpret.Count; i++){
				var myJSON = JSON.Parse(PlayerConnectedInterpret[i]);
				PlayerConnected(myJSON["packet"]["playerConnected"]["ip"], myJSON["packet"]["playerConnected"]["networkType"], myJSON["packet"]["playerConnected"]["playerNumber"].AsInt, myJSON["packet"]["playerConnected"]["primaryR"].AsFloat, myJSON["packet"]["playerConnected"]["primaryG"].AsFloat, myJSON["packet"]["playerConnected"]["primaryB"].AsFloat, myJSON["packet"]["playerConnected"]["secondaryR"].AsFloat, myJSON["packet"]["playerConnected"]["secondaryG"].AsFloat, myJSON["packet"]["playerConnected"]["secondaryB"].AsFloat, myJSON["packet"]["playerConnected"]["playerName"], myJSON["packet"]["playerConnected"]["deviceID"].AsInt);
			}
			PlayerConnectedInterpret = new List<string>();
		}
		for(int i = 0; i < PlayerDisconnectedInterpret.Count; i++){
			PlayerDisconnected(GetPlayer(new JoviosUserID(PlayerDisconnectedInterpret[i])));
		}
		PlayerDisconnectedInterpret = new List<int>();
		foreach(int key in deviceIDToPlayerNumber.Keys){
			if(packetJSON[key] != ""){
				packetJSON[key] += "}}";
				switch(networkingStates[key]){

				case JoviosNetworkingState.Unity:
					networkView.RPC("SendPacket", GetPlayer(new JoviosUserID(key)).GetNetworkPlayer(), packetJSON[key]);
					break;

				case JoviosNetworkingState.WebServer:
					SendNodeLinux(packetJSON[key], key.ToString());
					break;

				default:
					Debug.Log("error in networking states");
					break;
				}
				packetJSON[key] = "";
			}
		}
	}
	public void AddToPacket(JoviosUserID jUID, string addition){
		if(packetJSON[jUID.GetIDNumber()] == ""){
			packetJSON[jUID.GetIDNumber()] += "{'packet':{" +connectionJSON[jUID.GetIDNumber()]+","+ addition;
		}
		else{
			packetJSON[jUID.GetIDNumber()] += "," + addition;
		}
	}





	//this picks up the Unity Networking connections
	[RPC] void SendPacket(string packet){
		packet = packet.Replace("'","\"");
		ParsePacket(packet);
	}
	[RPC] void SendImage(string imageName, byte[] imageData){

	}
	//this parses the incoming packets
	private void ParsePacket(string packet){
		var myJSON = JSON.Parse(packet);
		if(myJSON["packet"]["response"] != null){
			for(int i = 0; i < myJSON["packet"]["response"].Count; i++){
				switch(myJSON["packet"]["response"][i]["type"]){
				case "button":
					ButtonPress(myJSON["deviceID"].AsInt, myJSON["packet"]["response"][i].ToString ());
					break;
					
				case "direction":
					switch(myJSON["packet"]["response"][i]["action"]){
					case "hold":
						if(GetPlayer(new JoviosUserID(myJSON["deviceID"].AsInt)).GetControllerStyle().GetDirection(myJSON["packet"]["response"][i]["direction"]) != null){
							GetPlayer(new JoviosUserID(myJSON["deviceID"].AsInt)).GetControllerStyle().GetDirection(myJSON["packet"]["response"][i]["direction"]).SetDirection(new Vector2(myJSON["packet"]["response"][i]["position"][0].AsFloat, myJSON["packet"]["response"][i]["position"][1].AsFloat));
						}
						break;
						
					case "press":
						ButtonPress(myJSON["deviceID"].AsInt, myJSON["packet"]["response"][i].ToString ());
						break;
						
					case "release":
						ButtonPress(myJSON["deviceID"].AsInt, myJSON["packet"]["response"][i].ToString ());
						break;
						
					default:
						break;
					}
					break;
					
				case "accelerometer":
					var accInfo = myJSON["packet"]["response"][i];
					GetPlayer(new JoviosUserID(myJSON["deviceID"].AsInt)).GetControllerStyle().GetAccelerometer().SetGyro(new Quaternion(-accInfo["gyroX"].AsFloat, -accInfo["gyroY"].AsFloat, accInfo["gyroZ"].AsFloat, accInfo["gyroW"].AsFloat));
					GetPlayer(new JoviosUserID(myJSON["deviceID"].AsInt)).GetControllerStyle().GetAccelerometer().SetAcceleration(new Vector3(accInfo["accx"].AsFloat, accInfo["accy"].AsFloat, accInfo["accZ"].AsFloat));
					break;
					
				default:
					Debug.Log ("wrong response type");
					break;
				}
			}
		}
		if(myJSON["packet"]["playerConnected"] != null){
			PlayerConnectedInterpret.Add(myJSON.ToString());
		}
		if(myJSON["packet"]["playerUpdated"] != null){
			PlayerUpdated(myJSON["packet"]["playerUpdated"]["deviceID"].AsInt, myJSON["packet"]["playerUpdated"]["primaryR"].AsFloat, myJSON["packet"]["playerUpdated"]["primaryG"].AsFloat, myJSON["packet"]["playerUpdated"]["primaryB"].AsFloat, myJSON["packet"]["playerUpdated"]["secondaryR"].AsFloat, myJSON["packet"]["playerUpdated"]["secondaryG"].AsFloat, myJSON["packet"]["playerUpdated"]["secondaryB"].AsFloat, myJSON["packet"]["playerUpdated"]["playerName"]);
		}
	}

	public void ButtonPress(int player, string buttonJSON){
		var myJSON = JSON.Parse(buttonJSON);
		switch(myJSON["type"]){
		case "button":
			JoviosButtonEvent e = new JoviosButtonEvent(myJSON["button"], GetPlayer(new JoviosUserID(player)).GetControllerStyle(), myJSON["action"]);
			foreach(IJoviosControllerListener listener in GetPlayer(new JoviosUserID(player)).GetControllerListeners()){
				if(listener.ButtonEventReceived(e)){
					return;
				}
			}
			if(GetPlayer(new JoviosUserID(player)).GetControllerStyle().GetButton(myJSON["button"]) != null){
				if(myJSON["action"] == "press"){
					GetPlayer(new JoviosUserID(player)).GetControllerStyle().GetButton(myJSON["button"]).is_pressed = true;
				}
				else{
					GetPlayer(new JoviosUserID(player)).GetControllerStyle().GetButton(myJSON["button"]).is_pressed = false;
				}
			}
			break;

		case "direction":
			JoviosButtonEvent e1 = new JoviosButtonEvent(myJSON["direction"], GetPlayer(new JoviosUserID(player)).GetControllerStyle(), myJSON["action"]);
			foreach(IJoviosControllerListener listener in GetPlayer(new JoviosUserID(player)).GetControllerListeners()){
				if(listener.ButtonEventReceived(e1)){
					return;
				}
			}
			if(GetPlayer(new JoviosUserID(player)).GetControllerStyle().GetButton(myJSON["direction"]) != null){
				if(myJSON["action"] == "press"){
					GetPlayer(new JoviosUserID(player)).GetControllerStyle().GetDirection(myJSON["direction"]).is_pressed = true;
				}
				else{
					GetPlayer(new JoviosUserID(player)).GetControllerStyle().GetDirection(myJSON["direction"]).is_pressed = false;
				}
			}
			break;

		default:
			break;
		}
	}
	
	//This will be called by the connection scripts and will manage player connections
	public string assetBundleURL{get; set;}
	public void PlayerConnected(string ip, string networkType, int playerNumber, float primaryR, float primaryG, float primaryB, float secondaryR, float secondaryG, float secondaryB, string playerName, int deviceID){
		players.Add(new JoviosPlayer(players.Count, new JoviosUserID(deviceID), playerName, new Color(primaryR, primaryG, primaryB, 1), new Color(secondaryR, secondaryG, secondaryB, 1)));
		if(!deviceIDToPlayerNumber.ContainsKey(deviceID)){
			deviceIDToPlayerNumber.Add(deviceID, players.Count - 1);
			packetJSON.Add(deviceID, "");
			connectionJSON.Add(deviceID, "'action':'blah','deviceID':"+deviceID.ToString()+",'gameCode':'"+gameCode.ToString()+"','ip':'"+ip+"'");
			switch(networkType){
			case "unity":
				SetNetworkPlayer(deviceID, playerNumber);
				if(networkingStates.ContainsKey(deviceID)){
					networkingStates[deviceID] = JoviosNetworkingState.Unity;
				}
				else{
					networkingStates.Add(deviceID, JoviosNetworkingState.Unity);
				}
				break;
			case "udp":
				SetNetworkPlayer(deviceID, playerNumber);
				if(networkingStates.ContainsKey(deviceID)){
					networkingStates[deviceID] = JoviosNetworkingState.Unity;
				}
				else{
					networkingStates.Add(deviceID, JoviosNetworkingState.Unity);
				}
				break;
			case "webServer":
				networkingStates.Add(deviceID, JoviosNetworkingState.WebServer);
				break;
			default:
				SetNetworkPlayer(deviceID, playerNumber);
				if(networkingStates.ContainsKey(deviceID)){
					networkingStates[deviceID] = JoviosNetworkingState.Unity;
				}
				else{
					networkingStates.Add(deviceID, JoviosNetworkingState.Unity);
				}
				break;
			}
		}
		else{
			deviceIDToPlayerNumber[deviceID] = playerNumber;
		}
		if(assetBundleURL != "" && assetBundleURL != null){
			AddToPacket(new JoviosUserID(deviceID), "'assetBundle':'"+assetBundleURL+"'");
		}
		foreach(IJoviosPlayerListener listener in playerListeners){
			if(listener.PlayerConnected(GetPlayer(new JoviosUserID(deviceID)))){
				break;
			}
		}
	}
	
	// this will be triggered when information about a player is updated, like colors or names
	public void PlayerUpdated(int deviceID, float primaryR, float primaryG, float primaryB, float secondaryR, float secondaryG, float secondaryB, string playerName){
		GetPlayer(new JoviosUserID(deviceID)).NewPlayerInfo(deviceIDToPlayerNumber[deviceID], playerName, new Color(primaryR, primaryG, primaryB, 1), new Color(secondaryR, secondaryG, secondaryB, 1));
		foreach(IJoviosPlayerListener listener in playerListeners){
			if(listener.PlayerUpdated(GetPlayer(new JoviosUserID(deviceID)))){
				break;
			}
		}
	}
	
	// this will trigger when a player disconnects,
	public void PlayerDisconnected(JoviosPlayer p){
		players.Remove(p);
		packetJSON.Remove(p.GetUserID().GetIDNumber());
		networkingStates.Remove(p.GetUserID().GetIDNumber());
		connectionJSON.Remove(p.GetUserID().GetIDNumber());
		deviceIDToPlayerNumber.Remove(p.GetUserID().GetIDNumber());
		for(int i = 0; i < deviceIDToPlayerNumber.Count; i++){
			deviceIDToPlayerNumber[GetPlayer(i).GetUserID().GetIDNumber()] = i;
			players[i].NewPlayerInfo(i, players[i].GetPlayerName(), players[i].GetColor("primary"), players[i].GetColor("secondary"));
		}
		for(int i = 0; i < p.PlayerObjectCount(); i++){
			Destroy(p.GetPlayerObject(i));
		}
		foreach(IJoviosPlayerListener listener in playerListeners){
			if(listener.PlayerDisconnected(p)){
				break;
			}
		}
	}

	static private bool isTrue = true;
	//this disconnects when the application quits
	void OnApplicationQuit(){
		Network.Disconnect();
		pclient.disconnect();
		try{
			isTrue = false;
			udpClient.Close();
		}
		catch(Exception e){
			Debug.Log (e.ToString());
		}
		Debug.Log ("sockets closed");
	}

}