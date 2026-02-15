using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class TowerTarget
{
    public enum TargetPriority
    {
        First,
        Last,
        Closest,
        Farthest,
        Strongest,
        Weakest
    }

    GameManager gameManager;
    EnemyManager enemyManager;
    public TargetPriority currentPriority;
    int nodeLength;
    public void Init()
    {
        gameManager = GameManager.Instance;
        enemyManager = EnemyManager.Instance;
        currentPriority = TargetPriority.First;
    }

    /// <summary>
    /// Gets the enemy the target should target based on its range and set priority
    /// </summary>
    /// <param name="currentTower">The tower to calculate the enemy it should fire at</param>
    /// <param name="priority">The algorithm used to determine what enemy it should prioritize attacking</param>
    /// <returns></returns>
    public Enemy GetTarget(Tower currentTower, TargetPriority priority)
    {
        currentPriority = priority;
        float minimumRange = currentTower.MinRange;

        List<Collider> enemiesInRange;

        // If Tower has minimum range...
        if (minimumRange > 0)
        {
            // Find all enemies inside minimum range
            Collider[] enemiesTooClose = Physics.OverlapSphere(currentTower.transform.position, minimumRange, currentTower.targetLayer);

            //Find all enemies within maximum range
            enemiesInRange = Physics.OverlapSphere(currentTower.transform.position, currentTower.MaxRange, currentTower.targetLayer).ToList();

            // Remove all enemies that are below the minimum range for consideration
            foreach (Collider collider in enemiesTooClose)
            {
                enemiesInRange.Remove(collider);
            }
        }
        // If tower does not have minimum range, do not calculate it - it is faster
        else
        {
            enemiesInRange = Physics.OverlapSphere(currentTower.transform.position, currentTower.MaxRange, currentTower.targetLayer).ToList();
        }

        // If there are no enemies close enough, stop
        if (enemiesInRange.Count == 0)
            return null;

        NativeArray<EnemyDataValue> enemiesToCalculate = new(enemiesInRange.Count, Allocator.TempJob); // All canindates to consider to be fired upon
        NativeArray<Vector3> nodePositions = new(gameManager.nodePositions, Allocator.TempJob); // Track all node positions
        NativeArray<float> nodeDistances = new(gameManager.nodeDistances, Allocator.TempJob); // Track all distances from one node to another node
        NativeArray<int> enemyToIndex = new(1, Allocator.TempJob); // The index of all enemies
        int enemyIndexReturn = -1; // The index of the enemy chosen to be riddled with bullet holes

        int enemyLength = enemiesInRange.Count; // Size of canidate list
        for (int i = 0; i < enemyLength; i++)
        {
            // Gets enemy object from canidate
            Enemy tempEnemy = enemiesInRange[i].transform.parent.GetComponent<Enemy>();

            // Gets the index of the enemy
            int enemyIndexInList = enemyManager.spawnedEnemies.FindIndex(x => x == tempEnemy);

            // Adds enemy to canidate list
            if (enemyIndexInList != -1)
                enemiesToCalculate[i] = new EnemyDataValue(enemyIndexInList, tempEnemy.transform.position, tempEnemy.NodeIndex, tempEnemy.Health);
        }

        // Variables for determing enemy priority
        float compareDistance = 0f;
        float compareHealth = 0f;

        // Sets variables to fit needs of chosen algorithm
        switch (currentPriority)
        {
            case TargetPriority.First:
            case TargetPriority.Closest:
                compareDistance = Mathf.Infinity;
                break;
            case TargetPriority.Last:
            case TargetPriority.Farthest:
                compareDistance = Mathf.NegativeInfinity;
                break;
            case TargetPriority.Strongest:
                compareHealth = Mathf.NegativeInfinity;
                compareDistance = Mathf.Infinity;
                break;
            case TargetPriority.Weakest:
                compareHealth = Mathf.Infinity;
                compareDistance = Mathf.Infinity;
                break;
        }

        // Initializes algorithm
        Search search = new Search
        {
            sEnemiesToCalculate = enemiesToCalculate,
            sNodePositions = nodePositions,
            sNodeDistances = nodeDistances,
            sEnemiesToIndex = enemyToIndex,
            sTowerPosition = currentTower.transform.position,
            CompareDistance = compareDistance,
            CompareHealth = compareHealth,
            sTargetPriority = currentPriority
        };

        // Runs and gets result of algorithm
        JobHandle dependency = new();
        JobHandle searchJobHandle = search.Schedule(enemiesToCalculate.Length, dependency);

        searchJobHandle.Complete();

        // Gets the enemy marked for death by chosen algorithm
        enemyIndexReturn = enemiesToCalculate[enemyToIndex[0]].EnemyIndex;

        // Gets rid of data to avoid data leaks
        enemiesToCalculate.Dispose();
        nodePositions.Dispose();
        nodeDistances.Dispose();
        enemyToIndex.Dispose();

        // Returns enemy to fire upon. Returns nothing if no enemy was found
        if (enemyIndexReturn != -1)
            return enemyManager.spawnedEnemies[enemyIndexReturn];
        return null;
    }

    // Enemy Information to store
    struct EnemyDataValue
    {
        public int EnemyIndex;
        public Vector3 EnemyPosition;
        public int NodeIndex;
        public float Health;
        public EnemyDataValue(int index, Vector3 pos, int nodeIndex, float hp)
        {
            EnemyIndex = index;
            EnemyPosition = pos;
            NodeIndex = nodeIndex;
            Health = hp;
        }
    }

    // Searches for a enemy that best fits algorithm chosen
    struct Search : IJobFor
    {
        [ReadOnly] public NativeArray<EnemyDataValue> sEnemiesToCalculate;
        [ReadOnly] public NativeArray<Vector3> sNodePositions;
        [ReadOnly] public NativeArray<float> sNodeDistances;
        [NativeDisableParallelForRestriction] public NativeArray<int> sEnemiesToIndex;

        public Vector3 sTowerPosition;

        public float CompareDistance;
        public float CompareHealth;
        public TargetPriority sTargetPriority;

        public void Execute(int index)
        {
            float currentDistance;
            float currentHealth;

            switch (sTargetPriority)
            {
                case TargetPriority.First:
                    //Gets Distance from end
                    currentDistance = GetDistanceFromEnd(sEnemiesToCalculate[index]);
                    //Checks if distance is less than previous record of first enemy
                    if (currentDistance < CompareDistance)
                    {
                        //Updates first enemy if new enemy has 
                        //less distance to cover to reach the end
                        sEnemiesToIndex[0] = index;
                        CompareDistance = currentDistance;
                    }
                    break;
                case TargetPriority.Last:
                    //Gets distance from end
                    currentDistance = GetDistanceFromEnd(sEnemiesToCalculate[index]);
                    //Checks if distance is more than the previous record of last enemy
                    if (currentDistance > CompareDistance)
                    {
                        //Updates the last enemy if new enemy
                        //has more distance to cover to reach the end
                        sEnemiesToIndex[0] = index;
                        CompareDistance = currentDistance;
                    }
                    break;
                case TargetPriority.Closest:
                    //Checks distance from enemy to tower
                    currentDistance = Vector3.Distance(sTowerPosition, sEnemiesToCalculate[index].EnemyPosition);

                    //Checks if enemy is closer to tower than last record
                    if (currentDistance < CompareDistance)
                    {
                        //Updates the closest enemy
                        sEnemiesToIndex[0] = index;
                        CompareDistance = currentDistance;
                    }
                    break;
                case TargetPriority.Farthest:
                    //Checks distance from enemy to tower
                    currentDistance = Vector3.Distance(sTowerPosition, sEnemiesToCalculate[index].EnemyPosition);

                    //Checks if enemy is further away from last record
                    if (currentDistance > CompareDistance)
                    {
                        //Updates the farthest enemy
                        sEnemiesToIndex[0] = index;
                        CompareDistance = currentDistance;
                    }
                    break;
                case TargetPriority.Strongest:
                    //Checks the remaining health of the enemy
                    currentHealth = sEnemiesToCalculate[index].Health;

                    //Checks if enemy has more health than previous highest
                    if (currentHealth > CompareHealth)
                    {
                        //Updates the highest health entry and stores its distance
                        CompareHealth = currentHealth;
                        sEnemiesToIndex[0] = index;
                        CompareDistance = GetDistanceFromEnd(sEnemiesToCalculate[index]);
                    }

                    //Check to see if enemy can contest secondary priority (whichever enemy is first)
                    else if (currentHealth == CompareHealth)
                    {
                        //If enemy has the same health as the strongest enemy, check its distance
                        currentDistance = GetDistanceFromEnd(sEnemiesToCalculate[index]);
                        if (currentDistance < CompareDistance)
                        {
                            //If enemy is tied for highest health and further along in the track,
                            //Target it
                            CompareDistance = currentDistance;
                            sEnemiesToIndex[0] = index;
                        }
                    }
                    break;
                case TargetPriority.Weakest:
                    //Checks the remaining health of the enemy
                    currentHealth = sEnemiesToCalculate[index].Health;

                    //Checks if enemy has less health than previous lowest
                    if (currentHealth < CompareHealth)
                    {
                        //Updates the lowest health entry and stores its distance
                        CompareHealth = currentHealth;
                        sEnemiesToIndex[0] = index;
                        CompareDistance = GetDistanceFromEnd(sEnemiesToCalculate[index]);
                    }

                    //Check to see if enemy can contest secondary priority (whichever enemy is first)
                    else if (currentHealth == CompareHealth)
                    {
                        //If enemy has the same health as the weakest enemy, check its distance
                        currentDistance = GetDistanceFromEnd(sEnemiesToCalculate[index]);
                        if (currentDistance < CompareDistance)
                        {
                            //If enemy is tied for lowest health and further along in the track,
                            //Target it
                            CompareDistance = currentDistance;
                            sEnemiesToIndex[0] = index;
                        }
                    }

                    break;
            }
        }

        /// <summary>
        /// Gets the distance from the enemy position to the end of the map
        /// Enemy must follow the current path
        /// </summary>
        /// <param name="enemyToEval"> Enemy to Evaluate distance from end</param>
        /// <returns></returns>
        private float GetDistanceFromEnd(EnemyDataValue enemyToEval)
        {
            // Gets the remaining distance from the enemy position to the next node
            float finalDistance = Vector3.Distance(enemyToEval.EnemyPosition, sNodePositions[enemyToEval.NodeIndex]);
            int nodeLength = sNodeDistances.Length;

            // Adds the remaining nodes' full length to the distance left to travel
            for (int i = enemyToEval.NodeIndex; i < nodeLength; i++)
            {
                finalDistance += sNodeDistances[i];
            }

            // Returns the distance found
            return finalDistance;
        }
    }
}