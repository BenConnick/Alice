using UnityEngine;
using UnityEditor;

public class PixelEditorWindow : EditorWindow
{
    private static Texture2D inMemoryTexture;
    private static Texture2D assetReference;
    private bool firstShow = true;

    [MenuItem("Assets/Edit")]
    public static void ShowPixelEditor()
    {
        var selectedAsset = Selection.activeObject;
        Debug.Log(selectedAsset);
        if (selectedAsset is Texture2D t2d)
        {
            assetReference = t2d;
            var w = GetWindow<PixelEditorWindow>();
            w.firstShow = true;
            w.Show();
        }
    }

    private void OnGUI()
    {
        if (assetReference == null) return;
        if (firstShow)
        {
            firstShow = false;
            inMemoryTexture = new Texture2D(assetReference.width, assetReference.height, assetReference.format, false);
            inMemoryTexture.filterMode = FilterMode.Point;
            Graphics.CopyTexture(assetReference, inMemoryTexture);
        }
        const int scaleFactor = 10;
        Rect r = new Rect(0, 0, inMemoryTexture.width * scaleFactor, inMemoryTexture.height * scaleFactor);
        GUI.DrawTexture(r, inMemoryTexture);

        if (Event.current.isMouse && r.Contains(Event.current.mousePosition))
        {
            int x = Mathf.RoundToInt(Event.current.mousePosition.x - scaleFactor * .5f) / scaleFactor;
            int y = (inMemoryTexture.height - Mathf.RoundToInt(Event.current.mousePosition.y + scaleFactor * .5f) / scaleFactor);
            if (Event.current.button == 0 || Event.current.button == 1)
            {
                Debug.Log("click" + Event.current.mousePosition);
                Debug.Log(inMemoryTexture.GetPixel(x, y));
                inMemoryTexture.SetPixel(x, y, Event.current.button == 0 ? Color.white : Color.clear);
                inMemoryTexture.Apply();
                Debug.Log(inMemoryTexture.GetPixel(x, y));
                Repaint();
            }
        }

        // Save
        if (GUI.Button(new Rect(0, r.height, 200, 20), "Save")) {
            Graphics.CopyTexture(inMemoryTexture, assetReference);
            assetReference.Apply();
            new SerializedObject(assetReference).ApplyModifiedProperties();
            AssetDatabase.SaveAssets();
        }

        // Discard
        if (GUI.Button(new Rect(0, r.height+20, 200, 20), "Discard"))
        {
            inMemoryTexture = new Texture2D(assetReference.width, assetReference.height, assetReference.format, false);
            inMemoryTexture.filterMode = FilterMode.Point;
            Graphics.CopyTexture(assetReference, inMemoryTexture);
        }
    }
}
