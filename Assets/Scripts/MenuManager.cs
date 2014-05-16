using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ControlStyle{
	PlayAgain,
	Cursor,
	Dragon,
	Powerup
}

public class MenuManager : MonoBehaviour, IJoviosPlayerListener{
	
	public static Jovios jovios;
	public GameObject playerObject;
	public Transform playerController;
	public Transform powerupController;
	public Dictionary<string, JoviosControllerStyle> controls = new Dictionary<string, JoviosControllerStyle>();
	public List<GameObject> controlStyles = new List<GameObject>();
	
	void Start(){
		jovios = Jovios.Create();
		jovios.AddPlayerListener(this);
		jovios.StartServer(controlStyles, new List<Texture2D>(), "Wizard Lizards");
	}
	
	void OnGUI(){
		GUI.Box(new Rect(0,0,100,50), jovios.gameCode);
	}
	
	bool IJoviosPlayerListener.PlayerConnected(JoviosPlayer p){
		jovios.SetControls(p.GetUserID(), "Dragon");
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

	public static JoviosControllerStyle SetControls(ControlStyle controls){
		JoviosControllerStyle controllerStyle = new JoviosControllerStyle();
		switch(controls){
		case ControlStyle.PlayAgain:
			controllerStyle.AddButton1(new Vector2(0, 0), new Vector2(1.5F, 1.5F), "mc", "Play Again!", "Play Again!");
			break;
		case ControlStyle.Cursor:
			controllerStyle.AddJoystick(new Vector2(0.7F, 1F), new Vector2(1.2F, 1.6F), "bl", "left", "left");
			controllerStyle.AddButton1(new Vector2(-0.7F, 1F), new Vector2(1.2F, 1.6F), "br", "Click cursor", "Click");
			break;
		case ControlStyle.Dragon:
			controllerStyle.AddJoystick(new Vector2(0.7F, 1F), new Vector2(1.2F, 1.6F), "bl", "left");
			controllerStyle.AddButton1(new Vector2(-0.4F, 1F), new Vector2(0.6F, 0.6F), "br", "Jump", "Jump");
			controllerStyle.AddButton1(new Vector2(-1F, 0.4F), new Vector2(0.6F, 0.6F), "br", "Fire", "Fire");
			break;
		case ControlStyle.Powerup:
			controllerStyle.AddJoystick(new Vector2(0.7F, 1F), new Vector2(1.2F, 1.6F), "bl", "left");
			controllerStyle.AddButton1(new Vector2(-0.4F, 1F), new Vector2(0.6F, 0.6F), "br", "Jump", "Jump");
			controllerStyle.AddButton1(new Vector2(-1F, 0.4F), new Vector2(0.6F, 0.6F), "br", "Fire", "Fire");
			controllerStyle.AddButton1(new Vector2(0F, 0.4F), new Vector2(0.4F, 0.4F), "mc", "Use Powerup", "powerup");
			break;
		}
		return controllerStyle;
	}
}