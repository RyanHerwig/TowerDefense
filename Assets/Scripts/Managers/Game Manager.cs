using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using Unity.Collections;
using UnityEngine.Jobs;
using Unity.Jobs;
using System;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindAnyObjectByType<GameManager>();
            return instance;
        }
    }
    #endregion
    public bool runGame;
    public Vector3[] nodePositions;
    [SerializeField] Transform nodeFolder;
    [NonSerialized] public float[] nodeDistances;
    [NonSerialized] public Vector3 enemySpawnNode;
    private Queue<DamageData> damageQueue;
    private Queue<EnemyType> enemySpawnQueue;
    private Queue<Enemy> enemyRemoveQueue;
    private Queue<TowerEffectData> towerEffectQueue;
    private Queue<EnemyEffectData> enemyEffectQueue;
    private Queue<EnemyEffectData> enemyRemoveEffectQueue;
    [NonSerialized] public List<Tower> towersInGame;

    AmmoManager ammoManager;
    EnemyManager enemyManager;
    TowerTarget towerTarget;

    public bool isWaveActive;

    void Start()
    {
        InitVariables();


        runGame = true;
        StartCoroutine(GameLoop());
    }

    IEnumerator GameLoop()
    {
        while (runGame)
        {
            // Spawn Enemies
            if (enemySpawnQueue.Count > 0)
            {
                for (int i = 0; i < enemySpawnQueue.Count; i++)
                {
                    enemyManager.SpawnEnemy(enemySpawnQueue.Dequeue());
                }
            }
            // Spawn Towers

            // Move Enemies
            NativeArray<Vector3> activeNodes = new NativeArray<Vector3>(nodePositions, Allocator.TempJob);
            NativeArray<int> nodeIndices = new NativeArray<int>(enemyManager.spawnedEnemies.Count, Allocator.TempJob);
            NativeArray<float> enemySpeeds = new NativeArray<float>(enemyManager.spawnedEnemies.Count, Allocator.TempJob);
            TransformAccessArray enemyAccess = new TransformAccessArray(enemyManager.spawnedEnemiesTransform.ToArray(), 2);

            for (int i = 0; i < enemyManager.spawnedEnemies.Count; i++)
            {
                enemySpeeds[i] = enemyManager.spawnedEnemies[i].Speed;
                nodeIndices[i] = enemyManager.spawnedEnemies[i].NodeIndex;
            }

            MoveEnemies moveEnemies = new MoveEnemies
            {
                nodePos = activeNodes,
                nodeIndex = nodeIndices,
                enemySpeed = enemySpeeds,
                deltaTime = Time.deltaTime
            };

            JobHandle moveJobHandle = moveEnemies.Schedule(enemyAccess);
            moveJobHandle.Complete();

            for (int i = 0; i < enemyManager.spawnedEnemies.Count; i++)
            {
                Enemy currentEnemy = enemyManager.spawnedEnemies[i];
                currentEnemy.NodeIndex = nodeIndices[i];

                // Checks if enemy has reached the end
                // If so, lose life
                if (currentEnemy.NodeIndex == nodePositions.Length)
                {
                    EnqueueRemoveEnemy(currentEnemy);
                }
            }

            activeNodes.Dispose();
            enemySpeeds.Dispose();
            nodeIndices.Dispose();
            enemyAccess.Dispose();

            // Tick Towers
            foreach (Tower tower in towersInGame)
            {
                tower.Target = towerTarget.GetTarget(tower, tower.currentPriority);
                tower.Tick();
            }

            // Apply Effects
            if (enemyEffectQueue.Count > 0)
            {
                int effectAmount = enemyEffectQueue.Count;
                for (int i = 0; i < effectAmount; i++)
                {
                    EnemyEffectData currentEffectData = enemyEffectQueue.Dequeue();

                    // Attempts to get effect with exact same properties
                    EnemyEffect effectDuplicate = currentEffectData.Target.ActiveEffects.Find(x => x.Name == currentEffectData.Effect.Name
                            && x.Id == currentEffectData.Effect.Id);
                    if (effectDuplicate == null)
                    {
                        currentEffectData.Target.ActiveEffects.Add(currentEffectData.Effect);
                    }
                    else if (effectDuplicate.Duration < currentEffectData.Effect.Duration)
                    {
                        effectDuplicate.Duration = currentEffectData.Effect.Duration;
                    }

                    // Apply Buffs
                }
            }

            // Removes Buffs
            if (enemyRemoveEffectQueue.Count > 0)
            {
                int enemyBuffRemoveSize = enemyRemoveEffectQueue.Count;
                for (int i = 0; i < enemyBuffRemoveSize; i++)
                {
                    EnemyEffectData currentEffectData = enemyRemoveEffectQueue.Dequeue();

                    Enemy target = currentEffectData.Target;

                    //Finds an identical buff and removes it
                    //for (int j = 0; j < target.activeBuffs.Count; j++)
                    //{
                    //    if (target.activeBuffs[j].buffName == currentEffectData.buffToApply.buffName
                    //        && target.activeBuffs[j].modifier == currentEffectData.buffToApply.modifier
                    //        && target.activeBuffs[j].duration == currentEffectData.buffToApply.duration)
                    //    {
                    //        target.activeBuffs.RemoveAt(j);
                    //        currentEffectData.enemyToAffect.ApplyBuffs();
                    //        break;
                    //    }
                    //}
                }
            }


            // Tick Enemies
            foreach (Enemy enemy in enemyManager.spawnedEnemies)
            {
                enemy.Tick();
            }

            // Damage Enemies
            if (damageQueue.Count > 0)
            {
                /* Damage Formula:
                * Damage Received = Total Damage * (100 / (100 + resistance))
                * For Example...
                *              if the enemy had a resistance of 100, they would take 0.5x damage
                *              if the enemy had a resistance of -100, they would take 2x damage
                */
                int amountToDamage = damageQueue.Count;
                for (int i = 0; i < amountToDamage; i++)
                {
                    DamageData currentDamageData = damageQueue.Dequeue();
                    Enemy damagedEnemy = currentDamageData.TargetEnemy;

                    // Enemy Takes Physical Damage
                    if (currentDamageData.AttackDamage > 0 && !damagedEnemy.CheckImmunities(Immunities.Physical))
                    {
                        if (damagedEnemy.Defense >= 0)
                        {
                            // Enemy Takes reduced damage
                            damagedEnemy.Health = Mathf.Round((damagedEnemy.Health -
                                currentDamageData.AttackDamage * (100 / (100 + damagedEnemy.Defense))) * 100f) / 100f; //Rounds damage to nearest hundredths place
                        }
                        else
                        {
                            // Enemy Takes More damage
                            damagedEnemy.Health = Mathf.Round((damagedEnemy.Health -
                                currentDamageData.AttackDamage * ((100 - damagedEnemy.Defense) / 100)) * 100f) / 100f; //Rounds damage to nearest hundredths place
                        }
                    }

                    // Enemy Takes Special Damage (if it isn't already dead)
                    if (damagedEnemy.Health > 0 && currentDamageData.SpecialDamage > 0 && !damagedEnemy.CheckImmunities(Immunities.Special))
                    {
                        if (damagedEnemy.Resistance >= 0)
                        {
                            // Enemy Takes reduced damage
                            damagedEnemy.Health = Mathf.Round((damagedEnemy.Health -
                                currentDamageData.SpecialDamage * (100 / (100 + damagedEnemy.Resistance))) * 100f) / 100f; //Rounding removes floating point errors
                        }
                        else
                        {
                            // Enemy Takes More damage
                            damagedEnemy.Health = Mathf.Round((damagedEnemy.Health -
                                currentDamageData.SpecialDamage * ((100 - damagedEnemy.Resistance) / 100)) * 100f) / 100f; //Rounding removes floating point errors
                        }
                    }

                    // Enemy Takes True Damage (if it isn't already dead)
                    if (damagedEnemy.Health > 0)
                        damagedEnemy.Health -= currentDamageData.TrueDamage;

                    if (currentDamageData.TargetEnemy.Health <= 0)
                    {
                        // Failsafe to make sure same enemy does not get queued twice for removal
                        if (!enemyRemoveQueue.Contains(currentDamageData.TargetEnemy))
                            EnqueueRemoveEnemy(currentDamageData.TargetEnemy);
                    }
                }
            }

            // Remove Enemies
            if (enemyRemoveQueue.Count > 0)
            {
                int amountToRemove = enemyRemoveQueue.Count;
                for (int i = 0; i < amountToRemove; i++)
                {
                    enemyManager.RemoveEnemy(enemyRemoveQueue.Dequeue());
                }
            }

            // Remove Towers

            yield return null;
        }
    }

    public void EnqueueTowerEffects(TowerEffectData effectData)
    {
        towerEffectQueue.Enqueue(effectData);
    }

    public void EnqueueAddEnemyEffects(EnemyEffectData effectData)
    {
        enemyEffectQueue.Enqueue(effectData);
    }

    public void EnqueueRemoveEnemyEffects(EnemyEffectData effectData)
    {
        enemyRemoveEffectQueue.Enqueue(effectData);
    }

    public void EnqueueDamageData(DamageData damageData)
    {
        damageQueue.Enqueue(damageData);
    }

    public void EnqueueSpawnEnemy(EnemyType id)
    {
        enemySpawnQueue.Enqueue(id);
    }

    public void EnqueueRemoveEnemy(Enemy enemy)
    {
        enemyRemoveQueue.Enqueue(enemy);
    }

    private void InitVariables()
    {
        enemyManager = EnemyManager.Instance;
        enemyManager.Init();

        ammoManager = AmmoManager.Instance;
        ammoManager.Init();

        nodePositions = new Vector3[nodeFolder.childCount];
        int nodeCount = nodePositions.Length;
        for (int i = 0; i < nodeCount; i++)
        {
            nodePositions[i] = nodeFolder.GetChild(i).position;
        }
        enemySpawnNode = nodePositions[0];

        nodeDistances = new float[nodeFolder.childCount - 1];

        for (int i = 0; i < nodeCount - 1; i++)
        {
            nodeDistances[i] = Vector3.Distance(nodePositions[i], nodePositions[i + 1]);
        }
        damageQueue = new Queue<DamageData>();
        enemySpawnQueue = new Queue<EnemyType>();
        enemyRemoveQueue = new Queue<Enemy>();
        towerEffectQueue = new Queue<TowerEffectData>();
        enemyEffectQueue = new Queue<EnemyEffectData>();
        enemyRemoveEffectQueue = new Queue<EnemyEffectData>();
        isWaveActive = false;

        towersInGame = new();

        towerTarget = new TowerTarget();
        towerTarget.Init();
    }
}

