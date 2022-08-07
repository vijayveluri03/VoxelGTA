using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace GTA
{
    public class Application : MonoBehaviour
    {
        void Start()
        {
            CreateSharedObject();
            InitializeLogger();
            InitializeStateManager();
        }

        public void Update()
        {
            if (gameStateManager != null)
                gameStateManager.Update();

            if (uiManager != null)
                uiManager.DoUpdate();
            else
                FetchUIStateMachine();
        }

        private void InitializeLogger()
        {
            Core.QLogger.SetLoggingLevel(Core.QLogger.Level.Info);
        }
        private void InitializeStateManager()
        {
            gameStateManager = new GStateManager(sharedObjects);
            gameStateManager.PushNextState(GStateManager.eState.Loading);
        }
        private void FetchUIStateMachine()
        {
            uiManager = sharedObjects.TryFetch<Core.UIManager>(Constants.SOKeys.UIManager);
        }
        private void CreateSharedObject()
        {
            sharedObjects = new Core.SharedObjects<object>();
        }

        private Core.SharedObjects<System.Object> sharedObjects = null;
        private GStateManager gameStateManager;
        private Core.UIManager uiManager;

    }
}
