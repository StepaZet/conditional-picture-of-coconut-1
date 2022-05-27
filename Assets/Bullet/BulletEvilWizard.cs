using System.Collections;
using System.Collections.Generic;
using Assets.Enemies;
using Health;
using UnityEngine;

public class BulletEvilWizard : MonoBehaviour
{
    private int damageAmount = 5;
    private ParticleSystem fly;
    public ParticleSystem flyPrefab;
    public ParticleSystem BoomPrefab;

    private void Start()
    {
        fly = Instantiate(flyPrefab, transform.position, Quaternion.identity);
    }

    private void Update()
    {
        fly.transform.position = transform.position;
    }

    private void Boom()
    {
        Destroy(fly);
        Instantiate(BoomPrefab, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<MimicBoss>()
            || collision.gameObject.GetComponentInParent<MimicBoss>())
        {
            Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
            return;
        }

        Boom();
        if (collision.collider.GetComponentInChildren<HealthObj>())
            collision.collider.GetComponentInChildren<HealthObj>().Damage(damageAmount);
    }
}
