using UnityEngine;

public class PixelAlignmentBehavior : MonoBehaviour
{
    [SerializeField] private float pixelsPerUnit;

    public Vector3 PrevPos;

    public void LateUpdate()
    {
        PixelAlignmentHelper.Add(this);
        //if (gameObject == GM.FindComp<CharacterController>().gameObject) Debug.Log(transform.position.y);
        PrevPos = transform.position;
        transform.position = new Vector3(
            Round(PrevPos.x),
            Round(PrevPos.y),
            Round(PrevPos.z));
        //if (gameObject == GM.FindComp<CharacterController>().gameObject) Debug.Log(transform.position.y);
    }

    private float Round(float unitPos)
    {
        return Mathf.Round(unitPos * pixelsPerUnit) / pixelsPerUnit;
    }
}
