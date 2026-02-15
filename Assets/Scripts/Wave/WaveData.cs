using System;
using UnityEngine;

public class WaveData
{
    #region Singleton
    private static WaveData instance;

    public static WaveData Instance
    {
        get
        {
            if (instance == null)
                instance = new WaveData();
            return instance;
        }
    }
    #endregion

    private Wave[] waves;
    public void Init()
    {
        #region Waves
        waves = new Wave[]
        {
            #region First Wave
            new(
                enemyId: new EnemyType[] {EnemyType.Basic},
                count: new int[] {5},
                delay: new float[] {0},
                interval: new float[] {1.5f}),
            #endregion

            #region Second Wave
            new(
                enemyId: new EnemyType[] {EnemyType.Basic, EnemyType.Fast},
                count: new int[] {5, 10},
                delay: new float[] {0, 0.5f},
                interval: new float[] {2, 0.5f}),

            #endregion

            #region Third Wave
            new (
                enemyId: new EnemyType[] {EnemyType.Slow},
                count: new int[] {1},
                delay: new float[] {0},
                interval: new float[] {0})
            #endregion
        };
        #endregion
    }
    public Wave[] GetWave()
    {
        return waves;
    }

}

public struct Wave
{
    /// <summary>
    /// Enemy ID to Spawn in group
    /// </summary>
    public readonly EnemyType[] EnemyId;

    /// <summary>
    /// Amount of enemy to spawn
    /// </summary>
    public readonly int[] CountOfEachEnemy;

    /// <summary>
    /// How long to wait before spawning first enemy in group. Ignores Interval.
    /// </summary>
    public readonly float[] Delay;

    /// <summary>
    /// The time to wait before spawning the next enemy in the group
    /// </summary>
    public readonly float[] Interval;

    /// <summary>
    /// How many enemy groups there are in the wave
    /// </summary>
    public readonly int WaveSize;

    /// <summary>
    /// How long it takes for wave to end
    /// </summary>
    public readonly float WaveLength;

    public Wave(EnemyType[] enemyId, int[] count, float[] delay, float[] interval)
    {
        WaveLength = 0;
        if (enemyId.Length != count.Length && count.Length != delay.Length && delay.Length != interval.Length)
        {
            Debug.LogError("INVALID WAVE");

            EnemyId = new EnemyType[] { };
            CountOfEachEnemy = new int[] { };
            Delay = new float[] { };
            Interval = new float[] { };
            WaveSize = 0;
        }
        else
        {
            EnemyId = enemyId;
            CountOfEachEnemy = count;
            Delay = delay;
            Interval = interval;
            WaveSize = enemyId.Length;

            // Gets Wave Length
            // Gets time between each enemy, then adds the delay to it
            for (int i = 0; i < WaveSize; i++)
            {
                int tempCount = CountOfEachEnemy[i] - 1;
                float tempInterval = Interval[i];

                if (tempCount == 0)
                    tempCount = 1;
                if (tempInterval == 0)
                    tempInterval = 0.1f;

                float temp = delay[i] + tempInterval * tempCount;
                if (temp > WaveLength)
                    WaveLength = temp;
            }
        }
    }
}