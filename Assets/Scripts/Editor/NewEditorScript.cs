﻿using UnityEngine;
using UnityEditor;

public class NewEditorScript : ScriptableObject
{
    [MenuItem("Assets/MyTool/Do It in C#")]
    static void DoIt()
    {
        EditorUtility.DisplayDialog("MyTool", "Do It in C# !", "OK", "");
    }
}
