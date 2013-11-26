using UnityEngine;
using System.Collections;

public class Dragon : MonoBehaviour {
	
	public int segmentCount = 2;
	private int segmentLag = 10;
	private float speed = 2;
	private float turningSpeed = 2;
	public float turningAngle = 0;
	public GameObject segmentObject;
	public bool is_attacking = false;
	public float attackDuration = 2;
	public float attackTime;
	
	private NetworkPlayer myPlayer;
	private int playerNumber;
	public string playerName;
	private string playerCharacter;
	private TextMesh playerCharacterMesh;
	private Color primary;
	private Color secondary;
	
	private Transform[] segments = new Transform[2];
	private Quaternion[] segmentRotations = new Quaternion[1000];
	private Vector3[] segmentPositions = new Vector3[1000];
	private Transform fire;
	private Transform rightEye;
	private Transform leftEye;
	private Transform rightGill;
	private Transform leftGill;
	private Transform snout;
	private Transform head;
	private Transform segment1;
	private int thisSegment;

	// Use this for initialization
	void Start () {
		head = transform.FindChild("Head");
		segment1 = transform.FindChild("Segment");
		fire = head.FindChild("Fire");
		rightEye = head.FindChild("RightEye");
		leftEye = head.FindChild("LeftEye");
		rightGill = head.FindChild("RightGill");
		leftGill = head.FindChild("LeftGill");
		snout = head.FindChild("Snout");
		segments = new Transform [2] {head, segment1};
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		head.Rotate(0,0,turningSpeed * turningAngle);
		head.rigidbody.velocity = head.up * speed;
		segmentPositions[thisSegment] = head.position;
		segmentRotations[thisSegment] = head.rotation;
		for(int i = 1; i < segmentCount; i++){
			if(thisSegment - i * segmentLag < 0){
				segments[i].position = segmentPositions[thisSegment - i * segmentLag + 1000];
				segments[i].rotation = segmentRotations[thisSegment - i * segmentLag + 1000];
			}
			else{
				segments[i].position = segmentPositions[thisSegment - i * segmentLag];
				segments[i].rotation = segmentRotations[thisSegment - i * segmentLag];
			}
		}
		if(Input.GetKeyDown("a")){
			AddSegment();
		}
		thisSegment++;
		if(thisSegment >= 1000){
			thisSegment=0;
		}
	}
	
	public void AddSegment(){
		segmentCount++;
		Transform [] newSegments = new Transform[segmentCount];
		newSegments[0] = head;
		newSegments[1] = segment1;
		for(int i = 2; i < segmentCount; i++){
			if(i == segmentCount - 1){
				GameObject newSegment = (GameObject) GameObject.Instantiate(segmentObject, Vector3.zero, Quaternion.identity);
				newSegments[i] = newSegment.transform;
				newSegment.transform.parent = transform;
				newSegment.transform.renderer.material.color = primary;
				newSegment.transform.FindChild("Character").GetComponent<TextMesh>().color = secondary;
				newSegment.transform.FindChild("Character").GetComponent<TextMesh>().text = playerCharacter;
			}
			else{
				newSegments[i] = segments[i];
			}
			if(thisSegment - i * segmentLag < 0){
				newSegments[i].position = segmentPositions[thisSegment - i * segmentLag + 1000];
				newSegments[i].rotation = segmentRotations[thisSegment - i * segmentLag + 1000];
			}
			else{
				newSegments[i].position = segmentPositions[thisSegment - i * segmentLag];
				newSegments[i].rotation = segmentRotations[thisSegment - i * segmentLag];
			}
		}
		segments = newSegments;
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
		head.renderer.material.color = primary;
		snout.renderer.material.color = primary;
		rightEye.particleSystem.startColor = secondary;
		leftEye.particleSystem.startColor = secondary;
		rightGill.particleSystem.startColor = secondary;
		leftGill.particleSystem.startColor = secondary;
		fire.particleSystem.startColor = secondary;
		head.FindChild("Character").GetComponent<TextMesh>().color = secondary;
		head.FindChild("Character").GetComponent<TextMesh>().text = playerCharacter;
		segment1.renderer.material.color = primary;
		segment1.FindChild("Character").GetComponent<TextMesh>().color = secondary;
		segment1.FindChild("Character").GetComponent<TextMesh>().text = playerCharacter;
	}
	
	public void Hit(){
		Debug.Log ("Hit");
		if(segmentCount > 2){
			segmentCount--;
			Destroy(segments[segmentCount].gameObject);
		}
	}
}
