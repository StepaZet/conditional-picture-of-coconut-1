using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGameObj : MonoBehaviour
{
    public WinMenu winMenu;
    private void OnTriggerEnter2D(Collider2D other)
    {
        winMenu.Open();
    }
}
