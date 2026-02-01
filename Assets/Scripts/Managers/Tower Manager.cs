using UnityEngine;

public class TowerManager : MonoBehaviour
{
    #region Singleton
    private static TowerManager instance;

    public static TowerManager GetInstance
    {
        get
        {
            if (instance == null)
                instance = FindAnyObjectByType<TowerManager>();
            return instance;
        }
    }
    #endregion
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
