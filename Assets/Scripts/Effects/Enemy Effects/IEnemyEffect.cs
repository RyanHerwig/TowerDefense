public interface IEnemyEffect
{
    EnemyEffectName GetName();

    /// <summary>
    /// The Initialization of the Effect
    /// 
    /// <para></para>
    /// Additionally, all Effects that happen Immediately
    /// </summary>
    /// <param name="enemy">The enemy that is effected</param>
    /// <param name="data">The effect data</param>
    void ApplyEffect(Enemy enemy, EnemyEffectData data);

    /// <summary>
    /// The Over Time Effects that happen
    /// 
    /// <para></para>
    /// Example: Burn Damage Gets Applied Over Time
    /// </summary>
    void TickEffect();

    /// <summary>
    /// The Removal of the Effect from the Game
    /// 
    /// <para></para>
    /// 
    /// Additionally, all Effects that happen at the end
    /// </summary>
    void RemoveEffect();
    float GetDuration();
    void SetDuration(float duration);
    float GetDamage();
    void SetDamage(float attackDamage = 0, float specialDamage = 0, float trueDamage = 0);

    /// <summary>
    /// Determines if the Effect has a set Duration
    /// <para>
    /// If True - Effect lasts indefinitely (unless Dispelled)
    /// </para>
    /// If False - Effect will be removed when its Duration runs out
    /// </para>
    /// </summary>
    /// <returns>Determines if the Effect is Permanent or not</returns>
    bool GetIsPermanent();

    /// <summary>
    /// Determines if Effect can be dispelled by another Effect
    /// <para>
    /// If Effect is a Buff - Determines if Player can dispel the buff
    /// </para>
    /// <para>
    /// If Effect is a Debuff - Determines if (another) Enemy can dispel the buff
    /// </para>
    /// </summary>
    /// <returns>If Effect can be Dispelled or not</returns>
    bool GetIsDispellable();

    /// <summary>
    /// Determines if the Effect is a Buff or a Debuff
    /// <para>
    /// If True - Effect is considered a Buff
    /// </para>
    /// <para>
    /// If False - Effect is considered a Debuff
    /// </para>
    /// </summary>
    /// <returns>If Effect is Considered a Buff or not</returns>
    bool GetIsBuff();
}