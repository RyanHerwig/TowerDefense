using UnityEngine;

public class NullEffect : IEnemyEffect
{
    public const EnemyEffectName Name = EnemyEffectName.NullPhysical;

    private GameManager gameManager;
    private EnemyEffectData effectData;

    public void ApplyEffect(Enemy enemy, EnemyEffectData data)
    {
        gameManager = GameManager.Instance;
        effectData = data;

        //TODO - Add Immunity to Enemy
        Debug.Log("IMMUNITY APPLIED");
    }

    public float GetDamage()
    {
        return 0f;
    }

    public float GetDuration()
    {
        return 0f;
    }

    public EnemyEffectName GetName()
    {
        return Name;
    }

    public void RemoveEffect()
    {
        gameManager.EnqueueRemoveEnemyEffects(effectData);
    }

    public void SetDamage(float attackDamage = 0, float specialDamage = 0, float trueDamage = 0)
    {

    }

    public void SetDuration(float duration)
    {

    }

    public void TickEffect()
    {

    }

    public bool GetIsPermanent()
    {
        return true;
    }

    public bool GetIsDispellable()
    {
        return true;
    }

    public bool GetIsBuff()
    {
        return true;
    }
}