using UnityEngine;

public class CannonController : MonoBehaviour
{
    [Header("Disparo")]
    public Transform firePoint;       // Punta del cañón
    public GameObject bulletPrefab;   // Prefab de la bala
    public float bulletSpeed = 15f;

    [Header("Rotación")]
    public bool limit180 = true;      // Limitar arco superior
    public float minAngle = 0f;       // 0° = derecha
    public float maxAngle = 180f;     // 180° = izquierda


    void Start()
    {
        transform.localRotation = Quaternion.Euler(0f, 0f, 0f); // mirar a la derecha al inicio
    }


    void Update()
    {
        AimAtMouse();

        // Disparo con click izquierdo
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

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Limitar rotación solo a arco superior
        if (dir.y < 0)
        {
            // Mouse debajo: apuntar horizontal según X
            if (dir.x >= 0)
                angle = 0f;   // derecha
            else
                angle = 180f; // izquierda
        }

        transform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }


    void Shoot()
    {
        if (bulletPrefab == null || firePoint == null) return;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            // Disparo recto hacia delante
            Vector2 dir = (firePoint.position - transform.position).normalized;
            rb.linearVelocity = dir * bulletSpeed;
        }
    }
}
