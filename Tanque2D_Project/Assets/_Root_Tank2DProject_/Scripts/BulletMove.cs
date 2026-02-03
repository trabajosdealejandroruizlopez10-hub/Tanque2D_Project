using UnityEngine;

public class BulletMove : MonoBehaviour
{
    [Header("Bullet Settings")]
    public int damage = 1;

    private Vector3 targetPos;
    private float speed;
    private bool moving = false;

    public void Init(Vector3 target, float spd)
    {
        targetPos = target;
        speed = spd;
        moving = true;
    }

    void Update()
    {
        if (!moving) return;

        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, targetPos, step);

        // Si llega al destino, desactivar
        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            moving = false;
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Comprobar si colisiona con enemigo
        EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
            moving = false;
            gameObject.SetActive(false);
            return;
        }

        // Comprobar si colisiona con el suelo/obstáculos
        if (collision.CompareTag("Ground") || collision.CompareTag("Obstacle"))
        {
            moving = false;
            gameObject.SetActive(false);
        }
    }
}
