public interface IEnemyEffect
{
    EnemyEffectName GetName();
    void ApplyEffect(Enemy enemy, EnemyEffectData data);
    void TickEffect();
    void RemoveEffect();

    float GetDuration();
    void SetDuration(float duration);
    float GetDamage();
    void SetDamage(float attackDamage = 0, float specialDamage = 0, float trueDamage = 0);
}