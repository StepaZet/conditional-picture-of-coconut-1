using UnityEngine;

public class Weapon : MonoBehaviour, IWeapon
{
	public GameObject bulletPrefab;
	public GameObject weaponPrefab;

	public Transform firePoint;

	public float fireForce = 20f;

	public void Fire()
	{
		var bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
		bullet.GetComponent<Rigidbody2D>().AddForce(firePoint.up * fireForce, ForceMode2D.Impulse);
	}
}