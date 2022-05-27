using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] protected Sprite emptySprite;
    private bool isEmpty;

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (isEmpty)
            return;
        if (!collider.GetComponent<Character>())
            return;
        isEmpty = true;
        GetComponent<SpriteRenderer>().sprite = emptySprite;
    }
}
