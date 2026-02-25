using UnityEngine;

public class Explosion : IEnemyEffect
{
    public const EnemyEffectName Name = EnemyEffectName.Explode;
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
        gameManager = GameManager.Instance;
        target = enemy;
        effectData = data;
    }

    public void TickEffect()
    {
        // Explode
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