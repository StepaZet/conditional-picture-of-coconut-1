using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    [SerializeField] private GameObject deathMenu;
    public static bool IsOpened { get; private set; }
    public void Start()
    {
        Player.PlayerObj.OnNoCharactersLeft += SetUp;
    }
    
    public void SetUp(object sender, System.EventArgs eventArgs)
    {
        if (deathMenu == null)
            return;
        deathMenu.SetActive(true);
        IsOpened = true;
    }
    
    public void TryAgain()
    {
        deathMenu.SetActive(false);
        IsOpened = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMenu()
    {
        IsOpened = false;
        SceneManager.LoadScene("Scenes/Menu");
    }
}
