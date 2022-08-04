using System.Collections;
using System.Collections.Generic;


namespace Core
{
    public class CameraSystem<eType> where eType : struct
    {
        public void AddCamera(eType type, UnityEngine.Camera camera)
        {
            Core.QLogger.Assert(!cameras.ContainsKey(type));
            cameras.Add(type, camera);
        }

        public void SwitchCamera(eType type)
        {
            Core.QLogger.Assert(cameras.ContainsKey(type));

            if ( activeType.HasValue && activeType.Value.Equals(type))
                return;

            if (activeType.HasValue && DoesCameraExist(activeType.Value))
                SwitchOff(activeType.Value);
            SwitchOn(type);
            activeType = type;
        }

        public void SwitchOff(eType type)
        {
            Core.QLogger.Assert(cameras.ContainsKey(type));
            cameras[type].enabled = false;
        }

        public void SwitchOn(eType type)
        {
            Core.QLogger.Assert(cameras.ContainsKey(type));
            cameras[type].enabled = true;
        }

        public bool DoesCameraExist(eType type)
        {
            return cameras.ContainsKey(type);
        }

        private eType? activeType = null;
        private Dictionary<eType, UnityEngine.Camera> cameras = new Dictionary<eType, UnityEngine.Camera>();
    }
}