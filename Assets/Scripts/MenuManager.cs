using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour, IJoviosPlayerListener{
	
	public static Jovios jovios;
	public GameObject playerObject;
	public Transform playerController;
	public Transform powerupController;
	
	void Start(){
		jovios = Jovios.Create();
		jovios.AddPlayerListener(this);
		jovios.StartServer();
	}
	
	void OnGUI(){
		GUI.Box(new Rect(0,0,100,50), jovios.GetGameName());
	}
	
	bool IJoviosPlayerListener.PlayerConnected(JoviosPlayer p){
		JoviosControllerStyle controllerStyle = new JoviosControllerStyle();
		controllerStyle.AddAbsoluteJoystick("left", "Move Character", "Move");
		controllerStyle.AddButton2("right", new string[] {"Jump"}, new string[] {"Jump"});
		jovios.SetControls(p.GetUserID(), controllerStyle);
		GameObject newGameObject = (GameObject) GameObject.Instantiate(playerObject, GameObject.Find ("PlayerSpawnLocations").transform.GetChild (Mathf.FloorToInt(GameObject.Find ("PlayerSpawnLocations").transform.childCount * Random.value)).position, Quaternion.identity);
		jovios.GetPlayer(p.GetUserID()).AddPlayerObject(newGameObject);

		//Assign new player as child of Player Controller object
		newGameObject.GetComponent<Player> ().playerController = playerController;
		newGameObject.GetComponent<Player> ().powerupController = powerupController;

		newGameObject.GetComponent<Player>().PlayerSetup(p);
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
}
