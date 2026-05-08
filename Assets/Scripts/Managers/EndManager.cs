using UnityEngine;

public class EndManager : MonoBehaviour
{
    #region Singleton
    private static EndManager instance;

    public static EndManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindAnyObjectByType<EndManager>();
            return instance;
        }
    }
    #endregion

    public bool HasWonGame = false;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}