using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Core
{
    public class UIPanel : MonoBehaviour
    {
        public virtual void DoUpdate() { }
        public virtual void OnPusedBack() { Core.QLogger.LogInfo("OnPushedBack called for " + this.GetType()); }
        public virtual void OnPushedToFront() { Core.QLogger.LogInfo("OnPushedToFront called for " + this.GetType()); }
        public virtual void OnExit() { Core.QLogger.LogInfo("OnExit called for " + this.GetType()); }
        public virtual void Reset() { Core.QLogger.LogInfo("Reset called for " + this.GetType()); }
    }
}