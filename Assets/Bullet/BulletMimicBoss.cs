using System.Collections;
using System.Collections.Generic;
using Assets.Enemies;
using Health;
using UnityEngine;

public class BulletMimicBoss : MonoBehaviour
{
    public AudioClip DieSound;
    private int damageAmount = 6;

    private float boomRadius = 2;
    private float spawnTime;
    private float lifeTime = 0.5f;
    public ParticleSystem BoomPrefab;

    private void Start()
    {
        spawnTime = Time.time;
        MakeBoomSound();
    }

    private void Update()
    {
        var difference = Time.time - spawnTime;
        if (difference < lifeTime)
            return;
        Boom();
    }

    private void MakeBoomSound()
    {
        if (DieSound != null)
        {
            AudioSource.PlayClipAtPoint(DieSound, transform.position);
        }
    }

    private void Boom()
    {
        MakeBoomSound();
        Instantiate(BoomPrefab, transform.position, Quaternion.identity);
        
        var objectsToGetDamage = Physics2D.OverlapCircleAll(transform.position, boomRadius, layerMask: 9);
        
        foreach (var obj in objectsToGetDamage)
        {
            if (obj.GetComponent<MimicBoss>() || obj.GetComponentInParent<MimicBoss>() || !obj.GetComponentInChildren<HealthObj>())
                continue;

            var healthObj = obj.GetComponentInChildren<HealthObj>();
            if (healthObj != null)
                healthObj.Damage(damageAmount);
        }

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        //if (collision.gameObject.GetComponent<BulletMimicBoss>()
        //    || collision.gameObject.GetComponent<MimicBoss>()
        //    || collision.gameObject.GetComponentInParent<MimicBoss>())
        //{
        //    Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        //    return;
        //}

        //if (!collision.collider.GetComponent<HealthObj>())
        //    return;
        
        Boom();
    }
}
