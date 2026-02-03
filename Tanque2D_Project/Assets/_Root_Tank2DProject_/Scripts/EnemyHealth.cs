using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 3;
    private int currentHealth;

    [Header("Visual Feedback")]
    public Color hitColor = Color.red;
    public float hitFlashDuration = 0.1f;

    [Header("Death Settings")]
    public GameObject deathEffectPrefab; // Opcional: efecto de partículas al morir
    public float deathDelay = 0.1f; // Pequeño delay antes de desaparecer

    private SpriteRenderer sr;
    private Color originalColor;

    void Start()
    {
        currentHealth = maxHealth;

        sr = GetComponent<SpriteRenderer>();
        if (sr != null)
            originalColor = sr.color;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log(gameObject.name + " recibió daño. HP restante: " + currentHealth);

        // Feedback visual
        if (sr != null)
            StartCoroutine(HitFlash());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private IEnumerator HitFlash()
    {
        sr.color = hitColor;
        yield return new WaitForSeconds(hitFlashDuration);
        sr.color = originalColor;
    }

    void Die()
    {
        Debug.Log(gameObject.name + " ha muerto.");

        // Notificar al GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.EnemyKilled();
        }

        // Spawn death effect si existe
        if (deathEffectPrefab != null)
        {
            Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
        }

        // Opcional: reproducir sonido de muerte aquí
        // AudioManager.Instance.PlaySound("EnemyDeath");

        // Desactivar o destruir
        StartCoroutine(DeathSequence());
    }

    IEnumerator DeathSequence()
    {
        // Desactivar componentes para que no siga actuando
        var patrol = GetComponent<EnemyPatrol>();
        if (patrol != null) patrol.enabled = false;

        var shooter = GetComponent<EnemyShoot>();
        if (shooter != null) shooter.enabled = false;

        yield return new WaitForSeconds(deathDelay);

        // Desactivar (para pooling) o destruir
        gameObject.SetActive(false);
        // O si prefieres destruir: Destroy(gameObject);
    }

    // Método público para resetear enemigo (útil para pooling)
    public void ResetEnemy()
    {
        currentHealth = maxHealth;
        if (sr != null)
            sr.color = originalColor;

        GetComponent<EnemyPatrol>().enabled = true;
        GetComponent<EnemyShoot>().enabled = true;
        gameObject.SetActive(true);
    }
}
