using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class MouseNameEntryWidget : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler
{
    private bool isMouseOver;
    public bool IsMouseOver => isMouseOver;
    public RectTransform RectTransform;
    public System.Action<PointerEventData> PointerClickAction;

    public void OnPointerClick(PointerEventData eventData)
    {
        // if is over letter, add the letter to the word
        PointerClickAction?.Invoke(eventData);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // start detecting mouse pointer over letters
        isMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // stop detecting mouse pointer over letters
        isMouseOver = false;
    }

    public void OnPointerUp(PointerEventData eventData) { } // required for click IPointerClick

    public void OnPointerDown(PointerEventData eventData) { } // required for click IPointerClick
}
