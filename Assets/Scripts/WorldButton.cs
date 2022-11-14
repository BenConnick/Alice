using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SpriteRenderer))]
public class WorldButton: MonoBehaviour
{
    public UnityEvent ClickAction; 

    private SpriteRenderer spriteRenderer;
    private bool hover;
    //private RabbitHoleDisplay owner;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        //owner = transform.GetComponentInParent<RabbitHole>().OwnerLink;
        //if (owner == null) Debug.LogError($"[Interactable] '{name}' Could not find required parent comp");
    }

    private void Update()
    {
        // while active
        if (ContextualInputSystem.Context?.isActiveAndEnabled ?? false)
        {
            // check for overlap
            var pos = ContextualInputSystem.ViewWorldCursorPos;
            bool newHover = ContainsPoint2D(pos);
            if (newHover != hover)
            {
                SetHover(newHover);
            }
            if (hover)
            {
                // click confirmed
                if (Input.GetMouseButtonUp(0))
                {
                    OnClicked();
                }
                // press and hold color feedback
                else if (Input.GetMouseButton(0))
                {
                    SetHover(false); // visual only
                }
            }
        }
    }

    public bool ContainsPoint2D(Vector3 worldPos)
    {
        return spriteRenderer.bounds.IntersectRay(new Ray(worldPos - Vector3.forward * 100, Vector3.forward));
    }

    //public void OnMouseEnter()
    //{
    //    // set to hover state
    //    SetHover(true);
    //}

    //public void OnMouseExit()
    //{
    //    // set back to default
    //    SetHover(false);
    //}

    public void SetHover(bool isHovering)
    {
        hover = isHovering;
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, hover ? 0.5f : 1);
    }

    public void OnClicked()
    {
        ClickAction?.Invoke();
    }

    public void Release()
    {
        SetHover(false);
        enabled = false;
    }
}
