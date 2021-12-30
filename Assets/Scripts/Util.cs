using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class Util
{
    public const int UNSET = -1;

    private static Canvas CachedCanvas;
    private static Camera CachedCamera;

    private static readonly Vector2 referenceRes = new Vector2(1920, 1080);
    public static Vector2 ScreenPosToCanvasPos(Vector2 screenPos, Transform canvasTransform)
    {
        var worldPos = GetScreenWorldPosition(screenPos, canvasTransform);
        var canvasPos = canvasTransform.InverseTransformPoint(worldPos);
        //Vector2 canv
        return canvasPos;
    }

    // extension method for TMP
    public static void SetValue(this TextMeshProUGUI label, int val, int total)
    {
        string colString;
        if (val >= total) colString = "\"black\"";
        else colString = colString = "\"red\"";
        label.text = $"<b><color={colString}>{val}</color></b><color=#888888>/{total}</color>";
    }

    public static void Clear(int size, int[,] arrayToClear)
    {
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                arrayToClear[i, j] = 0;
            }
        }
    }

    // list extension method
    public static T GetRandomElement<T>(this List<T> list)
    {
        return list[Random.Range(0, list.Count)];
    }

    public static T Get2D<T>(this T[] twoDimensionalArray, int x, int y, int width)
    {
        return twoDimensionalArray[width * x + y];
    }

    public static void Set2d(this int[] twoDimensionalArray, int x, int y, int value, int width)
    {
        twoDimensionalArray[width * x + y] = value;
    }

    public static int GetIndex2d(int x, int y, int width)
    {
        return width * x + y;
    }

    public static Vector3 CanvasToWorldPosition(Vector2 canvasPos)
    {
        if (CachedCamera == null)
        {
            CachedCamera = Camera.main;
            CachedCanvas = Object.FindObjectOfType<Canvas>();
        }

        Vector3 canvasPos3 = new Vector3(canvasPos.x, canvasPos.y, 0);

        return CachedCanvas.transform.TransformPoint(canvasPos3);
    }

    public static Vector3 GetScreenWorldPosition(Vector2 screenPosition, Transform canvasTransform)
    {
        if (CachedCamera == null)
        {
            CachedCamera = Camera.main;
        }

        Ray ray = CachedCamera.ScreenPointToRay(screenPosition);
        Plane canvasPlane = new Plane(canvasTransform.forward, canvasTransform.position);
        canvasPlane.Raycast(ray, out float enter);
        return ray.GetPoint(enter);
    }

    public static void Delay1Frame(System.Action action)
    {
        GM.StartCoroutine(Delay1FrameCoroutine(action));
    }

    private static IEnumerator Delay1FrameCoroutine(System.Action action)
    {
        yield return null;
        action?.Invoke();
    }

    private const int pixelsPerUnit = 32;
    public static Vector3 GetLowResPosition(Vector3 floatingPointPosition)
    {
        float GetResRounded(float val)
        {
            return Mathf.Floor(val * pixelsPerUnit) / pixelsPerUnit;
        }
        return new Vector3(
            GetResRounded(floatingPointPosition.x),
            GetResRounded(floatingPointPosition.y),
            GetResRounded(floatingPointPosition.z));
    }



    // finds the index closest to the specified height 
    // in a specially formatted array of positions
    // assumption: heights list is sorted such that heights[i].y >= heights[i+1].y 
    // (each succesive value is lower down)
    // the "greater" boolean allows the caller to decide if it wants to use 
    // the index greater than the value, or the index less than the value
    public static int FindNearestHeightIndex(Vector3[] heights, float heightTarget, bool greater, int startingIndex)
    {
        // prevent IOOB
        if (startingIndex < 0 || startingIndex >= heights.Length)
            return -1;

        // bounds check, if the target lower than the lowest point, return bottom
        if (heightTarget < heights[heights.Length - 1].y)
            return heights.Length - 1;

        // bounds check, if the target is higher than the highest point, return highest
        if (heightTarget > heights[startingIndex].y)
            return startingIndex;

        // binary search
        // we know at this point that there are at least some positions in range
        int minIdx = startingIndex;
        int maxIdx = heights.Length - 1;
        int probe;
        while (maxIdx - minIdx > 1)
        {
            probe = (maxIdx - minIdx) / 2 + minIdx;
            if (heightTarget > heights[probe].y)
            {
                maxIdx = probe;
            }
            else
            {
                minIdx = probe;
            }
        }

        return greater ? maxIdx : minIdx;
    }

    public interface ISpeedZone {
        float StartHeight { get; }
    }

    // DUPLICATED from Vector3 version
    public static int FindNearestHeightIndex(ISpeedZone[] heights, float heightTarget, bool greater, int startingIndex)
    {
        // prevent IOOB
        if (startingIndex < 0 || startingIndex >= heights.Length)
            return -1;

        // bounds check, if the target lower than the lowest point, return bottom
        if (heightTarget < heights[heights.Length - 1].StartHeight)
            return heights.Length - 1;

        // bounds check, if the target is higher than the highest point, return highest
        if (heightTarget > heights[startingIndex].StartHeight)
            return startingIndex;

        // binary search
        // we know at this point that there are at least some positions in range
        int minIdx = startingIndex;
        int maxIdx = heights.Length - 1;
        int probe;
        while (maxIdx - minIdx > 1)
        {
            probe = (maxIdx - minIdx) / 2 + minIdx;
            if (heightTarget > heights[probe].StartHeight)
            {
                maxIdx = probe;
            }
            else
            {
                minIdx = probe;
            }
        }

        return greater ? maxIdx : minIdx;
    }

    public static bool TryGet<T>(this T[] arr, int index, out T res)
    {
        if (index < 0 || index >= arr.Length)
        {
            res = default;
            return false;
        }
        res = arr[index];
        return true;
    }

    public static bool TryGet<T>(this List<T> list, int index, out T res)
    {
        if (index < 0 || index >= list.Count)
        {
            res = default;
            return false;
        }
        res = list[index];
        return true;
    }

    public static string FromZeroIndexed(this int zeroIndexedIndex)
    {
        return (zeroIndexedIndex + 1).ToString();
    }

    public static void DestroyAllChildren(this Transform t)
    {
        // destroy all children
        for (int i = t.childCount - 1; i >= 0; i--)
        {
            if (Application.isPlaying)
            {
                Object.Destroy(t.GetChild(i).gameObject);
            }
            else
            {
                Object.DestroyImmediate(t.GetChild(i).gameObject);
            }
        }
    }
}

