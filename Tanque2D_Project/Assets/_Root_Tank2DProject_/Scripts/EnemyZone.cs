using UnityEngine;

public class EnemyZone : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject[] enemies; // enemigos de esta zona

    // Propiedad pública solo lectura
    public bool PlayerInside { get; private set; } = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!PlayerInside && other.CompareTag("Player"))
        {
            PlayerInside = true;
            SpawnEnemies();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (PlayerInside && other.CompareTag("Player"))
        {
            PlayerInside = false;
        }
    }

    void SpawnEnemies()
    {
        foreach (GameObject e in enemies)
            e.SetActive(true);
    }
}
