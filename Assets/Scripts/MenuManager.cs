using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {
	
	static public bool is_menu = true;
	static public bool is_gameOver = false;
	static public bool is_credits = false;
	static public bool is_countdown = false;
	static public bool is_loadingNewGame = false;
	static public int countdownTime = 3;
	static public int timer = 0;
	static public float lastTickTime;
	public Font font;
	private Transform mainCamera;
	private string muteText = "Mute";
	
	static public int roundDuration = 60;
	private float roundStart;
	private bool is_roundStarted = false;
	
	void Start(){
		mainCamera = GameObject.Find ("MainCamera").transform;
	}
	
	void OnGUI(){
		GUI.skin.font = font;
		GUI.skin.box.wordWrap = true;
		GUI.skin.button.wordWrap = true;
		GUI.skin.box.fontSize = Mathf.RoundToInt((Camera.main.WorldToScreenPoint(new Vector2(1,1)).x-Camera.main.WorldToScreenPoint(new Vector2(0,0)).x)*0.7F);
		GUI.skin.button.fontSize = Mathf.RoundToInt((Camera.main.WorldToScreenPoint(new Vector2(1,1)).x-Camera.main.WorldToScreenPoint(new Vector2(0,0)).x)*0.7F);
		GUI.skin.textArea.fontSize = Mathf.RoundToInt((Camera.main.WorldToScreenPoint(new Vector2(1,1)).x-Camera.main.WorldToScreenPoint(new Vector2(0,0)).x)*0.7F);
		GUI.skin.textField.fontSize = Mathf.RoundToInt((Camera.main.WorldToScreenPoint(new Vector2(1,1)).x-Camera.main.WorldToScreenPoint(new Vector2(0,0)).x)*0.7F);
		if(is_countdown){
			GUI.Box(new Rect(Screen.width - Screen.width/2.5F,0,Screen.width/5,Screen.height/5), "Game Starts In");
			GUI.skin.box.fontSize *= 2;
			GUI.Box(new Rect(Screen.width - Screen.width/2.5F,Screen.height/5,Screen.width/5,Screen.height/5), countdownTime.ToString());
			GUI.skin.box.fontSize /= 2;
			if(timer < 1){
				transform.GetComponent<NetworkManager>().StartRound();
				timer = roundDuration;
				is_countdown = false;
				is_menu = false;
				PlayerControls.is_gameOn = true;
				for(int i = 0; i < NetworkManager.readyList.Length; i++){
					if(NetworkManager.readyList[i]){
						networkView.RPC ("SetControls", NetworkManager.playerList[i], 0,0);
					}
					else{
						transform.GetComponent<PlayerControls>().SentBasicButtons("Waiting for round to complete.", NetworkManager.playerList[i]);
					}
					NetworkManager.readyList[i] = false;
				}
			}
			else if(lastTickTime + 1 < Time.time){
				lastTickTime = Time.time;
				timer--;
			}
		}
		if (is_loadingNewGame){
			GUI.Box(new Rect(Screen.width - Screen.width/5,0,Screen.width/5,Screen.height/5), "Loading New Game");
		}
		else if(is_menu){
			GUI.Box(new Rect(Screen.width - Screen.width/5,0,Screen.width/5,Screen.height/5), "Waiting for Players\n" + NetworkManager.playerList.Length.ToString() + " in Lobby");
			GUI.Box(new Rect(Screen.width - Screen.width/5,Screen.height/5,Screen.width/5,Screen.height/5), "Players\n" + NetworkManager.readyCount.ToString() + " Ready");
			GUI.Box(new Rect(Screen.width/2 - Screen.width/5,0,Screen.width/2.5F,Screen.height/20), "Menu");
			if(!Screen.fullScreen){
				if (GUI.Button(new Rect(Screen.width/2-Screen.width/10,Screen.height/10,Screen.width/5,Screen.height/5), "FullScreen")){
					Screen.fullScreen = !Screen.fullScreen;
				}
			}
			if(Application.isWebPlayer){
				if (GUI.Button(new Rect(Screen.width/2-Screen.width/10,Screen.height/10*4,Screen.width/5,Screen.height/5), "Reload Player")){
					transform.GetComponent<NetworkManager>().NewUrl(Application.absoluteURL);
				}
			}
			if (GUI.Button(new Rect(0,0,Screen.width/10,Screen.height/20), "Menu")){
				is_menu = false;
			}
		}
		else if(PlayerControls.is_gameOn){
			if (GUI.Button(new Rect(0,0,Screen.width/10,Screen.height/20), "Menu")){
				is_menu = true;
			}
			GUI.Box(new Rect(Screen.width - Screen.width/5,0,Screen.width/5,Screen.height/5), "Time Remaining");
			GUI.skin.box.fontSize *= 2;
			GUI.Box(new Rect(Screen.width - Screen.width/5,Screen.height/5,Screen.width/5,Screen.height/5), roundDuration.ToString());
			GUI.skin.box.fontSize /= 2;
			if(timer < 1){
				transform.GetComponent<NetworkManager>().EndRound();
				timer = countdownTime;
				PlayerControls.is_gameOn = false;
				is_menu = true;
			}
			else if(lastTickTime + 1 < Time.time){
				lastTickTime = Time.time;
				timer--;
			}
			if(!Screen.fullScreen){
				if (GUI.Button(new Rect(Screen.width/10,0,Screen.width/5,Screen.height/20), "FullScreen")){
					Screen.fullScreen = !Screen.fullScreen;
				}
			}
			if(Application.isWebPlayer){
				if (GUI.Button(new Rect(Screen.width/2.5F,0,Screen.width/5,Screen.height/20), "Reload Player")){
					transform.GetComponent<NetworkManager>().NewUrl(Application.absoluteURL);
				}
			}
		}
		else{
			if (GUI.Button(new Rect(0,0,Screen.width/10,Screen.height/20), "Menu")){
				is_menu = true;
			}
		}
		GUI.Box(new Rect(0,Screen.height/20,Screen.width/10,Screen.height/20), NetworkManager.gameName);
	}
		
	
	Rect WorldRect(Rect rect){
		Vector3 pos;
		Vector3 dim;
		pos = Camera.main.WorldToScreenPoint(new Vector2(rect.x, -rect.y));
		dim = Camera.main.WorldToScreenPoint(new Vector2(rect.xMax, -rect.yMax));
		rect = new Rect(pos.x, pos.y, dim.x - pos.x, pos.y - dim.y);
		return rect;
	}
}
