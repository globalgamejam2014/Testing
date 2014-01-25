#pragma strict


var s_baseOrthographicSize : float;



function Start () {

// set the camera to the correct orthographic size (so scene pixels are 1:1)
s_baseOrthographicSize = Screen.height / 64.0f / 2.0f;
Camera.main.orthographicSize = s_baseOrthographicSize;

}

function Update () {

}



