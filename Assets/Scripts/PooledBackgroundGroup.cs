using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PooledBackgroundGroup : MonoBehaviour
{
    [Header("Instantiated GameObject Pool, must be children of this transform")]
    public Transform[] Tiles;

    [Header("Height of one GameObject")]
    public float ChunkHeight;

    //[Header("Optional Global Pooling Origin")]
    //public float GlobalY;

    private float globalOriginY;
    private int cursorIndex;
    private int prevIndex;

    // Start is called before the first frame update
    void Start()
    {
        globalOriginY = transform.position.y;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // shift tiles up or down such that the middle is centered in the viewport
        float newY = -transform.position.y;

        cursorIndex = Mathf.RoundToInt(newY / ChunkHeight);
        if (prevIndex != cursorIndex)
        {
            int midpoint = Mathf.CeilToInt(Tiles.Length / 2f);
            for (int i = 0; i < Tiles.Length; i++)
            {
                Transform tile = Tiles[i];
                tile.transform.SetLocalY((i + cursorIndex - midpoint) * ChunkHeight);
            }
            prevIndex = cursorIndex;
        }
    }
}
