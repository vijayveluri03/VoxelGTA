using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiHomeScreen : UIPanel 
{
	public Action OnStartButtonPressedCallback;
	
	public void OnStartButtonClicked () { if ( OnStartButtonPressedCallback != null ) OnStartButtonPressedCallback(); }
}
