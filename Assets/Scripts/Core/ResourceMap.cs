using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


namespace Core
{
	public class ResourceMap : ScriptableObject
	{

		// Creates a new scriptable Object file
		[MenuItem("Assets/Create/ResouceMap ScriptableObject")]
		public static ResourceMap Create()
		{
			ResourceMap asset = ScriptableObject.CreateInstance<ResourceMap>();
			AssetDatabase.CreateAsset(asset, "Assets/Resources/ResouceMap.asset");
			AssetDatabase.SaveAssets();
			return asset;
		}

		[System.Serializable]
		public class Map
		{
			public UnityEngine.Object obj;
			public string path;
			public bool overridePath;
		}

		[SerializeField]
		public Map[] Mapping;

		[SerializeField]
		public Map[] FolderMapping;


		public UnityEngine.Object GetObjectAtPath(string path)
		{
			if (Mapping == null)
				QLogger.LogErrorAndThrowException("Mapping is empty");
			foreach (Map map in Mapping)
			{
				if (map.path == path)
					return map.obj;
			}
			foreach (Map map in FolderMapping)
			{
				if (map.path == path)
					return map.obj;
			}
			return null;
		}
	}
}