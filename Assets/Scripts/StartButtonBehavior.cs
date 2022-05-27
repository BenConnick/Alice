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
        GetComponent<Button>().interactable = true;
    }

    public void OnPressed()
    {
        GetComponent<Button>().interactable = false;
    }

#if UNITY_EDITOR
    public void Update()
    {
        if (Input.GetKey(KeyCode.Alpha1))
        {
            GM.OnGameEvent(GM.NavigationEvent.SkipIntroDialogue);
        } 
        else if (Input.GetKey(KeyCode.Alpha2))
        {
            GM.OnGameEvent(GM.NavigationEvent.StartLevel);
        }
    }

#endif
}
