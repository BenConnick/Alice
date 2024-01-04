using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Events;

[RequireComponent(typeof(TextMeshProUGUI))]
public class ButtonLabel : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Color normalColor = Color.white;
    public Color hoverColor = new Color(1,1,1,0.4f);
    public UnityEvent clickAction;
    public UnityEvent hoverAction;
    public UnityEvent hoverExitAction;

    private TextMeshProUGUI label;
    private bool hover;

    private void Awake()
    {
        label = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        label.color = hover ? hoverColor : normalColor;
    }

    public void OnPointerDown(PointerEventData eventData)
    {

    }

    public void OnPointerUp(PointerEventData eventData)
    {

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        clickAction?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hover = true;
        hoverAction?.Invoke();
        UpdateUI();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hover = false;
        hoverExitAction?.Invoke();
        UpdateUI();
    }
}
