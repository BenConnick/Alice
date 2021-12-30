using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PixelMorphFX : MonoBehaviour
{
    [SerializeField] private BWTexData allSprites;

    private PixelMorphRenderer morphRenderer;
    private readonly Dictionary<int, Dictionary<int, Morph>> morphCache = new Dictionary<int, Dictionary<int, Morph>>();

    public class Morph
    {
        public BWTexData.BWTexDataSingle Start;
        public BWTexData.BWTexDataSingle End;

        private List<(Color, BWTexData.int2[], BWTexData.int2[], BWTexData.int2[] midpoint)> combined;
        public List<(Color color, BWTexData.int2[] start, BWTexData.int2[] end, BWTexData.int2[] midpoint)> Combined => combined;

        public Morph(BWTexData.BWTexDataSingle start, BWTexData.BWTexDataSingle end)
        {
            Start = start;
            End = end;

            combined = new List<(Color, BWTexData.int2[], BWTexData.int2[], BWTexData.int2[] midpoint)>();
            foreach (var d in start.PixelData)
            {
                foreach (var d2 in end.PixelData)
                {
                    if (d2.Color.Equals(d.Color))
                    {
                        int numPositions = Mathf.Max(d.Positions.Length, d2.Positions.Length);
                        BWTexData.int2[] midpoints = new BWTexData.int2[numPositions];
                        for (int i = 0; i < numPositions; i++)
                        {
                            Vector2 midpointRandomOffset = Random.insideUnitCircle * 100;
                            midpoints[i] = new BWTexData.int2
                            {
                                x = (int)(midpointRandomOffset.x + Mathf.Lerp(GetCl(d.Positions, i).x, GetCl(d2.Positions, i).x, 0.5f)),
                                y = (int)(midpointRandomOffset.y + Mathf.Lerp(GetCl(d.Positions, i).y, GetCl(d2.Positions, i).y, 0.5f))
                            };
                        }
                        combined.Add((d.Color, d.Positions, d2.Positions, midpoints));
                        break;
                    }
                }
            }
        }

        public int GetNumPositions(int colorIndex)
        {
            return Mathf.Max(Combined[colorIndex].start.Length, Combined[colorIndex].end.Length);
        }

        public float GetInterpolatedX(int colorIndex, int i, float t)
        {
            if (t < 0.5f) return Mathf.Lerp(GetCl(Combined[colorIndex].start, i).x, GetCl(Combined[colorIndex].midpoint, i).x, t*2f);
            return Mathf.Lerp(GetCl(Combined[colorIndex].midpoint, i).x, GetCl(Combined[colorIndex].end, i).x, t * 2f - 1);
        }

        public float GetInterpolatedY(int colorIndex, int i, float t)
        {
            if (t < 0.5f) return Mathf.Lerp(GetCl(Combined[colorIndex].start, i).y, GetCl(Combined[colorIndex].midpoint, i).y, t * 2f);
            return Mathf.Lerp(GetCl(Combined[colorIndex].midpoint, i).y, GetCl(Combined[colorIndex].end, i).y, t * 2f - 1);
        }

        private BWTexData.int2 GetCl(BWTexData.int2[] arr, int i)
        {
            if (arr.Length == 0) return default;
            if (i < 0 || i > arr.Length) return arr[arr.Length - 1];
            return arr[i];
        }
    }

    private float t = 0;
    private void Awake()
    {
        var cams = FindObjectsOfType<Camera>();
        if (cams.Length == 0) return; // ERROR no cameras
        for (int i = 0; i < cams.Length; i++)
        {
            if (cams[i].tag == "GameplayCamera")
            {
                morphRenderer = new PixelMorphRenderer(gameObject, cams[i]);
                break;
            }
        }
        // fallback, should never happen
        morphRenderer = new PixelMorphRenderer(gameObject, cams[0]);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.K))
        {
            t += Time.deltaTime;
            if (t > 1) t = 1;
            Draw(0, 1, default, t);
        }
        else if (Input.GetKey(KeyCode.L))
        {
            t -= Time.deltaTime;
            if (t < 0) t = 0;
            Draw(0, 1, default, t);
        }
    }

    private void Draw(int index1, int index2, Vector2 pos, float interp = 0)
    {
        var start = allSprites.Images[index1];
        var end = allSprites.Images[index2];

        // merge the color data
        if (!morphCache.ContainsKey(index1))
        {
            morphCache[index1] = new Dictionary<int, Morph>();
        }
        if (!morphCache[index1].ContainsKey(index2))
        {
            morphCache[index1][index2] = new Morph(start, end);
        }
        Morph m = morphCache[index1][index2];

        // use the data in the scriptable object to prepare the list
        // of quads that the renderer can draw
        morphRenderer.BeginFrame();
        var ppu = start.Original.pixelsPerUnit;
        for (int c = 0; c < m.Combined.Count; c++)
        {
            for (int i = 0; i < m.GetNumPositions(c); i++)
            {
                int x = (int) m.GetInterpolatedX(c, i, interp);
                int y = (int)m.GetInterpolatedY(c, i, interp);
                Vector2 pixelOffset = new Vector2(x / ppu, y / ppu);
                Vector2 size = new Vector2(1 / ppu, 1 / ppu); // represents one pixel;
                morphRenderer.Draw(null, pixelOffset + pos, size, default, 0, 0, m.Combined[c].color, false, Vector2.one);
            }
        }
        morphRenderer.EndFrame();
    }

    private BWTexData.int2 GetCl(BWTexData.int2[] arr, int i)
    {
        if (arr.Length == 0) return default;
        if (i < 0 || i > arr.Length) return arr[arr.Length - 1];
        return arr[i];
    }
}
