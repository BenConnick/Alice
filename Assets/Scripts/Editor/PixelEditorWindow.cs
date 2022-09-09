using UnityEngine;
using UnityEditor;
using System.IO;

public class PixelEditorWindow : EditorWindow
{
    private const bool DEBUG = false;
    private Texture2D assetReference;
    private Texture2D inMemoryTexture;
    private Texture2D overlayTexture;
    private bool firstShow = true;
    private int prevX;
    private int prevY;
    private Vector2 scrollPos;

    [MenuItem("Assets/Edit")]
    public static void ShowPixelEditor()
    {
        var selectedAsset = Selection.activeObject;
        DebugLog(selectedAsset);
        if (selectedAsset is Texture2D t2d)
        {
            var w = GetWindow<PixelEditorWindow>();
            w.assetReference = t2d;
            w.firstShow = true;
            w.Show();
        }
    }

    private void OnInspectorUpdate()
    {
        if (Selection.activeObject is Texture2D selectedTexture && assetReference != selectedTexture)
        {
            assetReference = selectedTexture;
            firstShow = true;
            Repaint();
        }
    }

    private void OnGUI()
    {
        if (assetReference == null) return;
        if (firstShow)
        {
            firstShow = false;
            inMemoryTexture = new Texture2D(assetReference.width, assetReference.height, TextureFormat.RGBA32, false);
            overlayTexture = new Texture2D(assetReference.width, assetReference.height, TextureFormat.RGBA32, false);
            overlayTexture.SetPixels(0, 0, assetReference.width, assetReference.height, new Color[assetReference.width * assetReference.height].Populate(Color.clear));
            overlayTexture.Apply();
            inMemoryTexture.filterMode = FilterMode.Point;
            overlayTexture.filterMode = FilterMode.Point;
            Graphics.CopyTexture(assetReference, inMemoryTexture);
        }
        const int scaleFactor = 10;
        Rect scrollAreaRect = new Rect(0, 0, position.width, position.height - 40);
        Rect viewportSize = new Rect(0, 0, inMemoryTexture.width * scaleFactor, inMemoryTexture.height * scaleFactor);
        scrollPos = GUI.BeginScrollView(scrollAreaRect, scrollPos, viewportSize);
        GUI.DrawTexture(viewportSize, inMemoryTexture);
        //var prev = GUI.color;
        GUI.color = new Color(1, 1, 1, 0.5f);
        GUI.DrawTexture(viewportSize, overlayTexture);
        GUI.color = new Color(1,1,1,1);

        int x = Mathf.CeilToInt((Event.current.mousePosition.x - scaleFactor) / scaleFactor);
        int y = Mathf.CeilToInt((inMemoryTexture.height * scaleFactor - Event.current.mousePosition.y - scaleFactor*.75f) / scaleFactor);
        if (viewportSize.Contains(Event.current.mousePosition))
        {
            if (Event.current.isMouse && Event.current.button == 0 || Event.current.button == 1)
            {
                DebugLog("click" + Event.current.mousePosition);
                DebugLog(inMemoryTexture.GetPixel(x, y));
                inMemoryTexture.SetPixel(x, y, Event.current.button == 0 ? Color.white : Color.clear);
                inMemoryTexture.Apply();
                DebugLog(inMemoryTexture.GetPixel(x, y));
                Repaint();
            }
            else
            {
                overlayTexture.SetPixel(prevX, prevY, Color.clear);
                prevX = x;
                prevY = y;
                overlayTexture.SetPixel(x, y, new Color(1,1,0,1f));
                overlayTexture.Apply();
                Repaint();
            }
        }
        GUI.EndScrollView();
        GUI.color = new Color(1, 1, 1, 1);

        // mini map
        GUI.color = new Color(0, 0, 0, 0.2f);
        Rect miniMap = new Rect(position.width - inMemoryTexture.width, 0, inMemoryTexture.width, inMemoryTexture.height);
        GUI.Box(miniMap, "");
        GUI.color = new Color(1, 1, 1, 0.8f);
        GUI.DrawTexture(miniMap, inMemoryTexture);

        // Save
        if (GUI.Button(new Rect(0, scrollAreaRect.height, 200, 20), "Save")) {
            //Graphics.CopyTexture(inMemoryTexture, assetReference);
            //assetReference.Apply();
            //new SerializedObject(assetReference).ApplyModifiedProperties();
            //AssetDatabase.SaveAssets();
            string path = AssetDatabase.GetAssetPath(assetReference);
            if (!string.IsNullOrEmpty(path))
            {
                File.WriteAllBytes(path, inMemoryTexture.EncodeToPNG());
                AssetDatabase.ImportAsset(path);
                AssetDatabase.Refresh();
                Debug.Log("Saved '" + path + "'");
            }
            else
            {
                Debug.LogError("Save failed. Path error in asset reference: " + path);
            }
        }

        // Discard
        if (GUI.Button(new Rect(220, scrollAreaRect.height, 200, 20), "Discard"))
        {
            inMemoryTexture = new Texture2D(assetReference.width, assetReference.height, assetReference.format, false);
            inMemoryTexture.filterMode = FilterMode.Point;
            Graphics.CopyTexture(assetReference, inMemoryTexture);
        }
    }

    private static void DebugLog(System.Object log)
    {
        if (DEBUG) Debug.Log(log);
    }
}