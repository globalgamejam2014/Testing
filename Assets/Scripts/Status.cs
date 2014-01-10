using UnityEngine;
using System.Collections;

public class Status : MonoBehaviour {

	public int playerNumber;
	public NetworkPlayer myPlayer;
	public Color primary;
	public Color secondary;
	public string playerName;
	public string playerCharacter;
	public Transform checkMark;
	public Transform xMark;
	public Transform crown;
	public GameObject playerObject;
	public TextMesh character;
	public TextMesh score;
	public bool is_ready = false;
	public int chosenArena;
		
	private Transform body;

	// Use this for initialization
	void Start () {
		transform.parent = GameObject.Find ("PlayerStatus").transform;
		if(playerNumber < 4){
			transform.localPosition = new Vector3(-4.5F + (playerNumber -1) * 4, -1.75F, 0);
		}
		else{
			transform.localPosition = new Vector3(-4.5F + (playerNumber -5) * 4, -3F, 0);
		}
		transform.localRotation = Quaternion.identity;
		body = transform.FindChild("Primary");
		score = transform.FindChild("Score").GetComponent<TextMesh>();
		crown = transform.FindChild("Crown");
		xMark = transform.Find("X");
		checkMark = transform.Find("Check");
		xMark.renderer.enabled = true;
		checkMark.renderer.enabled = false;
		crown.renderer.enabled = false;
		myPlayer = Jovios.players[Jovios.players.Length - 1].networkPlayer;
		playerNumber = Jovios.players.Length - 1;
		Jovios.SentBasicButtons("Join Game", "Ready to Play?", myPlayer);
		score.text = "";
	}
	
	public void SetMyPlayer (Player playerInfo){
		myPlayer = playerInfo.networkPlayer;
		playerNumber = playerInfo.playerNumber;
		primary = playerInfo.primary;
		secondary = playerInfo.secondary;
		playerName = playerInfo.playerName;
		if(playerInfo.playerName.Length>0){
			playerCharacter = playerInfo.playerName[0].ToString();
		}
		else{
			playerCharacter = "";
		}
		body = transform.FindChild("Primary");
		character = transform.FindChild("Character").GetComponent<TextMesh>();
		character.color = secondary;
		character.text = playerCharacter;
		body.renderer.material.color = primary;
	}
	
	private void OnButton(string button){
		switch(button){
		case "Join Game":
			xMark.renderer.enabled = false;
			checkMark.renderer.enabled = true;
			if(MenuManager.gameState == GameState.Countdown){
				Jovios.SentControls(myPlayer, 0, "Move Character", 0, "Move for Direction\nRelease to Fire");
			}
			else{
				Jovios.SentControls(myPlayer, 0, "Move Character", 3, "SelectLevel");
			}
			if(Jovios.players[playerNumber].playerObject == null){
				GameObject newPlayerObject = (GameObject) GameObject.Instantiate(playerObject, new Vector3(0,-4,0.5F), Quaternion.identity);
				newPlayerObject.transform.RotateAround(Vector3.zero, Vector3.forward, 360 - 360 / (playerNumber + 1) * Jovios.players.Length);
				newPlayerObject.transform.Rotate(new Vector3(0, 0, - 360 + 360 / (playerNumber + 1) * Jovios.players.Length));
				newPlayerObject.transform.parent = GameObject.Find ("PlayerObjects").transform;
				newPlayerObject.SendMessage("SetMyPlayer", Jovios.players[playerNumber], SendMessageOptions.DontRequireReceiver);
				Jovios.players[playerNumber].playerObject = newPlayerObject;
			}
			is_ready = true;
			break;
			
		case "Play Again!":
			if(MenuManager.gameState != GameState.ChooseArena && MenuManager.gameState != GameState.Countdown){
				GameManager.EndRound();
			}
			OnButton("Join Game");
			break;
			
		case "Jovios--right--press--A":
			GameManager.ChooseArena(chosenArena);
			break;
			
		default:
			Debug.Log (button);
			break;
		}
	}
	
	public void Ready(){
		
	}
	
	public void StartRound(){
		
	}
}
