using System.Collections.Generic;
using UnityEngine;

public class PixelAlignmentHelper : MonoBehaviour
{
    public readonly List<PixelAlignmentBehavior> pixelAlignmentBehaviors = new List<PixelAlignmentBehavior>();

    private static PixelAlignmentHelper inst;
    public static PixelAlignmentHelper Instance
    {
        get
        {
            if (inst == null)
                inst = new GameObject("PixelAlignmentHelper").AddComponent<PixelAlignmentHelper>();
            return inst;
        }
    }

    public static void Add(PixelAlignmentBehavior b)
    {
        if (!Instance.pixelAlignmentBehaviors.Contains(b))
            Instance.pixelAlignmentBehaviors.Add(b);
    }

    // this update must run before everything
    void Update()
    {
        // singleton
        if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // set all the inheritors back to their positions
        for (int i = pixelAlignmentBehaviors.Count-1; i >= 0; i--)
        {
            PixelAlignmentBehavior b = pixelAlignmentBehaviors[i];
            if (b == null)
            {
                pixelAlignmentBehaviors.RemoveAt(i);
                continue;
            }
            Transform t = b.transform;
            if (t == null)
            {
                pixelAlignmentBehaviors.RemoveAt(i);
                continue;
            }
            // set back to old position
            //if (b.gameObject == GM.FindComp<CharacterController>().gameObject) Debug.Log(b.transform.position.y);
            b.transform.position = b.PrevPos;
            //if (b.gameObject == GM.FindComp<CharacterController>().gameObject) Debug.Log(b.transform.position.y);
        }
    }
}
