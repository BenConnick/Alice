public class ShrinkBehavior
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

    public static void GetSizeValues(AliceCharacter.SizeCategory sizeCategory, out float scale)
    {
        scale = 1;
        switch (sizeCategory)
        {
            
            case AliceCharacter.SizeCategory.Default:
                scale = 1;
                break;
            case AliceCharacter.SizeCategory.Small:
                scale = 0.5f;
                break;
            case AliceCharacter.SizeCategory.Large:
                scale = 2;
                break;
        }
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
        
    }
}