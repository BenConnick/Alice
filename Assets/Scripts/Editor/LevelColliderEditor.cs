using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LevelCollider))]
public class LevelColliderEditor : Editor
{
    private static readonly string[] TagNames = { "Damage", "Shrink", "Grow" };

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var c = target as LevelCollider;

        if (c.Collider == null)
        {
            var colProp = serializedObject.FindProperty("Collider");
            colProp.objectReferenceValue = c.GetComponent<Collider2D>();
            serializedObject.ApplyModifiedProperties();
        }
        
        GUILayout.BeginHorizontal();
        for (int i = 0; i < 8; i++)
        {
            bool prevState = (c.Tags & (0x1 << i)) > 0;
            bool toggled = GUILayout.Toggle(prevState,TagNames.TryGet(i, out string n) ? n : "<N/A>");
            if (toggled != prevState)
            {
                var tagsProp = serializedObject.FindProperty("Tags");
                if (toggled)
                {
                    tagsProp.intValue |= (byte)(0x1 << i);
                }
                else
                {
                    tagsProp.intValue &= ~(byte)(0x1 << i);
                }
                serializedObject.ApplyModifiedProperties();
            }
        }
        GUILayout.EndHorizontal();
    }
}
