using UnityEngine;
using Weapon;

namespace Bullet
{
    public class ShotgunAmmoPickUp : MonoBehaviour
    {
        private int bulletsAmount = 10;
        public ParticleSystem SpawnAnimation;
        public AudioClip PickUpSound;
        
        private void Start()
        {
            Instantiate(SpawnAnimation, transform.position + new Vector3(0, 0, -2), Quaternion.identity);
        }

        public void OnTriggerEnter2D(Collider2D other)
        {   
            var ammoPickup = new AmmoPickUp<Shotgun>();
            ammoPickup.PickUp(gameObject, other, bulletsAmount, PickUpSound);
        }
    }
}
