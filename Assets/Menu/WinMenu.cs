using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinMenu : MonoBehaviour
{
    public GameObject winMenuUI;
    
    public void Close()
    {
        winMenuUI.SetActive(false);
    }

    public void Open()
    {
        winMenuUI.SetActive(true);
    }

    public void LoadNextLevel()
    {
        Time.timeScale = 1f;
        Game.GameData.IsPaused = false;
        SceneManager.LoadScene("Scenes/Endless 1");
    }
}

