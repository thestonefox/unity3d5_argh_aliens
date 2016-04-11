using UnityEngine;
using System.Collections;

public class MenuButtons : MonoBehaviour {

    public AudioClip selectSound;

    private AudioSource source;

    void Awake()
    {
        source = GetComponent<AudioSource>();
    }

    public void NewGame()
    {
        source.PlayOneShot(selectSound);
        GameManager.instance.ResetGame();
        GameManager.instance.LoadScene("Level");
    }

    public void MainMenu()
    {
        source.PlayOneShot(selectSound);
        GameManager.instance.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        source.PlayOneShot(selectSound);
        Application.Quit();
    }
}
