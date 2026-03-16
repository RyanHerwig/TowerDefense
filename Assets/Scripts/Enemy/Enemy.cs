using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq.Expressions;

public abstract class Enemy : MonoBehaviour
{
    [Header("Basic Stats")]

    [Tooltip("Total amount of starting HP the enemy has")]
    [SerializeField] float maxHealth;

    [Tooltip("How fast the enemy moves")]
    public float Speed;

    [Tooltip("The minimum time interval between two elemental combos")]
    public float BaseElementCooldown;

    [Header("Resistances")]
    [Tooltip("How Effective Physical Damage is")]
    public float Defense;

    [Tooltip("How Effective Special Damage is")]
    public float SpecialDefense;

    [Header("Immunities")]
    [Tooltip("List of possible immunities. Check each item that enemy is immune to")]
    [SerializeField] Immunities baseImmunities;
    [NonSerialized] public Immunities AllImmunities;

    [Header("Debug Info")]

    /// <summary>
    /// All Effects that are currently on the Enemy, including duplicates
    /// </summary>
    public List<IEnemyEffect> ActiveEffects { get; private set; }
    [NonSerialized] public bool isAlive;
    public float Health;
    [NonSerialized] public int NodeIndex;
    [NonSerialized] public EnemyType Id;
    [NonSerialized] public Transform body;
    public ElementName ElementName;
    [NonSerialized] public float ElementDuration;
    [NonSerialized] public TowerDamage ElementOrigin;
    [NonSerialized] public float CurrentElementCooldown;

    GameManager gameManager;
    ElementManager elementManager;
    bool isActivateElementTimerRunning;
    public virtual void Init()
    {
        gameManager = GameManager.Instance;
        elementManager = ElementManager.Instance;
        Health = maxHealth;
        NodeIndex = 0;
        transform.position = GameManager.Instance.enemySpawnNode;
        ActiveEffects = new();
        body = transform;
        ElementName = ElementName.None;
        ElementDuration = 0f;
        ElementOrigin = null;
        CurrentElementCooldown = 0;
        isAlive = true;
        AllImmunities = baseImmunities;

        isActivateElementTimerRunning = false;
    }

    public void AddEffect(EnemyEffectData effectData)
    {
        IEnemyEffect effect = effectData.Effect;
        ActiveEffects.Add(effect);
        effect.ApplyEffect(this, effectData);
    }

    public void RemoveEffect(IEnemyEffect effect)
    {
        ActiveEffects.Remove(effect);
    }

    public void ApplyElement(ElementEffectData data)
    {
        if (CurrentElementCooldown <= 0)
        {
            if (ElementName == ElementName.None)
            {
                ElementName = data.Element.ElementName;
                ElementDuration = data.Element.Duration;
                ElementOrigin = data.Origin;

                if (!isActivateElementTimerRunning)
                    StartCoroutine(ActivateElementTimer());
            }
            else
            {
                IEnemyEffect effect = elementManager.GetEffect(ElementName, data.Element.ElementName);
                ActiveEffects.Add(effect);
                EnemyEffectData effectData = new(effect, this);
                effect.ApplyEffect(this, effectData);
                RemoveElement();
            }
        }
    }

    public void RemoveElement()
    {
        StopCoroutine(ActivateElementTimer());
        ElementName = ElementName.None;
        ElementDuration = 0f;
        ElementOrigin = null;
    }

    public void TickEffects()
    {
        foreach (IEnemyEffect effect in ActiveEffects)
        {
            effect.TickEffect();
        }

        if (CurrentElementCooldown > 0)
            CurrentElementCooldown -= Time.deltaTime;
    }

    // Caching Coroutine return to remove garbage
    readonly WaitForFixedUpdate WaitForFixedUpdate = new();
    IEnumerator ActivateElementTimer()
    {
        isActivateElementTimerRunning = true;
        while (ElementDuration > 0)
        {
            ElementDuration -= Time.deltaTime;
            yield return WaitForFixedUpdate;
        }
        RemoveElement();
        isActivateElementTimerRunning = false;
        yield return null;
    }

    #region Immunity Bitwise Functions
    /// <summary>
    /// Checks if Enemy is Immune to specified effect or damage type
    /// </summary>
    /// <param name="immunities">The effect or damage type to check</param>
    /// <returns>Returns True if enemy is immune; if enemy is not immune, returns false</returns>
    public bool CheckImmunities(Immunities immunities)
    {
        return AllImmunities.HasFlag(immunities);
    }

    /// <summary>
    /// Adds or Removes specified enum values from list of immunities. If state is already true per immunity, does nothing for specified immunity.
    /// </summary>
    /// <param name="flag">The specified immunities to add or remove from immunity list</param>
    /// <param name="state">Set to True to add immunity to list. Set to False to remove immunity from list</param>
    public void SetImmunities(Immunities flag, bool state)
    {
        uint currentImmunities = Convert.ToUInt32(AllImmunities);
        uint newImmunities = Convert.ToUInt32(flag);
        uint result = state ? currentImmunities | newImmunities : currentImmunities & ~newImmunities;

        AllImmunities = (Immunities) Convert.ChangeType(result, Enum.GetUnderlyingType(typeof(Immunities)));
    }

    /// <summary>
    /// Resets all immunities to starting value
    /// </summary>
    public void ResetImmunities()
    {
        AllImmunities = baseImmunities;
    }

    /// <summary>
    /// Removes All Immunities (including starting immunities) from enemy
    /// This function is typically called if debuff that strips immunities from enemy is applied
    /// </summary>
    public void RemoveAllImmunities()
    {
        AllImmunities = Immunities.Nothing;
    }
    #endregion
}

[Flags]
public enum Immunities
{
    // Values Must Be Explicitly Stated and in Powers of Two (for Bitwise Operations)
    Nothing = 1,
    Physical = 2,
    Special = 4,
    Fire = 8,
}