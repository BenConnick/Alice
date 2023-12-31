using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class TunnelBottomEdgeCamera : MonoBehaviour
{
    public float NormalHeight;
    public float FullSize;
    public float EdgeSize;
    public Camera AssociatedClearCamera;

    private Camera cam;
    private float z;

    // Start is called before the first frame update
    void Awake()
    {
        cam = GetComponent<Camera>();
        z = cam.transform.localPosition.z;
    }

    public void SetLevel(LevelType level)
    {
        switch (level)
        {
            case LevelType.GardenOfChange:
                cam.transform.localPosition = new Vector3(0, EdgeSize * NormalHeight - EdgeSize, z);
                cam.orthographicSize = NormalHeight * EdgeSize;
                cam.rect = new Rect(0, 0, 1, EdgeSize);
                break;
            default:
                cam.transform.localPosition = new Vector3(0, 0, z);
                cam.orthographicSize = NormalHeight * FullSize;
                cam.rect = new Rect(0, 0, 1, 1);
                break;
        }
        AssociatedClearCamera.orthographicSize = cam.orthographicSize;
        AssociatedClearCamera.transform.localPosition = cam.transform.localPosition;
        AssociatedClearCamera.rect = cam.rect;
    }
}
