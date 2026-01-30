using UnityEngine;

public class EnemyShoot : MonoBehaviour
{
    [Header("References")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public Transform player;

    [Header("Settings")]
    public float fireRate = 1f;
    public float viewPadding = 0.05f; // margen para evitar disparos raros

    private float nextFireTime;

    void Update()
    {
        if (!IsVisibleByMainCamera())
            return;

        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    bool IsVisibleByMainCamera()
    {
        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

        return viewportPos.z > 0 &&
               viewportPos.x > -viewPadding && viewportPos.x < 1 + viewPadding &&
               viewportPos.y > -viewPadding && viewportPos.y < 1 + viewPadding;
    }

    void Shoot()
    {
        Vector2 dir = (player.position - firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        bullet.GetComponent<EnemyBullet>().SetDirection(dir);
    }
}

