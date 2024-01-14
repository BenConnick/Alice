using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCursorFollow : MonoBehaviour
{
    Canvas context;

    // Start is called before the first frame update
    void Start()
    {
        context = gameObject.GetComponentInParent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mouse = Input.mousePosition;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(context.transform as RectTransform, mouse, context.worldCamera, out Vector2 localPoint);
        transform.localPosition = localPoint;
    }
}
