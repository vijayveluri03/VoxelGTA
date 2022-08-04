using System.Collections;
using System.Collections.Generic;

namespace GTA
{
    // Register any kind of inputs coming through Scene or prefab
    public class RegisterInputs : UnityEngine.MonoBehaviour
    {
        const string INPUT_TAG = "Inputs";

        [System.Serializable]
        public class CameraInputs
        {
            public eCameraType type;
            public UnityEngine.Camera camera;
        }

        public List<CameraInputs> cameraInputs = new List<CameraInputs>();


        // Start is called before the first frame update
        public void RegisterCameraInputs( Core.CameraSystem<eCameraType> cameraSystem)
        {
            if (cameraInputs == null || cameraInputs.Count == 0)
                return;

            foreach( var input in cameraInputs)
            {
                cameraSystem.AddCamera(input.type, input.camera);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Destroy(bool includingParent)
        {
            UnityEngine.GameObject parent = this.gameObject;
            UnityEngine.Component.Destroy(this);
            if (includingParent)
                UnityEngine.GameObject.Destroy(parent);
        }

        public static RegisterInputs GetInputsFromScene()
        {
            UnityEngine.GameObject inputObject = UnityEngine.GameObject.FindGameObjectWithTag(INPUT_TAG);

            if (inputObject)
                return inputObject.GetComponent<RegisterInputs>();
            return null;
        }
    }
}