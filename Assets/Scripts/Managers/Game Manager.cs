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
    [SerializeField] Vector3[] nodePositions;
    [SerializeField] Transform nodeFolder;
    [NonSerialized] public Vector3 enemySpawnNode;
    private Queue<int> enemySpawnQueue;
    private Queue<Enemy> enemyRemoveQueue;

    EnemyManager enemyManager;
    void Start()
    {
        InitVariables();


        runGame = true;
        StartCoroutine(GameLoop());
        InvokeRepeating("TestSpawn", 0, 1);
    }

    private void TestSpawn()
    {
        EnqueueSpawnEnemy(1);
    }

    IEnumerator GameLoop()
    {
        while (runGame)
        {
            //Spawn Enemies
            if (enemySpawnQueue.Count > 0)
            {
                for (int i = 0; i < enemySpawnQueue.Count; i++)
                {
                    enemyManager.SpawnEnemy(enemySpawnQueue.Dequeue());
                }
            }
            //Spawn Towers

            //Move Enemies
            NativeArray<Vector3> activeNodes = new NativeArray<Vector3>(nodePositions, Allocator.TempJob);
            NativeArray<int> nodeIndices = new NativeArray<int>(enemyManager.spawnedEnemies.Count, Allocator.TempJob);
            NativeArray<float> enemySpeeds = new NativeArray<float>(enemyManager.spawnedEnemies.Count, Allocator.TempJob);
            TransformAccessArray enemyAccess = new TransformAccessArray(enemyManager.spawnedEnemiesTransform.ToArray(), 2);

            for (int i = 0; i < enemyManager.spawnedEnemies.Count; i++)
            {
                enemySpeeds[i] = enemyManager.spawnedEnemies[i].speed;
                nodeIndices[i] = enemyManager.spawnedEnemies[i].nodeIndex;
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
                currentEnemy.nodeIndex = nodeIndices[i];

                //Checks if enemy has reached the end
                //If so, lose life
                if (currentEnemy.nodeIndex == nodePositions.Length)
                {
                    EnqueueRemoveEnemy(currentEnemy);
                }
            }

            activeNodes.Dispose();
            enemySpeeds.Dispose();
            nodeIndices.Dispose();
            enemyAccess.Dispose();

            //Update Towers

            //Apply Effects

            //Damage Enemies

            //Remove Enemies
            if (enemyRemoveQueue.Count > 0)
            {
                int amountToRemove = enemyRemoveQueue.Count;
                for (int i = 0; i < amountToRemove; i++)
                {
                    enemyManager.RemoveEnemy(enemyRemoveQueue.Dequeue());
                }
            }

            //Remove Towers

            yield return null;
        }
    }

    public void EnqueueSpawnEnemy(int id)
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

        nodePositions = new Vector3[nodeFolder.childCount];

        for (int i = 0; i < nodePositions.Length; i++)
        {
            nodePositions[i] = nodeFolder.GetChild(i).position;
        }
        enemySpawnNode = nodePositions[0];

        enemySpawnQueue = new Queue<int>();
        enemyRemoveQueue = new Queue<Enemy>();
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