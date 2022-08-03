using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.IO;
using System.Linq;


namespace Core
{
	[CustomEditor(typeof(ResourceMap))]
	public class ResourceMapEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			ResourceMap myTarget = (ResourceMap)target;

			EditorList.Show(serializedObject.FindProperty("Mapping"), EditorListOption.All);

			EditorList.Show(serializedObject.FindProperty("FolderMapping"), EditorListOption.All);

			if (GUILayout.Button("ReImport"))
			{
				List<ResourceMap.Map> folderAssets = new List<ResourceMap.Map>();

				foreach (ResourceMap.Map map in myTarget.Mapping)
				{
					if (map.overridePath)
					{
                        //Debug.LogWarning(Application.dataPath + "/Content/" + map.path);
                        List<ResourceMap.Map> maps = GetAllAssetsAt ( map.path);
						if (maps != null && maps.Count > 0 )
						{
							folderAssets.AddRange ( maps );
							// foreach (UnityEngine.Object obj in objs)
							// {
							// 	string fullPath = AssetDatabase.GetAssetPath(obj);
							// 	if (fullPath.StartsWith("Assets/Content/"))
							// 	{
							// 		fullPath = fullPath.Substring("Assets/Content/".Length);  // removes Assets/Content/
							// 		fullPath = fullPath.Substring(0, fullPath.IndexOf('.'));   // removes file extentions

							// 		ResourceMap.Map newMap = new ResourceMap.Map ();
							// 		newMap.obj = obj;
							// 		newMap.path = fullPath;
							// 		newMap.isFolder = false;

							// 		folderAssets.Add (newMap);
							// 	}
							// }
						}
					}
				}

				// write it down in serialized property
                if ( folderAssets.Count > 0 )
                {
					SerializedProperty list = serializedObject.FindProperty ("FolderMapping");
					list.ClearArray ();

					for (int i = 0; i < folderAssets.Count; i++) 
					{
						list.InsertArrayElementAtIndex (i);
						SerializedProperty sr = list.GetArrayElementAtIndex (i);

						SerializedProperty pathSerObj = sr.FindPropertyRelative ("path");
						SerializedProperty objSerObj = sr.FindPropertyRelative ("obj");
						SerializedProperty isFolderSerObj = sr.FindPropertyRelative ("overridePath");

						pathSerObj.stringValue = folderAssets[i].path;
						objSerObj.objectReferenceValue = folderAssets[i].obj;
						isFolderSerObj.boolValue = folderAssets[i].overridePath;
					}

                    //myTarget.FolderMapping = folderAssets.ToArray();
                }

				//myTarget.FolderMapping = new ResourceMap.Map[];
			}

			serializedObject.ApplyModifiedProperties();
			// for ( int i = 0)

			// myTarget.experience = EditorGUILayout.ObjectField ("Resouce", myta)    ("Experience", myTarget.experience);
			// EditorGUILayout.LabelField("Level", myTarget.Level.ToString());
		}

        public static List<ResourceMap.Map> GetAllAssetsAt(string path)
		{
			
			if ( !Directory.Exists (UnityEngine.Application.dataPath + "/Content/" + path ) ) 
				return null;

			List<ResourceMap.Map> al = new List<ResourceMap.Map>();
			string[] fileEntries = Directory.GetFiles(UnityEngine.Application.dataPath + "/Content/" + path);
			foreach (string fileName in fileEntries)
			{
				ResourceMap.Map newMap = new ResourceMap.Map();

				int index = fileName.LastIndexOf("/");
				string fullPath = "Assets/Content/" + path;
				string localName = fileName.Substring(index);;
				string localPath = path + localName;
	
				fullPath += localName;

				Object t = AssetDatabase.LoadAssetAtPath(fullPath, typeof(System.Object));
				
				if (t != null)
				{
					newMap.obj = t;
					newMap.overridePath = false;
					newMap.path = localPath;
					newMap.path = newMap.path.Substring(0, newMap.path.IndexOf('.'));   // removes file extentions
					al.Add(newMap);
				}

				// Adding sprites seperately
				if ( t is Texture2D )
				{
					ResourceMap.Map newMapForSprites = new ResourceMap.Map();
					var objects = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(fullPath);
					var sprites = objects.Where(q => q is Sprite).Cast<Sprite>();
					foreach ( Sprite spr in sprites)
					{
						newMapForSprites.obj = spr;
						newMapForSprites.overridePath = true;	// this is needed as the sprite path should be different from its parent image;
						newMapForSprites.path = localPath;
						newMapForSprites.path = newMapForSprites.path.Substring(0, newMapForSprites.path.IndexOf('.'));   // removes file extentions
						newMapForSprites.path += "/" + spr.name;
						al.Add(newMapForSprites);
					}
				}
			}
			return al;
		}
	}

}
// [CustomEditor(typeof(ListTester)), CanEditMultipleObjects]
// public class ListTesterInspector : Editor {

// 	public override void OnInspectorGUI () {
// 		serializedObject.Update();
// 		EditorList.Show(serializedObject.FindProperty("integers"), EditorListOption.ListSize);
// 		EditorList.Show(serializedObject.FindProperty("vectors"));
// 		EditorList.Show(serializedObject.FindProperty("colorPoints"), EditorListOption.Buttons);
// 		EditorList.Show(serializedObject.FindProperty("objects"), EditorListOption.ListLabel | EditorListOption.Buttons);
// 		EditorList.Show(serializedObject.FindProperty("notAList"));
// 		serializedObject.ApplyModifiedProperties();
// 	}
// }
