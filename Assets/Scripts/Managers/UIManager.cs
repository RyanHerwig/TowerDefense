using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region Singleton
    private static UIManager instance;

    public static UIManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindAnyObjectByType<UIManager>();
            return instance;
        }
    }
    #endregion

    [SerializeField] TextMeshProUGUI healthText;
    [SerializeField] TextMeshProUGUI moneyText;

    public void UpdateHealth(int health)
    {
        if (health < 0)
            health = 0;
        healthText.text = "Health: " + health;
    }

    public void UpdateMoney(float money)
    {
        moneyText.text = "Money: " + money;
    }
}