using UnityEngine;

public abstract class Effect
{
    /// <summary>
    /// Bool for if the Effect is a buff or a debuff
    /// </summary>
    public bool IsBuff;

    /// <summary>
    /// Bool for if Effect can expire naturally
    /// </summary>
    public bool IsPermanent;

    /// <summary>
    /// Bool for if Effect can be forcibly removed
    /// </summary>
    public bool IsDispellable;

    /// <summary>
    /// How much damage is applied per effect rate
    /// </summary>
    public float AttackDamage;
    public float SpecialDamage;
    public float TrueDamage;

    /// <summary>
    /// How long the effect lasts (if effect is not permenant)
    /// </summary>
    public float Duration;

    /// <summary>
    /// Bool for if the effect is flat or percentage based
    /// </summary>
    public bool IsPercentBased;

    public GameObject Origin;
}