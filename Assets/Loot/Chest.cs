using System;
using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] protected Sprite emptySprite;
    [SerializeField] protected int score = 100;
    private bool isEmpty;
    public ParticleSystem SpawnAnimation;
    public static event EventHandler OnScoreChanged;

    private void Start()
    {
        Instantiate(SpawnAnimation, transform.position + new Vector3(0, 0, -2), Quaternion.identity);
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (isEmpty)
            return;
        if (!collider.GetComponent<Character>())
            return;
        Game.GameData.Score += score;
        OnScoreChanged?.Invoke(this, EventArgs.Empty);
        isEmpty = true;
        GetComponent<SpriteRenderer>().sprite = emptySprite;
    }
}
