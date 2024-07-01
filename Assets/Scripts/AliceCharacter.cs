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

    public void HandleObstacleCollision(LevelCollider obstacle)
    {
        var player = this;

        if (obstacle.HasTag(LevelCollider.Tag_DamageOnHit))
        {
            // invert the collider
            var inv = obstacle.gameObject.AddComponent<InvertSpriteTintBehavior>();
            inv.Initialize();

            // flash the collider
            //var flashing = obstacle.gameObject.AddComponent<FlashingBehavior>();
            //flashing.flashOffTime = 0.07f;
            //flashing.StartFlashing();

            // bump up the removal time (if applicable)
            var destroyer = obstacle.gameObject.GetComponent<DestroyAfterTimeBehavior>();
            if (destroyer != null) destroyer.SecondsUntilDestruction = Mathf.Min(destroyer.SecondsUntilDestruction, 2);

            // shake, flash, subtract lives
            gameContext.Viewport.InvertColor(0.01f);
            TimeDistortionController.PlayImpactFrame(.9f);
            gameContext.Viewport.GameplayCamera.GetComponent<GameplayInnerDisplayCamera>().Shake(); // DISABLED FOR EDITING
            player.StartFlashing();
            SubtractLife();
        }
        if (obstacle.HasTag(LevelCollider.Tag_GrowOnHit))
        {
            player.GetComponent<ShrinkBehavior>().OnGrow();
        }
        if (obstacle.HasTag(LevelCollider.Tag_ShrinkOnHit))
        {
            player.GetComponent<ShrinkBehavior>().OnShrink();
        }
        if (obstacle.HasTag(LevelCollider.Tag_MoneyOnHit))
        {
            int plusOne = ApplicationLifetime.GetPlayerData().Money.Value + 1;
            ApplicationLifetime.GetPlayerData().Money.Set(plusOne);
            obstacle.gameObject.SetActive(false);
            // TODO spawn collection celebration VFX
        }
    }

    private void SubtractLife()
    {
        // per the current design, lives and score are NOT global,
        // this is different from a normal game
        // the game has multiple contexts, with different lives and score
        // late in the game, the player can switch between these contexts
        // and in doing so can effectively gain or lose lives
        // this may eventually switch to a global context if that idea doesn't end up in the game
        if (gameContext == null) return;
        gameContext.VpLives--;
        if (gameContext.VpLives <= 0)
        {
            GameplayManager.Fire(GlobalGameEvent.AllLivesLost);
        }
    }

    private void SetStanding(bool standing)
    {
        string newAnimation = standing ? StandingAnimName : FallingAnimName;
        spriteAnimator.Validate();
        if (spriteAnimator.GetAnimation() != newAnimation)
            spriteAnimator.SetAnimation(newAnimation);
        if (standing)
            transform.localRotation = Quaternion.identity;
    }
}
