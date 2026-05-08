using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void CloseGame()
    {
        Application.Quit();
    }

    public void EndGame()
    {
        SceneManager.LoadScene("End Screen");
    }

    public void ReturnToMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}