using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System;

public class JoviosControllerStyle{
	private Dictionary<string, JoviosControllerAreaStyle> areaStyles = new Dictionary<string, JoviosControllerAreaStyle>();
	public JoviosControllerAreaStyle GetAreaStyle(string side){
		return areaStyles[side.ToLower()];
	}
	private JoviosControllerOverallStyle overallStyle;
	public JoviosControllerOverallStyle GetOverallStyle(){
		return overallStyle;
	}
	public void AddRelativeJoystick(string sideRightOrLeft, string description, string response = ""){
		areaStyles.Add(sideRightOrLeft.ToLower(), new JoviosControllerAreaStyle().RelativeJoystick(sideRightOrLeft.ToLower(), description, response));
		is_splitScreen = true;
	}
	public void AddRelativeDPad(string sideRightOrLeft, string description, string response = ""){
		areaStyles.Add(sideRightOrLeft.ToLower(), new JoviosControllerAreaStyle().RelativeDPad(sideRightOrLeft.ToLower(), description, response));
		is_splitScreen = true;
	}
	public void AddRelativeDiagonalDPad(string sideRightOrLeft, string description, string response = ""){
		areaStyles.Add(sideRightOrLeft.ToLower(), new JoviosControllerAreaStyle().RelativeDiagonalDPad(sideRightOrLeft.ToLower(), description, response));
		is_splitScreen = true;
	}
	public void AddButton1(string sideRightOrLeft, string description, string response){
		JoviosControllerAreaStyle areaStyle = new JoviosControllerAreaStyle().Button1(sideRightOrLeft.ToLower(), description, response);
		areaStyles.Add(sideRightOrLeft.ToLower(), areaStyle);
		is_splitScreen = true;
	}
	public void AddButton2(string sideRightOrLeft, string[] description_2, string[] response_2){
		areaStyles.Add(sideRightOrLeft.ToLower(), new JoviosControllerAreaStyle().Button2(sideRightOrLeft.ToLower(), description_2, response_2));
		is_splitScreen = true;
	}
	public void AddCardinalSwipes(string sideRightOrLeft, string[] description_4, string[] response_4){
		areaStyles.Add(sideRightOrLeft.ToLower(), new JoviosControllerAreaStyle().CardinalSwipes(sideRightOrLeft.ToLower(), description_4, response_4));
		is_splitScreen = true;
	}
	public void AddAllTouches(string sideRightOrLeft, string description, string response = ""){
		areaStyles.Add(sideRightOrLeft.ToLower(), new JoviosControllerAreaStyle().AllTouches(sideRightOrLeft.ToLower(), description, response));
		is_splitScreen = true;
	}
	
	
	public void SetBasicButtons(string question, string[] setResponses, string submitButton = ""){
		overallStyle = new JoviosControllerOverallStyle().BasicButtons(question, setResponses, submitButton);
		is_splitScreen = false;
	}
	public void SetSingleButtons(string question, string[] setResponses, string submitButton = ""){
		overallStyle = new JoviosControllerOverallStyle().SingleButtons(question, setResponses, submitButton);
		is_splitScreen = false;
	}
	public void SetMultiButtons(string question, string[] setResponses, string submitButton = ""){
		overallStyle = new JoviosControllerOverallStyle().MultiButtons(question, setResponses, submitButton);
		is_splitScreen = false;
	}
	public void SetTextInput(string question, string submitButton = ""){
		overallStyle = new JoviosControllerOverallStyle().TextInput(question, submitButton);
		is_splitScreen = false;
	}
	public void SetNumericInput(string question, string submitButton = ""){
		overallStyle = new JoviosControllerOverallStyle().NumericInput(question, submitButton);
		is_splitScreen = false;
	}
	
	
	private JoviosControllerAccelerometerStyle accelerometerStyle;
	public void SetAccelerometerStyle(JoviosControllerAccelerometerStyle setAccelerometerStyle){
		accelerometerStyle = setAccelerometerStyle;
	}
	public JoviosControllerAccelerometerStyle GetAccelerometerStyle(){
		return accelerometerStyle;
	}
	private bool is_splitScreen;
	public bool IsSplitScreen(){
		return is_splitScreen;
	}
}