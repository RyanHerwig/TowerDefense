using TMPro;
using UnityEngine;

public class EndScreen : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI statusText;

    EndManager endManager;

    void Start()
    {
        endManager = EndManager.Instance;
        
        if (endManager.HasWonGame)
            statusText.text = "YOU WIN!";
        else
            statusText.text = "YOU LOSE!";
    }
}