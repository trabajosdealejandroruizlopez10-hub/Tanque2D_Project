using UnityEngine;
using System.Collections;

[System.Serializable]
public class Biome
{
    public string biomeName;
    public Color skyColor = Color.cyan;
    public GameObject biomePrefab; // Prefab del nivel del bioma
    public AudioClip biomeMusic;
    public Vector3 spawnOffset = new Vector3(50f, 0f, 0f); // Donde se genera el siguiente bioma
}

public class BiomeManager : MonoBehaviour
{
    public static BiomeManager Instance { get; private set; }

    [Header("Biome Configuration")]
    public Biome[] biomes; // Array de biomas: 0=Bosque, 1=Desierto, 2=Nieve
    public int currentBiomeIndex = 0;

    [Header("Transition Settings")]
    public Transform player;
    public Camera mainCamera;
    public float transitionDuration = 2f;
    public float cameraColorLerpSpeed = 1f;

    [Header("Current Biome Objects")]
    private GameObject currentBiomeObject;
    private GameObject nextBiomeObject;
    private bool isTransitioning = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        if (biomes.Length > 0)
        {
            LoadBiome(0, Vector3.zero);
        }
    }

    void Update()
    {
        // Lerp gradual del color de la cámara
        if (mainCamera != null && biomes.Length > currentBiomeIndex)
        {
            Color targetColor = biomes[currentBiomeIndex].skyColor;
            mainCamera.backgroundColor = Color.Lerp(
                mainCamera.backgroundColor,
                targetColor,
                Time.deltaTime * cameraColorLerpSpeed
            );
        }
    }

    void LoadBiome(int biomeIndex, Vector3 spawnPosition)
    {
        if (biomeIndex >= biomes.Length) return;

        Biome biome = biomes[biomeIndex];

        // Instanciar el prefab del bioma
        if (biome.biomePrefab != null)
        {
            currentBiomeObject = Instantiate(biome.biomePrefab, spawnPosition, Quaternion.identity);
        }

        // Cambiar música
        if (biome.biomeMusic != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(biome.biomeMusic);
        }

        // Actualizar UI
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowBiomeTransition(biome.biomeName);
        }

        Debug.Log("Cargado bioma: " + biome.biomeName);
    }

    public void StartBiomeTransition(int nextBiomeIndex)
    {
        if (isTransitioning || nextBiomeIndex >= biomes.Length) return;

        StartCoroutine(TransitionBiomeCoroutine(nextBiomeIndex));
    }

    IEnumerator TransitionBiomeCoroutine(int nextBiomeIndex)
    {
        isTransitioning = true;

        // Calcular posición donde spawnnear el siguiente bioma
        Vector3 nextBiomePosition = currentBiomeObject.transform.position + biomes[currentBiomeIndex].spawnOffset;

        // Pre-cargar el siguiente bioma fuera de vista
        Biome nextBiome = biomes[nextBiomeIndex];
        if (nextBiome.biomePrefab != null)
        {
            nextBiomeObject = Instantiate(nextBiome.biomePrefab, nextBiomePosition, Quaternion.identity);
            nextBiomeObject.SetActive(true); // Ya está cargado pero fuera de vista
        }

        Debug.Log("Siguiente bioma pre-cargado: " + nextBiome.biomeName);

        // Esperar a que el jugador entre en la zona de transición
        // (Esto lo controlarás con un trigger en el árbol/cueva)
        yield return new WaitForSeconds(transitionDuration);

        // Cambiar bioma activo
        currentBiomeIndex = nextBiomeIndex;

        // Destruir el bioma anterior (opcional, o desactivar para pooling)
        if (currentBiomeObject != null)
        {
            Destroy(currentBiomeObject, 5f); // Destruir con delay
        }

        currentBiomeObject = nextBiomeObject;

        // Actualizar música y UI
        if (nextBiome.biomeMusic != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic(nextBiome.biomeMusic);
        }

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowBiomeTransition(nextBiome.biomeName);
        }

        isTransitioning = false;
    }

    // Método para llamar desde triggers de transición
    public void TriggerBiomeTransition(int nextBiomeIndex)
    {
        if (!isTransitioning && nextBiomeIndex < biomes.Length)
        {
            StartBiomeTransition(nextBiomeIndex);
        }
    }
}