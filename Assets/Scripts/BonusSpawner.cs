using UnityEngine;
using System.Collections;

public enum BonusType{
	Speed,
	Immunity,
	Rampage,
	Strength,
	Range,
	None
}

public class BonusSpawner : MonoBehaviour {
	
	public BonusType bonusType = BonusType.None;
	public Transform bonusBox;
	
	// Use this for initialization
	void Start () {
		bonusBox = transform.FindChild("BonusBox");
	}
	
	// Update is called once per frame
	void Update () {
		if(bonusType != BonusType.None){
			bonusBox.renderer.enabled = true;
			bonusBox.collider.enabled = true;
			bonusBox.localPosition = new Vector3(0, 0, Mathf.Sin(Time.time)/3);
			bonusBox.Rotate(new Vector3(0, 0, 2));
			switch(bonusType){
			case BonusType.Speed:
				bonusBox.renderer.material.color = Color.yellow;
				break;
			case BonusType.Immunity:
				bonusBox.renderer.material.color = Color.white;
				break;
			case BonusType.Rampage:
				bonusBox.renderer.material.color = Color.red;
				break;
			case BonusType.Strength:
				bonusBox.renderer.material.color = Color.green;
				break;
			case BonusType.Range:
				bonusBox.renderer.material.color = Color.blue;
				break;
			}
		}
		else{
			bonusBox.renderer.enabled = false;
			bonusBox.collider.enabled = false;
		}
	}
}
