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
    List<EnemyEffectData> allEnemeyEffectData;
    public override void Init()
    {
        base.Init();

        gameManager = GameManager.Instance;
        enemyManager = EnemyManager.Instance;
        allEnemeyEffectData = new();

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

            NullEffect nullPhysicalEffect = new();
            EnemyEffectData enemyEffectData = new(nullPhysicalEffect, target);
            allEnemeyEffectData.Add(enemyEffectData);
            gameManager.EnqueueAddEnemyEffects(enemyEffectData);
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        Enemy target = other.GetComponent<Enemy>();

        if (CheckNullBuff(Immunities.Physical))
        {
            NullEffect nullPhysicalEffect = new();
            EnemyEffectData enemyEffectData = new(nullPhysicalEffect, target);
            allEnemeyEffectData.Add(enemyEffectData);
            gameManager.EnqueueAddEnemyEffects(enemyEffectData);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        //WARNING - Will Error if Enemy Spawns within Range of Buff / Debuff
        Enemy target = other.GetComponent<Enemy>();
        EnemyEffectData data = allEnemeyEffectData.Find(x => x.Target == target);
        gameManager.EnqueueRemoveEnemyEffects(data);
        allEnemeyEffectData.Remove(data);
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