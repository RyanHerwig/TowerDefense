using UnityEngine;

public class TowerEffect : Effect
{
    /// <summary>
    /// The name of the Effect Applied
    /// </summary>
    public TowerEffectName Name;
    public float FireRate;
    private int percentIntIdentifier;
    public float Id;
    public TowerEffect(TowerEffectName name, bool isBuff, float duration, float attackDamage = 0, float specialDamage = 0,
        float trueDamage = 0, float fireRate = 0, bool isPermanent = false, bool isDispellable = true, bool isPercentBased = false)
    {
        Name = name;
        IsBuff = isBuff;
        Duration = duration;
        AttackDamage = attackDamage;
        SpecialDamage = specialDamage;
        TrueDamage = trueDamage;
        FireRate = fireRate;
        IsPermanent = isPermanent;
        IsDispellable = isDispellable;
        IsPercentBased = isPercentBased;

        percentIntIdentifier = IsPercentBased ? 1 : 2;
        Id = (fireRate * 100000) + (AttackDamage * 100) + SpecialDamage + (TrueDamage / 100) + (percentIntIdentifier / 1000);
    }
}

public struct TowerEffectData
{
    public EnemyEffect Effect;
    public Tower Target;

    public TowerEffectData(EnemyEffect effect, Tower enemy)
    {
        Effect = effect;
        Target = enemy;
    }
}

public enum TowerEffectName
{
    // *** TOWER BUFF EFFECTS ***

    // *** ENEMY DEBUFF EFFECTS ***
}