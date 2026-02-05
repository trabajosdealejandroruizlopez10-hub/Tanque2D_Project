using UnityEngine;

/// <summary>
/// Coloca este script en el collider del árbol gigante o la cueva.
/// Cuando el jugador entre, iniciará la transición al siguiente bioma.
/// </summary>
public class BiomeTransitionTrigger : MonoBehaviour
{
    [Header("Transition Settings")]
    [Tooltip("Índice del bioma al que transicionar (0=Bosque, 1=Desierto, 2=Nieve)")]
    public int nextBiomeIndex = 1;

    [Header("Transition Type")]
    public bool isEntrance = true; // true = entrada (árbol/cueva), false = salida
    public GameObject transitionVisuals; // Opcional: oscuridad, efectos visuales

    private bool hasTriggered = false;

    void Start()
    {
        // El collider debe ser trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            col.isTrigger = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hasTriggered) return;

        if (collision.CompareTag("Player"))
        {
            if (isEntrance)
            {
                // Entrada: empezar a cargar el siguiente bioma
                Debug.Log("Jugador entró en zona de transición");
                
                BiomeManager biomeManager = FindFirstObjectByType<BiomeManager>();
                if (biomeManager != null)
                {
                    biomeManager.TriggerBiomeTransition(nextBiomeIndex);
                }

                // Activar efectos visuales (oscurecer pantalla, etc.)
                if (transitionVisuals != null)
                {
                    transitionVisuals.SetActive(true);
                }

                hasTriggered = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isEntrance)
        {
            // Salida: desactivar efectos visuales
            if (transitionVisuals != null)
            {
                transitionVisuals.SetActive(false);
            }
        }
    }

    private void OnDrawGizmos()
    {
        // Visualizar en el editor
        Gizmos.color = isEntrance ? Color.green : Color.red;
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Gizmos.DrawWireCube(col.bounds.center, col.bounds.size);
        }
    }
}
