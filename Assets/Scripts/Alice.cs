using UnityEngine;

public class Alice : AliceCharacterMovement
{
    public enum SizeCategory
    {
        Default,
        Small,
        Large
    }

    public SizeCategory ShrinkStatus { get; private set; }
    public const float DefaultScale = 0.8f;

    private float targetScale = DefaultScale;

    // Start is called before the first frame update
    public virtual void Start()
    {
        
    }

    // Update is called once per frame
    public override void Update()
    {
        base.Update();
        float scale = Mathf.Lerp(transform.localScale.x, targetScale, Time.deltaTime);
        transform.localScale = new Vector3(scale, scale, scale);
    }

    public void OnShrink()
    {
        switch (ShrinkStatus)
        {
            case SizeCategory.Default:
                ChangeSizeTo(SizeCategory.Small); // shrink
                break;
            case SizeCategory.Small:
                break; // minimum, cannot shrink more
            case SizeCategory.Large:
                ChangeSizeTo(SizeCategory.Default); // shrink
                break;
        }
    }

    public void OnGrow()
    {
        switch (ShrinkStatus)
        {
            case SizeCategory.Default:
                ChangeSizeTo(SizeCategory.Large); // grow
                break;
            case SizeCategory.Small:
                ChangeSizeTo(SizeCategory.Default); // grow
                break;
            case SizeCategory.Large:
                break; // maximum
        }
    }

    private void ChangeSizeTo(SizeCategory newSize)
    {
        ShrinkStatus = newSize;
        float scale = 1;
        switch (newSize)
        {
            
            case SizeCategory.Default:
                scale = 1;
                WidthLanes = 2;
                CharacterWidth = 1.5f;
                break;
            case SizeCategory.Small:
                scale = 0.5f;
                WidthLanes = 1;
                CharacterWidth = 0.75f;
                break;
            case SizeCategory.Large:
                scale = 2;
                WidthLanes = 4;
                CharacterWidth = 3f;
                break;
        }
        targetScale = DefaultScale * scale;
    }

    public void OnStartLevelPressed()
    {
        GM.OnGameEvent(GM.NavigationEvent.StartButton);
        GetComponent<WorldButton>().Release();
    }

    public void BecomeButton()
    {
        GetComponent<WorldButton>().enabled = true;
    }
}
