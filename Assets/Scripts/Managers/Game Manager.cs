using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    #region Singleton
    private static GameManager instance;

    public static GameManager GetInstance
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
    private Queue<int> spawnQueue;

    EnemyManager enemyManager;
    void Start()
    {
        GetScriptReferences();

        spawnQueue = new Queue<int>();
        runGame = true;
        StartCoroutine(GameLoop());
        InvokeRepeating("TestSpawn", 0, 1);
        InvokeRepeating("RemoveTest", 0, 2);
    }

    private void TestSpawn()
    {
        EnqueueSpawnEnemy(1);
    }

    private void RemoveTest()
    {
        if (enemyManager.spawnedEnemies.Count > 0)
        {
            enemyManager.RemoveEnemy(enemyManager.spawnedEnemies[0]);
        }
    }

    IEnumerator GameLoop()
    {
        while (runGame)
        {
            //Spawn Enemies
            if (spawnQueue.Count > 0)
            {
                for(int i = 0; i < spawnQueue.Count; i++)
                {
                    enemyManager.SpawnEnemy(spawnQueue.Dequeue());
                }
            }
            //Spawn Towers

            //Move Enemies

            //Update Towers

            //Apply Effects

            //Damage Enemies

            //Remove Enemies

            //Remove Towers

            yield return null;
        }
    }

    public void EnqueueSpawnEnemy(int id)
    {
        spawnQueue.Enqueue(id);
    }

    private void GetScriptReferences()
    {
        enemyManager = EnemyManager.GetInstance;
        enemyManager.Init();
    }
}