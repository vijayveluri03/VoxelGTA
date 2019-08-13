using UnityEngine;
using System.Collections;
using UnityEditor;
using Core;

[CustomEditor(typeof(ResourceMap))]
public class ResourceMapEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        ResourceMap myTarget = (ResourceMap)target;
        
		EditorList.Show(serializedObject.FindProperty("Mapping"), EditorListOption.All);

		serializedObject.ApplyModifiedProperties();
		// for ( int i = 0)

        // myTarget.experience = EditorGUILayout.ObjectField ("Resouce", myta)    ("Experience", myTarget.experience);
        // EditorGUILayout.LabelField("Level", myTarget.Level.ToString());
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
