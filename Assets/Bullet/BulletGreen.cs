using System.Collections;
using System.Collections.Generic;
using Health;
using Player;
using UnityEngine;

public class BulletGreen : MonoBehaviour
{
    private int damageAmount = 6;

    private float boomRadius = 2;
    private float spawnTime;
    private float lifeTime = 0.5f;
    public ParticleSystem BoomPrefab;

    private void Start()
    {
        spawnTime = Time.time;
    }

    private void Update()
    {
        var difference = Time.time - spawnTime;
        if (difference < lifeTime)
            return;
        Boom();
    }

    private void Boom()
    {
        Instantiate(BoomPrefab, transform.position, Quaternion.identity);

        var objectsToGetDamage = Physics2D.OverlapCircleAll(transform.position, boomRadius);

        foreach (var obj in objectsToGetDamage)
        {
            if (!obj.GetComponent<HealthObj>() || obj.GetComponent<Character>())
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

        //if (collision.gameObject.GetComponent<MimicBoss>() || collision.gameObject.GetComponentInParent<MimicBoss>())
        //{
        //    Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
        //    return;
        //}

        //if (!collision.collider.GetComponent<HealthObj>())
        //    return;

        Boom();
    }
}
