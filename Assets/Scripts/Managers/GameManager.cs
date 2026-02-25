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
    private Queue<ElementEffectData> elementEffectQueue;
    private Queue<EnemyEffectData> enemyRemoveEffectQueue;
    private Queue<ElementEffectData> elementRemoveEffectQueue;
    [NonSerialized] public List<Tower> towersInGame;

    AmmoManager ammoManager;
    EnemyManager enemyManager;
    TowerTarget towerTarget;

    public bool isWaveActive;
    private int gameLoopQueueCount;

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
                gameLoopQueueCount = enemySpawnQueue.Count;
                for (int i = 0; i < gameLoopQueueCount; i++)
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

            // Apply Element on Enemy

            if (elementEffectQueue.Count > 0)
            {
                gameLoopQueueCount = elementEffectQueue.Count;

                for (int i = 0; i < gameLoopQueueCount; i++)
                {
                    ElementEffectData currentElementData = elementEffectQueue.Dequeue();
                    Enemy target = currentElementData.Target;
                    if (target.CurrentElementCooldown <= 0 && currentElementData.Origin != target.ElementOrigin)
                    {
                        target.ApplyElement(currentElementData);
                    }
                    else
                    {
                        if (currentElementData.Element.Duration > target.ElementDuration)
                            target.ElementDuration = currentElementData.Element.Duration;
                    }
                }
            }

            // Apply Effects
            if (enemyEffectQueue.Count > 0)
            {
                gameLoopQueueCount = enemyEffectQueue.Count;
                for (int i = 0; i < gameLoopQueueCount; i++)
                {
                    EnemyEffectData currentEffectData = enemyEffectQueue.Dequeue();

                    // Attempts to get effect with exact same properties
                    IEnemyEffect effectDuplicate = currentEffectData.Target.ActiveEffects.Find(x => x.GetName() == currentEffectData.Effect.GetName());
                    if (effectDuplicate == null)
                    {
                        currentEffectData.Target.AddEffect(currentEffectData);
                    }
                    else
                    {
                        // Once an effect is applied, it can be extended indefinitely, so long as the 
                        // effect is reapplied before it expires - regardless how weak the reapply is

                        // Reset duration to highest duration
                        if (effectDuplicate.GetDuration() < currentEffectData.Effect.GetDuration())
                        {
                            effectDuplicate.SetDuration(currentEffectData.Effect.GetDuration());
                        }

                        // Takes the stronger damage
                        if (effectDuplicate.GetDamage() < currentEffectData.Effect.GetDamage())
                        {
                            effectDuplicate.SetDamage(currentEffectData.Effect.GetDamage());
                        }
                    }
                }
            }

            // Remove Elemental
            if (elementRemoveEffectQueue.Count > 0)
            {
                gameLoopQueueCount = elementRemoveEffectQueue.Count;

                for (int i = 0; i < gameLoopQueueCount; i++)
                {
                    ElementEffectData currentElementData = elementRemoveEffectQueue.Dequeue();

                    Enemy target = currentElementData.Target;
                    target.RemoveElement();
                }
            }

            // Remove Effects
            if (enemyRemoveEffectQueue.Count > 0)
            {
                gameLoopQueueCount = enemyRemoveEffectQueue.Count;
                for (int i = 0; i < gameLoopQueueCount; i++)
                {
                    EnemyEffectData currentEffectData = enemyRemoveEffectQueue.Dequeue();

                    Enemy target = currentEffectData.Target;
                    target.RemoveEffect(currentEffectData.Effect);
                }
            }


            // Tick Enemies
            foreach (Enemy enemy in enemyManager.spawnedEnemies)
            {
                enemy.TickEffects();
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
                gameLoopQueueCount = damageQueue.Count;
                for (int i = 0; i < gameLoopQueueCount; i++)
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
                        if (damagedEnemy.SpecialDefense >= 0)
                        {
                            // Enemy Takes reduced damage
                            damagedEnemy.Health = Mathf.Round((damagedEnemy.Health -
                                currentDamageData.SpecialDamage * (100 / (100 + damagedEnemy.SpecialDefense))) * 100f) / 100f; //Rounding removes floating point errors
                        }
                        else
                        {
                            // Enemy Takes More damage
                            damagedEnemy.Health = Mathf.Round((damagedEnemy.Health -
                                currentDamageData.SpecialDamage * ((100 - damagedEnemy.SpecialDefense) / 100)) * 100f) / 100f; //Rounding removes floating point errors
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
                gameLoopQueueCount = enemyRemoveQueue.Count;
                for (int i = 0; i < gameLoopQueueCount; i++)
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

    public void EnqueueAddElement(ElementEffectData elementData)
    {
        elementEffectQueue.Enqueue(elementData);
    }

    public void EnqueueRemoveEnemyEffects(EnemyEffectData effectData)
    {
        enemyRemoveEffectQueue.Enqueue(effectData);
    }

    public void EnqueueRemoveElement(ElementEffectData elementData)
    {
        elementRemoveEffectQueue.Enqueue(elementData);
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
        elementEffectQueue = new Queue<ElementEffectData>();
        enemyRemoveEffectQueue = new Queue<EnemyEffectData>();
        elementRemoveEffectQueue = new Queue<ElementEffectData>();
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