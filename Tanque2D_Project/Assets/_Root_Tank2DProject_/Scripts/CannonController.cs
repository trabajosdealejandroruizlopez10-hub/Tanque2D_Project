using UnityEngine;
using UnityEngine.InputSystem;

public class CannonController : MonoBehaviour
{
    [Header("References")]
    public Transform cannonTransform; // el ca��n
    public GameObject bulletPrefab;
    public Transform firePoint; // desde donde sale la bala

    [Header("Settings")]
    public float bulletSpeed = 15f;

    private Vector3 mousePosition;

    // Llamado por el Input System para mover el mouse
    public void OnAim(InputAction.CallbackContext context)
    {
        Vector2 mousePos = context.ReadValue<Vector2>();
        mousePosition = Camera.main.ScreenToWorldPoint(mousePos);
        mousePosition.z = 0;
    }

    // Llamado al click de disparo
    public void OnShoot(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Shoot();
        }
    }

    void Update()
    {
        AimAtMouse();
    }

    void AimAtMouse()
    {
        if (cannonTransform == null) return;
        Vector3 direction = (mousePosition - cannonTransform.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        cannonTransform.rotation = Quaternion.Euler(0, 0, angle);
    }

    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = firePoint.right * bulletSpeed;
        }
    }
}

