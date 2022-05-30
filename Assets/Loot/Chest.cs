using System.Collections;
using System.Collections.Generic;
using Player;
using UnityEngine;

public class Chest : MonoBehaviour
{
    [SerializeField] protected Sprite emptySprite;
    private bool isEmpty;
    public ParticleSystem SpawnAnimation;
    public AudioSource PickUpSound;

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
        isEmpty = true;
        PickUpSound.Play();
        GetComponent<SpriteRenderer>().sprite = emptySprite;
    }
}
