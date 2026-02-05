using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 5;
    private int currentHealth;

    [Header("Hit Feedback")]
    public Color hitColor = Color.red;
    public float hitFlashDuration = 0.1f;

    [Header("Invulnerability")]
    public float invulnerabilityTime = 1f; // Tiempo de invulnerabilidad tras recibir daño
    private bool isInvulnerable = false;

    // Eventos para que otros sistemas reaccionen
    public UnityEvent<int, int> OnHealthChanged; // (currentHP, maxHP)
    public UnityEvent OnPlayerDeath;

    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;

        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
            originalColor = spriteRenderer.color;

        // Notificar vida inicial
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    public void TakeDamage(int damage)
    {
        if (isDead || isInvulnerable) return;

        currentHealth -= damage;
        currentHealth = Mathf.Max(0, currentHealth);
        
        Debug.Log("Player HP: " + currentHealth + "/" + maxHealth);

        // Notificar cambio de vida
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        // Activar invulnerabilidad temporal
        StartCoroutine(InvulnerabilityCoroutine());

        // Feedback visual
        if (spriteRenderer != null)
            StartCoroutine(HitFlash());

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        if (isDead) return;

        currentHealth += amount;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
        Debug.Log("Player healed! HP: " + currentHealth + "/" + maxHealth);
    }

    IEnumerator HitFlash()
    {
        spriteRenderer.color = hitColor;
        yield return new WaitForSeconds(hitFlashDuration);
        spriteRenderer.color = originalColor;
    }

    IEnumerator InvulnerabilityCoroutine()
    {
        isInvulnerable = true;
        
        // Parpadeo durante invulnerabilidad
        float elapsed = 0f;
        while (elapsed < invulnerabilityTime)
        {
            spriteRenderer.enabled = !spriteRenderer.enabled;
            yield return new WaitForSeconds(0.1f);
            elapsed += 0.1f;
        }
        
        spriteRenderer.enabled = true;
        isInvulnerable = false;
    }

    void Die()
    {
        if (isDead) return;
        
        isDead = true;
        Debug.Log("PLAYER DEAD");
        
        // Notificar muerte
        OnPlayerDeath?.Invoke();
        
        // Desactivar controles
        GetComponent<PlayerMovement>().enabled = false;
        GetComponent<CannonController>().enabled = false;
    }

    // Getters públicos
    public int GetCurrentHealth() => currentHealth;
    public int GetMaxHealth() => maxHealth;
    public bool IsDead() => isDead;
}
