using UnityEngine;

public class BulletMove : MonoBehaviour
{
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

        if (Vector3.Distance(transform.position, targetPos) < 0.01f)
        {
            moving = false;
            gameObject.SetActive(false); // se resetea para pool
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Comprobamos si colisiona con enemigo
        EnemyHealth enemy = collision.GetComponent<EnemyHealth>();
        if (enemy != null)
        {
            enemy.TakeDamage(1); // daño de bala
            moving = false;
            gameObject.SetActive(false);
        }
    }
}
