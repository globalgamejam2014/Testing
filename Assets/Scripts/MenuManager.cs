using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {
	
	static public bool is_menu = true;
	static public bool is_gameOver = false;
	static public bool is_credits = false;
	static public bool is_countdown = false;
	static public bool is_loadingNewGame = false;
	private int countdownTime = 5;
	static public float lastTickTime;
	public Font font;
	private Transform mainCamera;
	private string muteText = "Mute";
	
	void Start(){
		mainCamera = GameObject.Find ("MainCamera").transform;
	}
	
	void OnGUI(){
		GUI.skin.font = font;
		if(is_countdown){
			GUI.skin.box.fontSize = 30;
			GUI.Box(new Rect(Screen.width - Screen.width/5,0,Screen.width/5,Screen.height/5), "Game Starts In");
			GUI.skin.box.fontSize = 60;
			GUI.Box(new Rect(Screen.width - Screen.width/5,Screen.height/5,Screen.width/5,Screen.height/5), countdownTime.ToString());
			GUI.skin.box.fontSize =15;
			if(countdownTime < 1){
				transform.GetComponent<NetworkManager>().StartRound();
				countdownTime = 5;
				is_countdown = false;
				is_menu = false;
				PlayerControls.is_gameOn = true;
			}
			else if(lastTickTime + 1 < Time.time){
				lastTickTime = Time.time;
				countdownTime--;
			}
		}
		if (is_loadingNewGame){
			GUI.Box(new Rect(Screen.width - Screen.width/5,0,Screen.width/5,Screen.height/5), "Loading New Game");
		}
		else if(is_menu){
			GUI.Box(new Rect(Screen.width/2 - Screen.width/5,0,Screen.width/2.5F,Screen.height/5), "Menu");
			if (GUI.Button(new Rect(Screen.width/2-Screen.width/10,Screen.height/10,Screen.width/5,Screen.height/5), "FullScreen")){
				Screen.fullScreen = !Screen.fullScreen;
			}
			if (GUI.Button(new Rect(Screen.width/2-Screen.width/10,Screen.height/10*4,Screen.width/5,Screen.height/5), "Reload Player")){
				transform.GetComponent<NetworkManager>().NewUrl("http://beta.catduo.com/tf/WebPlayerBeta.unity3d");
			}
			if (GUI.Button(new Rect(0,0,Screen.width/20,Screen.height/20), "Menu")){
				is_menu = false;
			}
		}
		else if(PlayerControls.is_gameOn){
			if (GUI.Button(new Rect(0,0,Screen.width/20,Screen.height/20), "Menu")){
				is_menu = true;
			}
		}
		else{
			GUI.Box(new Rect(Screen.width - Screen.width/5,0,Screen.width/5,Screen.height/5), "Waiting for Players\n" + NetworkManager.playerList.Length.ToString() + " in Lobby");
			if (GUI.Button(new Rect(0,0,Screen.width/20,Screen.height/20), "Menu")){
				is_menu = true;
			}
		}
		GUI.Box(new Rect(0,Screen.height/20,Screen.width/20,Screen.height/20), NetworkManager.gameName);
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
