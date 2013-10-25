using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {
	
	static public bool is_menu = true;
	static public bool is_gameOver = false;
	static public bool is_credits = false;
	public Font font;
	private Transform mainCamera;
	private string muteText = "Mute";
	
	void Start(){
		mainCamera = GameObject.Find ("MainCamera").transform;
	}
	
	void OnGUI(){
		GUI.skin.font = font;
		if(is_menu){
			if (GUI.Button(WorldRect(new Rect(-13,10,3,1)), "Menu")){
				is_menu = false;
			}
		}
		else{
			if (GUI.Button(WorldRect(new Rect(-13,10,3,1)), "Menu")){
				is_menu = true;
			}
			GUI.Box(WorldRect(new Rect(-13,9,3,1)), NetworkManager.gameName);
		}
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