public struct MoveEnemies : IJobParallelForTransform
{
    [NativeDisableParallelForRestriction]
    public NativeArray<Vector3> nodePos;

    [NativeDisableParallelForRestriction]
    public NativeArray<int> nodeIndex;

    [NativeDisableParallelForRestriction]
    public NativeArray<float> enemySpeed;

    public float deltaTime;
    public void Execute(int index, TransformAccess transform)
    {
        if (nodeIndex[index] < nodePos.Length)
        {
            Vector3 positionToMoveTo = nodePos[nodeIndex[index]];
            transform.position = Vector3.MoveTowards(transform.position, positionToMoveTo, enemySpeed[index] * deltaTime);

            if (transform.position == positionToMoveTo)
            {
                nodeIndex[index]++;
            }
        }
    }
}

public struct DamageData
{
    public Enemy TargetEnemy;
    public float AttackDamage;
    public float SpecialDamage;
    public float TrueDamage;
    public float Defense;
    public float Resistance;

    public DamageData(Enemy target, float attackDamage, float specialDamage, float trueDamage, float defense, float resistance)
    {
        TargetEnemy = target;
        AttackDamage = attackDamage;
        SpecialDamage = specialDamage;
        TrueDamage = trueDamage;
        Defense = defense;
        Resistance = resistance;
    }
}