using System;

public class JoviosControllerOverallStyle{
	public JoviosControllerOverallStyle BasicButtons(string question, string[] setResponses, string submitButton = ""){
		JoviosControllerOverallStyle overallStyle = new JoviosControllerOverallStyle();
		overallStyle.questionPrompt = question;
		overallStyle.responses = new string[8] {"","","","","","","",""};
		for(int i = 0; i < setResponses.Length; i++){
			overallStyle.responses = setResponses;
		}
		overallStyle.submit = submitButton;
		overallStyle.type = "BasicButtons";
		return overallStyle;
	}
	public JoviosControllerOverallStyle SingleButtons(string question, string[] setResponses, string submitButton = ""){
		JoviosControllerOverallStyle overallStyle = new JoviosControllerOverallStyle();
		overallStyle.questionPrompt = question;
		overallStyle.responses = new string[8] {"","","","","","","",""};
		for(int i = 0; i < setResponses.Length; i++){
			overallStyle.responses = setResponses;
		}
		overallStyle.submit = submitButton;
		overallStyle.type = "SingleButtons";
		return overallStyle;
	}
	public JoviosControllerOverallStyle MultiButtons(string question, string[] setResponses, string submitButton = ""){
		JoviosControllerOverallStyle overallStyle = new JoviosControllerOverallStyle();
		overallStyle.questionPrompt = question;
		overallStyle.responses = new string[8] {"","","","","","","",""};
		for(int i = 0; i < setResponses.Length; i++){
			overallStyle.responses = setResponses;
		}
		overallStyle.submit = submitButton;
		overallStyle.type = "MultiButtons";
		return overallStyle;
	}
	public JoviosControllerOverallStyle TextInput(string question, string submitButton = ""){
		JoviosControllerOverallStyle overallStyle = new JoviosControllerOverallStyle();
		overallStyle.questionPrompt = question;
		overallStyle.submit = submitButton;
		overallStyle.type = "TextInput";
		return overallStyle;
	}
	public JoviosControllerOverallStyle NumericInput(string question, string submitButton = ""){
		JoviosControllerOverallStyle overallStyle = new JoviosControllerOverallStyle();
		overallStyle.questionPrompt = question;
		overallStyle.submit = submitButton;
		overallStyle.type = "NumericInput";
		return overallStyle;
	}
	private string type;
	public string GetOverallType(){
		return type;
	}
	private string questionPrompt;
	public string GetQuestionPrompt(){
		return questionPrompt;
	}
	private string[] responses = new string[8];
	public string[] GetResponses(){
		return responses;
	}
	public string GetResponse(int responseNumber){
		if(responseNumber < responses.Length){
			return responses[responseNumber];
		}
		else{
			return "";
		}
	}
	private string submit;
	public string GetSubmit(){
		return submit;
	}
}