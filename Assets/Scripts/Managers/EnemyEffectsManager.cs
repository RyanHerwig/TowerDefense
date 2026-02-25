using UnityEngine;

public class EnemyEffectsManager : MonoBehaviour
{
    #region Singleton
    private static EnemyEffectsManager instance;

    public static EnemyEffectsManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindAnyObjectByType<EnemyEffectsManager>();
            return instance;
        }
    }
    #endregion

    public void GetEffect(EnemyEffectName effectName, Enemy target)
    {
        switch (effectName)
        {
            case EnemyEffectName.Burning:
            
            break;
            default:
            print($"ERROR: COULD NOT FIND '{effectName}'. CONSIDER ADDING '{effectName}' TO SWITCH STATEMENT");
            break;
        }
    }
}

public struct EnemyEffectData
{
    public IEnemyEffect Effect;
    public Enemy Target;

    public EnemyEffectData(IEnemyEffect effect, Enemy enemy)
    {
        Effect = effect;
        Target = enemy;
    }
}

public enum EnemyEffectName
{
    // *** TOWER DEBUFF EFFECTS ***
    Burning,

    // *** ENEMY BUFF EFFECTS ***

    // *** COMBO EFFECTS ***
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