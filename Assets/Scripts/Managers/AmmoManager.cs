using System.Collections.Generic;
using UnityEngine;

public class AmmoManager : MonoBehaviour
{
    #region Singleton
    private static AmmoManager instance;

    public static AmmoManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindAnyObjectByType<AmmoManager>();
            return instance;
        }
    }
    #endregion

    GameManager gameManager;

    public List<Ammo> AllSpawnedAmmo;

    public Dictionary<AmmoType, GameObject> AmmoPrefabs;

    public Dictionary<AmmoType, Queue<Ammo>> AmmoObjectPools;

    [SerializeField] Transform ammoFolder;

    public void Init()
    {
        gameManager = GameManager.Instance;
        AllSpawnedAmmo = new();
        AmmoPrefabs = new();
        AmmoObjectPools = new();

        // Loads Enemy Data on runtime
        AmmoData[] allAmmo = Resources.LoadAll<AmmoData>("Ammo");

        foreach (AmmoData ammo in allAmmo)
        {
            AmmoPrefabs.Add(ammo.AmmoId, ammo.AmmoPrefab);
            AmmoObjectPools.Add(ammo.AmmoId, new Queue<Ammo>());
        }
    }

    public Ammo SpawnAmmo(AmmoType ammoID, TowerDamage source, Enemy enemy, Transform spawnPoint)
    {
        Ammo spawnedAmmo;
        if (AmmoPrefabs.ContainsKey(ammoID))
        {
            Queue<Ammo> queue = AmmoObjectPools[ammoID];

            if (queue.Count > 0)
            {
                // Dequeue enemy and initialize it
                spawnedAmmo = queue.Dequeue();
                spawnedAmmo.Init(source, enemy, spawnPoint);

                spawnedAmmo.gameObject.SetActive(true);
            }
            else
            {
                // Instantiate new insatnce of enemy and initialize
                GameObject newAmmo = Instantiate(AmmoPrefabs[ammoID], Vector3.zero, Quaternion.identity, ammoFolder);
                spawnedAmmo = newAmmo.GetComponent<Ammo>();
                spawnedAmmo.Init(source, enemy, spawnPoint);
            }
        }
        else
        {
            print($"AMMO SPAWNER: INVALID AMMO ID: {ammoID} DETECTED WHEN SPAWNING");
            return null;
        }

        // Failsafe - If enemy is already inside list, do not duplicate it
        // Adds ememies to acttive enemies list
        if (!AllSpawnedAmmo.Contains(spawnedAmmo)) AllSpawnedAmmo.Add(spawnedAmmo);
        spawnedAmmo.Id = ammoID;
        return spawnedAmmo;
    }

    public void RemoveAmmo(Ammo ammo)
    {
        AmmoObjectPools[ammo.Id].Enqueue(ammo);
        ammo.gameObject.SetActive(false);
        AllSpawnedAmmo.Remove(ammo);
    }
}