using UnityEngine;
using System.Collections;

public class MenuButtons : MonoBehaviour {

    public void NewGame()
    {
        GameManager.instance.ResetGame();
        GameManager.instance.LoadScene("Level");
    }

    public void MainMenu()
    {
        GameManager.instance.LoadScene("MainMenu");
    }

    public void NextLevel()
    {
        GameManager.instance.NextLevel();
        GameManager.instance.LoadScene("Level");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
