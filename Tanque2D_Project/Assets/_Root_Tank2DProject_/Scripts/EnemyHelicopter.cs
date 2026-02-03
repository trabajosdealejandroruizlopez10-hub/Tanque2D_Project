using UnityEngine;

/// <summary>
/// Enemigo tipo Helicóptero: Más lento pero más resistente, puede moverse verticalmente
/// Persigue al jugador manteniendo cierta distancia
/// </summary>
public class EnemyHelicopter : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float hoverHeight = 5f; // Altura preferida de vuelo
    public float heightTolerance = 1f; // Tolerancia para mantener altura
    public float followDistance = 8f; // Distancia horizontal deseada del jugador

    [Header("Shooting")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float fireRate = 1.5f; // Dispara más frecuentemente que el avión
    public int burstCount = 3; // Número de disparos por ráfaga
    public float burstDelay = 0.2f; // Tiempo entre disparos de la ráfaga

    [Header("References")]
    public Transform player;

    [Header("Hover Effect")]
    public float hoverAmplitude = 0.2f; // Pequeña oscilación
    public float hoverFrequency = 2f;

    private float nextFireTime;
    private bool isShooting = false;
    private Vector3 targetPosition;
    private float hoverTime;

    void Start()
    {
        nextFireTime = Time.time + Random.Range(1f, 3f);

        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }

        targetPosition = transform.position;
    }

    void Update()
    {
        if (player != null)
        {
            FollowPlayer();
            ApplyHoverEffect();
            TryShoot();
        }
    }

    void FollowPlayer()
    {
        // Calcular posición objetivo
        float targetX = player.position.x + followDistance;
        float targetY = hoverHeight;

        // Ajustar altura según el terreno (opcional)
        targetPosition = new Vector3(targetX, targetY, transform.position.z);

        // Moverse suavemente hacia la posición objetivo
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        // Voltear sprite para mirar al jugador
        Vector3 scale = transform.localScale;
        if (player.position.x < transform.position.x)
            scale.x = Mathf.Abs(scale.x);
        else
            scale.x = -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    void ApplyHoverEffect()
    {
        // Pequeña oscilación para simular vuelo de helicóptero
        hoverTime += Time.deltaTime;
        float yOffset = Mathf.Sin(hoverTime * hoverFrequency) * hoverAmplitude;

        Vector3 pos = transform.position;
        pos.y = targetPosition.y + yOffset;
        transform.position = pos;
    }

    void TryShoot()
    {
        if (isShooting || player == null || firePoint == null || bulletPrefab == null)
            return;

        if (!IsVisibleByCamera())
            return;

        if (Time.time >= nextFireTime)
        {
            StartCoroutine(ShootBurst());
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    System.Collections.IEnumerator ShootBurst()
    {
        isShooting = true;

        for (int i = 0; i < burstCount; i++)
        {
            ShootSingle();
            yield return new WaitForSeconds(burstDelay);
        }

        isShooting = false;
    }

    void ShootSingle()
    {
        if (player == null) return;

        // Dirección hacia el jugador con un poco de predicción
        Vector3 targetPos = player.position;

        // Predicción simple (opcional)
        Rigidbody2D playerRb = player.GetComponent<Rigidbody2D>();
        if (playerRb != null)
        {
            targetPos += (Vector3)playerRb.linearVelocity * 0.5f;
        }

        Vector2 direction = (targetPos - firePoint.position).normalized;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        EnemyBullet enemyBullet = bullet.GetComponent<EnemyBullet>();

        if (enemyBullet != null)
        {
            enemyBullet.SetDirection(direction);
        }

        // Reproducir sonido
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayEnemyShoot();
        }
    }

    bool IsVisibleByCamera()
    {
        if (Camera.main == null) return false;

        Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);
        return viewportPos.z > 0 &&
               viewportPos.x > -0.1f && viewportPos.x < 1.1f &&
               viewportPos.y > -0.1f && viewportPos.y < 1.1f;
    }

    void OnDrawGizmosSelected()
    {
        // Visualizar zona de seguimiento
        Gizmos.color = Color.yellow;

        if (player != null)
        {
            Vector3 targetPos = player.position + Vector3.right * followDistance + Vector3.up * hoverHeight;
            Gizmos.DrawWireSphere(targetPos, 1f);
            Gizmos.DrawLine(transform.position, targetPos);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position + Vector3.up * hoverHeight, 1f);
        }
    }
}