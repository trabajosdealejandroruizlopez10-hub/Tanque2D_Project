using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    [Header("Settings")]
    public EnemyZone zone; // referencia a la zona
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;

    private float nextFire = 0f;

    void Update()
    {
        // Dispara solo si el player está dentro de la zona
        if (zone != null && zone.PlayerInside)
        {
            if (Time.time >= nextFire)
            {
                Shoot();
                nextFire = Time.time + 1f / fireRate;
            }
        }
    }

    void Shoot()
    {
        Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
    }
}
