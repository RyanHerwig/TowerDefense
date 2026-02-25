using UnityEngine;

public class Conductive : IEnemyEffect
{
    public const EnemyEffectName Name = EnemyEffectName.Conductive;
    public float AttackDamage;
    public float SpecialDamage;
    public float TrueDamage;
    public float Duration;

    public GameManager gameManager;
    private Enemy target;
    private EnemyEffectData effectData;

    public EnemyEffectName GetName()
    {
        return Name;
    }

    public void ApplyEffect(Enemy enemy, EnemyEffectData data)
    {
        target = enemy;
        gameManager = GameManager.Instance;
        effectData = data;
    }

    public void TickEffect()
    {
        // Conductive
    }

    public void RemoveEffect()
    {
        target.CurrentElementCooldown = 5f;
        gameManager.EnqueueRemoveEnemyEffects(effectData);
    }

    public float GetDuration()
    {
        return Duration;
    }

    public void SetDuration(float duration)
    {
        Duration = duration;
    }

    public float GetDamage()
    {
        return AttackDamage + SpecialDamage + TrueDamage;
    }

    public void SetDamage(float attackDamage = 0, float specialDamage = 0, float trueDamage = 0)
    {
        AttackDamage = attackDamage;
        SpecialDamage = specialDamage;
        TrueDamage = trueDamage;
    }
}