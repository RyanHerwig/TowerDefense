using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    #region Singleton
    private static EnemyManager instance;

    public static EnemyManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindAnyObjectByType<EnemyManager>();
            return instance;
        }
    }
    #endregion

    GameManager gameManager;
    public List<Enemy> spawnedEnemies;
    public List<Transform> spawnedEnemiesTransform;
    public Dictionary<EnemyType, GameObject> enemyPrefabs;
    public Dictionary<EnemyType, Queue<Enemy>> enemyObjectPools;

    public Dictionary<Transform, Enemy> enemyTransformDict;
    [SerializeField] Transform enemyFolder;
    public void Init()
    {

        gameManager = GameManager.Instance;
        spawnedEnemies = new();
        spawnedEnemiesTransform = new();
        enemyPrefabs = new();
        enemyObjectPools = new();
        enemyTransformDict = new();


        // Loads Enemy Data on runtime
        EnemyData[] enemies = Resources.LoadAll<EnemyData>("Enemy");

        // Loads enemy data into Dictionaries
        foreach (EnemyData enemy in enemies)
        {
            enemyPrefabs.Add(enemy.EnemyID, enemy.EnemyPrefab);
            enemyObjectPools.Add(enemy.EnemyID, new Queue<Enemy>());
        }
    }

    public Enemy SpawnEnemy(EnemyType enemyID)
    {
        Enemy spawnedEnemy;
        if (enemyPrefabs.ContainsKey(enemyID))
        {
            Queue<Enemy> queue = enemyObjectPools[enemyID];

            if (queue.Count > 0)
            {
                // Dequeue enemy and initialize it
                spawnedEnemy = queue.Dequeue();
                spawnedEnemy.Init();

                spawnedEnemy.gameObject.SetActive(true);
            }
            else
            {
                // Instantiate new insatnce of enemy and initialize
                GameObject newEnemy = Instantiate(enemyPrefabs[enemyID], gameManager.enemySpawnNode, Quaternion.identity, enemyFolder);
                spawnedEnemy = newEnemy.GetComponent<Enemy>();
                spawnedEnemy.Init();
            }
        }
        else
        {
            print($"ENEMY SPAWNER: INVALID ENEMY ID: {enemyID} DETECTED WHEN SPAWNING");
            return null;
        }

        // Failsafe - If enemy is already inside list, do not duplicate it
        // Adds ememies to active enemies list
        if (!spawnedEnemies.Contains(spawnedEnemy)) spawnedEnemies.Add(spawnedEnemy);
        if (!spawnedEnemiesTransform.Contains(spawnedEnemy.transform)) spawnedEnemiesTransform.Add(spawnedEnemy.transform);
        if (!enemyTransformDict.ContainsKey(spawnedEnemy.transform))
        {
            enemyTransformDict.Add(spawnedEnemy.transform, spawnedEnemy);
        }
        spawnedEnemy.Id = enemyID;
        return spawnedEnemy;
    }

    public void RemoveEnemy(Enemy enemy)
    {
        enemyObjectPools[enemy.Id].Enqueue(enemy);
        enemy.gameObject.SetActive(false);
        enemy.isAlive = false;
        spawnedEnemies.Remove(enemy);
        spawnedEnemiesTransform.Remove(enemy.transform);
        enemyTransformDict.Remove(enemy.transform);

        if (spawnedEnemies.Count == 0)
        {
            gameManager.isWaveActive = false;
        }
    }
}