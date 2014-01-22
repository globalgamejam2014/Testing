using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour, IJoviosPlayerListener, IJoviosControllerListener{
	
	public static Jovios jovios;
	
	void Start(){
		jovios = Jovios.Create();
		jovios.AddPlayerListener(this);
	}
	
	void OnGUI(){
		GUI.Box(new Rect(0,0,100,50), jovios.GetGameName());
		if(GUI.Button(new Rect(0,50,100,50), "Start Server")){
			jovios.StartServer();
		}
	}
	
	bool IJoviosPlayerListener.PlayerConnected(JoviosPlayer p){
		Debug.Log (p.GetPlayerName());
		JoviosControllerStyle controllerStyle = new JoviosControllerStyle(JoviosControllerOverallStyle.BasicButtons, "What are you doing", new string[] {"this", "that", "whiffle ball bat"}, "go");
		//JoviosControllerStyle controllerStyle = new JoviosControllerStyle(JoviosControllerAreaStyle.Joystick, "One", JoviosControllerAreaStyle.Joystick, "Two");		
		controllerStyle.SetAccelerometerStyle(JoviosControllerAccelerometerStyle.Full);
		jovios.SetControls(p.GetUserID(), controllerStyle);
		jovios.AddControllerListener(this);
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
		Debug.Log (e.GetResponse());
		Debug.Log (e.GetControllerStyle().GetQuestionPrompt());
		return false;
	}
}
