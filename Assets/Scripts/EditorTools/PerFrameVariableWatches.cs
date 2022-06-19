using System.Collections.Generic;
using UnityEngine;

public class PerFrameVariableWatches : MonoBehaviour
{
    private static PerFrameVariableWatches _instance;
    private static Dictionary<string, float> debugQuantities = new Dictionary<string, float>();
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

    public static void SetDebugQuantity(string key, float value)
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
            fontSize = 30,
            fontStyle = FontStyle.Bold,
            normal = new GUIStyleState
            {
                textColor = new Color(0, 0, 1, 1f)
            }
        };
        const float labelHeight = 30;
        float labelY = 10;
        foreach (var item in debugQuantities)
        {
            string debugString = item.Key + ": " + item.Value;
            GUI.Label(new Rect(100, labelY, 1000, labelHeight), debugString, fontStyle);
            labelY += labelHeight;
        }
    }
#endif
}
