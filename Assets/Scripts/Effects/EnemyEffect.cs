
using UnityEngine;

public class EnemyEffect : Effect
{
    /// <summary>
    /// The name of the Effect Applied
    /// </summary>
    public EnemyEffectName Name;

    /// <summary>
    /// How often the effect happens per second
    /// </summary>
    public float EffectDelay;

    /// <summary>
    /// How much the effect reduces defense and resistance by
    /// </summary>
    public float Shred;

    public float EffectRate;
    public float Id;
    private int percentIntIdentifier;

    public EnemyEffect(EnemyEffectName name, bool isBuff, float duration, GameObject origin, float attackDamage = 0, float specialDamage = 0,
        float trueDamage = 0, float shred = 0, bool isPermanent = false, bool isDispellable = true, float effectRate = 0, bool isPercentBased = false)
    {
        Name = name;
        Origin = origin;
        IsBuff = isBuff;
        AttackDamage = attackDamage;
        SpecialDamage = specialDamage;
        TrueDamage = trueDamage;
        Shred = shred;
        Duration = duration;
        IsPermanent = isPermanent;
        IsDispellable = isDispellable;
        EffectDelay = effectRate;
        EffectRate = effectRate;
        IsPercentBased = isPercentBased;
        percentIntIdentifier = IsPercentBased ? 1 : 2;
        Id = (Shred * 100000) + (AttackDamage * 100) + SpecialDamage + (TrueDamage / 100) + (percentIntIdentifier / 1000);
    }
}

public struct EnemyEffectData
{
    public EnemyEffect Effect;
    public Enemy Target;

    public EnemyEffectData(EnemyEffect effect, Enemy enemy)
    {
        Effect = effect;
        Target = enemy;
    }
}

public enum EnemyEffectName
{
    // *** TOWER DEBUFF EFFECTS ***
    Fire, // Damage Over Time
    Ice, // Debuffs
    Water, // Crowd Control
    Electric, // Damage
    Burning

    // *** ENEMY BUFF EFFECTS ***
}

public enum ComboEffectName
{
    Scorched, // Fire + Fire - Deals DoT
    Melt, // Fire + Ice - Shreds Armor and Resistance
    Vaporize, // Fire + Water - Deals damage and knocks back enemy
    Explode, // Fire + Electric - Causes mini Explosion centered on unit and small DoT in surrounding area
    Brittle, // Ice + Ice - Target takes true damage for duration of Brittle
    Frozen, // Ice + Water - Stuns target, target has slightly reduced armor and resistance
    Superconduct, // Ice + Electric - Damage is applied twice, after a set duration and at reduced damage - Subject to Rename
    Soaked, // Water + Water - Removes all Dispellable Buffs from target
    Conductive, // Water + Electric - Slows and damages target, enemies that touch target also become conductive 
    Overloaded // Electric + Electric - Deals current health percent true damage
}