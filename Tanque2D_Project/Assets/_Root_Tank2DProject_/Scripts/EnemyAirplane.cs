using UnityEngine;

/// <summary>
/// Enemigo tipo Avión: Vuela en patrón sinusoidal, dispara ocasionalmente
/// </summary>
public class EnemyAirplane : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 3f;
    public float amplitude = 2f; // Amplitud de la onda
    public float frequency = 1f; // Frecuencia de la onda
    public bool moveLeft = true; // Dirección de vuelo

    [Header("Shooting")]
    public Transform firePoint;
    public GameObject bulletPrefab;
    public float fireRate = 2f; // Dispara cada 2 segundos
    public Transform player;

    [Header("Boundaries")]
    public float destroyXOffset = 20f; // Se destruye si sale muy lejos de la pantalla

    private Vector3 startPosition;
    private float nextFireTime;
    private float timeCounter;

    void Start()
    {
        startPosition = transform.position;
        nextFireTime = Time.time + Random.Range(0.5f, 2f); // Delay inicial random

        // Buscar jugador si no está asignado
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
        }
    }

    void Update()
    {
        Move();
        TryShoot();
        CheckBoundaries();
    }

    void Move()
    {
        // Movimiento horizontal
        float direction = moveLeft ? -1f : 1f;
        transform.position += Vector3.right * direction * moveSpeed * Time.deltaTime;

        // Movimiento vertical sinusoidal
        timeCounter += Time.deltaTime;
        float yOffset = Mathf.Sin(timeCounter * frequency) * amplitude;
        transform.position = new Vector3(
            transform.position.x,
            startPosition.y + yOffset,
            transform.position.z
        );

        // Voltear sprite según dirección
        Vector3 scale = transform.localScale;
        scale.x = moveLeft ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

    void TryShoot()
    {
        if (player == null || firePoint == null || bulletPrefab == null)
            return;

        // Solo disparar si está visible en cámara
        if (!IsVisibleByCamera())
            return;

        if (Time.time >= nextFireTime)
        {
            Shoot();
            nextFireTime = Time.time + 1f / fireRate;
        }
    }

    void Shoot()
    {
        // Calcular dirección hacia el jugador
        Vector2 direction = (player.position - firePoint.position).normalized;

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

    void CheckBoundaries()
    {
        // Destruir si sale demasiado de la pantalla
        if (Camera.main != null)
        {
            Vector3 viewportPos = Camera.main.WorldToViewportPoint(transform.position);

            if (moveLeft && viewportPos.x < -0.5f)
            {
                Destroy(gameObject);
            }
            else if (!moveLeft && viewportPos.x > 1.5f)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        // Visualizar el patrón de vuelo
        if (!Application.isPlaying)
            startPosition = transform.position;

        Gizmos.color = Color.cyan;
        Vector3 prevPos = startPosition;

        for (int i = 0; i <= 50; i++)
        {
            float t = i / 50f;
            float x = startPosition.x + (moveLeft ? -1f : 1f) * t * 10f;
            float y = startPosition.y + Mathf.Sin(t * 10f * frequency) * amplitude;
            Vector3 pos = new Vector3(x, y, startPosition.z);

            Gizmos.DrawLine(prevPos, pos);
            prevPos = pos;
        }
    }
}