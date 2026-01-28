using UnityEngine;
using System.Collections;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    private int currentHealth;

    private SpriteRenderer sr;
    public Color hitColor = Color.red; // color al recibir daño
    public float hitFlashDuration = 0.1f; // tiempo que dura el parpadeo

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

        // Parpadeo
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
        gameObject.SetActive(false);
    }
}
