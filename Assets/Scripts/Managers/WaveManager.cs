using System.Collections;
using UnityEngine;


public class WaveManager : MonoBehaviour
{
    #region Singleton
    private static WaveManager instance;

    public static WaveManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindAnyObjectByType<WaveManager>();
            return instance;
        }
    }
    #endregion
    GameManager gameManager;
    WaveData waveData;
    TowerPlacement player;
    int currentWave;
    int totalNumberOfWaves;

    Wave[] waves;
    void Start()
    {
        gameManager = GameManager.Instance;
        waveData = new WaveData();
        player = TowerPlacement.Instance;
        waveData.Init();
        waves = waveData.GetWave();
        totalNumberOfWaves = waves.Length;
        currentWave = -1;
    }

    public void StartWave()
    {
        if (!gameManager.isWaveActive && currentWave < totalNumberOfWaves - 1)
        {
            currentWave++;
            gameManager.isWaveActive = true;
            StartCoroutine(Wave());
        }
    }

    public int GetTotalWaves()
    {
        return waves.Length;
    }

    public int GetCurrentWave()
    {
        return currentWave;
    }

    public Wave GetWaveData()
    {
        return waves[currentWave];
    }

    public void GiveWaveBonus()
    {
        player.UpdateMoney(waves[currentWave].WaveBonus);
    }

    // Caching Coroutine return to remove garbage
    readonly WaitForFixedUpdate WaitForFixedUpdate = new();
    IEnumerator Wave()
    {
        // Get Wave
        Wave wave = waves[currentWave];

        // Get Wave Size
        int waveSize = wave.WaveSize;

        // Init Timer Variables
        float[] waveTimer = new float[waveSize];
        float waveLength = wave.WaveLength;
        float time = 0;
        float deltaTime;

        // Init wave variables
        int[] waveCount = new int[waveSize];
        bool[] addInterval = new bool[waveSize];

        while (time <= waveLength)
        {
            deltaTime = Time.deltaTime;
            time += deltaTime;

            // Loops over amount of groups to spawn in wave
            for (int i = 0; i < waveSize; i++)
            {
                waveTimer[i] += deltaTime;
                // Continue spawning group until the amount spawned 
                // reaches the amount that needs to be spawned
                if (waveCount[i] < wave.CountOfEachEnemy[i])
                {
                    //If 
                    if (addInterval[i] && waveTimer[i] >= wave.Delay[i] + wave.Interval[i]
                        || !addInterval[i] && waveTimer[i] >= wave.Delay[i])
                    {
                        gameManager.EnqueueSpawnEnemy(wave.EnemyId[i]);
                        waveCount[i]++;

                        if (addInterval[i])
                            waveTimer[i] -= wave.Interval[i];
                        addInterval[i] = true;
                    }
                }
            }
            yield return WaitForFixedUpdate;
        }
    }
}