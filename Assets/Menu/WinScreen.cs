using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using UnityEngine;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    [SerializeField] private GameObject characterImageObj;
    [SerializeField] private Text scoreText;


    private void OnEnable()
    {
        characterImageObj.GetComponent<Image>().sprite = GameData.player.character.sprite.sprite;
        scoreText.text = $"You  found  {GameData.Score}  gold";
        Time.timeScale = 0f;
        GameData.IsPaused = true;
    }
}
