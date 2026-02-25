using UnityEngine;

public class BurnEffect : IEnemyEffect
{
    public const EnemyEffectName Name = EnemyEffectName.Burning;
    public float SpecialDamage;

    public float Duration;
    public float EffectRate;

    private float effectDelay;
    private GameManager gameManager;
    private Enemy target;
    private EnemyEffectData effectData;

    public BurnEffect(float specialDamage, float duration, float effectRate)
    {
        SpecialDamage = specialDamage;
        Duration = duration;
        EffectRate = effectRate;
    }

    public EnemyEffectName GetName()
    {
        return Name;
    }

    public void ApplyEffect(Enemy enemy, EnemyEffectData data)
    {
        gameManager = GameManager.Instance;
        effectData = data;
        target = enemy;
    }

    public void TickEffect()
    {
        if (effectDelay > 0)
        {
            effectDelay -= Time.deltaTime;
        }
        else
        {
            gameManager.EnqueueDamageData(new DamageData(target, 0, SpecialDamage, 0, target.Defense, target.SpecialDefense));

            effectDelay = 1f / EffectRate;
        }

        if (Duration <= 0)
            RemoveEffect();

        Duration -= Time.deltaTime;
    }

    public void RemoveEffect()
    {
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
        return SpecialDamage;
    }

    public void SetDamage(float attackDamage = 0, float specialDamage = 0, float trueDamage = 0)
    {
        SpecialDamage = specialDamage;
    }
}