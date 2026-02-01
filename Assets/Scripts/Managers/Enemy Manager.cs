using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    #region Singleton
    private static EnemyManager instance;

    public static EnemyManager GetInstance
    {
        get
        {
            if (instance == null)
                instance = FindAnyObjectByType<EnemyManager>();
            return instance;
        }
    }
    #endregion

    public List<Enemy> spawnedEnemies;
    public Dictionary<int, GameObject> enemyPrefabs;
    public Dictionary<int, Queue<Enemy>> enemyObjectPools;
    [SerializeField] Transform enemyFolder;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void Init()
    {
        enemyPrefabs = new Dictionary<int, GameObject>();
        enemyObjectPools = new Dictionary<int, Queue<Enemy>>();
        spawnedEnemies = new List<Enemy>();

        //Grabs all data for enemies inside Resources/Enemy folder
        EnemyData[] enemies = Resources.LoadAll<EnemyData>("Enemy");

        foreach(EnemyData enemy in enemies)
        {
            enemyPrefabs.Add(enemy.EnemyID, enemy.EnemyPrefab);
            enemyObjectPools.Add(enemy.EnemyID, new Queue<Enemy>());
        }
    }

    public Enemy SpawnEnemy(int enemyId)
    {
        Enemy spawnedEnemy = null;

        if (enemyPrefabs.ContainsKey(enemyId))
        {
            //Checks Object Pool for enemy type
            Queue<Enemy> queue = enemyObjectPools[enemyId];

            //If object pool is not empty, grab object out of pool
            if (queue.Count > 0)
            {
                spawnedEnemy = queue.Dequeue();
                spawnedEnemy.Init();
                spawnedEnemy.gameObject.SetActive(true);
            }
            //If pool is empty, instantiate a new object for the pool
            else
            {
                GameObject newEnemy = Instantiate(enemyPrefabs[enemyId], Vector3.zero, Quaternion.identity, enemyFolder);
                spawnedEnemy = newEnemy.GetComponent<Enemy>();
                spawnedEnemy.id = enemyId;
                spawnedEnemy.Init();
            }
        } else
        {
            print($"ENEMY SPAWNER: INVALID ENEMY ID: {enemyId} DETECTED WHEN SPAWNING");
            return null;
        }

        spawnedEnemies.Add(spawnedEnemy);
        return spawnedEnemy;
    }

    public void RemoveEnemy(Enemy enemy)
    {
        //Return enemy to pool to be reused later
        enemyObjectPools[enemy.id].Enqueue(enemy);
        spawnedEnemies.Remove(enemy);
        enemy.gameObject.SetActive(false);
    }
}