using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class StartButtonBehavior : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Animator LinkedAnimator;

    public void OnPointerEnter(PointerEventData eventData)
    {
        LinkedAnimator.SetBool("ShouldShow", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        LinkedAnimator.SetBool("ShouldShow", false);
    }

    private void OnEnable()
    {

    }

    public void OnPressed()
    {
        GM.OnGameEvent(GM.NavigationEvent.StartButton);
    }
}
