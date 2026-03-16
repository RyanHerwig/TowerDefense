using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyNullBuffer : Enemy
{
    [Header("Null Aura Buff")]
    [SerializeField] Collider parent;
    [SerializeField] float auraRadius;
    [SerializeField] Immunities nullBuffs;
    [SerializeField] LayerMask AuraEffectLayer;

    EnemyManager enemyManager;
    List<Enemy> effectedEnemies;
    public override void Init()
    {
        base.Init();

        enemyManager = EnemyManager.Instance;
        effectedEnemies = new();

        ApplyBuff();
    }

    public void ApplyBuff()
    {
        List<Collider> enemiesInAura = Physics.OverlapSphere(transform.position, auraRadius, AuraEffectLayer).ToList();

        if (enemiesInAura.Contains(parent))
            enemiesInAura.Remove(parent);

        int enemiesInRadiusCount = enemiesInAura.Count;
        for (int j = 0; j < enemiesInRadiusCount; j++)
        {
            Enemy target = enemyManager.enemyTransformDict[enemiesInAura[j].transform];
            if (!target.CheckImmunities(nullBuffs))
                target.SetImmunities(nullBuffs, true);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Enemy target = other.GetComponent<Enemy>();
        target.SetImmunities(nullBuffs, true);
    }

    public void OnTriggerExit(Collider other)
    {
        Enemy target = other.GetComponent<Enemy>();
        target.SetImmunities(nullBuffs, false);
    }

    /// <summary>
    /// Checks if Enemy is Immune to specified effect or damage type
    /// </summary>
    /// <param name="immunities">The effect or damage type to check</param>
    /// <returns>Returns True if enemy is immune; if enemy is not immune, returns false</returns>
    public bool CheckNullBuff(Immunities nullBuff)
    {
        return (nullBuffs & nullBuff) != 0;
    }
}