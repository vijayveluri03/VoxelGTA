﻿using UnityEditor;
using UnityEngine;
using Core;

[CustomPropertyDrawer(typeof(ResourceMap.Map))]
public class ResouceMapDrawer : PropertyDrawer 
{

	public override float GetPropertyHeight (SerializedProperty property, GUIContent label) {
		return label != GUIContent.none && Screen.width < 333 ? (16f + 18f) : 16f;
	}

	public override void OnGUI (Rect position, SerializedProperty property, GUIContent label) {
		int oldIndentLevel = EditorGUI.indentLevel;
		label = EditorGUI.BeginProperty(position, label, property);
		Rect contentPosition = EditorGUI.PrefixLabel(position, label);
		if (position.height > 16f) {
			position.height = 16f;
			EditorGUI.indentLevel += 1;
			contentPosition = EditorGUI.IndentedRect(position);
			contentPosition.y += 18f;
		}
		contentPosition.width *= 0.75f;
		EditorGUI.indentLevel = 0;

		SerializedProperty serObj = property.FindPropertyRelative("obj");
		UnityEngine.Object obj = serObj.objectReferenceValue;

		EditorGUI.PropertyField(contentPosition, serObj, GUIContent.none);
		contentPosition.x += contentPosition.width;
		contentPosition.width /= 3f;
		EditorGUIUtility.labelWidth = 14f;
		

		SerializedProperty pathObj = property.FindPropertyRelative("path");

		if ( obj != null )
		{
			pathObj.stringValue = AssetDatabase.GetAssetPath( obj );
			if ( !pathObj.stringValue.StartsWith ( "Assets/Content/") ) 
			{ 
				serObj.objectReferenceValue = null; 
				QLogger.LogError("Only assets from /Assets/Content/ should be added"); 
			}
			pathObj.stringValue = pathObj.stringValue.Substring( "Assets/Content/".Length  );	// removes Assets/Content/
			pathObj.stringValue = pathObj.stringValue.Substring(0, pathObj.stringValue.IndexOf('.') );	// removes file extentions
			EditorGUILayout.LabelField("Path:" + pathObj.stringValue );
		}
		else 
			pathObj.stringValue = "";
		
		EditorGUI.EndProperty();
		EditorGUI.indentLevel = oldIndentLevel;
	}
}