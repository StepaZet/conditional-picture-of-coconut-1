using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}

