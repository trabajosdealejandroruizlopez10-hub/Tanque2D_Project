using UnityEngine;

public class CannonController : MonoBehaviour
{
    [Header("Disparo")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float bulletSpeed = 15f;

    [Header("Rotación")]
    public float rotationSpeed = 5f; // velocidad de rotación del cañón

    void Update()
    {
        AimAtMouse();

        if (Input.GetMouseButtonDown(0))
        {
            Shoot();
        }
    }

    void AimAtMouse()
    {
        Vector3 mouseScreen = Input.mousePosition;
        mouseScreen.z = Mathf.Abs(Camera.main.transform.position.z);

        Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScreen);
        mouseWorld.z = 0f;

        Vector2 dir = mouseWorld - transform.position;

        float targetAngle;

        // Si el mouse está debajo del tanque
        if (dir.y < 0)
        {
            targetAngle = dir.x >= 0 ? 0f : 180f;
        }
        else
        {
            // Mouse por encima → arco superior
            targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            // Convertir a 0-360 para LerpAngle
            if (targetAngle < 0) targetAngle += 360f;

            // Limitar arco superior 0-180
            targetAngle = Mathf.Clamp(targetAngle, 0f, 180f);
        }

        // Rotación suave
        float currentAngle = transform.localEulerAngles.z;

        // Mathf.LerpAngle maneja correctamente el paso 0°↔360°
        float newAngle = Mathf.LerpAngle(currentAngle, targetAngle, Time.deltaTime * rotationSpeed);

        transform.localRotation = Quaternion.Euler(0f, 0f, newAngle);
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            Vector2 dir = (firePoint.position - transform.position).normalized;
            rb.linearVelocity = dir * bulletSpeed;
        }
    }
}
