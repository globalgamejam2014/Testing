using System;

public class JoviosControllerAreaStyle{
	public JoviosControllerAreaStyle RelativeJoystick(string nSide, string nDescription, string nResponse = ""){
		JoviosControllerAreaStyle areaStyle = new JoviosControllerAreaStyle();
		areaStyle.side = nSide;
		areaStyle.description = new string[] {nDescription};
		areaStyle.response = new string[] {nResponse};
		areaStyle.type = "RelativeJoystick";
		return areaStyle;
	}
	public JoviosControllerAreaStyle RelativeDPad(string nSide, string nDescription, string nResponse = ""){
		JoviosControllerAreaStyle areaStyle = new JoviosControllerAreaStyle();
		areaStyle.side = nSide;
		areaStyle.description = new string[] {nDescription};
		areaStyle.response = new string[] {nResponse};
		areaStyle.type = "RelativeDPad";
		return areaStyle;
	}
	public JoviosControllerAreaStyle RelativeDiagonalDPad(string nSide, string nDescription, string nResponse = ""){
		JoviosControllerAreaStyle areaStyle = new JoviosControllerAreaStyle();
		areaStyle.side = nSide;
		areaStyle.description = new string[] {nDescription};
		areaStyle.response = new string[] {nResponse};
		areaStyle.type = "RelativeDiagonalDPad";
		return areaStyle;
	}
	public JoviosControllerAreaStyle Button1(string nSide, string nDescription, string nResponse){
		JoviosControllerAreaStyle areaStyle = new JoviosControllerAreaStyle();
		areaStyle.side = nSide;
		areaStyle.description = new string[] {nDescription};
		areaStyle.response = new string[] {nResponse};
		areaStyle.type = "Button1";
		return areaStyle;
	}
	public JoviosControllerAreaStyle Button2(string nSide, string[] nDescription, string[] nResponse){
		JoviosControllerAreaStyle areaStyle = new JoviosControllerAreaStyle();
		areaStyle.description = new string[2];
		areaStyle.response = new string[2];
		for(int i = 0; i < 2; i++){
			if(nDescription.Length > i){
				areaStyle.description[i] = nDescription[i];
			}
			else{
				areaStyle.description[i] = "";
			}
			if(nResponse.Length > i){
				areaStyle.response[i] = nResponse[i];
			}
			else{
				areaStyle.response[i] = "";
			}
		}
		areaStyle.side = nSide;
		areaStyle.type = "Button2";
		return areaStyle;
	}
	public JoviosControllerAreaStyle CardinalSwipes(string nSide, string[] nDescription, string[] nResponse){
		JoviosControllerAreaStyle areaStyle = new JoviosControllerAreaStyle();
		areaStyle.description = new string[4];
		areaStyle.response = new string[4];
		for(int i = 0; i < 4; i++){
			if(nDescription.Length >= i){
				areaStyle.description[i] = nDescription[i];
			}
			else{
				areaStyle.description[i] = "";
			}
			if(nResponse.Length >= i){
				areaStyle.response[i] = nResponse[i];
			}
			else{
				areaStyle.response[i] = "";
			}
		}
		areaStyle.side = nSide;
		areaStyle.type = "CardinalSwipes";
		return areaStyle;
	}
	public JoviosControllerAreaStyle AllTouches(string nSide, string nDescription, string nResponse = ""){
		JoviosControllerAreaStyle areaStyle = new JoviosControllerAreaStyle();
		areaStyle.side = nSide;
		areaStyle.description = new string[] {nDescription};
		areaStyle.response = new string[] {nResponse};
		areaStyle.type = "AllTouches";
		return areaStyle;
	}
	private string type;
	public string GetAreaType(){
		return type;
	}
	private string side;
	public string GetSide(){
		return side;
	}
	private string[] description;
	public string[] GetDescription(){
		return description;
	}
	private string[] response;
	public string[] GetResponse(){
		return response;
	}
}