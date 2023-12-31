using System;
using UnityEditor;
using UnityEngine;

//[CustomPropertyDrawer(typeof(LevelTextAsset))]
[Obsolete]
public class PerLevelInGameTextDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);
        {
            // set label to name
            if (property.objectReferenceValue != null)
            {
                SerializedObject innerObject = new SerializedObject(property.objectReferenceValue);
                SerializedProperty innerObjectData = innerObject.FindProperty("Data");
                label.text = innerObjectData.FindPropertyRelative("Name").stringValue;
                if (string.IsNullOrEmpty(label.text))
                {
                    label.text = "Unnamed";
                }
            }

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
    
            // Calculate rects
            var fieldRect = new Rect(position.x, position.y, position.width, position.height);
    
            // Draw fields - pass GUIContent.none to each so they are drawn without labels
            EditorGUI.PropertyField(fieldRect, property, GUIContent.none);
        }
        EditorGUI.EndProperty();
    }

    // public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    // {
    //     // base.OnGUI(position, property, label);
    //     
    //     // Using BeginProperty / EndProperty on the parent property means that
    //     // prefab override logic works on the entire property.
    //     EditorGUI.BeginProperty(position, label, property);
    //
    //     if (property.objectReferenceValue == null)
    //     {
    //         // Draw label
    //         position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
    //
    //         // Calculate rects
    //         var fieldRect = new Rect(position.x, position.y, position.width, position.height);
    //
    //         // Draw fields - pass GUIContent.none to each so they are drawn without labels
    //
    //         EditorGUI.PropertyField(fieldRect, property, GUIContent.none);
    //     }
    //     else
    //     {
    //         SerializedObject innerObject = new SerializedObject(property.objectReferenceValue);
    //         SerializedProperty innerObjectData = innerObject.FindProperty("Data");
    //         
    //         // Draw label
    //         label.text = innerObjectData.FindPropertyRelative("Name").stringValue;
    //         position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
    //         
    //         EditorGUI.BeginProperty(position, label, innerObjectData);
    //
    //         // Calculate rects
    //         var dataRect = new Rect(position.x, position.y, position.width, position.height * 5);
    //
    //         {
    //             
    //         }
    //
    //         // Draw fields - pass GUIContent.none to each so they are drawn without labels
    //         //EditorGUI.PropertyField(dataRect, innerObjectData, GUIContent.none);
    //         EditorGUI.EndProperty();
    //
    //         const float closeButtonSize = 20;
    //         Rect buttonRect = new Rect(position.x + position.width - closeButtonSize, position.y, 
    //             closeButtonSize, closeButtonSize);
    //         if (GUI.Button(buttonRect, "X"))
    //         {
    //             property.objectReferenceValue = null;
    //             property.serializedObject.ApplyModifiedProperties();
    //             property.serializedObject.Update();
    //         }
    //     }
    //     EditorGUI.EndProperty();
    // }

    // public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    // {
    //     if (property.objectReferenceValue != null)
    //     {
    //         return base.GetPropertyHeight(property, label) * 5;
    //     }
    //     return base.GetPropertyHeight(property, label);
    // }
}