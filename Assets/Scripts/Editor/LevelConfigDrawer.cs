using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(LevelConfig))]
[Obsolete]
public class LevelConfigDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        int levelEnumRaw = property.FindPropertyRelative("LevelType").enumValueIndex;
        label.text = "" + levelEnumRaw + ": " + Enum.GetName(typeof(LevelType), (LevelType)levelEnumRaw);

        if (property.isExpanded)
        {
            EditorGUI.PropertyField(position, property, label, true); // TODO figure out how to draw this correctly
        }
        else
        {
            EditorGUI.PropertyField(position, property, label);
        }

        EditorGUI.EndProperty();

        // todo better rect
        if (GUI.Button(new Rect(position.x + position.width - 10, position.y, 12, 12), "+"))
        {
            Debug.Log("a button was pressed");
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent content)
    {
        const float labelHeight = 18;
        if (property.isExpanded)
        {
            return labelHeight * (1+property.CountInProperty());
        }

        return labelHeight;
    }
}