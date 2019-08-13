using UnityEngine;
using System;
using System.Collections;
using System.Linq;

namespace Core
{
    public class Updater : SingletonSpawningMonoBehaviour<Updater>
    {
        public Action PreUpdater;
        public Action NormalUpdater;

        public Action FixedUpdater;
        public Action LateUpdater;

        public Action OnApplicationPausedListener;
        public Action OnApplicationQuitListener;

        protected override void Awake()
        {
            base.Awake();
        }

        void Update()
        {
            if (PreUpdater != null)
                PreUpdater();

            if (NormalUpdater != null)
                NormalUpdater();
        }

        void FixedUpdate()
        {
            if (FixedUpdater != null)
                FixedUpdater();
        }

        void LateUpdate()
        {
            if (LateUpdater != null)
                LateUpdater();
        }

        public void RunNextFrame(Action action)
        {
            if (action == null)
            {
                Debug.LogWarning("RunNextFrame called with null action!");
                return;
            }

            StartCoroutine(RunNextFrameInternal(action, 0));
        }

        public void WaitAndRun(float timeWait, Action action)
        {
            if (action == null)
            {
                Debug.LogWarning("WaitAndRun called with null action!");
                return;
            }

            StartCoroutine(RunNextFrameInternal(action, timeWait));
        }

        public void WaitOnAsyncOperation(AsyncOperation operation, Action onComplete)
        {
            if (operation == null)
            {
                Debug.LogWarning("WaitOnAsyncOperation called with null operation!");
                return;
            }

            if (onComplete == null)
            {
                Debug.LogWarning("WaitOnAsyncOperation called with null onComplete!");
                return;
            }

            StartCoroutine(WaitOnAsyncOperationInternal(operation, onComplete));
        }

        private System.Collections.IEnumerator RunNextFrameInternal(Action action, float timeWait)
        {
            if (timeWait <= 0)
            {
                yield return 0;
            }
            else
            {
                yield return new WaitForSeconds(timeWait);
            }

            action();
        }

        private System.Collections.IEnumerator WaitOnAsyncOperationInternal(AsyncOperation operation, Action onComplete)
        {
            while (!operation.isDone)
            {
                yield return 0;
            }

            onComplete();
        }

        private void OnApplicationPause(bool state)
        {
            if (OnApplicationPausedListener != null)
                OnApplicationPausedListener();
        }

        private void OnApplicationQuit()
        {
            if (OnApplicationQuitListener != null)
                OnApplicationQuitListener();
        }

    }
}
