using System;
using UnityEditor;
using UnityEngine;

// [CustomPropertyDrawer(typeof(LevelConfig))]
[Obsolete]
public class LevelConfigDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);
            
        EditorGUI.PropertyField(position, property, label);
            
        const float igtButtonSize = 20;
        Rect buttonRect = new Rect(position.x + position.width - igtButtonSize, position.y, 
            igtButtonSize, igtButtonSize);
            
        var containingArray = property.serializedObject.GetIterator();
        if (containingArray != null && GUI.Button(buttonRect, "IGT"))
        {
            bool success = false;
            var levels = Resources.Load<IGT>(IGT.EditorResourceName).Levels;
            int size = containingArray.arraySize;
            for (int i = 0; i < size; i++)
            {
                if (containingArray.GetArrayElementAtIndex(i) == property)
                {
                    if (i < levels.Length)
                    {
                        Selection.activeObject = levels[i];
                        success = true;
                    }
                    break;
                } 
            }

            if (success == false)
            {
                Debug.LogWarning("Failed to find matching asset");
            }
        }
            
        EditorGUI.EndProperty();
    }
}