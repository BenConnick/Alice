using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AnimatedMesh))]
public class AnimatedMeshEditor : Editor
{
    private int handleIndex = -1;

    // Custom in-scene UI for when ExampleScript
    // component is selected.
    public void OnSceneGUI()
    {
        var t = target as AnimatedMesh;


        Matrix4x4 localToWorld = t.transform.localToWorldMatrix;
        Matrix4x4 worldToLocal = t.transform.worldToLocalMatrix;

        
        {
            for (int i = 0; i < t.verts.Length; i++)
            {
                Vector3 pos = localToWorld.MultiplyPoint3x4(t.verts[i]);
                float size = HandleUtility.GetHandleSize(pos) * 0.05f;
                //Vector3 snap = Vector3.one * 0.5f;
                bool pressed = Handles.Button(pos, Quaternion.identity, size, size, Handles.DotHandleCap);
                if (pressed)
                {
                    handleIndex = i;
                    break;
                }
            }
        }
        if (handleIndex >= 0)
        {
            int i = handleIndex;
            {
                Vector3 pos = localToWorld.MultiplyPoint3x4(t.verts[i]);

                float size = HandleUtility.GetHandleSize(pos) * 0.05f;
                //Vector3 snap = Vector3.one * 0.5f;

                EditorGUI.BeginChangeCheck();

                Vector3 newTargetPosition = worldToLocal.MultiplyPoint(Handles.PositionHandle(pos, Quaternion.identity));
                if (EditorGUI.EndChangeCheck())
                {
                    t.verts[i] = newTargetPosition;
                    Undo.RecordObject(t, "Change Look At Target Position");
                    EditorUtility.SetDirty(t);
                }
            }
        }
    }
}
