using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour, IJoviosPlayerListener, IJoviosControllerListener{
	
	public static Jovios jovios;
	public GameObject playerObject;
	
	void Start(){
		jovios = Jovios.Create();
		jovios.AddPlayerListener(this);
		jovios.StartServer();
	}
	
	void OnGUI(){
		GUI.Box(new Rect(0,0,100,50), jovios.GetGameName());
	}
	
	bool IJoviosPlayerListener.PlayerConnected(JoviosPlayer p){
		Debug.Log (p.GetPlayerName());
		JoviosControllerStyle controllerStyle = new JoviosControllerStyle();
		controllerStyle.AddRelativeJoystick("left", "Move Character", "Move");
		controllerStyle.AddButton2("right", new string[] {"Jump"}, new string[] {"Jump"});
		//controllerStyle.SetTextInput("What is your quest?", "Submit");
		jovios.SetControls(p.GetUserID(), controllerStyle);
		GameObject gameObject = (GameObject) GameObject.Instantiate(playerObject, Vector3.up, Quaternion.identity);
		gameObject.GetComponent<Player>().PlayerSetup(p);
		return false;
	}
	bool IJoviosPlayerListener.PlayerUpdated(JoviosPlayer p){
		Debug.Log (p.GetPlayerName());
		return false;
	}
	bool IJoviosPlayerListener.PlayerDisconnected(JoviosPlayer p){
		Debug.Log (p.GetPlayerName());
		return false;
	}
	
	bool IJoviosControllerListener.ButtonEventReceived(JoviosButtonEvent e){
		Debug.Log (e.GetResponse() + e.GetAction());
		//Debug.Log (e.GetControllerStyle().GetQuestionPrompt());
		return false;
	}
}
