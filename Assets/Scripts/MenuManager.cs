using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour, IJoviosPlayerListener, IJoviosControllerListener{
	
	public static Jovios jovios;
	public GameObject playerObject;
	public Transform playerController;
	public Transform powerupController;
	public Transform projectile;
	
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
		controllerStyle.AddAbsoluteJoystick("left", "Move Character", "Move");
		controllerStyle.AddButton2("right", new string[] {"Jump"}, new string[] {"Jump"});
		jovios.SetControls(p.GetUserID(), controllerStyle);
		GameObject gameObject = (GameObject) GameObject.Instantiate(playerObject, Vector3.up, Quaternion.identity);

		//Assign new player as child of Player Controller object
		gameObject.GetComponent<Player> ().playerController = playerController;
		gameObject.GetComponent<Player> ().powerupController = powerupController;
		gameObject.GetComponent<Player> ().projectile = projectile;

		gameObject.GetComponent<Player>().PlayerSetup(p);
		powerupController.GetComponent<PU_Controller>().UpdatePlayerScripts();

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
