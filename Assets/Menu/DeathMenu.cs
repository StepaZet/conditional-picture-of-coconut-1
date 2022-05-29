using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenu : MonoBehaviour
{
    [SerializeField] private GameObject deathMenu;
    public void Start()
    {
        Player.PlayerObj.OnNoCharactersLeft += SetUp;
    }
    
    public void SetUp(object sender, System.EventArgs eventArgs)
    {
        if (deathMenu == null)
            return;
        deathMenu.SetActive(true);
    }
    
    public void TryAgain()
    {
        deathMenu.SetActive(false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene("Scenes/Menu");
    }
}
