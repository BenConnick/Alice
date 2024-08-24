using UnityEngine;

public class AliceCharacter : AliceCharacterMovement
{
    public const string FallingAnimName = "falling";
    public const string StandingAnimName = "standing";

    public Vector3 DefaultPositionInMenu;

    // Inspector
    [SerializeField] private SpriteAnimator spriteAnimator;

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

    public void ActivateGameplayMode()
    {
        SetStanding(false);
    }

    public void ActivateMenuMode()
    {
        SetStanding(true);
        transform.localPosition = DefaultPositionInMenu;
    }

    public bool CheckOverlap(LevelCollider levelCollider)
    {
        if (!levelCollider.isActiveAndEnabled) return false;
        Vector3 pointToCheck = transform.position;
        if (levelCollider.HasTag(LevelCollider.Tag_MoneyOnHit))
        {
            const float moneyRadius = 0.5f;
            Vector3 toVec = levelCollider.transform.position - transform.position;
            pointToCheck += Vector3.ClampMagnitude(toVec, moneyRadius);
        }
        return levelCollider.OverlapPoint(pointToCheck);
    }

    private void SetStanding(bool standing)
    {
        IsHijacked = standing;
        string newAnimation = standing ? StandingAnimName : FallingAnimName;
        spriteAnimator.Validate();
        if (spriteAnimator.GetAnimation() != newAnimation)
            spriteAnimator.SetAnimation(newAnimation);
        if (standing)
            transform.localRotation = Quaternion.identity;
    }
}
