
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class UnityInputSystem<ActionType>
    {
        public delegate void OnKeyPressedDelegate();
        public delegate void OnKeyReleasedDelegate();

        public UnityInputSystem()
        {
            Core.Updater.Instance.PreUpdater += Update;
        }

        ~UnityInputSystem()
        {
            Core.Updater.Instance.PreUpdater -= Update;
        }

        public void Init(Dictionary<ActionType, List<KeyCode>> keyMapping)
        {
            this.keyMapping = keyMapping;
        }

        public bool IsPressed(ActionType action)
        {
            if (actionStatus.ContainsKey(action))
                return actionStatus[action];
            return false;
        }

        public void RegisterPressEvent(ActionType action, OnKeyPressedDelegate listener)
        {
            {
                if (!keyPressedListeners.ContainsKey(action))
                    keyPressedListeners[action] = listener;
                else
                    keyPressedListeners[action] += listener;
            }
        }
        public void RegisterReleaseEvent(ActionType action, OnKeyReleasedDelegate listener)
        {
            {
                if (!keyReleasedListeners.ContainsKey(action))
                    keyReleasedListeners[action] = listener;
                else
                    keyReleasedListeners[action] += listener;
            }
        }

        public void UnRegisterPressEvent(ActionType action, OnKeyPressedDelegate listener)
        {
            {
                if (keyPressedListeners.ContainsKey(action))
                    keyPressedListeners[action] -= listener;
            }
        }
        public void UnRegisterReleaseEvent(ActionType action, OnKeyReleasedDelegate listener)
        {
            {
                if (keyReleasedListeners.ContainsKey(action))
                    keyReleasedListeners[action] = listener;
            }
        }

        void Update()
        {

            // REGISTERS ALL THE KEY PRESSES AND RELEASES IN THIS FRAME 
            // ALSO CACHES THE STATUS OF EACH ACTION 

            keysPressesInThisFrame.Clear();
            keysReleasesInThisFrame.Clear();

            foreach (var keymap in keyMapping)
            {
                bool newStatus = false;
                foreach (var key in keymap.Value)
                {
                    newStatus |= Input.GetKey(key);
                }

                bool oldStatus = actionStatus.ContainsKey(keymap.Key) ? actionStatus[keymap.Key] : false;

                if (!oldStatus && newStatus)
                {
                    if (!keysPressesInThisFrame.Contains(keymap.Key))
                        keysPressesInThisFrame.Add(keymap.Key);
                }
                else if (oldStatus && !newStatus)
                {
                    if (!keysReleasesInThisFrame.Contains(keymap.Key))
                        keysReleasesInThisFrame.Add(keymap.Key);
                }

                // We wouldnt need key held event. As it can be fetched from 'IsPressed' API

                actionStatus[keymap.Key] = newStatus;
            }

            // FIRES THE EVENTS TO THE REGISTED LISTENERS 
            // THIS STEP IS INTENTIONALLY DELAYED TILL THE END OF ALL THE KEYS BEING UPDATED. THIS IS TO PREVENT HALF BAKED STATUS VALUES

            foreach (var key in keysPressesInThisFrame)
            {
                FirePressedEvent(key);
            }
            foreach (var key in keysReleasesInThisFrame)
            {
                FireReleasedEvent(key);
            }
        }

        void FirePressedEvent(ActionType action)
        {
            if (keyPressedListeners.ContainsKey(action))
                keyPressedListeners[action].Invoke();
        }

        void FireReleasedEvent(ActionType action)
        {
            if (keyReleasedListeners.ContainsKey(action))
                keyReleasedListeners[action].Invoke();
        }

        private Dictionary<ActionType, List<KeyCode>> keyMapping = new Dictionary<ActionType, List<KeyCode>>();     // ACTION <> KEY(MULTIPLE) | MAPPING
        private Dictionary<ActionType, bool> actionStatus = new Dictionary<ActionType, bool>();                     // ACTION <> STATUS | MAPPING FOR EACH ACTION TO KNOW IF ITS PRESSED OR HELD

        // REGISTERED LISTENERS 
        private Dictionary<ActionType, OnKeyPressedDelegate> keyPressedListeners = new Dictionary<ActionType, OnKeyPressedDelegate>();
        private Dictionary<ActionType, OnKeyReleasedDelegate> keyReleasedListeners = new Dictionary<ActionType, OnKeyReleasedDelegate>();

        // CACHES THE CURRENT FRAME PRESSES AND RELEASES 
        private List<ActionType> keysPressesInThisFrame = new List<ActionType>();
        private List<ActionType> keysReleasesInThisFrame = new List<ActionType>();
    }
}