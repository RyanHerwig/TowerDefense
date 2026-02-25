using UnityEngine;

public class TowerEffectsManager : MonoBehaviour
{
    #region Singleton
    private static TowerEffectsManager instance;

    public static TowerEffectsManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindAnyObjectByType<TowerEffectsManager>();
            return instance;
        }
    }
    #endregion

    public void GetEffect()
    {

    }
}

public struct TowerEffectData
{
    public IEnemyEffect Effect;
    public Tower Target;

    public TowerEffectData(IEnemyEffect effect, Tower enemy)
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