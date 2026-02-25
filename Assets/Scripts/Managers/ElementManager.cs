using UnityEngine;

public class ElementManager : MonoBehaviour
{
    #region  Singleton
    private static ElementManager instance;

    public static ElementManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindAnyObjectByType<ElementManager>();
            return instance;
        }
    }
    #endregion

    public IEnemyEffect GetEffect(ElementName element1, ElementName element2)
    {
        switch((element1, element2))
        {
            case (ElementName.Fire, ElementName.Fire):
            return new Scorched();
            case (ElementName.Fire, ElementName.Ice):
            // Melt
            break;
            case (ElementName.Fire, ElementName.Water):
            // Vaporize
            break;
            case (ElementName.Fire, ElementName.Electric):
            // EXPLOSION
            break;
            case (ElementName.Ice, ElementName.Ice):
            // Brittle
            break;
            case (ElementName.Ice, ElementName.Water):
            // Frozen
            break;
            case (ElementName.Ice, ElementName.Electric):
            // Superconduct
            break;
            case (ElementName.Water, ElementName.Water):
            // Soaked
            break;
            case (ElementName.Water, ElementName.Electric):
            // Conductive
            break;
            case (ElementName.Electric, ElementName.Electric):
            // Overloaded
            break;
        }
        return null;
    }
}
/**
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
    **/