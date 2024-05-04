using System;
using UnityEngine;
using UnityEngine.Serialization;

public class PixelAlignmentBehavior : MonoBehaviour
{
    [SerializeField] private float pixelsPerUnit;

    [NonSerialized]
    public Vector3 PrevPos;
    [NonSerialized]
    public Vector3 RoundedPos;

    public void LateUpdate()
    {
        PixelAlignmentHelper.Add(this);
        //if (gameObject == GM.FindComp<CharacterController>().gameObject) Debug.Log(transform.position.y);
        PrevPos = transform.position;
        RoundedPos = new Vector3(
            Round(PrevPos.x),
            Round(PrevPos.y),
            Round(PrevPos.z));
        transform.position = RoundedPos;
        //if (gameObject == GM.FindComp<CharacterController>().gameObject) Debug.Log(transform.position.y);
    }

    private float Round(float unitPos)
    {
        return Mathf.Round(unitPos * pixelsPerUnit) / pixelsPerUnit;
    }

    public void ForceUpdate()
    {
        PrevPos = transform.position;
    }
}
