
using System;
using System.Collections.Generic;
using System.Reflection;
using Utils;
using UnityEngine;
using UnityEngine.SceneManagement;



namespace Core
{
	public class ResourceManager : Singleton<ResourceManager>
	{

		public ResourceManager()
		{

		}

		public T LoadAsset<T>(string path) where T : UnityEngine.Object
		{
			if (!isInitiated) Initialize();

			UnityEngine.Object obj = resouceMap.GetObjectAtPath(path);
			if (obj == null) Core.QLogger.LogErrorAndThrowException("Object not found for " + path);
			if ((obj as T) == null) Core.QLogger.LogErrorAndThrowException("Object casting failed " + path);

			return obj as T;
		}

		public void LoadLevel(string sceneFilePath, bool additive, Action completeAction, bool async)
		{
			if (!isInitiated) Initialize();
			Core.QLogger.LogInfo("Load scene " + sceneFilePath + " has begun");
			if (async)
			{
				AsyncOperation operation = SceneManager.LoadSceneAsync(sceneFilePath, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
				operation.completed += delegate (AsyncOperation o)
				{
					Core.QLogger.LogInfo("Load scene " + sceneFilePath + " has completed");
					if (completeAction != null) completeAction();
				};
			}
			else
			{
				SceneManager.LoadScene(sceneFilePath, additive ? LoadSceneMode.Additive : LoadSceneMode.Single);
				if (completeAction != null)
				{
					Core.QLogger.LogInfo("Load scene " + sceneFilePath + " has completed");
					completeAction();
				}
			}
		}

		public void UnloadLevel(string sceneFilePath, Action completeAction, bool async)
		{
			if (async)
			{
				Core.QLogger.LogInfo("Unload of scene " + sceneFilePath + " has begun");

				AsyncOperation operation = SceneManager.UnloadSceneAsync(sceneFilePath);
				operation.completed += delegate (AsyncOperation o)
				{
					Core.QLogger.LogInfo("Unload of scene " + sceneFilePath + " has completed");
					if (completeAction != null) completeAction();
				};
			}
			else
			{
				Core.QLogger.LogErrorAndThrowException("We only have async version for unload as direct one was made obselete by unity");
			}
		}

		private void Initialize()
		{
			resouceMap = Resources.Load<ResourceMap>("ResouceMap");
			isInitiated = true;
		}

		private ResourceMap resouceMap = null;
		private bool isInitiated = false;
	}
}