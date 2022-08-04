using System;
using System.Collections;
using System.Collections.Generic;


namespace GTA
{
    public class UiHomeScreen : Core.UIPanel
    {
        public Action OnStartButtonPressedCallback;

        public void OnStartButtonClicked() { if (OnStartButtonPressedCallback != null) OnStartButtonPressedCallback(); }
    }
}