using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BWTexData))]
public class BWTexDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        BWTexData texData = (BWTexData)target;
        if (GUILayout.Button("Recompute All")) ReserializeAll();
    }

    private void ReserializeAll()
    {
        var list = serializedObject.FindProperty("Images");

        for (int i = 0; i < list.arraySize; i++)
        {
            var entry = list.GetArrayElementAtIndex(i);
            var image = (Sprite)entry.FindPropertyRelative("Original").objectReferenceValue; // TODO test
            entry.FindPropertyRelative("Width").intValue = (int)image.rect.width;
            Serialize(image, entry.FindPropertyRelative("PixelData"));
        }

        if (serializedObject.hasModifiedProperties)
            serializedObject.ApplyModifiedProperties();
    }

    private void Serialize(Sprite image, SerializedProperty serializedPixels)
    {
        // Read the texture
        Texture2D readableTexture = image.texture;
        Color[] colors = readableTexture.GetPixels();
        Rect r = image.rect;
        var pixelGroups = new Dictionary<Color, List<BWTexData.int2>>();
        for (int i = 0; i < colors.Length; i++)
        {
            // do not include clear pixels
            if (colors[i] == default || colors[i].a < 0.1f) continue;

            // by cropping the image we can do this operation on sprites that 
            // are packed into an atlas / spritesheet without using the whole sheet
            int y = i / readableTexture.width;
            if (y < r.yMin || y >= r.yMax) continue;
            int x = i % readableTexture.width;
            if (x >= r.xMax || x < r.xMin) continue;

            // group by color
            // for now we only support black or white as colors
            Color blackOrWhite = colors[i].r < 0.5f ? Color.black : Color.white;
            if (!pixelGroups.ContainsKey(blackOrWhite)) {
                pixelGroups[blackOrWhite] = new List<BWTexData.int2>();
            }
            pixelGroups[blackOrWhite].Add(new BWTexData.int2 { x = x, y = y });
        }

        // serialize into groups by color
        serializedPixels.arraySize = pixelGroups.Count;
        int groupNum = 0;
        foreach (var kv in pixelGroups)
        {
            var el = serializedPixels.GetArrayElementAtIndex(groupNum);
            var colorProp = el.FindPropertyRelative("Color");
            colorProp.colorValue = kv.Key;
            var posArr = el.FindPropertyRelative("Positions");
            posArr.arraySize = kv.Value.Count;
            for (int j = 0; j < kv.Value.Count; j++)
            {
                BWTexData.int2 pos = kv.Value[j];
                posArr.GetArrayElementAtIndex(j).FindPropertyRelative("x").intValue = pos.x;
                posArr.GetArrayElementAtIndex(j).FindPropertyRelative("y").intValue = pos.y;
            }
            groupNum++;
        }
    }
}
