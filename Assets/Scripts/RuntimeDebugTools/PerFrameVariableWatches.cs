﻿using System.Collections.Generic;
using UnityEngine;

public class PerFrameVariableWatches : MonoBehaviour
{
    private static PerFrameVariableWatches _instance;
    private static Dictionary<string, string> debugQuantities = new Dictionary<string, string>();
    private static GUIStyle fontStyle;

    [RuntimeInitializeOnLoadMethod]
    private static void InstantiateAutomatically()
    {
        var instance = new GameObject("PerFrameVariableWatches", typeof(PerFrameVariableWatches));
        _instance = instance.GetComponent<PerFrameVariableWatches>();
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (_instance == null)
            _instance = this; 
        else if (_instance != this)
            Destroy(gameObject);
    }

    public static void SetDebugQuantity(string key, string value)
    {
        if (debugQuantities.ContainsKey(key)) debugQuantities[key] = value;
        else debugQuantities.Add(key, value);
    }

    public static void ClearDebugQuantity(string key)
    {
        debugQuantities.Remove(key);
    }

#if UNITY_EDITOR
    private void OnGUI()
    {
        if (fontStyle == null) fontStyle = new GUIStyle
        {
            fontSize = Screen.height / 25,
            fontStyle = FontStyle.Bold,
            normal = new GUIStyleState
            {
                textColor = new Color(0, 0, 1, 1f)
            }
        };
        float labelHeight = fontStyle.fontSize*1.1f;
        float labelY = fontStyle.fontSize;
        foreach (var item in debugQuantities)
        {
            string debugString = item.Key + ": " + item.Value;
            GUI.Label(new Rect(fontStyle.fontSize, labelY, 1000, labelHeight), debugString, fontStyle);
            labelY += labelHeight;
        }
    }
#endif
}
