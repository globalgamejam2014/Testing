using UnityEngine;
using System.Collections;

public class Accelerometer : MonoBehaviour {
	
	private Jovios jovios;
	
	void Start(){
		jovios = MenuManager.jovios;
	}
	
	void Update(){
		if(jovios.GetPlayer(0) != null){
			if(jovios.GetPlayer(0).GetInput("accelerometer") != null){
				transform.localRotation = jovios.GetPlayer(0).GetInput("accelerometer").GetGyro();
				transform.localPosition = jovios.GetPlayer(0).GetInput("accelerometer").GetAcceleration();
			}
		}
	}
}
