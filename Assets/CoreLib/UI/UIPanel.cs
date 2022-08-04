using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Core
{
    public class UIPanel : MonoBehaviour
    {
        public virtual void DoUpdate() { }
        public virtual void OnPusedBack() { if (Core.QLogger.CanLogInfo) Core.QLogger.LogInfo("OnPushedBack called for " + this.GetType()); }
        public virtual void OnPushedToFront() { if (Core.QLogger.CanLogInfo) Core.QLogger.LogInfo("OnPushedToFront called for " + this.GetType()); }
        public virtual void OnExit() { if (Core.QLogger.CanLogInfo) Core.QLogger.LogInfo("OnExit called for " + this.GetType()); }
        public virtual void Reset() { if (Core.QLogger.CanLogInfo) Core.QLogger.LogInfo("Reset called for " + this.GetType()); }
    }
}