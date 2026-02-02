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
    public Dictionary<int, GameObject> enemyPrefabs;
    public Dictionary<int, Queue<Enemy>> enemyObjectPools;
    [SerializeField] Transform enemyFolder;
    public void Init()
    {

        gameManager = GameManager.Instance;
        spawnedEnemies = new();
        spawnedEnemiesTransform = new();
        enemyPrefabs = new();
        enemyObjectPools = new();


        //Loads Enemy Data on runtime
        EnemyData[] enemies = Resources.LoadAll<EnemyData>("Enemy");

        //Loads enemy data into Dictionaries
        foreach (EnemyData enemy in enemies)
        {
            enemyPrefabs.Add(enemy.EnemyID, enemy.EnemyPrefab);
            enemyObjectPools.Add(enemy.EnemyID, new Queue<Enemy>());
        }
    }

    public Enemy SpawnEnemy(int enemyID)
    {
        Enemy spawnedEnemy;
        if (enemyPrefabs.ContainsKey(enemyID))
        {
            Queue<Enemy> queue = enemyObjectPools[enemyID];

            if (queue.Count > 0)
            {
                //Dequeue enemy and initialize it
                spawnedEnemy = queue.Dequeue();
                spawnedEnemy.Init();

                spawnedEnemy.gameObject.SetActive(true);
            }
            else
            {
                //Instantiate new insatnce of enemy and initialize
                GameObject newEnemy = Instantiate(enemyPrefabs[enemyID], gameManager.enemySpawnNode, Quaternion.identity);
                spawnedEnemy = newEnemy.GetComponent<Enemy>();
                spawnedEnemy.Init();
            }
        }
        else
        {
            print($"ENEMY SPAWNER: INVALID ENEMY ID: {enemyID} DETECTED WHEN SPAWNING");
            return null;
        }

        spawnedEnemies.Add(spawnedEnemy);
        spawnedEnemiesTransform.Add(spawnedEnemy.transform);
        spawnedEnemy.id = enemyID;
        return spawnedEnemy;
    }

    public void RemoveEnemy(Enemy enemy)
    {
        enemyObjectPools[enemy.id].Enqueue(enemy);
        enemy.gameObject.SetActive(false);
        spawnedEnemies.Remove(enemy);
        spawnedEnemiesTransform.Remove(enemy.transform);
    }
}