using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Extensions;
using UnityEngine;

public class Wave : MonoBehaviour
{
    [SerializeField] protected List<GameObject> Enemies;
    public Vector2 roomScale;

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (Enemies.All(obj => obj == null))
        {
            Enemies.Clear();
            var newEnemies = Physics2D.OverlapAreaAll(
                transform.position.ToVector2() - roomScale / 2,
                transform.position.ToVector2() + roomScale / 2);

            foreach (var newEnemy in newEnemies)
            {
                if (newEnemy.gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    Enemies.Add(newEnemy.gameObject);
                }
            }

            if (Enemies.Count == 0)
            {
                WinWave();
            }

        }
    }

    private void WinWave()
    {
        Destroy(gameObject);
    }
}
