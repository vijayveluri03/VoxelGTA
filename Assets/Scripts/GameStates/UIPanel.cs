using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

public class UIPanel : MonoBehaviour 
{
	public virtual void DoUpdate () {}
	public virtual void OnPusedBack() { if( QLogger.CanLogInfo) QLogger.LogInfo ("OnPushedBack called for " + this.GetType()); }
	public virtual void OnPushedToFront() { if( QLogger.CanLogInfo) QLogger.LogInfo ("OnPushedToFront called for " + this.GetType()); }
	public virtual void OnExit() { if( QLogger.CanLogInfo) QLogger.LogInfo ("OnExit called for " + this.GetType()); }
	public virtual void Reset () { if( QLogger.CanLogInfo) QLogger.LogInfo ("Reset called for " + this.GetType()); }
}
