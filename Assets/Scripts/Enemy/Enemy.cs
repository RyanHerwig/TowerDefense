using System;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Basic Stats")]

    [Tooltip("Total amount of starting HP the enemy has")]
    [SerializeField] float maxHealth;

    [Tooltip("How fast the enemy moves")]
    public float Speed;

    [Header("Resistances")]
    [Tooltip("How Effective Physical Damage is")]
    public float Defense;

    [Tooltip("How Effective Special Damage is")]
    public float Resistance;

    [Header("Immunities")]
    [Tooltip("List of possible immunities. Check each item that enemy is immune to")]
    [SerializeField] Immunities allImmunities;

    /// <summary>
    /// All Effects that are currently on the Enemy, including duplicates
    /// </summary>
    [NonSerialized] public List<EnemyEffect> ActiveEffects;

    /// <summary>
    /// All Effects that are currently affecting the enemy, strongest effect of each type only
    /// </summary>
    [NonSerialized] public List<EnemyEffect> AppliedEffects;

    [NonSerialized] public bool isAlive;
    [NonSerialized] public float Health;
    [NonSerialized] public int NodeIndex;
    [NonSerialized] public EnemyType Id;
    [NonSerialized] public Transform body;

    Dictionary<EnemyEffectName, List<EnemyEffectData>> enemyEffectDict;

    GameManager gameManager;
    public void Init()
    {
        gameManager = GameManager.Instance;
        Health = maxHealth;
        NodeIndex = 0;
        transform.position = GameManager.Instance.enemySpawnNode;
        ActiveEffects = new();
        AppliedEffects = new();
        enemyEffectDict = new();
        body = transform;
        isAlive = true;
    }

    public void Tick()
    {
        int effectCount = ActiveEffects.Count;
        for (int i = 0; i < effectCount; i++)
        {
            if (!ActiveEffects[i].IsPermanent)
            {
                if (ActiveEffects[i].Duration > 0)
                {
                    if (ActiveEffects[i].AttackDamage != 0 || ActiveEffects[i].SpecialDamage != 0 || ActiveEffects[i].TrueDamage != 0)
                    {
                        if (ActiveEffects[i].EffectDelay > 0)
                        {
                            ActiveEffects[i].EffectDelay -= Time.deltaTime;
                        }
                        else
                        {
                            gameManager.EnqueueDamageData(new DamageData(this, ActiveEffects[i].AttackDamage,
                                ActiveEffects[i].SpecialDamage, ActiveEffects[i].TrueDamage, Defense, Resistance));
                            ActiveEffects[i].EffectDelay = 1f / ActiveEffects[i].EffectRate;
                        }
                    }
                    ActiveEffects[i].Duration -= Time.deltaTime;
                }
            }
        }

        ActiveEffects.RemoveAll(x => x.Duration < 0);
    }

    public void ApplyEffects()
    {
        
    }

    /// <summary>
    /// Checks if Enemy is Immune to specified effect or damage type
    /// </summary>
    /// <param name="immunities">The effect or damage type to check</param>
    /// <returns>Returns True if enemy is immune; if enemy is not immune, returns false</returns>
    public bool CheckImmunities(Immunities immunities)
    {
        return (allImmunities & immunities) != 0;
    }
}

[Flags]
public enum Immunities
{
    Nothing = 1,
    Physical = 2,
    Special = 4,
    Fire = 8,
}