public class Map<T1, T2>
{
    private Dictionary<T1, T2> _forward = new Dictionary<T1, T2>();
    private Dictionary<T2, T1> _reverse = new Dictionary<T2, T1>();

    public Map()
    {
        this.Forward = new Indexer<T1, T2>(_forward);
        this.Reverse = new Indexer<T2, T1>(_reverse);
    }

    public class Indexer<T3, T4>
    {
        private Dictionary<T3, T4> _dictionary;
        public Indexer(Dictionary<T3, T4> dictionary)
        {
            _dictionary = dictionary;
        }
        public T4 this[T3 index]
        {
            get { return _dictionary[index]; }
            set { _dictionary[index] = value; }
        }

        public Dictionary<T3, T4> GetDictionary()
        {
            return _dictionary;
        }
    }

    public void Add(T1 t1, T2 t2)
    {
        _forward.Add(t1, t2);
        _reverse.Add(t2, t1);
    }

    public Indexer<T1, T2> Forward { get; private set; }
    public Indexer<T2, T1> Reverse { get; private set; }

    public void Clear()
    {
        _forward.Clear();
        _reverse.Clear();
    }

    public bool ContainsKey(T1 key)
    {
        return _forward.ContainsKey(key);
    }

    public bool ContainsValue(T2 value)
    {
        return _reverse.ContainsKey(value);
    }

    public void Remove(T1 key)
    {
        _reverse.Remove(_forward[key]);
        _forward.Remove(key);
    }

    public void RemoveValue(T2 value)
    {
        _forward.Remove(_reverse[value]);
        _reverse.Remove(value);
    }

    public int Count => _forward.Count;
}