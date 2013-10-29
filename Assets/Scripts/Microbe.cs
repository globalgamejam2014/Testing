using UnityEngine;
using System.Collections;

public class Microbe : MonoBehaviour {
	
	public float area;
	private float radius;
	public Vector2 movement;
	
	private NetworkPlayer myPlayer;
	private int playerNumber;
	public string playerName;
	private string playerCharacter;
	private TextMesh playerCharacterMesh;
	private Color primary;
	private Color secondary;
	
	private Transform outerWall;

	// Use this for initialization
	void Start () {
		outerWall = transform.FindChild("Outline");
		playerCharacterMesh = transform.FindChild("PlayerChar").GetComponent<TextMesh>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		rigidbody.velocity = new Vector3(movement.x, movement.y, 0);
	}
	
	public void SetMyPlayer (int player, Color primaryColor, Color secondaryColor, string newPlayerName){
		myPlayer = NetworkManager.playerList[player];
		playerNumber = player;
		primary = primaryColor;
		secondary = secondaryColor;
		playerName = newPlayerName;
		if(newPlayerName.Length>0){
			playerCharacter = newPlayerName[0].ToString();
		}
		else{
			playerCharacter = "";
		}
		renderer.material.color = primary;
		outerWall.renderer.material.color = secondary;
		playerCharacterMesh.color = secondary;
		playerCharacterMesh.text = playerCharacter;
	}
}
