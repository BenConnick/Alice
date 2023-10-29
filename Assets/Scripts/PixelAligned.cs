using UnityEngine;
using System.Collections;

public class PixelAligned : MonoBehaviour
{
    public Vector3 TruePosition;

    public float pixelsPerUnit;

    private int updatedFrame;

    public void SetWorldPosition(Vector3 pos)
    {
        transform.position = pos;
        TruePosition = transform.localPosition;
    }

    public Vector3 GetWorldPosition(Vector3 pos)
    {
        return transform.localToWorldMatrix * TruePosition;
    }

    public void SetLocalPosition(Vector3 pos)
    {
        TruePosition = pos;
        transform.localPosition = pos;
    }

    public Vector3 GetLocalPosition()
    {
        return TruePosition;
    }

    private void LateUpdate()
    {
        SetPositionToGrid();
    }

    public bool IsUpdated()
    {
        return Time.frameCount == updatedFrame;
    }

    public void SetPositionToGrid()
    {
        if (IsUpdated()) return;
        UpdateParentsRecursive(this);
    }

    private void SetPositionToGridInner()
    {
        Vector3 trueWorldPosition = transform.position;
        Vector3 alignedPosition = trueWorldPosition;
        alignedPosition.x = Mathf.Round(alignedPosition.x * pixelsPerUnit) / pixelsPerUnit;
        alignedPosition.y = Mathf.Round(alignedPosition.y * pixelsPerUnit) / pixelsPerUnit;
        updatedFrame = Time.frameCount;
    }

    private void UpdateParentsRecursive(PixelAligned target)
    {
        var parentObject = GetComponentInParent<PixelAligned>();
        if (parentObject != null)
        {
            UpdateParentsRecursive(parentObject);
            SetPositionToGridInner();
        }
    }
}
