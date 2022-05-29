using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private void Update()
    {
        if (!Input.GetKeyDown(KeyCode.Escape) || DeathMenu.IsOpened)
            return;
        if (Game.GameData.IsPaused)
            Resume();
        else
            Pause();
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        Game.GameData.IsPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        Game.GameData.IsPaused = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        Game.GameData.IsPaused = false;
        SceneManager.LoadScene("Scenes/Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
